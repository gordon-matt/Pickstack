(function (window, document) {
    "use strict";

    var configElement = document.getElementById("pickstack-file-provider");
    var config = configElement ? JSON.parse(configElement.textContent || "{}") : {};
    var commands = null;
    var finder = null;

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
                height: Math.max(element.clientHeight || 0, config.minimumHeight || 560),
                resizable: !!config.resizable,
                rememberLastDir: !!config.rememberLastDir,
                useBrowserHistory: !!config.useBrowserHistory,
                commands: getCommands(config.disabledCommands || []),
                uiOptions: {
                    toolbar: config.toolbar
                }
            };

            if (options.root) {
                elfOptions.customData = { root: options.root };
            }

            if (options.type === "image") {
                elfOptions.onlyMimes = ["image"];
            } else if (options.type === "media") {
                elfOptions.onlyMimes = ["video", "audio"];
            } else if (options.type === "folder") {
                elfOptions.onlyMimes = ["directory"];
            }

            if (options.type === "folder") {
                elfOptions.commandsOptions = {
                    getfile: { folders: true }
                };
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

            finder = window.jQuery(host).elfinder(elfOptions).elfinder("instance");
        },
        currentFolder: function () {
            if (!finder) {
                return "";
            }

            var cwd = finder.cwd();
            if (!cwd) {
                return "";
            }

            if (cwd.url) {
                return cwd.url;
            }

            return typeof finder.url === "function" ? finder.url(cwd.hash) || "" : "";
        }
    });

    function getCommands(disabled) {
        if (commands) {
            return commands.slice();
        }

        var available = window.elFinder.prototype._options.commands.slice();
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
