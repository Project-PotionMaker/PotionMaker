namespace Kamgam.SettingsGenerator
{
    // We need this class file because Unity requires one file to be named exactly
    // like the class it contains.

    // See .BuiltIn, .URP or .HDRP for the specific implementations.

    public partial class AntiAliasingConnection : ConnectionWithOptions<string>
    {
        /// <summary>
        /// Please notice that this has no effect in the Built-In render pipeline since there the anti aliasing settings is set globally in the GraphicsSettings. In URP and HDRP it's set per camera.
        /// </summary>
        public bool LimitToMainCamera;
        
        /// <summary>
        /// If enabled then the options list will include MSAA for renders that support it (URP AND HDRP).
        /// </summary>
        public bool IncludeMSAA = false;
    }
}
