#if KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP

using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Kamgam.SettingsGenerator
{
    public partial class VolumetricsEnabledConnection
    {
        /// <summary>
        /// Shadow Resolution connection controlling the shadow resolution on every light.
        /// </summary>
        public VolumetricsEnabledConnection()
        {
            lastKnownValue = Get();
            LightDetector.Instance.OnNewLightFound += onNewLightFound;
        }


        protected void onNewLightFound(Light light)
        {
            applyToLight(light, lastKnownValue);
        }

        public override bool Get()
        {
            // Get level from the first light.
            var light = LightDetector.Instance.GetPrimaryLight();
            if (light != null)
            {
                var data = light.GetComponent<HDAdditionalLightData>();
                if (data != null)
                {
                    return data.affectsVolumetric;
                }
            }

            return lastKnownValue;
        }

        public override void Set(bool enable)
        {
            // Update all the lights
            var lights = LightDetector.Instance.Lights;
            foreach (var light in lights)
            {
                applyToLight(light, enable);
            }

            NotifyListenersIfChanged(enable);
        }

        protected void applyToLight(Light light, bool enable)
        {
            var data = light.GetComponent<HDAdditionalLightData>();
            if (data == null)
                return;

            // set level
            data.affectsVolumetric = enable;
        }
    }
}

#endif
