#if KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System.Reflection;

namespace Kamgam.SettingsGenerator
{
    // Render Asset based setting. This changes values in the render asset associated
    // with the current quality level. This means we need some extra code to revert
    // these changes in the Editor (see "Editor RenderPipelineAsset Revert" region below).

    // NOTICE: With HDRP you can toggle things on/off from either
    // a) the camera
    // b) a Volume
    // c) or FrameSettings
    // Or you can swap the whole HDRP asset based on the quality.
    // The reasoning of Unity behind this is that you either override the settings outside
    // the assets or your constrain yourself to a set of predefined settings. This enables
    // them to do some shader code stripping etc. at compile time. Which is good. BUT this
    // also prevents us from implementing granular graphics settings for Pc users. If you use
    // this connection you are at the risk of getting errors because this directly changes
    // values in the render settings assets at runtime (something not officially supported).
    // Why? Because I have not yet found a way to override the msaa sampling count via the
    // methods mentionend above (Volumes, FrameSettings, Camera overrides).
    // Here is Unitys stance on this: https://discussions.unity.com/t/can-any-all-hdrenderpipelineasset-values-be-changed-at-runtime/737980/6
    //
    // To enable MSAA we have to enable it via FrameSettings overrides on the camera and
    // in the render settings (which is also where we control the strength). That's why this
    // class uses two mechanisms (FrameSettings on Camera & RenderPipelineSettings).


    public partial class MSAAConnection
    {
        protected List<string> _labels;

        public MSAAConnection()
        {
            CameraDetector.Instance.OnNewCameraFound += onNewCameraFound;
        }

        protected void onNewCameraFound(Camera cam)
        {
            setOnCamera(cam, lastKnownValue);
        }

        private static void setOnCamera(Camera cam, int index)
        {
            // Getting components
            var cameraData = cam.GetComponent<HDAdditionalCameraData>();
            if (cameraData == null)
                return;

            var frameSettings = cameraData.renderingPathCustomFrameSettings;
            var frameSettingsOverrideMask = cameraData.renderingPathCustomFrameSettingsOverrideMask;

            // Make sure Custom Frame Settings are enabled in the camera
            cameraData.customRenderingSettings = true;

            // Enabling MSAA override
            frameSettingsOverrideMask.mask[(uint)FrameSettingsField.MSAA] = true;

            // Enable or disable MSAA on the camera
            if (index == 0)
                frameSettings.SetEnabled(FrameSettingsField.MSAA, true); // Disabled
            else
                frameSettings.SetEnabled(FrameSettingsField.MSAA, false); // 2x, 4x, ...

            // Apply the frame setting mask back to the camera
            cameraData.renderingPathCustomFrameSettingsOverrideMask = frameSettingsOverrideMask;
        }

        #region reflections
        private static FieldInfo _renderPipelineSettingsFieldInfo;

        private static void cacheRenderPipelineSettingsFieldInfo()
        {
            if (_renderPipelineSettingsFieldInfo == null)
            {
                var type = typeof(HDRenderPipelineAsset);
                _renderPipelineSettingsFieldInfo = type.GetField(
                    "m_RenderPipelineSettings",
                    BindingFlags.Instance | BindingFlags.Default | BindingFlags.Public | BindingFlags.NonPublic
                );
            }
        }

        private static void setRenderPipelineSettings(HDRenderPipelineAsset asset, RenderPipelineSettings settings)
        {
            cacheRenderPipelineSettingsFieldInfo();

            if (_renderPipelineSettingsFieldInfo != null)
            {
                _renderPipelineSettingsFieldInfo.SetValue(asset, settings);
            }
        }
        #endregion

        public override List<string> GetOptionLabels()
        {
            if (_labels == null)
            {
                _labels = new List<string>();
                _labels.Add("Disabled");
                _labels.Add("2x");
                _labels.Add("4x");
                _labels.Add("8x");
            }

            return _labels;
        }

        public override void SetOptionLabels(List<string> optionLabels)
        {
            if (optionLabels == null || optionLabels.Count != 4)
            {
                Debug.LogError("Invalid new labels. Need to be four (disabled, 2, 4, 8).");
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
        /// Returns 0 (aka "disabled") if no data was found or if msaa is disabled.
        /// </summary>
        /// <returns></returns>
        public override int Get()
        {
            var rpAsset = GraphicsSettings.currentRenderPipeline as HDRenderPipelineAsset;

            if (rpAsset == null)
                return 0;

            if (rpAsset.currentPlatformRenderPipelineSettings.supportedLitShaderMode == RenderPipelineSettings.SupportedLitShaderMode.DeferredOnly)
            {
                Logger.LogWarning("Deferred lit mode does NOT support MSAA, see: https://docs.unity3d.com/Packages/com.unity.render-pipelines.high-definition@17.0/manual/Anti-Aliasing.html");
                return 0;
            }

            if (rpAsset.currentPlatformRenderPipelineSettings.msaaSampleCount == MSAASamples.None)
                return 0;
            else if (rpAsset.currentPlatformRenderPipelineSettings.msaaSampleCount == MSAASamples.MSAA2x)
                return 1;
            else if (rpAsset.currentPlatformRenderPipelineSettings.msaaSampleCount == MSAASamples.MSAA4x)
                return 2;
            else if (rpAsset.currentPlatformRenderPipelineSettings.msaaSampleCount == MSAASamples.MSAA8x)
                return 3;

            return 0;
        }

        public override void Set(int index)
        {
            // Set in render settings
            var rpAsset = GraphicsSettings.currentRenderPipeline as HDRenderPipelineAsset;

            if (rpAsset == null)
                return;

            if (rpAsset.currentPlatformRenderPipelineSettings.supportedLitShaderMode == RenderPipelineSettings.SupportedLitShaderMode.DeferredOnly)
            {
                Logger.LogWarning("Deferred lit mode does NOT support MSAA, see: https://docs.unity3d.com/Packages/com.unity.render-pipelines.high-definition@17.0/manual/Anti-Aliasing.html");
                return;
            }

            var settings = rpAsset.currentPlatformRenderPipelineSettings;

            if (index <= 0)
                settings.msaaSampleCount = MSAASamples.None;
            else if (index == 1)
                settings.msaaSampleCount = MSAASamples.MSAA2x;
            else if (index == 2)
                settings.msaaSampleCount = MSAASamples.MSAA4x;
            else if (index >= 3)
                settings.msaaSampleCount = MSAASamples.MSAA8x;

            setRenderPipelineSettings(rpAsset, settings);

            // Set on cameras
            var cameras = Camera.allCameras;
            foreach (var cam in cameras)
            {
                if (cam.gameObject.activeInHierarchy && cam.isActiveAndEnabled)
                {
                    setOnCamera(cam, index);
                }
            }


            // Notify listeners
            NotifyListenersIfChanged(index);
        }

        // Changes to Assets in the Editor are persistent, thus we have to
        // to revert them when leaving play mode.
        #region Editor RenderPipelineAsset Revert

#if UNITY_EDITOR
        // One value per asset (low, mid high, ...)
        protected static Dictionary<RenderPipelineAsset, MSAASamples> backupValues;

        [UnityEditor.InitializeOnLoadMethod]
        protected static void initAfterDomainReload()
        {
            RenderPipelineRestoreEditorUtils.InitAfterDomainReload<HDRenderPipelineAsset, MSAASamples>(
                ref backupValues,
                (rpAsset) => rpAsset.currentPlatformRenderPipelineSettings.msaaSampleCount,
                onPlayModeExit
            );
        }

        protected static void onPlayModeExit(UnityEditor.PlayModeStateChange state)
        {
            RenderPipelineRestoreEditorUtils.OnPlayModeExit<HDRenderPipelineAsset, MSAASamples>(
                state,
                backupValues,
                (rpAsset, value) => {
                    var settings = rpAsset.currentPlatformRenderPipelineSettings;
                    settings.msaaSampleCount = value;
                    setRenderPipelineSettings(rpAsset, settings);
                }
            );
        }
#endif

        #endregion

    }
}

#endif
