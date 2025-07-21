using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// This is a base class and is nothing but a marker for a Camera connection to get all the
    /// cameras that should be affected by that connection.
    /// </summary>
    public class CameraMarker<T> : MonoBehaviour where T : CameraMarker<T>
    {
        public static List<CameraMarker<T>> Markers = new List<CameraMarker<T>>();

        public static bool HasValidMarkers()
        {
            foreach (var marker in Markers)
            {
                if (marker.IsValid())
                    return true;
            }

            return false;
        }

        public static CameraMarker<T> GetFirstValidMarker()
        {
            foreach (var marker in Markers)
            {
                if (marker.IsValid())
                    return marker;
            }

            return null;
        }

        protected Camera _camera;
        public Camera Camera
        {
            get
            {
                if (_camera == null)
                {
                    _camera = this.GetComponent<Camera>();
                }
                return _camera;
            }
        }

        public void OnEnable()
        {
            // Exists only to ensure the enable checkbox is available in the editor
        }

        public void Awake()
        {
            Markers.Add(this);
        }

        public void OnDestroy()
        {
            Markers.Remove(this);
        }

        public bool IsValid()
        {
            return isActiveAndEnabled && gameObject != null && gameObject.activeInHierarchy && Camera != null;
        }
    }
}
