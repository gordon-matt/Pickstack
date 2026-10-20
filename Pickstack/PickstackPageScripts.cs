using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Pickstack.Abstractions;
using Pickstack.FileManager;
using Pickstack.RichTextEditor;

namespace Pickstack;

/// <summary>
/// The styles and scripts for whatever file manager and rich text editor are registered.
/// </summary>
public static class PickstackPageScripts
{
    public static IReadOnlyList<ClientInclude> Resolve(IServiceProvider services)
    {
        ArgumentNullException.ThrowIfNull(services);

        var fileManager = services.GetService<IFileManager>();
        var editor = services.GetService<IRichTextEditor>();
        var includes = new List<ClientInclude>();
        if (fileManager is null && editor is null)
        {
            return includes;
        }

        includes.Add(ClientInclude.Script(ClientAssets.RuntimeScriptUrl));
        if (fileManager is not null)
        {
            includes.AddRange(FileManagerAssets.Includes(fileManager));
        }

        if (editor is not null)
        {
            includes.AddRange(EditorAssets.Includes(editor));
        }

        return includes;
    }

    public static string Html(IServiceProvider services)
    {
        var builder = new StringBuilder();
        ClientAssets.Append(builder, Resolve(services));
        return builder.ToString();
    }
}
