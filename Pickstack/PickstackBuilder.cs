using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pickstack.FileManager;
using Pickstack.RichTextEditor;

namespace Pickstack;

public static class PickstackServiceCollectionExtensions
{
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

public sealed class PickstackBuilder
{
    public PickstackBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; }

    public PickstackBuilder AddFileManager<TFileManager>()
        where TFileManager : class, IFileManager
    {
        Services.RemoveAll<IFileManager>();
        Services.AddSingleton<IFileManager, TFileManager>();
        return this;
    }

    public PickstackBuilder AddRichTextEditor<TEditor>()
        where TEditor : class, IRichTextEditor
    {
        Services.RemoveAll<IRichTextEditor>();
        Services.AddSingleton<IRichTextEditor, TEditor>();
        return this;
    }
}
