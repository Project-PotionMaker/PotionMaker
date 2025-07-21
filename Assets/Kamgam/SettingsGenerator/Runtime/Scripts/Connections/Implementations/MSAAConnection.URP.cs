#if KAMGAM_RENDER_PIPELINE_URP && !KAMGAM_RENDER_PIPELINE_HDRP

using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kamgam.SettingsGenerator
{
    // Render Asset based setting. This changes values in the render asset associated
    // with the current quality level. This means we need some extra code to revert
    // these changes in the Editor (see "Editor RenderPipelineAsset Revert" region below).

    public partial class MSAAConnection
    {
        protected List<string> _labels;

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
            var rpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;

            if (rpAsset == null)
                return 0;

            if (rpAsset.msaaSampleCount <= 1)
                return 0;
            else if (rpAsset.msaaSampleCount == 2)
                return 1;
            if (rpAsset.msaaSampleCount == 4)
                return 2;
            if (rpAsset.msaaSampleCount >= 8)
                return 3;

            return 0;
        }

        public override void Set(int index)
        {
            var rpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;

            if (rpAsset == null)
                return;

            if (index <= 0)
                rpAsset.msaaSampleCount = (int)MsaaQuality.Disabled;
            else if (index == 1)
                rpAsset.msaaSampleCount = (int)MsaaQuality._2x;
            else if (index == 2)
                rpAsset.msaaSampleCount = (int)MsaaQuality._4x;
            else if (index >= 3)
                rpAsset.msaaSampleCount = (int)MsaaQuality._8x;

            NotifyListenersIfChanged(index);
        }

        // Changes to Assets in the Editor are persistent, thus we have to
        // to revert them when leaving play mode.
#region Editor RenderPipelineAsset Revert

#if UNITY_EDITOR
        // One value per asset (low, mid high, ...)
        protected static Dictionary<RenderPipelineAsset, int> backupValues;

        [InitializeOnLoadMethod]
        protected static void initAfterDomainReload()
        {
            RenderPipelineRestoreEditorUtils.InitAfterDomainReload<UniversalRenderPipelineAsset, int>(
                ref backupValues,
                (rpAsset) => rpAsset.msaaSampleCount,
                onPlayModeExit
            );
        }

        protected static void onPlayModeExit(PlayModeStateChange state)
        {
            RenderPipelineRestoreEditorUtils.OnPlayModeExit<UniversalRenderPipelineAsset, int>(
                state,
                backupValues,
                (rpAsset, value) => rpAsset.msaaSampleCount = value
            );
        }
#endif

#endregion

    }
}

#endif