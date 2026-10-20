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
        var baseUrl = loadedUrl.replace(/\/tinymce(\.min)?\.js.*$/, "");

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
                if (init.toolbar1 || init.toolbar2 || init.toolbar3) {
                    delete init.toolbar;
                }

                init.target = element;
                init.base_url = baseUrl;
                init.file_picker_callback = function (callback, value, meta) {
                    window.Pickstack.files.open({
                        type: meta && meta.filetype ? meta.filetype : "file",
                        root: options.fileRoot || "",
                        callback: function (file) {
                            callback(file.url, { alt: file.name || "" });
                        }
                    });
                };

                window.tinymce.init(init);
            }
        });

        if (typeof window.Pickstack.bind === "function") {
            window.Pickstack.bind();
        }
    }).catch(function () {
        console.error("Pickstack TinyMCE could not load.");
    });

    function loadScript(url) {
        return new Promise(function (resolve, reject) {
            if (!url) {
                reject(new Error("missing"));
                return;
            }

            var script = document.createElement("script");
            script.src = url;
            script.onload = function () { resolve(url); };
            script.onerror = function () {
                script.remove();
                reject(new Error(url));
            };
            document.head.appendChild(script);
        });
    }

    function applyInstance(init, options) {
        options = options || {};
        if (Object.prototype.hasOwnProperty.call(options, "height")) {
            init.height = options.height;
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
