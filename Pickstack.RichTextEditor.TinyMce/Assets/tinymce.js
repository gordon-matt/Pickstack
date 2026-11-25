(function (window, document) {
    "use strict";

    var configElement = document.getElementById("pickstack-editor-provider");
    var config = configElement ? JSON.parse(configElement.textContent || "{}") : {};

    if (!window.Pickstack) {
        return;
    }

    loadScript(config.scriptUrl).catch(function () {
        if (!config.fallbackUrl || config.fallbackUrl === config.scriptUrl) {
            throw new Error("Pickstack TinyMCE could not load.");
        }

        return loadScript(config.fallbackUrl);
    }).then(function (loadedUrl) {
        // Absolute, and not the page's <base href>. A host mounted at /admin/email/ would otherwise
        // send themes, plugins and skins to /admin/email/themes/... where they do not exist.
        var baseUrl = libraryBase(loadedUrl);
        installBaseGuard(baseUrl);

        window.Pickstack.editors.use({
            init: function (element, options) {
                if (!window.tinymce) {
                    return;
                }

                options = options || {};
                var init = Object.assign({}, config.init || {});
                if (options.init) {
                    Object.keys(options.init).forEach(function (key) {
                        if (key === "file_picker_callback" || key === "target" || key === "selector" || key === "base_url") {
                            return;
                        }

                        init[key] = key === "plugins" ? flattenPlugins(options.init[key]) : options.init[key];
                    });
                }

                applyInstance(init, options);
                applyTheme(init);
                if (init.toolbar1 || init.toolbar2 || init.toolbar3) {
                    delete init.toolbar;
                }

                init.target = element;
                init.base_url = baseUrl;
                var userSetup = init.setup;
                init.setup = function (editor) {
                    if (typeof userSetup === "function") {
                        userSetup(editor);
                    }

                    editor.on("init", function () {
                        paintEditor(editor);
                    });
                    themedEditors.push(editor);
                    watchHostTheme();
                };
                // No browse button unless this instance uses the registered file manager.
                // TinyMCE then keeps its own image, link, and media dialogs.
                if (window.Pickstack.files.enabledFor(options)) {
                    init.file_picker_callback = function (callback, value, meta) {
                        window.Pickstack.files.open({
                            type: meta && meta.filetype ? meta.filetype : "file",
                            root: options.fileRoot || "",
                            callback: function (file) {
                                callback(file.url, { alt: file.name || "" });
                            }
                        });
                    };
                }

                window.tinymce.init(init);
            }
        });

        if (typeof window.Pickstack.bind === "function") {
            window.Pickstack.bind();
        }
    }).catch(function () {
        console.error("Pickstack TinyMCE could not load.");
    });

    function absoluteUrl(url) {
        return new URL(url, window.location.origin).href;
    }

    function libraryBase(url) {
        return absoluteUrl(url).replace(/\/tinymce(\.min)?\.js.*$/, "");
    }

    // TinyMCE asks the browser for themes/plugins/skins with relative addresses. A document
    // <base> sends those to the mount path. Rewrite them onto the real TinyMCE directory.
    function installBaseGuard(libraryBaseUrl) {
        var root = libraryBaseUrl.replace(/\/$/, "");
        function fix(value) {
            if (typeof value !== "string" || value.length === 0) {
                return value;
            }

            var url;
            try {
                url = new URL(value, document.baseURI);
            } catch (e) {
                return value;
            }

            var tail = url.pathname.match(/(?:^|\/)((?:themes|plugins|models|skins|icons)\/.*)$/);
            if (!tail || url.pathname.indexOf("/_content/Pickstack.RichTextEditor.TinyMce") !== -1 || url.pathname.indexOf("/npm/tinymce") !== -1) {
                return value;
            }

            return root + "/" + tail[1] + url.search + url.hash;
        }

        function wrap(prototype, property) {
            var descriptor = Object.getOwnPropertyDescriptor(prototype, property);
            if (!descriptor || !descriptor.set || descriptor.set._pickstackTinyMce) {
                return;
            }

            var set = descriptor.set;
            function wrapped(value) {
                set.call(this, fix(value));
            }
            wrapped._pickstackTinyMce = true;
            try {
                Object.defineProperty(prototype, property, {
                    configurable: true,
                    enumerable: descriptor.enumerable,
                    get: descriptor.get,
                    set: wrapped
                });
            } catch (e) {
                // A browser that will not expose the setter still gets the absolute base set before TinyMCE starts.
            }
        }

        wrap(HTMLScriptElement.prototype, "src");
        wrap(HTMLLinkElement.prototype, "href");
    }

    function loadScript(url) {
        return new Promise(function (resolve, reject) {
            if (!url) {
                reject(new Error("missing"));
                return;
            }

            var absolute = absoluteUrl(url);
            var base = libraryBase(absolute);
            // Read by TinyMCE as it starts, before it scans script tags and can latch onto this provider file.
            window.tinyMCEPreInit = { base: base, suffix: /\.min\.js(\?|#|$)/.test(absolute) ? ".min" : "" };

            var script = document.createElement("script");
            script.src = absolute;
            script.onload = function () { resolve(absolute); };
            script.onerror = function () {
                script.remove();
                reject(new Error(url));
            };
            document.head.appendChild(script);
        });
    }

    var themedEditors = [];

    function watchHostTheme() {
        if (watchHostTheme.started) {
            return;
        }

        watchHostTheme.started = true;
        new MutationObserver(function () {
            themedEditors.forEach(paintEditor);
        }).observe(document.documentElement, { attributes: true, attributeFilter: ["data-theme"] });
    }

    function paintEditor(editor) {
        if (!editor || editor.removed || typeof editor.getDoc !== "function") {
            return;
        }

        var doc = editor.getDoc();
        if (!doc || !doc.head) {
            return;
        }

        var style = doc.getElementById("pickstack-content-theme");
        if (!style) {
            style = doc.createElement("style");
            style.id = "pickstack-content-theme";
            doc.head.appendChild(style);
        }

        style.textContent = isDarkTheme()
            ? "body{background:#1e1e1e !important;color:#e6edf3 !important;}"
            : "body{background:#fff !important;color:#1f2328 !important;}";
    }

    function isDarkTheme() {
        return document.documentElement.getAttribute("data-theme") === "dark";
    }

    // The editor iframe does not inherit the page theme, so the skin and content colours are chosen here.
    function applyTheme(init) {
        if (init.skin || init.skin_url) {
            return;
        }

        var dark = isDarkTheme();
        init.skin = dark ? "oxide-dark" : "oxide";
        if (!init.content_css) {
            init.content_css = dark ? "dark" : "default";
        }
    }

    function applyInstance(init, options) {
        options = options || {};
        if (Object.prototype.hasOwnProperty.call(options, "height")) {
            init.height = options.height;
        }

        if (typeof options.minHeight === "number") {
            init.min_height = options.minHeight;
        }

        if (typeof options.maxHeight === "number") {
            init.max_height = options.maxHeight;
        }

        if (options.plugins) {
            init.plugins = options.plugins;
        } else if (options.addPlugins) {
            init.plugins = joinPlugins(init.plugins, options.addPlugins, " ");
        }

        if (options.toolbar) {
            init.toolbar = options.toolbar;
        }

        if (typeof options.setup === "function") {
            var previous = init.setup;
            init.setup = function (editor) {
                if (typeof previous === "function") {
                    previous(editor);
                }

                options.setup(editor);
            };
        }
    }

    function flattenPlugins(plugins) {
        if (Array.isArray(plugins)) {
            return joinPlugins(plugins.join(" "), "", " ");
        }

        return plugins;
    }

    function joinPlugins(current, extra, separator) {
        var items = String(current || "").split(/[\s,]+/).concat(String(extra).split(/[\s,]+/));
        var seen = {};
        return items.filter(function (item) {
            var key = item.toLowerCase();
            if (!item || seen[key]) {
                return false;
            }

            seen[key] = true;
            return true;
        }).join(separator);
    }
})(window, document);
