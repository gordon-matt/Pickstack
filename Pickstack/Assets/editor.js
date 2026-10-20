export function mount(element, options, dotnet) {
    return new Promise(function (resolve, reject) {
        var attempts = 0;

        function tryInit() {
            if (!element || !element.isConnected) {
                reject(new Error("Pickstack editor element was removed."));
                return;
            }

            if (!window.Pickstack || !window.Pickstack.editors) {
                retry();
                return;
            }

            var started = window.Pickstack.editors.init(element, {
                height: options && options.height,
                plugins: options && options.plugins,
                addPlugins: options && options.addPlugins,
                toolbar: options && options.toolbar,
                fileRoot: options && options.fileRoot,
                setup: function (editor) {
                    element._pickstackEditor = editor;
                    editor.on("change keyup", function () {
                        dotnet.invokeMethodAsync("UpdateValue", editor.getContent());
                    });
                }
            });

            if (started === false) {
                retry();
                return;
            }

            resolve();
        }

        function retry() {
            attempts += 1;
            if (attempts > 200) {
                reject(new Error("Pickstack editor did not load."));
                return;
            }

            setTimeout(tryInit, 30);
        }

        tryInit();
    });
}

export function setContent(element, value) {
    var editor = element && element._pickstackEditor;
    if (!editor) {
        return;
    }

    var next = value || "";
    if (editor.getContent() !== next) {
        editor.setContent(next);
    }
}

export function destroy(element) {
    var editor = element && element._pickstackEditor;
    if (!editor) {
        return;
    }

    element._pickstackEditor = null;
    editor.remove();
}
