using UnityEngine;
using UnityEngine.Serialization;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "MasterAudioGroupMuteConnection", menuName = "SettingsGenerator/Connection/MasterAudio/GroupMuteConnection", order = 4)]
    public class MasterAudioGroupMuteConnectionSO : BoolConnectionSO
    {
        public string GroupName;
        public bool Invert;

        protected MasterAudioGroupMuteConnection _connection;

        public override IConnection<bool> GetConnection()
        {
            if(_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new MasterAudioGroupMuteConnection(GroupName, Invert);
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
