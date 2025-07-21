namespace Kamgam.SettingsGenerator
{
    // We need this class file because Unity requires one file to be named exactly
    // like the class it contains.

    // See .BuiltIn, .URP or .HDRP for the specific implementations.

    public partial class MSAAConnection
        // See MSAAConnection.BuiltIn, we inherit directly from AntiAliasingConnection as that already is MSAA in Built-In.
#if (!KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP) || (KAMGAM_RENDER_PIPELINE_URP && KAMGAM_RENDER_PIPELINE_HDRP)
        : AntiAliasingConnection
#else
        : ConnectionWithOptions<string>
#endif
    { }
}
