using UnityEngine;
using UnityEngine.Serialization;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "MasterAudioPlaylistMasterMuteConnection", menuName = "SettingsGenerator/Connection/MasterAudio/PlaylistMasterMuteConnection", order = 4)]
    public class MasterAudioPlaylistMasterMuteConnectionSO : BoolConnectionSO
    {
        public bool Invert;
        
        protected MasterAudioPlaylistMasterMuteConnection _connection;

        public override IConnection<bool> GetConnection()
        {
            if(_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new MasterAudioPlaylistMasterMuteConnection(Invert);
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
