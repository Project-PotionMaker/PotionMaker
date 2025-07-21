using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "VolumetricsEnabledConnection", menuName = "SettingsGenerator/Connection/VolumetricsEnabledConnection", order = 4)]
    public class VolumetricsEnabledConnectionSO : BoolConnectionSO
    {
        protected VolumetricsEnabledConnection _connection;

        public override IConnection<bool> GetConnection()
        {
            if(_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new VolumetricsEnabledConnection();
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
