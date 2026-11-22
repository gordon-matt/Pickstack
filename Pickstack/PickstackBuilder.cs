using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pickstack.FileManager;
using Pickstack.RichTextEditor;

namespace Pickstack;

/// <summary>Registers the shared upload folder and the browser runtime.</summary>
public static class PickstackServiceCollectionExtensions
{
    /// <summary>
    /// Registers storage, the embedded browser scripts, and the startup that creates the upload folders.
    /// A storage service, asset source, or startup the host already registered is left in place.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">
    /// Binds the <c>Pickstack:Storage</c> section when it is not <see langword="null"/>.
    /// The section is skipped when this is <see langword="null"/>.
    /// </param>
    /// <param name="configure">Adjusts the storage options after configuration is bound. <see langword="null"/> leaves them as bound.</param>
    /// <returns>A builder for a file manager and an editor.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null"/>.</exception>
    public static PickstackBuilder AddPickstack(this IServiceCollection services, IConfiguration? configuration = null, Action<FileStorageOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (configuration is not null)
        {
            services.Configure<FileStorageOptions>(configuration.GetSection(FileStorageOptions.SectionName));
        }

        if (configure is not null)
        {
            services.Configure(configure);
        }

        services.TryAddSingleton<IFileStorage, FileStorage>();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IPickstackAssetSource, PickstackRuntimeAssets>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IPickstackStartup, PickstackStartup>());
        return new PickstackBuilder(services);
    }
}

/// <summary>Chooses the file manager and the rich text editor. Each <c>Add</c> method replaces the previous choice.</summary>
public sealed class PickstackBuilder
{
    /// <summary>Keeps the collection the host passed to <c>AddPickstack</c>.</summary>
    /// <param name="services">The service collection.</param>
    public PickstackBuilder(IServiceCollection services)
    {
        Services = services;
    }

    /// <summary>The collection adapter methods register against.</summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Registers <typeparamref name="TFileManager"/> as the only <see cref="IFileManager"/>.
    /// An adapter that also serves an embedded script or maps endpoints needs its own registration method.
    /// </summary>
    /// <typeparam name="TFileManager">The file manager type.</typeparam>
    /// <returns>The same builder.</returns>
    public PickstackBuilder AddFileManager<TFileManager>()
        where TFileManager : class, IFileManager
    {
        Services.RemoveAll<IFileManager>();
        Services.AddSingleton<IFileManager, TFileManager>();
        return this;
    }

    /// <summary>Registers <typeparamref name="TEditor"/> as the only <see cref="IRichTextEditor"/>.</summary>
    /// <typeparam name="TEditor">The editor type.</typeparam>
    /// <returns>The same builder.</returns>
    public PickstackBuilder AddRichTextEditor<TEditor>()
        where TEditor : class, IRichTextEditor
    {
        Services.RemoveAll<IRichTextEditor>();
        Services.AddSingleton<IRichTextEditor, TEditor>();
        return this;
    }
}