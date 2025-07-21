using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public class MasterAudioPlaylistMasterMuteConnection : Connection<bool>
    {
        public bool Invert;
        
        public MasterAudioPlaylistMasterMuteConnection(bool invert)
        {
            Invert = invert;
        }

#if !KAMGAM_MASTER_AUDIO
        public override bool Get() => true;
        public override void Set(bool mute){}
#else
        public override bool Get()
        {
            return MasterAudioService.Utils.GetPlaylistMasterMute() != Invert;
        }

        public override void Set(bool mute)
        {
            MasterAudioService.Utils.SetPlaylistMasterMute(Invert ? !mute : mute);
        }
#endif
    }
}
