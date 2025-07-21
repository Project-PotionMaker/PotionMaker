using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "ColorGradeConnection", menuName = "SettingsGenerator/Connection/ColorGradeConnection", order = 4)]
    public class ColorGradeConnectionSO : FloatConnectionSO
    {
        public ColorGradeConnection.ColorGradeEffect Effect = ColorGradeConnection.ColorGradeEffect.Gamma;

        protected ColorGradeConnection _connection;

        public override IConnection<float> GetConnection()
        {
            if(_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new ColorGradeConnection(Effect);
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
