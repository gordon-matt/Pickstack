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

                    // init returns before the editor can take content. Apply anything set in that gap.
                    editor.on("init", function () {
                        applyPending(element, editor);
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
    if (!element) {
        return;
    }

    var next = value || "";
    var editor = element._pickstackEditor;

    // TinyMCE reports initialized === false until its init event. setContent before that is lost,
    // so hold the value and let the init handler apply it.
    if (!editor || editor.initialized === false) {
        element._pickstackPending = next;
        return;
    }

    element._pickstackPending = null;
    if (editor.getContent() !== next) {
        editor.setContent(next);
    }
}

function applyPending(element, editor) {
    if (element._pickstackPending == null) {
        return;
    }

    var pending = element._pickstackPending;
    element._pickstackPending = null;
    if (editor.getContent() !== pending) {
        editor.setContent(pending);
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
