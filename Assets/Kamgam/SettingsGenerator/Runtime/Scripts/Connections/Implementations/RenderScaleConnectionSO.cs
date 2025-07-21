using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "RenderScaleConnection", menuName = "SettingsGenerator/Connection/RenderScaleConnection", order = 4)]
    public class RenderScaleConnectionSO : FloatConnectionSO
    {
        /// <summary>
        /// Should the render scale be set again if the quality level has been changed? Usually no since
        /// most times higher quality level also means higher render scale.
        /// </summary>
        public bool ReapplyOnQualityChange = false;

        /// <summary>
        /// The default render scale that should be used if no render asset has been found.
        /// </summary>
        public float DefaultRenderScale = 1f;

        protected RenderScaleConnection _connection;

        public override IConnection<float> GetConnection()
        {
            if(_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new RenderScaleConnection();
            _connection.ReapplyOnQualityChange = ReapplyOnQualityChange;
            _connection.DefaultRenderScale = DefaultRenderScale;
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
