(function (window, document) {
    "use strict";

    var configElement = document.getElementById("pickstack-editor-provider");
    var config = configElement ? JSON.parse(configElement.textContent || "{}") : {};

    if (!window.Pickstack) {
        return;
    }

    Promise.all([
        loadStyle(config.styleUrl).catch(function () {
            return loadStyle(config.fallbackStyleUrl);
        }),
        loadScript(config.scriptUrl).catch(function () {
            return loadScript(config.fallbackUrl);
        })
    ]).then(function () {
        window.Pickstack.editors.use({
            init: function (element, options) {
                if (!window.FroalaEditor) {
                    return;
                }

                var init = Object.assign({}, config.init || {});
                applyInstance(init, options);
                init.events = Object.assign({}, init.events, {
                    "commands.before": function (cmd) {
                        return openPicker(this, cmd, options);
                    }
                });

                new window.FroalaEditor(element, init);
            }
        });

        if (typeof window.Pickstack.bind === "function") {
            window.Pickstack.bind();
        }
    }).catch(function () {
        console.error("Pickstack Froala could not load.");
    });

    function openPicker(editor, cmd, options) {
        var type = cmd === "insertImage" ? "image" : cmd === "insertVideo" ? "media" : cmd === "insertFile" ? "file" : "";
        if (!type) {
            return;
        }

        if (editor.selection && editor.selection.save) {
            editor.selection.save();
        }

        window.Pickstack.files.open({
            type: type,
            root: options && options.fileRoot,
            callback: function (file) {
                if (editor.selection && editor.selection.restore) {
                    editor.selection.restore();
                }

                if (cmd === "insertImage" && editor.image) {
                    editor.image.insert(file.url, false, { alt: file.name || "" }, null);
                } else if (cmd === "insertVideo" && editor.video) {
                    editor.video.insertByURL(file.url);
                } else if (cmd === "insertFile" && editor.file) {
                    editor.file.insert(file.url, file.name || "file", {});
                }
            }
        });

        return false;
    }

    function loadStyle(url) {
        return new Promise(function (resolve, reject) {
            if (!url) {
                reject(new Error("missing"));
                return;
            }

            var link = document.createElement("link");
            link.rel = "stylesheet";
            link.href = url;
            link.onload = function () { resolve(url); };
            link.onerror = function () {
                link.remove();
                reject(new Error(url));
            };
            document.head.appendChild(link);
        });
    }

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
            init.pluginsEnabled = splitList(options.plugins);
        } else if (options.addPlugins) {
            init.pluginsEnabled = joinList(init.pluginsEnabled, options.addPlugins);
        }

        if (options.toolbar) {
            init.toolbarButtons = splitList(options.toolbar);
        }
    }

    function joinList(current, extra) {
        var seen = {};
        return splitList(current).concat(splitList(extra)).filter(function (item) {
            var key = item.toLowerCase();
            if (!item || seen[key]) {
                return false;
            }

            seen[key] = true;
            return true;
        });
    }

    function splitList(value) {
        if (Array.isArray(value)) {
            return value.slice();
        }

        return String(value || "").split(/[\s,]+/).filter(function (item) {
            return item;
        });
    }
})(window, document);
