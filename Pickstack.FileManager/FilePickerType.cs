namespace Pickstack.FileManager;

/// <summary>
/// Specifies the file picker type.
/// </summary>
public enum FilePickerType
{
    /// <summary>No type filter. The browser script receives <c>any</c>.</summary>
    Any,

    /// <summary>A file. The browser script receives <c>file</c>.</summary>
    File,

    /// <summary>An image. A file field also shows a preview when the value is already set. The browser script receives <c>image</c>.</summary>
    Image,

    /// <summary>Media. The browser script receives <c>media</c>.</summary>
    Media,

    /// <summary>Directories. Opening a folder navigates into it, and choosing the open folder writes its path. The browser script receives <c>folder</c>.</summary>
    Folder
}