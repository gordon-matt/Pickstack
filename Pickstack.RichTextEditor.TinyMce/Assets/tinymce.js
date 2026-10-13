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
            if (options && Object.prototype.hasOwnProperty.call(options, "height")) {
                init.height = options.height;
            }

            init.file_picker_callback = function (callback, value, meta) {
                window.Pickstack.files.open({
                    type: meta && meta.filetype ? meta.filetype : "file",
                    callback: function (file) {
                        callback(file.url, { alt: file.name || "" });
                    }
                });
            };

            window.tinymce.init(init);
        }
    });
})(window, document);
