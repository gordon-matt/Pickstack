(function (window, document) {
    "use strict";

    if (!window.Pickstack || !window.tinymce) {
        console.error("Pickstack TinyMCE could not find tinymce.");
        return;
    }

    window.Pickstack.editors.use({
        init: function (element, options) {
            var script = document.querySelector("script[src*='tinymce']");
            var baseUrl = script ? script.src.replace(/\/tinymce(\.min)?\.js.*$/, "") : undefined;

            window.tinymce.init({
                target: element,
                license_key: "gpl",
                promotion: false,
                base_url: baseUrl,
                height: options.height || 420,
                menubar: false,
                plugins: "autolink lists link image media table code fullscreen",
                toolbar: "undo redo | bold italic underline | alignleft aligncenter alignright | bullist numlist | link image media | code",
                relative_urls: false,
                remove_script_host: false,
                convert_urls: true,
                file_picker_types: "file image media",
                file_picker_callback: function (callback, value, meta) {
                    window.Pickstack.files.open({
                        type: meta && meta.filetype ? meta.filetype : "file",
                        callback: function (file) {
                            callback(file.url, { alt: file.name || "" });
                        }
                    });
                }
            });
        }
    });
})(window, document);
