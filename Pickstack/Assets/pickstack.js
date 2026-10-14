(function (window, document) {
    "use strict";

    if (window.Pickstack && window.Pickstack.files) {
        return;
    }

    var fileProvider = null;
    var editorProvider = null;
    var listeners = [];
    var activeSubscription = null;

    var Pickstack = window.Pickstack = window.Pickstack || {};

    Pickstack.files = {
        use: function (provider) {
            fileProvider = provider;
        },
        open: function (options) {
            options = options || {};
            var modal = ensureModal();
            var body = modal.querySelector("[data-pickstack-modal-body]");
            var title = modal.querySelector("[data-pickstack-modal-title]");
            var useFolder = modal.querySelector("[data-pickstack-use-folder]");
            var selectingFolder = options.type === "folder";
            title.textContent = options.title || (selectingFolder ? "Select a folder" : "Select a file");
            if (useFolder) {
                useFolder.hidden = !selectingFolder;
            }
            body.innerHTML = "";
            modal.hidden = false;

            if (activeSubscription) {
                activeSubscription();
                activeSubscription = null;
            }

            activeSubscription = subscribe(function (file) {
                if (options.fieldId) {
                    var field = document.getElementById(options.fieldId);
                    if (field) {
                        field.value = file.url || "";
                        field.dispatchEvent(new Event("input", { bubbles: true }));
                        field.dispatchEvent(new Event("change", { bubbles: true }));
                        var root = field.closest("[data-pickstack-file-input]");
                        if (root) {
                            renderPreview(root, file.url || "");
                        }
                    }
                }

                if (typeof options.callback === "function") {
                    options.callback(file);
                }

                closeModal();
            });

            mount(body, {
                type: options.type || "any",
                select: true,
                root: options.root || ""
            });
        },
        mount: function (element, options) {
            mount(element, options || {});
        },
        notify: function (file) {
            if (!file) {
                return;
            }

            if (typeof file === "string") {
                file = { url: file };
            }

            file.url = rootRelativeUrl(file.url);
            listeners.slice().forEach(function (listener) {
                listener(file);
            });
        },
        onSelect: function (listener) {
            return subscribe(listener);
        }
    };

    Pickstack.editors = {
        use: function (provider) {
            editorProvider = provider;
        },
        init: function (element, options) {
            if (!editorProvider) {
                return false;
            }

            editorProvider.init(element, options || {});
            return true;
        }
    };

    function subscribe(listener) {
        listeners.push(listener);
        return function () {
            var index = listeners.indexOf(listener);
            if (index >= 0) {
                listeners.splice(index, 1);
            }
        };
    }

    function mount(element, options) {
        if (!element) {
            return;
        }

        if (!fileProvider) {
            element.textContent = "No Pickstack file manager is registered.";
            return;
        }

        options = options || {};
        if (!options.root) {
            fileProvider.mount(element, options);
            return;
        }

        fetch("/pickstack/files/root?root=" + encodeURIComponent(options.root), { cache: "no-store" })
            .finally(function () {
                fileProvider.mount(element, options);
            });
    }

    function rootRelativeUrl(url) {
        if (!url || !/^https?:\/\//i.test(url)) {
            return url || "";
        }

        try {
            var parsed = new URL(url);
            if (parsed.origin === window.location.origin) {
                return parsed.pathname + parsed.search + parsed.hash;
            }
        } catch (error) {
            return url;
        }

        return url;
    }

    function ensureModal() {
        var existing = document.getElementById("pickstack-modal");
        if (existing) {
            return existing;
        }

        injectStyles();

        var modal = document.createElement("div");
        modal.id = "pickstack-modal";
        modal.className = "pickstack-modal modal";
        modal.hidden = true;
        modal.setAttribute("role", "dialog");
        modal.setAttribute("aria-modal", "true");
        modal.innerHTML =
            "<div class=\"pickstack-modal-dialog\" role=\"document\">" +
            "<div class=\"pickstack-modal-header\">" +
            "<strong data-pickstack-modal-title>Select a file</strong>" +
            "<span class=\"pickstack-modal-actions\">" +
            "<button type=\"button\" class=\"pickstack-modal-use-folder\" data-pickstack-use-folder hidden>Use this folder</button>" +
            "<button type=\"button\" class=\"pickstack-modal-close\" data-pickstack-modal-close aria-label=\"Close\">&times;</button>" +
            "</span>" +
            "</div>" +
            "<div class=\"pickstack-modal-body\" data-pickstack-modal-body></div>" +
            "</div>";

        modal.addEventListener("click", function (event) {
            if (event.target === modal) {
                closeModal();
            }
        });
        modal.querySelector("[data-pickstack-modal-close]").addEventListener("click", closeModal);
        modal.querySelector("[data-pickstack-use-folder]").addEventListener("click", function () {
            if (!fileProvider || typeof fileProvider.currentFolder !== "function") {
                return;
            }

            var folder = fileProvider.currentFolder();
            var url = folder && (typeof folder === "string" ? folder : folder.url);
            if (!url) {
                return;
            }

            Pickstack.files.notify(typeof folder === "string" ? { url: url } : folder);
        });
        document.addEventListener("keydown", function (event) {
            if (event.key === "Escape" && !modal.hidden) {
                closeModal();
            }
        });
        document.body.appendChild(modal);
        return modal;
    }

    function closeModal() {
        if (activeSubscription) {
            activeSubscription();
            activeSubscription = null;
        }

        window.setTimeout(function () {
            var modal = document.getElementById("pickstack-modal");
            if (!modal) {
                return;
            }

            modal.hidden = true;
            var body = modal.querySelector("[data-pickstack-modal-body]");
            if (body) {
                body.innerHTML = "";
            }
        }, 0);
    }

    function injectStyles() {
        if (document.getElementById("pickstack-modal-style")) {
            return;
        }

        var style = document.createElement("style");
        style.id = "pickstack-modal-style";
        style.textContent =
            ".pickstack-modal{position:fixed;inset:0;z-index:100000;background:rgba(0,0,0,.45);display:flex;align-items:center;justify-content:center;padding:1rem;}" +
            ".pickstack-modal[hidden]{display:none !important;}" +
            ".pickstack-modal-dialog{background:#fff;width:min(1100px,96vw);height:min(760px,92vh);display:flex;flex-direction:column;border-radius:8px;overflow:hidden;box-shadow:0 12px 40px rgba(0,0,0,.2);}" +
            ".pickstack-modal-header{display:flex;align-items:center;justify-content:space-between;gap:1rem;padding:.75rem 1rem;border-bottom:1px solid #dee2e6;}" +
            ".pickstack-modal-actions{display:flex;align-items:center;gap:.75rem;}" +
            ".pickstack-modal-use-folder{border:1px solid #0d6efd;background:#0d6efd;color:#fff;border-radius:4px;padding:.25rem .65rem;font-size:.875rem;cursor:pointer;}" +
            ".pickstack-modal-use-folder[hidden]{display:none;}" +
            ".pickstack-modal-close{border:0;background:transparent;font-size:1.5rem;line-height:1;padding:0 .25rem;}" +
            ".pickstack-modal-body{flex:1;min-height:0;}" +
            ".pickstack-preview img{display:block;max-height:140px;margin-top:.5rem;border:1px solid #dee2e6;border-radius:.25rem;}";
        document.head.appendChild(style);
    }

    function renderPreview(root, url) {
        var preview = root.querySelector("[data-pickstack-preview]");
        if (!preview) {
            return;
        }

        var type = root.getAttribute("data-file-type");
        preview.innerHTML = "";
        if (type === "image" && url) {
            var image = document.createElement("img");
            image.src = url;
            image.alt = "";
            preview.appendChild(image);
            return;
        }

        preview.textContent = url || "";
    }

    function readOptions(element) {
        var raw = element.getAttribute("data-editor-options");
        if (!raw) {
            return {};
        }

        try {
            return JSON.parse(raw);
        } catch (error) {
            return {};
        }
    }

    function bind() {
        document.querySelectorAll("[data-pickstack-file-input]").forEach(function (root) {
            if (root.getAttribute("data-pickstack-bound") === "true") {
                return;
            }

            root.setAttribute("data-pickstack-bound", "true");
            var button = root.querySelector("[data-pickstack-browse]");
            var field = root.querySelector("input, textarea");
            if (!button || !field) {
                return;
            }

            button.addEventListener("click", function () {
                Pickstack.files.open({
                    fieldId: field.id,
                    type: root.getAttribute("data-file-type") || "any",
                    root: root.getAttribute("data-file-root") || ""
                });
            });
        });

        document.querySelectorAll("[data-pickstack-editor]").forEach(function (element) {
            if (element.getAttribute("data-pickstack-bound") === "true") {
                return;
            }

            if (Pickstack.editors.init(element, readOptions(element)) === false) {
                return;
            }

            element.setAttribute("data-pickstack-bound", "true");
        });

        document.querySelectorAll("[data-pickstack-browser]").forEach(function (element) {
            if (element.getAttribute("data-pickstack-bound") === "true") {
                return;
            }

            element.setAttribute("data-pickstack-bound", "true");
            Pickstack.files.mount(element, {
                type: element.getAttribute("data-file-type") || "any",
                select: element.getAttribute("data-select") === "true",
                root: element.getAttribute("data-file-root") || ""
            });
        });
    }

    Pickstack.bind = bind;

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", bind);
    } else {
        bind();
    }
})(window, document);
