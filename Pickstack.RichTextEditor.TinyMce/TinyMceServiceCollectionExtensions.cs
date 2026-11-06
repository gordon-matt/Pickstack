using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Pickstack.RichTextEditor.TinyMce;

/// <summary>
/// Provides extension methods for tiny mce service collections.
/// </summary>
public static class TinyMceServiceCollectionExtensions
{
    /// <summary>
    /// Adds the tiny mce.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="configure">The configure.</param>
    /// <returns>The tiny mce.</returns>
    public static PickstackBuilder AddTinyMce(this PickstackBuilder builder, Action<TinyMceOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (configure is not null)
        {
            builder.Services.Configure(configure);
        }

        builder.Services.RemoveAll<IRichTextEditor>();
        builder.Services.AddSingleton<TinyMceEditor>();
        builder.Services.AddSingleton<IRichTextEditor>(services => services.GetRequiredService<TinyMceEditor>());
        builder.Services.AddSingleton<IPickstackAssetSource>(services => services.GetRequiredService<TinyMceEditor>());
        return builder;
    }
}