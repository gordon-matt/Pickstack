using System.Text.Json;
using Extenso;

namespace Pickstack.FileManager.elFinder;

public class ElFinder : IFileManager
{
    public string Name => "elFinder";

    public string GetMarkup(FileManagerConfig config) => $@"<div id=""elfinder""></div>";

    public string GetJsonConfig(FileManagerConfig config)
    {
        return
$@"$(document).ready(function () {{
        var myCommands = elFinder.prototype._options.commands;
        var disabled = ['callback', 'chmod', 'editor', 'netmount', 'ping', 'help']; // Not yet implemented commands in elFinder.NetCore
        elFinder.prototype.i18.en.messages.TextArea = ""Edit"";
        elFinder.prototype.i18.en.messages.errNewNameSelection = 'Unable to create new file with name ""$1""';

        $.each(disabled, function (i, cmd) {{
            (idx = $.inArray(cmd, myCommands)) !== -1 && myCommands.splice(idx, 1);
        }});

        var options = {(config.Options.Values.Select(v => (JsonElement)v).Select(v => v.EnumerateArray().Select(e => e.GetString()).ToArray()).JsonSerialize())};
        $('#elfinder').elfinder(options).elfinder('instance');
    }});";
    }
}