namespace Kamgam.SettingsGenerator
{
    // We need this class file because Unity requires one file to be named exactly
    // like the class it contains.

    // See .BuiltIn, .URP or .HDRP for the specific implementations.

    public partial class RenderScaleConnection
    {
        /// <summary>
        /// Should the render scale be set again if the quality level has been changed? Usually no since
        /// most times higher quality level also means higher render scale.
        /// </summary>
        public bool ReapplyOnQualityChange = false;

        /// <summary>
        /// The default render scale that should be used if no render asset has been found.
        /// </summary>
        public float DefaultRenderScale = 1f;
    }
}
