using Pickstack.Abstractions;

namespace Pickstack.RichTextEditor;

/// <summary>
/// A replaceable rich text editor. File browsing goes through <c>Pickstack.files</c>,
/// so the editor does not know which file manager is installed.
/// </summary>
public interface IRichTextEditor : IPickstackComponent
{
}
