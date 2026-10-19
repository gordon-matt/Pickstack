(function (window, document) {
    "use strict";

    var configElement = document.getElementById("pickstack-file-provider");
    var config = configElement ? JSON.parse(configElement.textContent || "{}") : {};
    var frame = null;
    var folderUrl = "";

    if (!window.Pickstack) {
        return;
    }

    window.addEventListener("message", function (event) {
        if (event.origin !== window.location.origin || !frame || event.source !== frame.contentWindow) {
            return;
        }

        var data = event.data;
        if (!data || data.source !== "pickstack-webfilez") {
            return;
        }

        if (data.type === "navigated") {
            folderUrl = data.url || "";
            return;
        }

        if (data.type === "selected" && data.file) {
            window.Pickstack.files.notify({
                url: data.file.url || "",
                name: data.file.name || "",
                mime: data.file.mime || ""
            });
        }
    });

    window.Pickstack.files.use({
        mount: function (element, options) {
            options = options || {};
            folderUrl = "";

            var params = new URLSearchParams();
            params.set("type", options.type || "any");
            params.set("select", options.select ? "true" : "false");
            if (options.root) {
                params.set("root", options.root);
            }

            frame = document.createElement("iframe");
            frame.title = "File manager";
            frame.src = (config.browserUrl || "/pickstack/webfilez") + "?" + params.toString();
            frame.style.cssText = "display:block;flex:1 1 0;width:100%;height:100%;min-height:0;border:0;";
            element.style.display = "flex";
            element.style.flexDirection = "column";
            element.style.minHeight = "0";
            element.innerHTML = "";
            element.appendChild(frame);
        },
        currentFolder: function () {
            return folderUrl;
        }
    });
})(window, document);
