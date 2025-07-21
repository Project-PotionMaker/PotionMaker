using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "MSAAConnection", menuName = "SettingsGenerator/Connection/MSAAConnection", order = 4)]
    public class MSAAConnectionSO : OptionConnectionSO
    {
        protected MSAAConnection _connection;

        public override IConnectionWithOptions<string> GetConnection()
        {
            if (_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new MSAAConnection();
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
