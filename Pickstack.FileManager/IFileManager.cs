using Pickstack.Abstractions;

namespace Pickstack.FileManager;

/// <summary>
/// A replaceable file manager. Editors and file fields never reference a concrete
/// implementation; they call <c>Pickstack.files</c>, and the registered manager supplies it.
/// </summary>
public interface IFileManager : IPickstackComponent
{
}