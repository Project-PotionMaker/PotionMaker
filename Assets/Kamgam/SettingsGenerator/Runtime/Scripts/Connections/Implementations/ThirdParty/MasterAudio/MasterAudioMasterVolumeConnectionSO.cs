using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "MasterAudioMasterVolumeConnection", menuName = "SettingsGenerator/Connection/MasterAudio/MasterVolumeConnection", order = 4)]
    public class MasterAudioMasterVolumeConnectionSO : FloatConnectionSO
    {
        [Tooltip("How the input should be mapped to 0f..1f.\n" +
                 "Useful if you have a range in percent (from 0 to 100) but need output ranging from 0f to 1f.")]
        public Vector2 InputRange = new Vector2(0f, 100f);

        protected MasterAudioMasterVolumeConnection _connection;

        public override IConnection<float> GetConnection()
        {
            if(_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new MasterAudioMasterVolumeConnection(InputRange);
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
