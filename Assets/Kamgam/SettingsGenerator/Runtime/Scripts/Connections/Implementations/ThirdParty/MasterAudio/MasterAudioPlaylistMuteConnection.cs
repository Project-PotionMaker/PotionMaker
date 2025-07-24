using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public class MasterAudioPlaylistMuteConnection : Connection<bool>
    {
        public bool Invert;
        public string PlaylistName;

        public MasterAudioPlaylistMuteConnection(string playlistName, bool invert)
        {
            PlaylistName = playlistName;
            Invert = invert;
        }

#if !KAMGAM_MASTER_AUDIO
        public override bool Get() => true;
        public override void Set(bool mute){}
#else
        public override bool Get()
        {
            return MasterAudioService.Utils.GetPlaylistMute(PlaylistName) != Invert;
        }

        public override void Set(bool mute)
        {
            MasterAudioService.Utils.SetPlaylistMute(PlaylistName, Invert ? !mute : mute);
        }
#endif
    }
}
