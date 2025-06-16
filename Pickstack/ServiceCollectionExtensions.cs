using Microsoft.Extensions.DependencyInjection;
using Pickstack.FileManager;
using Pickstack.RichTextEditor;

namespace Pickstack;

public static class ServiceCollectionExtensions
{
    public static PickstackBuilder AddPickstack(this IServiceCollection services)
    {
        var builder = new PickstackBuilder(services);
        return builder;
    }
}

public class PickstackBuilder
{
    public IServiceCollection Services { get; }

    public PickstackBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public PickstackBuilder AddFileManager<T>() where T : class, IFileManager
    {
        Services.AddScoped<IFileManager, T>();
        return this;
    }

    public PickstackBuilder AddRichTextEditor<T>() where T : class, IRichTextEditor
    {
        Services.AddScoped<IRichTextEditor, T>();
        return this;
    }
}