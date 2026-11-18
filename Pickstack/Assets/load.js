// One load at a time. Two PickstackScripts on the same page otherwise both see a script tag that
// has not finished running, and the file manager script gives up because Pickstack is not there yet.
var chain = Promise.resolve();
var loadedScripts = Object.create(null);

export function load(items) {
    var run = chain.then(function () {
        return loadItems(items);
    });
    chain = run.catch(function () { });
    return run;
}

async function loadItems(items) {
    if (!items || !items.length) {
        return;
    }

    for (var index = 0; index < items.length; index++) {
        var item = items[index];
        if (!item || !item.kind) {
            continue;
        }

        if (item.kind === "style") {
            ensureStyle(item.url);
        } else if (item.kind === "json") {
            ensureJson(item.id, item.content);
        } else if (item.kind === "script") {
            await ensureScript(item.url);
        }
    }
}

function ensureStyle(url) {
    if (!url || hasUrl("link", "href", url)) {
        return;
    }

    var link = document.createElement("link");
    link.rel = "stylesheet";
    link.href = url;
    document.head.appendChild(link);
}

function ensureJson(id, content) {
    if (!id || document.getElementById(id)) {
        return;
    }

    var element = document.createElement("script");
    element.type = "application/json";
    element.id = id;
    element.textContent = content || "";
    document.head.appendChild(element);
}

function ensureScript(url) {
    if (!url) {
        return Promise.resolve();
    }

    var absolute = new URL(url, document.baseURI).href;
    if (loadedScripts[absolute]) {
        return Promise.resolve();
    }

    return new Promise(function (resolve, reject) {
        var script = document.createElement("script");
        script.src = url;
        script.onload = function () {
            loadedScripts[absolute] = true;
            resolve();
        };
        script.onerror = function () { reject(new Error("Failed to load " + url)); };
        document.head.appendChild(script);
    });
}

function hasUrl(tagName, attribute, url) {
    var absolute = new URL(url, document.baseURI).href;
    var nodes = document.getElementsByTagName(tagName);
    for (var index = 0; index < nodes.length; index++) {
        var node = nodes[index];
        var value = node.getAttribute(attribute) || "";
        if (value === url || node[attribute] === absolute) {
            return true;
        }
    }

    return false;
}
