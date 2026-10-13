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
            if (options && Object.prototype.hasOwnProperty.call(options, "height")) {
                settings.height = options.height;
            }

            settings.filebrowserBrowseUrl = "/pickstack/ckeditor/browse?type=file";
            settings.filebrowserImageBrowseUrl = "/pickstack/ckeditor/browse?type=image";
            window.CKEDITOR.replace(element, settings);
        }
    });
})(window, document);
