(function (window) {
    "use strict";

    if (!window.Pickstack || !window.CKEDITOR) {
        console.error("Pickstack CKEditor could not find CKEDITOR.");
        return;
    }

    window.Pickstack.editors.use({
        init: function (element, options) {
            window.CKEDITOR.replace(element, {
                height: options.height || 420,
                filebrowserBrowseUrl: "/pickstack/ckeditor/browse?type=file",
                filebrowserImageBrowseUrl: "/pickstack/ckeditor/browse?type=image",
                filebrowserWindowWidth: 980,
                filebrowserWindowHeight: 680
            });
        }
    });
})(window);
