#if KAMGAM_MASTER_AUDIO
using DarkTonic.MasterAudio;
using Unity.VisualScripting;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public class MasterAudioUtils : IMasterAudioUtils
    {
        /// <summary>
        /// Called automatically before any objects are loaded.
        /// We use this to overcome the assembly divide between the settings system (use asmdefs)
        /// and the third party asset (does not asmdefs by default).
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void init()
        {
            Kamgam.SettingsGenerator.MasterAudioService.Utils = new MasterAudioUtils();
        }
        
        // VOLUME
        
        public float GetMasterVolume()
        {
            return MasterAudio.MasterVolumeLevel;
        }
        
        public void SetMasterVolume(float volume)
        {
            MasterAudio.MasterVolumeLevel = volume;
        }
        
        public float GetPlaylistMasterVolume()
        {
            return MasterAudio.PlaylistMasterVolume;
        }
        
        public void SetPlaylistMasterVolume(float volume)
        {
            MasterAudio.PlaylistMasterVolume = volume;
        }
        
        public float GetGroupVolume(string groupName)
        {
            return MasterAudio.GetGroupVolume(groupName);
        }
        
        public void SetGroupVolume(string groupName, float volume)
        {
            MasterAudio.SetGroupVolume(groupName, volume);
        }
        
        public float GetBusVolume(string busName)
        {
            var buses = MasterAudio.GroupBuses;
            foreach (var bus in buses)
            {
                if (bus.busName == busName)
                    return bus.volume;
            }

            return 1f;
        }
        
        public void SetBusVolume(string busName, float volume)
        {
            var buses = MasterAudio.GroupBuses;
            foreach (var bus in buses)
            {
                if (bus.busName == busName)
                {
                    bus.volume = volume;
                }       
            }
        }

        public float GetPlaylistVolume(string playlistName)
        {
            var controllers = PlaylistController.Instances;
            foreach (var controller in controllers)
            {
                if (controller.PlaylistName == playlistName)
                {
                    return controller.PlaylistVolume;
                }
            }

            return 1f;
        }
        
        public void SetPlaylistVolume(string playlistName, float volume)
        {
            var controllers = PlaylistController.Instances;
            foreach (var controller in controllers)
            {
                if (controller.PlaylistName == playlistName)
                {
                    controller.PlaylistVolume = volume;
                }
            }
        }
        
        
        // MUTE
        
        public bool GetEverythingMute()
        {
            return MasterAudio.MixerMuted && MasterAudio.PlaylistsMuted;
        }
        
        public void SetEverythingMute(bool mute)
        {
            if (mute)
                MasterAudio.MuteEverything();
            else
                MasterAudio.UnmuteEverything();
        }
        
        public bool GetMasterMute()
        {
            return MasterAudio.MixerMuted;
        }
        
        public void SetMasterMute(bool mute)
        {
            MasterAudio.MixerMuted = mute;
        }
        
        public bool GetPlaylistMasterMute()
        {
            return MasterAudio.PlaylistsMuted;
        }
        
        public void SetPlaylistMasterMute(bool mute)
        {
            MasterAudio.PlaylistsMuted = mute;
        }
        
        public bool GetGroupMute(string groupName)
        {
            var group = MasterAudio.GrabGroup(groupName);
            if (group != null)
                return group.isMuted;
            return false;
        }
        
        public void SetGroupMute(string groupName, bool mute)
        {
            if (mute)
                MasterAudio.MuteGroup(groupName);
            else
                MasterAudio.UnmuteGroup(groupName);
        }
        
        public bool GetBusMute(string busName)
        {
            var buses = MasterAudio.GroupBuses;
            foreach (var bus in buses)
            {
                if (bus.busName == busName)
                    return bus.isMuted;
            }

            return false;
        }
        
        public void SetBusMute(string busName, bool mute)
        {
            var buses = MasterAudio.GroupBuses;
            foreach (var bus in buses)
            {
                if (bus.busName == busName)
                {
                    bus.isMuted = mute;
                }       
            }
        }

        public bool GetPlaylistMute(string playlistName)
        {
            var controllers = PlaylistController.Instances;
            foreach (var controller in controllers)
            {
                if (controller.PlaylistName == playlistName)
                {
                    return controller.isMuted;
                }
            }

            return false;
        }
        
        public void SetPlaylistMute(string playlistName, bool mute)
        {
            var controllers = PlaylistController.Instances;
            foreach (var controller in controllers)
            {
                if (controller.PlaylistName == playlistName)
                {
                    controller.isMuted = mute;
                }
            }
        }
    }
}
#endif