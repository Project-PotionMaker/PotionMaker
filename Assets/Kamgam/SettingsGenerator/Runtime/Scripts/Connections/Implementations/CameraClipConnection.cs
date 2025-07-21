using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public class CameraClipConnection : Connection<float>
    {
        public const float DefaultFallbackNear = 0.3f;
        public const float DefaultFallbackFar = 1000f;

        public bool UseMain;
        public bool UseMarkers;

        public enum ClippingMode { Near, Far };
        public ClippingMode Mode;
        public float ClipMin;
        public float ClipMax;

        // Cache for the fov value in case no camera exists yet.
        [System.NonSerialized]
        protected float _clipValue;

        public CameraClipConnection(ClippingMode mode = ClippingMode.Far, float clipMin = 1f, float clipMax = 1000f, bool useMain = true, bool useMarkers = true)
        {
            Mode = mode;
            ClipMin = clipMin;
            ClipMax = clipMax;

            // Init default
            if (mode == ClippingMode.Near)
                _clipValue = DefaultFallbackNear;
            else
                _clipValue = DefaultFallbackFar;

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
            Set(_clipValue);
        }

        public override float Get()
        {
            if (UseMain && Camera.main != null)
            {
                return getClipValue(Camera.main);
            }
            
            if (UseMarkers)
            {
                var marker = FieldOfViewMarker.GetFirstValidMarker();
                if(marker != null)
                {
                    return getClipValue(marker.Camera);
                }
            }

            return _clipValue;
        }

        public override void Set(float value)
        {
            _clipValue = value;

            if (UseMain && Camera.main != null)
            {
                setClipValue(Camera.main, value);
            }
            else if (UseMarkers)
            {
                foreach (var marker in CameraClipMarker.Markers)
                {
                    if (marker.IsValid())
                    {
                        setClipValue(marker.Camera, value);
                    }
                }
            }
        }

        public float getClipValue(Camera cam)
        {
            if (Mode == ClippingMode.Far)
            {
                return cam.farClipPlane;
            }
            else
            {
                return cam.nearClipPlane;
            }
        }

        public void setClipValue(Camera cam, float value)
        {
            if (Mode == ClippingMode.Far)
            {
                if (cam.farClipPlane > cam.nearClipPlane)
                    cam.farClipPlane = value;
                else
                    Logger.LogWarning("CameraCipConnection: You can not set the far clipping distance lower than the near clipping distance!");
            }
            else
            {
                if (cam.nearClipPlane < cam.farClipPlane)
                    cam.nearClipPlane = value;
                else
                    Logger.LogWarning("CameraCipConnection: You can not set the near clipping distance higher than the far clipping distance!");
            }
        }
    }
}
