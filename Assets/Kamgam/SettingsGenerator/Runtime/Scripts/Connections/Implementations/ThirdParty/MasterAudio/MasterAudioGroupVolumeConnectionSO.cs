using UnityEngine;
using UnityEngine.Serialization;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "MasterAudioGroupVolumeConnection", menuName = "SettingsGenerator/Connection/MasterAudio/GroupVolumeConnection", order = 4)]
    public class MasterAudioGroupVolumeConnectionSO : FloatConnectionSO
    {
        public string GroupName;
        
        [Tooltip("How the input should be mapped to 0f..1f.\n" +
                 "Useful if you have a range in percent (from 0 to 100) but need output ranging from 0f to 1f.")]
        public Vector2 InputRange = new Vector2(0f, 100f);

        protected MasterAudioGroupVolumeConnection _connection;

        public override IConnection<float> GetConnection()
        {
            if(_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new MasterAudioGroupVolumeConnection(InputRange, GroupName);
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
