using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pickstack.FileManager;
using Pickstack.RichTextEditor;

namespace Pickstack;

/// <summary>
/// Provides extension methods for pickstack service collections.
/// </summary>
public static class PickstackServiceCollectionExtensions
{
    /// <summary>
    /// Adds the pickstack.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="configure">The configure.</param>
    /// <returns>The pickstack.</returns>
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

/// <summary>
/// Represents the pickstack builder.
/// </summary>
public sealed class PickstackBuilder
{
    /// <summary>
    /// Initializes a new instance of the PickstackBuilder class.
    /// </summary>
    /// <param name="services">The services.</param>
    public PickstackBuilder(IServiceCollection services)
    {
        Services = services;
    }

    /// <summary>
    /// Gets the services.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Adds the file manager.
    /// </summary>
    /// <typeparam name="TFileManager">The type of the file manager.</typeparam>
    /// <returns>The file manager.</returns>
    public PickstackBuilder AddFileManager<TFileManager>()
        where TFileManager : class, IFileManager
    {
        Services.RemoveAll<IFileManager>();
        Services.AddSingleton<IFileManager, TFileManager>();
        return this;
    }

    /// <summary>
    /// Adds the rich text editor.
    /// </summary>
    /// <typeparam name="TEditor">The type of the editor.</typeparam>
    /// <returns>The rich text editor.</returns>
    public PickstackBuilder AddRichTextEditor<TEditor>()
        where TEditor : class, IRichTextEditor
    {
        Services.RemoveAll<IRichTextEditor>();
        Services.AddSingleton<IRichTextEditor, TEditor>();
        return this;
    }
}