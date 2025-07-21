#if KAMGAM_RENDER_PIPELINE_URP && !KAMGAM_RENDER_PIPELINE_HDRP

using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Kamgam.SettingsGenerator
{
    // Built-In does not have a render scale.

    public partial class RenderScaleConnection : Connection<float>
    {
        public UniversalRenderPipelineAsset QualityRenderAsset
        {
            get
            {
                var qualityAsset = QualitySettings.GetRenderPipelineAssetAt(QualitySettings.GetQualityLevel());
                return (qualityAsset as UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset);
            }
        }

        /// <summary>
        /// Notice that the render scale is a property of an ASSET in the editor. Therefore all changes
        /// to it will automatically persist while being used in the Editor but NOT in a build!
        /// </summary>
        public RenderScaleConnection()
        {
        }

        [System.NonSerialized]
        protected float scale = -1f;

        public override float Get()
        {
            // Init as DefaultRenderScale or fetch from asset.
            if (scale < 0f)
                scale = DefaultRenderScale;

            if (QualityRenderAsset != null)
                scale = QualityRenderAsset.renderScale;

            return scale;
        }

        public override void Set(float scale)
        {
            if (QualityRenderAsset != null)
                QualityRenderAsset.renderScale = scale;

            this.scale = scale;
        }

        public override void OnQualityChanged(int qualityLevel)
        {
            // Re-apply render scale on quality change.
            if (ReapplyOnQualityChange)
                Set(scale);

            base.OnQualityChanged(qualityLevel);
        }
    }
}

#endif