using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "MasterAudioMasterMuteConnection", menuName = "SettingsGenerator/Connection/MasterAudio/MasterMuteConnection", order = 4)]
    public class MasterAudioMasterMuteConnectionSO : BoolConnectionSO
    {
        public bool Invert;
        
        protected MasterAudioMasterMuteConnection _connection;

        public override IConnection<bool> GetConnection()
        {
            if(_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new MasterAudioMasterMuteConnection(Invert);
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
