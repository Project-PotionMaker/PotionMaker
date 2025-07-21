#if KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP
using UnityEngine.Rendering.HighDefinition;

namespace Kamgam.SettingsGenerator
{
    public partial class FogConnection : Connection<bool>
    {
        // Volume based setting. This means it will generate a volume game object
        // with a high priority to override the default settings made by other volumes.

        protected Fog _fog;

        public FogConnection()
        {
            // If this is called during EDIT Mode then the Instance will be null.
            if (SettingsVolume.Instance == null)
                return;

            // NOTICE:
            // Currently there is no "disable" override for VolumeComponents.
            // We achieve the same effect by teaking the values so that the
            // component no longer has any visible effect. Caveat: it will still
            // cost performance as technically it is still active.
            // TODO:
            // Implement a "per volume" mode where we actively disable the
            // VolumeComponent on the currently active volume. Editor revert
            // will be needed since it's an asset.

            // Add the component to the settings volume to override all other.
            _fog = SettingsVolume.Instance.GetOrAddComponent<Fog>();
            _fog.Override(_fog, 1f);
            _fog.active = false;

            // We use this only to disable the effect. We assume
            // the user has it enabled on the volumes by default.
            _fog.enabled.overrideState = true;
            _fog.enabled.value = false;


            // Warn if there is no default effect
#if UNITY_EDITOR
            var defaultBloom = SettingsVolume.Instance.FindDefaultVolumeComponent<Fog>();
            if (defaultBloom == null)
            {
                var name = GetType().Name;
                var effectName = "Fog";
                Logger.LogWarning(
                    name + ": There was no '" + effectName + "' PostProcessing Component found on any volume. " +
                    "Please add a PostPro Volume with a profile containing '" + effectName + "'.\n\n" +
                    "Find out more here: https://docs.unity3d.com/Packages/com.unity.render-pipelines.high-definition@13.1/manual/Volumes.html"
                    );
            }
#endif
        }

        public override bool Get()
        {
            // If this is called during EDIT Mode then the Instance will be null.
            if (_fog == null)
                return true;

            // We use this only to disable the effect.
            // That's why it's inverted here. This means is the "disable override" enabled?
            return !_fog.active;
        }

        public override void Set(bool enable)
        {
            // If this is called during EDIT Mode then the Instance will be null.
            if (_fog == null)
                return;

            // We use this only to disable the effect.
            // That's why it's inverted here. This means enabling the "disable override".
            _fog.active = !enable;

            NotifyListenersIfChanged(enable);
        }
    }
}
#endif