using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    // We need this class file because Unity requires one file to be named exactly
    // like the class it contains.

    // See .BuiltIn, .URP or .HDRP for the specific implementations.

    public partial class ColorGradeConnection
    {
        public enum ColorGradeEffect
        {
            // There are a lot of defines here because the enum is used across all RPs.
            // Not all values are supported in all RPs.
            // [na] = not available in this render pipeline.

#if KAMGAM_RENDER_PIPELINE_URP && !KAMGAM_RENDER_PIPELINE_HDRP
            [InspectorName("[na] Brigthness")]
#endif
            Brightness = 1, // Not supported in URP but present in Built-In

            // Lift Gamma Gain
            [InspectorName("Gain   (-1 to 1)")]
            Gain = 3,  // (uses W)
            [InspectorName("Gamma   (-1 to 1)")]
            Gamma = 4, // (uses W)
            [InspectorName("Lift   (-1 to 1)")]
            Lift = 5,  // (uses W)

            // Color Adjustments
            /// <summary>
            /// In Built-In this only has an effect if the color grading MODE is set to HDR (that's not the default).
            /// </summary>
            [InspectorName("PostExposure   (-10 to 10)")]
            PostExposure = 17,
            [InspectorName("Saturation   (-100 to 100)")]
            Saturation = 18,
            [InspectorName("[na] ColorFilter")]
            ColorFilter = 27, // Not supported in URP but present in Built-In
            [InspectorName("Contrast   (-100 to 100 or more)")]
            Contrast = 2,
            [InspectorName("HueShift   (-175 to 175)")]
            HueShift = 6,

#if KAMGAM_RENDER_PIPELINE_URP && !KAMGAM_RENDER_PIPELINE_HDRP
            [InspectorName("[na] LdrLutContribution")]
#endif
            LdrLutContribution = 7,

            // Channel Mixer
            [InspectorName("MixerBlueOutBlueIn   (-200 to 200)")]
            MixerBlueOutBlueIn = 8,
            MixerBlueOutGreenIn = 9,
            MixerBlueOutRedIn = 10,

            MixerGreenOutBlueIn = 11,
            MixerGreenOutGreenIn = 12,
            MixerGreenOutRedIn = 13,

            MixerRedOutBlueIn = 14,
            MixerRedOutGreenIn = 15,
            MixerRedOutRedIn = 16,

            // White Balance
            [InspectorName("Temperature   (-100 to 100)")]
            Temperature = 19,
            [InspectorName("Tint   (-100 to 100)")]
            Tint = 20,

            // Not supported in URP but present in Built-In
#if KAMGAM_RENDER_PIPELINE_URP && !KAMGAM_RENDER_PIPELINE_HDRP
            [InspectorName("[na] ToneCurveGamma")]
#endif
            ToneCurveGamma = 21,

#if KAMGAM_RENDER_PIPELINE_URP && !KAMGAM_RENDER_PIPELINE_HDRP
            [InspectorName("[na] ToneCurveShoulderAngle")]
#endif
            ToneCurveShoulderAngle = 22,

#if KAMGAM_RENDER_PIPELINE_URP && !KAMGAM_RENDER_PIPELINE_HDRP
            [InspectorName("[na] ToneCurveShoulderLength")]
#endif
            ToneCurveShoulderLength = 23,

#if KAMGAM_RENDER_PIPELINE_URP && !KAMGAM_RENDER_PIPELINE_HDRP
            [InspectorName("[na] ToneCurveShoulderStrength")]
#endif
            ToneCurveShoulderStrength = 24,

#if KAMGAM_RENDER_PIPELINE_URP && !KAMGAM_RENDER_PIPELINE_HDRP
            [InspectorName("[na] ToneCurveToeLength")]
#endif
            ToneCurveToeLength = 25,

#if KAMGAM_RENDER_PIPELINE_URP && !KAMGAM_RENDER_PIPELINE_HDRP
            [InspectorName("[na] ToneCurveToeStrength")]
#endif
            ToneCurveToeStrength = 26,

            // Shadows Midtones Highlights
            [InspectorName("SMH Shadows   (-1 to 1)")]
            SMHShadows = 28,   // (uses W)
            [InspectorName("SMH Midtones   (-1 to 1)")]
            SMHMidtones = 29,  // (uses W)
            [InspectorName("SMH Highlights   (-1 to 1)")]
            SMHHighlights = 30 // (uses W)
        }

        protected ColorGradeEffect _effect;
        public ColorGradeEffect Effect => _effect;
    }
}
