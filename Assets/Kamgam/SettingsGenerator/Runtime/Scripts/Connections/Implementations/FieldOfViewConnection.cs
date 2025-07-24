using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public partial class FieldOfViewConnection : Connection<float>
    {
        public const float DefaultFallback = 60f;

        public bool UseMain;
        public bool UseMarkers;

        // Cache for the fov value in case no camera exists yet.
        [System.NonSerialized]
        protected float _fieldOfView = DefaultFallback;

        public FieldOfViewConnection(bool useMain = true, bool useMarkers = true)
        {
            UseMain = useMain;
            UseMarkers = useMarkers;

            CameraDetector.Instance.OnNewCameraFound += onNewCamera;
        }

        protected void onNewCamera(Camera cam)
        {
            Apply();
        }

        public void Apply()
        {
            Set(_fieldOfView);
        }

        public override float Get()
        {
            if (UseMain && Camera.main != null)
            {
#if KAMGAM_CINEMACHINE
                if (tryGetCinemachineValue(Camera.main, out var fov))
                {
                    return fov;
                }
                else
                {
#endif
                return Camera.main.fieldOfView;
#if KAMGAM_CINEMACHINE
                }
#endif
            }
            
            if (UseMarkers)
            {
                var marker = FieldOfViewMarker.GetFirstValidMarker();
                if(marker != null)
                {
#if KAMGAM_CINEMACHINE
                    if (tryGetCinemachineValue(marker.Camera, out var fov))
                    {
                        return fov;
                    }
                    else
                    {
#endif
                    return marker.Camera.fieldOfView;
#if KAMGAM_CINEMACHINE
                    }
#endif
                }
            }

            return _fieldOfView;
        }

        public override void Set(float fieldOfView)
        {
            _fieldOfView = fieldOfView;

            if (UseMain && Camera.main != null)
            {
#if KAMGAM_CINEMACHINE
                if (!trySetCinemachineValue(Camera.main, fieldOfView))
                {
#endif
                    Camera.main.fieldOfView = fieldOfView;
#if KAMGAM_CINEMACHINE
                }
#endif
            }
            else if (UseMarkers)
            {
                foreach (var marker in FieldOfViewMarker.Markers)
                {
                    if (marker.IsValid())
                    {
#if KAMGAM_CINEMACHINE
                        if (!trySetCinemachineValue(marker.Camera, fieldOfView))
                        {
#endif
                            marker.Camera.fieldOfView = fieldOfView;
#if KAMGAM_CINEMACHINE
                        }
#endif
                    }
                }
            }
        }
    }
}
