#if KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Kamgam.SettingsGenerator
{
    // This setting is camera based. Thus we need to keep track of active cameras.

    public partial class AntiAliasingConnection : ConnectionWithOptions<string>
    {
        protected MSAAConnection _msaaConnection;

        public MSAAConnection MsaaConnection
        {
            get
            {
                if (_msaaConnection == null)
                    _msaaConnection = new MSAAConnection();
                return _msaaConnection;
            }
        }
        
        protected List<string> _labels;

        public AntiAliasingConnection()
        {
            CameraDetector.Instance.OnNewCameraFound += onNewCameraFound;
        }

        protected void onNewCameraFound(Camera cam)
        {
            setOnCamera(cam, lastKnownValue);
        }

        public override List<string> GetOptionLabels()
        {
            if (_labels == null)
            {
                _labels = new List<string>();
                _labels.Add("Disabled");
                _labels.Add("FAA");
                _labels.Add("SMAA");
                _labels.Add("TAA");
                if (IncludeMSAA)
                {
                    _labels.Add("MSAA 2x");
                    _labels.Add("MSAA 4x");
                    _labels.Add("MSAA 8x");
                }
            }
            return _labels;
        }

        public override void SetOptionLabels(List<string> optionLabels)
        {
            if (optionLabels == null || optionLabels.Count != 4)
            {
                Debug.LogError("Invalid new labels. Need to be four.");
                return;
            }

            _labels = optionLabels;
        }

        public override void RefreshOptionLabels()
        {
            _labels = null;
            GetOptionLabels();
        }

        /// <summary>
        /// Returns 0 if no camera is active.
        /// </summary>
        /// <returns></returns>
        public override int Get()
        {
            if (Camera.main == null)
                return 0;

            // Fetch from current camera
            var settings = Camera.main.GetComponent<HDAdditionalCameraData>();
            if (settings == null)
                return 0;

            // MSAA 
            if (IncludeMSAA)
            {
                // <= 0 is disabled, 1 is 2x, 2 is 4x, 3 is 8x.
                int msaaIndex = MsaaConnection.Get();
                if (msaaIndex > 0)
                {
                    return 3 + msaaIndex;
                }
            }

            // Others
            switch (settings.antialiasing)
            {
                case HDAdditionalCameraData.AntialiasingMode.None:
                    return 0;
                case HDAdditionalCameraData.AntialiasingMode.FastApproximateAntialiasing:
                    return 1;
                case HDAdditionalCameraData.AntialiasingMode.SubpixelMorphologicalAntiAliasing:
                    return 2;
                case HDAdditionalCameraData.AntialiasingMode.TemporalAntialiasing:
                    return 3;
                default:
                    return 0;
            }
        }

        public override void Set(int index)
        {
            var cameras = Camera.allCameras;
            foreach (var cam in cameras)
            {
                if (cam.gameObject.activeInHierarchy && cam.isActiveAndEnabled)
                {
                    setOnCamera(cam, index);
                }
            }

            NotifyListenersIfChanged(index);
        }

        private void setOnCamera(Camera cam, int index)
        {
            if (LimitToMainCamera && cam != Camera.main)
                return;

            var settings = cam.GetComponent<HDAdditionalCameraData>();
            if (settings == null)
                return;

            if (index == 0)
                settings.antialiasing = HDAdditionalCameraData.AntialiasingMode.None;
            else if (index == 1)
                settings.antialiasing = HDAdditionalCameraData.AntialiasingMode.FastApproximateAntialiasing;
            else if (index == 2)
                settings.antialiasing = HDAdditionalCameraData.AntialiasingMode.SubpixelMorphologicalAntiAliasing;
            else if (index == 3)
                settings.antialiasing = HDAdditionalCameraData.AntialiasingMode.TemporalAntialiasing;

            // MSAA 
            if (IncludeMSAA)
            {
                int msaaIndex = index - 3;

                // <= 0 is disabled, 1 is 2x, 2 is 4x, 3 is 8x.
                msaaIndex = Mathf.Max(msaaIndex, 0);
                
                // Disable other AA is MSAA is used.
                if (msaaIndex > 0)
                {
                    settings.antialiasing = HDAdditionalCameraData.AntialiasingMode.None;
                }
                
                // Finally set MSAA
                MsaaConnection.Set(msaaIndex);
            }
        }
    }
}

#endif