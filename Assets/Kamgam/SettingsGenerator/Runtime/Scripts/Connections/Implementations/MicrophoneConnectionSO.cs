using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "MicrophoneConnection", menuName = "SettingsGenerator/Connection/MicrophoneConnection", order = 4)]
    public class MicrophoneConnectionSO : OptionConnectionSO
    {
        [Tooltip("If > 0 then every # seconds the connection will check for new microphones and update the options list.")]
        public float PollIntervalInSec = -1f;
            
        protected MicrophoneConnection _connection;

        public override IConnectionWithOptions<string> GetConnection()
        {
            if(_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new MicrophoneConnection(PollIntervalInSec);
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
