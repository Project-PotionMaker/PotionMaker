#if KAMGAM_RENDER_PIPELINE_URP && !KAMGAM_RENDER_PIPELINE_HDRP
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