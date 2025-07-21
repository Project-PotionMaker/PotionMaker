using UnityEngine;
using UnityEngine.Serialization;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "MasterAudioPlaylistMuteConnection", menuName = "SettingsGenerator/Connection/MasterAudio/PlaylistMuteConnection", order = 4)]
    public class MasterAudioPlaylistMuteConnectionSO : BoolConnectionSO
    {
        public string PlaylistName;
        public bool Invert;

        protected MasterAudioPlaylistMuteConnection _connection;

        public override IConnection<bool> GetConnection()
        {
            if(_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new MasterAudioPlaylistMuteConnection(PlaylistName, Invert);
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
