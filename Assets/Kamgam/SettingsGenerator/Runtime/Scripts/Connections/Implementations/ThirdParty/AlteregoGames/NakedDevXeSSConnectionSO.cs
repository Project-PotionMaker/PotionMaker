using UnityEngine;
using UnityEngine.Audio;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "NakedDevXeSSConnection", menuName = "SettingsGenerator/Connection/NakedDevXeSSConnection", order = 4)]
    public class NakedDevXeSSConnectionSO : OptionConnectionSO
    {
        protected NakedDevXeSSConnection _connection;

        public override IConnectionWithOptions<string> GetConnection()
        {
            if(_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new NakedDevXeSSConnection(); 
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
