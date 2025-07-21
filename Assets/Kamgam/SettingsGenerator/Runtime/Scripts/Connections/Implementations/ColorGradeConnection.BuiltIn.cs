// If it's neither URP nor HDRP then it is the old BuiltIn renderer
// If both HDRP and URP are set then we also assume BuiltIn until this ambiguity is resolved by the AssemblyDefinitionUpdater
#if (!KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP) || (KAMGAM_RENDER_PIPELINE_URP && KAMGAM_RENDER_PIPELINE_HDRP)

// Is the PostProcessing stack installed?
#if KAMGAM_POST_PRO_BUILTIN

using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Kamgam.SettingsGenerator
{
    // The post processing effects are camera based. Thus we need to keep track of active cameras.

    public partial class ColorGradeConnection : Connection<float>
    {

        public ColorGradeConnection(ColorGradeEffect effect = ColorGradeEffect.Gamma)
        {
            _effect = effect;

            CameraDetector.Instance.OnNewCameraFound += onNewCameraFound;

            // Log Warnings for effects that are NOT supported in Built-In
            if (   _effect == ColorGradeEffect.SMHShadows
                || _effect == ColorGradeEffect.SMHMidtones
                || _effect == ColorGradeEffect.SMHHighlights)
            {
                Logger.LogWarning("The '" + _effect.ToString() + "' color grading effect is not supported in the BuiltIn render pipeline.");
            }
        }

        protected void onNewCameraFound(Camera cam)
        {
            setOnCamera(cam, _effect, lastKnownValue);
        }

        public override float Get()
        {
            var cam = Camera.main;
            if (cam == null)
                return 0f;

            var volume = cam.GetComponentInChildren<PostProcessVolume>();
            if (volume != null && volume.profile != null)
            {
                ColorGrading grading;
                volume.profile.TryGetSettings(out grading);
                if (grading != null)
                {
                    return getEffectValue(grading, _effect);
                }
            }

            return 0f;
        }

        public override void Set(float value)
        {
            bool found = false;

            var cameras = Camera.allCameras;
            foreach (var cam in cameras)
            {
                if (cam != null && cam.isActiveAndEnabled && cam.gameObject.activeInHierarchy)
                {
                    found |= setOnCamera(cam, _effect, value);
                }
            }

            if (!found)
            {
#if UNITY_EDITOR
                var effectName = typeof(ColorGrading).Name;
                var name = this.GetType().Name;
                Logger.LogWarning(name + ": There was no '" + effectName + "' PostPro (BuiltIn) effect found. " +
                    "Please add a PostPro Volume with a profile containing '" + effectName + "' to your camera, make sure it is active and the layers do match and, if the volume is not global, add a trigger collider.\n\n" +
                    "Find out more here: https://docs.unity3d.com/Packages/com.unity.postprocessing@3.0/manual/Quick-start.html");
#endif
            }

            NotifyListenersIfChanged(value);
        }

        /// <summary>
        /// Returns true if the setting could be set.<br />
        /// Returns false if cam is null or if there is no PostPro volume with AO on it.
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="enable"></param>
        /// <returns></returns>
        private static bool setOnCamera(Camera cam, ColorGradeEffect effect, float value)
        {
            if (cam == null)
                return false;

            var volume = cam.GetComponentInChildren<PostProcessVolume>();
            if (volume != null && volume.profile != null)
            {
                ColorGrading grading;
                volume.profile.TryGetSettings(out grading);
                if (grading != null)
                {
                    setEffetValue(grading, effect, value);
                    return true;
                }
            }

            return false;
        }

        private static float getEffectValue(ColorGrading grading, ColorGradeEffect effect)
        {
            var param = getOverrideParameter(grading, effect);
            if (param == null)
                return 0f;

            float value = 0f;
                
            if (param is FloatParameter)
            {
                value = param.GetValue<float>();
            }
            else
            {
                // gain, gamma, lift
                if (param is Vector4Parameter)
                    value = param.GetValue<Vector4>().w;
            }

            return value;
        }

        private static void setEffetValue(ColorGrading grading, ColorGradeEffect effect, float value)
        {
            // Enable
            grading.active = true;
            grading.enabled.Override(true);
            grading.enabled.value = true;

            var param = getOverrideParameter(grading, effect);
            if (param == null)
                return;

            var floatParam = param as FloatParameter;
            if (floatParam != null)
            {
                floatParam.Override(value);
            }
            else
            {
                // gain, gamma, lift
                var v4Param = param as Vector4Parameter;
                if (v4Param != null)
                {
                    var tmp = param.GetValue<Vector4>();
                    tmp.w = value;
                    v4Param.Override(tmp);
                }
            }
        }

        private static ParameterOverride getOverrideParameter(ColorGrading grading, ColorGradeEffect effect)
        {
            switch (effect)
            {
                case ColorGradeEffect.Brightness:
                    return grading.brightness;
                    
                case ColorGradeEffect.Contrast:
                    return grading.contrast;

                case ColorGradeEffect.Gain:
                    return grading.gain;

                case ColorGradeEffect.Gamma:
                    return grading.gamma;

                case ColorGradeEffect.HueShift:
                    return grading.hueShift;

                case ColorGradeEffect.LdrLutContribution:
                    return grading.ldrLutContribution;

                case ColorGradeEffect.Lift:
                    return grading.lift;

                case ColorGradeEffect.MixerBlueOutBlueIn:
                    return grading.mixerBlueOutBlueIn;

                case ColorGradeEffect.MixerBlueOutGreenIn:
                    return grading.mixerBlueOutGreenIn;

                case ColorGradeEffect.MixerBlueOutRedIn:
                    return grading.mixerBlueOutRedIn;

                case ColorGradeEffect.MixerGreenOutBlueIn:
                    return grading.mixerGreenOutBlueIn;

                case ColorGradeEffect.MixerGreenOutGreenIn:
                    return grading.mixerGreenOutGreenIn;

                case ColorGradeEffect.MixerGreenOutRedIn:
                    return grading.mixerGreenOutRedIn;

                case ColorGradeEffect.MixerRedOutBlueIn:
                    return grading.mixerRedOutBlueIn;

                case ColorGradeEffect.MixerRedOutGreenIn:
                    return grading.mixerRedOutGreenIn;

                case ColorGradeEffect.MixerRedOutRedIn:
                    return grading.mixerRedOutRedIn;

                case ColorGradeEffect.PostExposure:
                    return grading.postExposure;

                case ColorGradeEffect.Saturation:
                    return grading.saturation;

                case ColorGradeEffect.Temperature:
                    return grading.temperature;

                case ColorGradeEffect.Tint:
                    return grading.tint;

                case ColorGradeEffect.ToneCurveGamma:
                    return grading.toneCurveGamma;

                case ColorGradeEffect.ToneCurveShoulderAngle:
                    return grading.toneCurveShoulderAngle;

                case ColorGradeEffect.ToneCurveShoulderLength:
                    return grading.toneCurveShoulderLength;

                case ColorGradeEffect.ToneCurveShoulderStrength:
                    return grading.toneCurveShoulderStrength;

                case ColorGradeEffect.ToneCurveToeLength:
                    return grading.toneCurveToeLength;

                case ColorGradeEffect.ToneCurveToeStrength:
                    return grading.toneCurveToeStrength;

                default:
#if UNITY_EDITOR
                    Logger.Log("Unknown Color Grade Effect in Built-In RP: " + effect);
#endif
                    return null;
            }
        }
    }
}

#else

            // Fallback if no PostProcessing stack is installed.

            using UnityEngine;
namespace Kamgam.SettingsGenerator
{
    public partial class ColorGradeConnection : Connection<float>
    {
        public ColorGradeConnection(ColorGradeEffect effect = ColorGradeEffect.Gamma)
        {
        }

        public override float Get()
        {
            return 0f;
        }

        public override void Set(float gamma)
        {
#if UNITY_EDITOR
            var name = this.GetType().Name;
            Logger.LogWarning(
                name + " (BuiltIn): There is no PostProcessing stack installed. This will do nothing.\n" +
                "Here is how to install it: https://docs.unity3d.com/Packages/com.unity.postprocessing@3.0/manual/Installation.html"
                );
#endif
        }
    }
}

#endif

#endif