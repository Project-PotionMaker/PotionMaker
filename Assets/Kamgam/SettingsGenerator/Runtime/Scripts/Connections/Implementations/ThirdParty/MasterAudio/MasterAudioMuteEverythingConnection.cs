using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public class MasterAudioMuteEverythingConnection : Connection<bool>
    {
        public bool Invert;
        
        public MasterAudioMuteEverythingConnection(bool invert)
        {
            Invert = invert;
        }

#if !KAMGAM_MASTER_AUDIO
        public override bool Get() => true;
        public override void Set(bool mute){}
#else
        public override bool Get()
        {
            return MasterAudioService.Utils.GetEverythingMute() != Invert;
        }

        public override void Set(bool mute)
        {
            MasterAudioService.Utils.SetEverythingMute(Invert ? !mute : mute);
        }
#endif
    }
}
