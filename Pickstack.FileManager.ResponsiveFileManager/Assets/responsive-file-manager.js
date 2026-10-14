(function (window, document) {
    "use strict";

    var configElement = document.getElementById("pickstack-file-provider");
    var config = configElement ? JSON.parse(configElement.textContent || "{}") : {};
    var fieldId = "pickstack-selected-file";

    if (!window.Pickstack) {
        return;
    }

    window.Pickstack.files.use({
        mount: function (element, options) {
            options = options || {};
            var type = 0;
            if (options.type === "image") {
                type = 1;
            } else if (options.type === "file" || options.type === "folder") {
                type = 2;
            } else if (options.type === "media") {
                type = 3;
            }

            var source = config.dialogUrl + "?type=" + type + "&ignore_last_position=1&popup=0&crossdomain=" + (config.crossDomain ? "1" : "0");
            if (options.type === "folder") {
                source += "&extensions=" + encodeURIComponent("[\"\"]");
            }

            if (options.root) {
                source += "&rootFolder=" + encodeURIComponent(options.root);
            }
            if (config.language) {
                source += "&lang=" + encodeURIComponent(config.language);
            }

            if (options.select) {
                ensureField();
                source += "&field_id=" + encodeURIComponent(fieldId) + "&relative_url=" + (config.relativeUrls ? "1" : "0");
                window.responsive_filemanager_callback = function () {
                    var field = document.getElementById(fieldId);
                    window.Pickstack.files.notify({
                        url: field ? field.value : ""
                    });
                };
            }

            var frame = document.createElement("iframe");
            frame.title = "File manager";
            frame.src = source;
            frame.style.cssText = "width:100%;height:100%;min-height:560px;border:0;";
            element.innerHTML = "";
            element.appendChild(frame);
        }
    });

    function ensureField() {
        var field = document.getElementById(fieldId);
        if (!field) {
            field = document.createElement("input");
            field.type = "hidden";
            field.id = fieldId;
            document.body.appendChild(field);
        }

        field.value = "";
    }
})(window, document);
