using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public class MasterAudioMasterMuteConnection : Connection<bool>
    {
        public bool Invert;
        
        public MasterAudioMasterMuteConnection(bool invert)
        {
            Invert = invert;
        }

#if !KAMGAM_MASTER_AUDIO
        public override bool Get() => true;
        public override void Set(bool mute){}
#else
        public override bool Get()
        {
            return MasterAudioService.Utils.GetMasterMute() != Invert;
        }

        public override void Set(bool mute)
        {
            MasterAudioService.Utils.SetMasterMute(Invert ? !mute : mute);
        }
#endif
    }
}
