using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Pickstack.RichTextEditor.TinyMce;

/// <summary>Registers the shared TinyMCE adapter.</summary>
public static class TinyMceServiceCollectionExtensions
{
    /// <summary>
    /// Registers TinyMCE as the only <see cref="IRichTextEditor"/> and serves <c>tinymce.js</c>.
    /// An editor already registered is replaced. A version package calls this after setting that version's script and license.
    /// </summary>
    /// <param name="builder">The Pickstack builder.</param>
    /// <param name="configure">Adjusts the options. <see langword="null"/> keeps the defaults, including the TinyMCE 8 CDN address.</param>
    /// <returns>The same builder.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="builder"/> is <see langword="null"/>.</exception>
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