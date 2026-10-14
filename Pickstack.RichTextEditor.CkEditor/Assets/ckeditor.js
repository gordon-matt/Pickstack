(function (window, document) {
    "use strict";

    var configElement = document.getElementById("pickstack-editor-provider");
    var config = configElement ? JSON.parse(configElement.textContent || "{}") : {};

    if (!window.Pickstack || !window.CKEDITOR) {
        console.error("Pickstack CKEditor could not find CKEDITOR.");
        return;
    }

    window.Pickstack.editors.use({
        init: function (element, options) {
            var settings = Object.assign({}, config);
            options = options || {};
            if (Object.prototype.hasOwnProperty.call(options, "height")) {
                settings.height = options.height;
            }

            if (options.addPlugins) {
                settings.extraPlugins = joinPlugins(settings.extraPlugins, options.addPlugins);
            }

            if (Array.isArray(options.toolbar)) {
                settings.toolbar = options.toolbar;
            }

            var root = options.fileRoot ? "&root=" + encodeURIComponent(options.fileRoot) : "";
            settings.filebrowserBrowseUrl = "/pickstack/ckeditor/browse?type=file" + root;
            settings.filebrowserImageBrowseUrl = "/pickstack/ckeditor/browse?type=image" + root;
            window.CKEDITOR.replace(element, settings);
        }
    });

    function joinPlugins(current, extra) {
        var items = String(current || "").split(/[\s,]+/).concat(String(extra).split(/[\s,]+/));
        var seen = {};
        return items.filter(function (item) {
            var key = item.toLowerCase();
            if (!item || seen[key]) {
                return false;
            }

            seen[key] = true;
            return true;
        }).join(",");
    }
})(window, document);
