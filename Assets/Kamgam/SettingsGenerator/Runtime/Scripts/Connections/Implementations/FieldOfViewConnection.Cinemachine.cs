#if KAMGAM_CINEMACHINE
#if UNITY_6000_0_OR_NEWER
using Unity.Cinemachine;
#else
using Cinemachine;
using CinemachineCamera = Cinemachine.CinemachineVirtualCamera;
#endif
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public partial class FieldOfViewConnection : Connection<float>
    {
        public bool trySetCinemachineValue(Camera camera, float value)
        {
            if (tryGetCinemachineCamera(camera, out var cinemaCamera))
            {
#if UNITY_6000_0_OR_NEWER
                cinemaCamera.Lens.FieldOfView = value;
#else
                cinemaCamera.m_Lens.FieldOfView = value;
#endif
                return true;
            }

            return false;
        }

        public bool tryGetCinemachineValue(Camera camera, out float fieldOfView)
        {
            if (tryGetCinemachineCamera(camera, out var cinemaCamera))
            {
#if UNITY_6000_0_OR_NEWER
                fieldOfView = cinemaCamera.Lens.FieldOfView;
#else
                fieldOfView = cinemaCamera.m_Lens.FieldOfView;
#endif
                return true;
            }
            else
            {
                fieldOfView = DefaultFallback;
                return false;
            }

            
        }

        private static bool tryGetCinemachineCamera(Camera camera, out CinemachineCamera cinemaCamera)
        {
            if (camera == null)
            {
                cinemaCamera = null;
                return false;
            }

            if (camera.gameObject.TryGetComponent<CinemachineBrain>(out var brain))
            {
                cinemaCamera = brain.ActiveVirtualCamera as CinemachineCamera;
                return cinemaCamera != null;
            }

            cinemaCamera = null;
            return false;
        }
    }
}
#endif