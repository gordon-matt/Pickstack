(function (window, document) {
    "use strict";

    var configElement = document.getElementById("pickstack-editor-provider");
    var config = configElement ? JSON.parse(configElement.textContent || "{}") : {};

    if (!window.Pickstack || !window.tinymce) {
        console.error("Pickstack TinyMCE could not find tinymce.");
        return;
    }

    window.Pickstack.editors.use({
        init: function (element, options) {
            var script = document.querySelector("script[src*='tinymce']");
            var baseUrl = script ? script.src.replace(/\/tinymce(\.min)?\.js.*$/, "") : undefined;
            var init = Object.assign({}, config);

            init.target = element;
            init.base_url = baseUrl;
            applyInstance(init, options);

            init.file_picker_callback = function (callback, value, meta) {
                window.Pickstack.files.open({
                    type: meta && meta.filetype ? meta.filetype : "file",
                    root: options && options.fileRoot,
                    callback: function (file) {
                        callback(file.url, { alt: file.name || "" });
                    }
                });
            };

            window.tinymce.init(init);
        }
    });

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
