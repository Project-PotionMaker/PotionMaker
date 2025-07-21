using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "CameraClipConnection", menuName = "SettingsGenerator/Connection/CameraClipConnection", order = 4)]
    public class CameraClipConnectionSO : FloatConnectionSO
    {
        public CameraClipConnection.ClippingMode Mode = CameraClipConnection.ClippingMode.Far;
        public float ClipMin = 1f;
        public float ClipMax = 1000f;

        public bool UseMain = true;
        public bool UseMarkers = true;

        protected CameraClipConnection _connection;

        public override IConnection<float> GetConnection()
        {
            if(_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new CameraClipConnection(Mode, ClipMin, ClipMax, UseMain, UseMarkers);
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
