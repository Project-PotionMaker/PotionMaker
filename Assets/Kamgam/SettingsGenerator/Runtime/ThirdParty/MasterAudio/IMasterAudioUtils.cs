#if KAMGAM_MASTER_AUDIO

namespace Kamgam.SettingsGenerator
{
    public interface IMasterAudioUtils
    {
        // VOLUME
        
        void SetMasterVolume(float volume);
        float GetMasterVolume();
        
        float GetPlaylistMasterVolume();
        void SetPlaylistMasterVolume(float volume);
        
        float GetPlaylistVolume(string playlistName);
        void SetPlaylistVolume(string playlistName, float volume);
        
        float GetGroupVolume(string groupName);
        void SetGroupVolume(string groupName, float volume);
        
        float GetBusVolume(string busName);
        void SetBusVolume(string busName, float volume);


        // MUTE
        
        bool GetEverythingMute();
        void SetEverythingMute(bool mute);
        
        void SetMasterMute(bool Mute);
        bool GetMasterMute();
        
        bool GetPlaylistMasterMute();
        void SetPlaylistMasterMute(bool Mute);
        
        bool GetPlaylistMute(string playlistName);
        void SetPlaylistMute(string playlistName, bool Mute);
        
        bool GetGroupMute(string groupName);
        void SetGroupMute(string groupName, bool Mute);
        
        bool GetBusMute(string busName);
        void SetBusMute(string busName, bool Mute);
        
    }
}
#endif