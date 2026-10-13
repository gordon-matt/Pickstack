(function (window, document) {
    "use strict";

    var configElement = document.getElementById("pickstack-file-provider");
    var config = configElement ? JSON.parse(configElement.textContent || "{}") : {};
    var commands = null;

    if (!window.Pickstack || !window.elFinder || !window.jQuery) {
        console.error("Pickstack elFinder requires jQuery, jQuery UI, and elFinder.");
        return;
    }

    window.Pickstack.files.use({
        mount: function (element, options) {
            options = options || {};
            var host = document.createElement("div");
            host.style.height = "100%";
            element.innerHTML = "";
            element.appendChild(host);

            var elfOptions = {
                baseUrl: config.baseUrl,
                url: config.connectorUrl,
                height: Math.max(element.clientHeight || 0, 560),
                resizable: false,
                rememberLastDir: false,
                useBrowserHistory: false,
                commands: getCommands(),
                uiOptions: {
                    toolbar: [
                        ["back", "forward"],
                        ["reload"],
                        ["home", "up"],
                        ["mkdir", "mkfile", "upload"],
                        ["open", "download", "getfile"],
                        ["info"],
                        ["quicklook"],
                        ["copy", "cut", "paste"],
                        ["rm"],
                        ["duplicate", "rename", "edit"],
                        ["view", "sort"],
                        ["search"]
                    ]
                }
            };

            if (options.type === "image") {
                elfOptions.onlyMimes = ["image"];
            } else if (options.type === "media") {
                elfOptions.onlyMimes = ["video", "audio"];
            }

            if (options.select) {
                elfOptions.getFileCallback = function (file) {
                    window.Pickstack.files.notify({
                        url: file.url,
                        name: file.name,
                        mime: file.mime
                    });
                };
            }

            window.jQuery(host).elfinder(elfOptions).elfinder("instance");
        }
    });

    function getCommands() {
        if (commands) {
            return commands.slice();
        }

        var available = window.elFinder.prototype._options.commands.slice();
        var disabled = ["callback", "chmod", "editor", "netmount", "ping", "help"];
        disabled.forEach(function (command) {
            var index = available.indexOf(command);
            if (index >= 0) {
                available.splice(index, 1);
            }
        });

        commands = available;
        return commands.slice();
    }
})(window, document);
