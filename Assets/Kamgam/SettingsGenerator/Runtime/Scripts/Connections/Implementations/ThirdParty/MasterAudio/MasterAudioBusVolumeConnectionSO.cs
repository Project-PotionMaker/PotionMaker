using UnityEngine;
using UnityEngine.Serialization;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "MasterAudioBusVolumeConnection", menuName = "SettingsGenerator/Connection/MasterAudio/BusVolumeConnection", order = 4)]
    public class MasterAudioBusVolumeConnectionSO : FloatConnectionSO
    {
        public string BusName;
        
        [Tooltip("How the input should be mapped to 0f..1f.\n" +
                 "Useful if you have a range in percent (from 0 to 100) but need output ranging from 0f to 1f.")]
        public Vector2 InputRange = new Vector2(0f, 100f);

        protected MasterAudioBusVolumeConnection _connection;

        public override IConnection<float> GetConnection()
        {
            if(_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new MasterAudioBusVolumeConnection(InputRange, BusName);
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
