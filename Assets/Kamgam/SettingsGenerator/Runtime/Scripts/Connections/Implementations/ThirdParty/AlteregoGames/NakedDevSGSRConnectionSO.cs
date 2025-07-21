using UnityEngine;
using UnityEngine.Audio;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "NakedDevSGSRConnection", menuName = "SettingsGenerator/Connection/NakedDevSGSRConnection", order = 4)]
    public class NakedDevSGSRConnectionSO : OptionConnectionSO
    {
        protected NakedDevSGSRConnection _connection;

        public override IConnectionWithOptions<string> GetConnection()
        {
            if(_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new NakedDevSGSRConnection(); 
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
