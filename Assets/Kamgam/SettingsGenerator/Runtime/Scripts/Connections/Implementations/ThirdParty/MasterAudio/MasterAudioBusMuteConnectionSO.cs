using UnityEngine;
using UnityEngine.Serialization;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "MasterAudioBusMuteConnection", menuName = "SettingsGenerator/Connection/MasterAudio/BusMuteConnection", order = 4)]
    public class MasterAudioBusMuteConnectionSO : BoolConnectionSO
    {
        public string BusName;
        public bool Invert;

        protected MasterAudioBusMuteConnection _connection;

        public override IConnection<bool> GetConnection()
        {
            if(_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new MasterAudioBusMuteConnection(BusName, Invert);
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
