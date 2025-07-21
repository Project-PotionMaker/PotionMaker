// If it's neither URP nor HDRP then it is the old BuiltIn renderer
// If both HDRP and URP are set then we also assume BuiltIn until this ambiguity is resolved by the AssemblyDefinitionUpdater
#if (!KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP) || (KAMGAM_RENDER_PIPELINE_URP && KAMGAM_RENDER_PIPELINE_HDRP)

using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    // Built-In does not have a render scale.

    public partial class RenderScaleConnection : Connection<float>
    {
        public RenderScaleConnection()
        {
            
        }

        protected float scale = 1f;

        public override float Get()
        {
            return scale;
        }

        public override void Set(float scale)
        {
            Logger.LogWarning("Built-In does not support render scale. This will do nothing.");

            this.scale = scale;
        }
    }
}

#endif