using UnityEngine;
using UnityEngine.Serialization;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "MasterAudioMuteEverythingConnection", menuName = "SettingsGenerator/Connection/MasterAudio/MuteEverythingConnection", order = 4)]
    public class MasterAudioMuteEverythingConnectionSO : BoolConnectionSO
    {
        public bool Invert;
            
        protected MasterAudioMuteEverythingConnection _connection;

        public override IConnection<bool> GetConnection()
        {
            if(_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new MasterAudioMuteEverythingConnection(Invert);
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
