// If it's neither URP nor HDRP then it is the old BuiltIn renderer
// If both HDRP and URP are set then we also assume BuiltIn until this ambiguity is resolved by the AssemblyDefinitionUpdater
#if (!KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP) || (KAMGAM_RENDER_PIPELINE_URP && KAMGAM_RENDER_PIPELINE_HDRP)
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public partial class FogConnection : Connection<bool>
    {
        public override bool Get()
        {
            return RenderSettings.fog;
        }

        public override void Set(bool enable)
        {
            RenderSettings.fog = enable;
        }
    }
}
#endif
