#if KAMGAM_RENDER_PIPELINE_URP && !KAMGAM_RENDER_PIPELINE_HDRP

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kamgam.SettingsGenerator
{
    public partial class VolumetricsEnabledConnection
    {
        public override bool Get()
        {
            Logger.LogWarning("Volumetrics no supported in URP.");
            return true;
        }

        public override void Set(bool enable)
        {
            Logger.LogWarning("Volumetrics no supported in URP.");
        }
    }
}

#endif