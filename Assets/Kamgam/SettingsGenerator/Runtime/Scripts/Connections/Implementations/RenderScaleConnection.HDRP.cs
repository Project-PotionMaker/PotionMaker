#if KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP

using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Kamgam.SettingsGenerator
{
    // HDRP does not have a render scale.

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
            Logger.LogWarning("HDRP does not support render scale. This will do nothing.");

            this.scale = scale;
        }
    }
}

#endif