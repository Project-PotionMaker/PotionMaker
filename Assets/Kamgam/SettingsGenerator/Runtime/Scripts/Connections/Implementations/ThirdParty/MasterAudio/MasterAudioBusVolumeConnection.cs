using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public class MasterAudioBusVolumeConnection : Connection<float>
    {
        public string BusName;
        
        /// <summary>
        /// How the input should be mapped to 0f..1f.<br />
        /// Useful if you have a range in percent (from 0 to 100) but need output ranging from 0f to 1f.
        /// </summary>
        public Vector2 InputRange = new Vector2(0f, 100f);

        public MasterAudioBusVolumeConnection(Vector2 inputRange, string busName)
        {
            InputRange = inputRange;
            BusName = busName;
        }
        
#if !KAMGAM_MASTER_AUDIO
        public override float Get() => 1f;
        public override void Set(float volume){}
#else
        public override float Get()
        {
            var volume = MasterAudioService.Utils.GetBusVolume(BusName);
            return MathUtils.MapWithAnchor(volume, 0f, 0f, 1f, InputRange.x, InputRange.x, InputRange.y, clamp: false);
        }

        public override void Set(float volume)
        {
            float clampedVolume = MathUtils.MapWithAnchor(volume, InputRange.x, InputRange.x, InputRange.y, 0f, 0f, 1f, clamp: false);
            MasterAudioService.Utils.SetBusVolume(BusName, clampedVolume);
        }
#endif
    }
}
