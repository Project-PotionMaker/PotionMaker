using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public class MasterAudioGroupMuteConnection : Connection<bool>
    {
        public bool Invert;
        public string GroupName;

        public MasterAudioGroupMuteConnection(string groupName, bool invert)
        {
            GroupName = groupName;
            Invert = invert;
        }
        
#if !KAMGAM_MASTER_AUDIO
        public override bool Get() => true;
        public override void Set(bool mute){}
#else
        public override bool Get()
        {
            return MasterAudioService.Utils.GetGroupMute(GroupName) != Invert;
        }

        public override void Set(bool mute)
        {
            MasterAudioService.Utils.SetGroupMute(GroupName, Invert ? !mute : mute);
        }
#endif
    }
}
