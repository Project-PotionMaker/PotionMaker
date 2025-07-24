using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public class MasterAudioBusMuteConnection : Connection<bool>
    {
        public bool Invert;
        public string BusName;

        public MasterAudioBusMuteConnection(string busName, bool invert)
        {
            BusName = busName;
            Invert = invert;
        }
        
#if !KAMGAM_MASTER_AUDIO
        public override bool Get() => true;
        public override void Set(bool mute){}
#else
        public override bool Get()
        {
            return MasterAudioService.Utils.GetBusMute(BusName) != Invert;
        }

        public override void Set(bool mute)
        {
            MasterAudioService.Utils.SetBusMute(BusName, Invert ? !mute : mute);
        }
#endif
    }
}
