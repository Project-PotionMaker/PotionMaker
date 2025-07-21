#if KAMGAM_RENDER_PIPELINE_URP && !KAMGAM_RENDER_PIPELINE_HDRP

using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Kamgam.SettingsGenerator
{
    public partial class ColorGradeConnection : Connection<float>
    {
        // Volume based setting. This means it will generate a volume game object
        // with a high priority to override the default settings made by other volumes.

        public static bool IsLiftGammaGain(ColorGradeEffect effect)
        {
            return effect == ColorGradeEffect.Lift
                || effect == ColorGradeEffect.Gamma
                || effect == ColorGradeEffect.Gain;
        }

        public static bool IsColorAdjustment(ColorGradeEffect effect)
        {
            return effect == ColorGradeEffect.PostExposure
                || effect == ColorGradeEffect.Saturation
             // || effect == ColorGradeEffect.ColorFilter
                || effect == ColorGradeEffect.Contrast
                || effect == ColorGradeEffect.HueShift;
        }

        public static bool IsChannelMixer(ColorGradeEffect effect)
        {
            return effect == ColorGradeEffect.MixerBlueOutBlueIn
                || effect == ColorGradeEffect.MixerBlueOutGreenIn
                || effect == ColorGradeEffect.MixerBlueOutRedIn
                || effect == ColorGradeEffect.MixerGreenOutBlueIn
                || effect == ColorGradeEffect.MixerGreenOutGreenIn
                || effect == ColorGradeEffect.MixerGreenOutRedIn
                || effect == ColorGradeEffect.MixerRedOutBlueIn
                || effect == ColorGradeEffect.MixerRedOutGreenIn
                || effect == ColorGradeEffect.MixerRedOutRedIn;
        }

        public static bool IsWhiteBalance(ColorGradeEffect effect)
        {
            return effect == ColorGradeEffect.Temperature
                || effect == ColorGradeEffect.Tint;
        }

        public static bool IsShadowsMidtonesHighlights(ColorGradeEffect effect)
        {
            return effect == ColorGradeEffect.SMHShadows
                || effect == ColorGradeEffect.SMHMidtones
                || effect == ColorGradeEffect.SMHHighlights;
        }

        protected LiftGammaGain _liftGammaGain;
        protected ColorAdjustments _colorAdjustment;
        protected ChannelMixer _channelMixer;
        protected WhiteBalance _whiteBalance;
        protected ShadowsMidtonesHighlights _shadowsMidtonesHighlights;

        public float _defaultValue = 0f;

        public ColorGradeConnection(ColorGradeEffect effect = ColorGradeEffect.Gamma)
        {
            // If this is called during EDIT Mode then the Instance will be null.
            if (SettingsVolume.Instance == null)
                return;

            _effect = effect;

            // Add the component to the settings volume to override all other.
            if (IsLiftGammaGain(_effect))
            {
                _liftGammaGain = SettingsVolume.Instance.GetOrAddComponent<LiftGammaGain>();
                _liftGammaGain.Override(_liftGammaGain, 1f);
                _liftGammaGain.active = false;
            }
            else if (IsColorAdjustment(_effect))
            {
                _colorAdjustment = SettingsVolume.Instance.GetOrAddComponent<ColorAdjustments>();
                _colorAdjustment.Override(_colorAdjustment, 1f);
                _colorAdjustment.active = false;
            }
            else if (IsChannelMixer(_effect))
            {
                _channelMixer = SettingsVolume.Instance.GetOrAddComponent<ChannelMixer>();
                // init default values (100 is the "no change" default for mixing the color itself.)
                _channelMixer.blueOutBlueIn.value = 100;
                _channelMixer.redOutRedIn.value = 100;
                _channelMixer.greenOutGreenIn.value = 100;
                _channelMixer.Override(_channelMixer, 1f);
                _channelMixer.active = false;
            }
            else if (IsWhiteBalance(_effect))
            {
                _whiteBalance = SettingsVolume.Instance.GetOrAddComponent<WhiteBalance>();
                _whiteBalance.Override(_whiteBalance, 1f);
                _whiteBalance.active = false;
            }
            else if (IsShadowsMidtonesHighlights(_effect))
            {
                _shadowsMidtonesHighlights = SettingsVolume.Instance.GetOrAddComponent<ShadowsMidtonesHighlights>();
                _shadowsMidtonesHighlights.Override(_shadowsMidtonesHighlights, 1f);
                _shadowsMidtonesHighlights.active = false;
            }
            else
            {
                Logger.LogWarning("The '" + _effect.ToString() + "' color grading effect is not supported in the universal render pipeline.");
            }

            UpdateDefaultValue();
        }

        /// <summary>
        /// Tries to find the default values based on all the existing volumes.<br />
        /// This value is used as base for the correction.
        /// </summary>
        public void UpdateDefaultValue()
        {
            // Warn if there is no default effect
            if (IsLiftGammaGain(_effect))
            {
                var defaultLiftGammaGain = SettingsVolume.Instance.FindDefaultVolumeComponent<LiftGammaGain>();

                if (defaultLiftGammaGain == null || defaultLiftGammaGain.active == false)
                {
                    _defaultValue = 0f;
                    return;
                }

                // Lift
                if (_effect == ColorGradeEffect.Lift)
                {
                    _defaultValue = defaultLiftGammaGain.lift.value.w;
                }
                // Gamma
                else if (_effect == ColorGradeEffect.Gamma)
                {
                    _defaultValue = defaultLiftGammaGain.gamma.value.w;
                }
                // Gain
                else if (_effect == ColorGradeEffect.Gain)
                {
                    _defaultValue = defaultLiftGammaGain.gain.value.w;
                }
            }
            else if (IsColorAdjustment(_effect))
            {
                var defaultColorAdjust = SettingsVolume.Instance.FindDefaultVolumeComponent<ColorAdjustments>();

                if (defaultColorAdjust == null || defaultColorAdjust.active == false)
                {
                    _defaultValue = 0f;
                    return;
                }

                // Post Exposure
                if (_effect == ColorGradeEffect.PostExposure && defaultColorAdjust.postExposure.overrideState)
                {
                    _defaultValue = defaultColorAdjust.postExposure.value;
                }
                // Saturation
                else if (_effect == ColorGradeEffect.Saturation && defaultColorAdjust.saturation.overrideState)
                {
                    _defaultValue = defaultColorAdjust.saturation.value;
                }
                // ColorFilter <- not supported
                // Contrast
                else if (_effect == ColorGradeEffect.Contrast && defaultColorAdjust.contrast.overrideState)
                {
                    _defaultValue = defaultColorAdjust.contrast.value;
                }
                // HueShift
                else if (_effect == ColorGradeEffect.HueShift && defaultColorAdjust.hueShift.overrideState)
                {
                    _defaultValue = defaultColorAdjust.hueShift.value;
                }
            }
            else if (IsChannelMixer(_effect))
            {
                var defaultChannelMixer = SettingsVolume.Instance.FindDefaultVolumeComponent<ChannelMixer>();

                if (defaultChannelMixer == null || defaultChannelMixer.active == false)
                {
                    if (_effect == ColorGradeEffect.MixerBlueOutBlueIn
                        || _effect == ColorGradeEffect.MixerGreenOutGreenIn
                        || _effect == ColorGradeEffect.MixerRedOutRedIn)
                    {
                        // 100 is the "no change" default for mixing the color itself.
                        _defaultValue = 100f;
                    }
                    else
                    {
                        _defaultValue = 0f;
                    }
                    return;
                }

                // Mixer BlueOutBlueIn
                if (_effect == ColorGradeEffect.MixerBlueOutBlueIn && defaultChannelMixer.blueOutBlueIn.overrideState)
                {
                    _defaultValue = defaultChannelMixer.blueOutBlueIn.value;
                }
                // Mixer BlueOutGreenIn
                else if (_effect == ColorGradeEffect.MixerBlueOutGreenIn && defaultChannelMixer.blueOutGreenIn.overrideState)
                {
                    _defaultValue = defaultChannelMixer.blueOutGreenIn.value;
                }
                // Mixer BlueOutRedIn
                else if (_effect == ColorGradeEffect.MixerBlueOutRedIn && defaultChannelMixer.blueOutRedIn.overrideState)
                {
                    _defaultValue = defaultChannelMixer.blueOutRedIn.value;
                }
                //
                // Mixer GreenOutBlueIn
                else if (_effect == ColorGradeEffect.MixerGreenOutBlueIn && defaultChannelMixer.greenOutBlueIn.overrideState)
                {
                    _defaultValue = defaultChannelMixer.greenOutBlueIn.value;
                }
                // Mixer GreenOutGreenIn
                else if (_effect == ColorGradeEffect.MixerGreenOutGreenIn && defaultChannelMixer.greenOutGreenIn.overrideState)
                {
                    _defaultValue = defaultChannelMixer.greenOutGreenIn.value;
                }
                // Mixer GreenOutRedIn
                else if (_effect == ColorGradeEffect.MixerGreenOutRedIn && defaultChannelMixer.greenOutRedIn.overrideState)
                {
                    _defaultValue = defaultChannelMixer.greenOutRedIn.value;
                }
                //
                // Mixer RedOutBlueIn
                else if (_effect == ColorGradeEffect.MixerRedOutBlueIn && defaultChannelMixer.redOutBlueIn.overrideState)
                {
                    _defaultValue = defaultChannelMixer.redOutBlueIn.value;
                }
                // Mixer RedOutGreenIn
                else if (_effect == ColorGradeEffect.MixerRedOutGreenIn && defaultChannelMixer.redOutGreenIn.overrideState)
                {
                    _defaultValue = defaultChannelMixer.redOutGreenIn.value;
                }
                // Mixer RedOutRedIn
                else if (_effect == ColorGradeEffect.MixerRedOutRedIn && defaultChannelMixer.redOutRedIn.overrideState)
                {
                    _defaultValue = defaultChannelMixer.redOutRedIn.value;
                }
            }
            else if (IsWhiteBalance(_effect))
            {
                var defaultWhiteBalance = SettingsVolume.Instance.FindDefaultVolumeComponent<WhiteBalance>();

                if (defaultWhiteBalance == null || defaultWhiteBalance.active == false)
                {
                    _defaultValue = 0f;
                    return;
                }

                // Temperature
                if (_effect == ColorGradeEffect.Temperature && defaultWhiteBalance.temperature.overrideState)
                {
                    _defaultValue = defaultWhiteBalance.temperature.value;
                }
                // Tint
                else if (_effect == ColorGradeEffect.Tint && defaultWhiteBalance.tint.overrideState)
                {
                    _defaultValue = defaultWhiteBalance.tint.value;
                }
            }
            else if (IsShadowsMidtonesHighlights(_effect))
            {
                var defaultShadowsMidtonesHighlights = SettingsVolume.Instance.FindDefaultVolumeComponent<ShadowsMidtonesHighlights>();

                if (defaultShadowsMidtonesHighlights == null || defaultShadowsMidtonesHighlights.active == false)
                {
                    _defaultValue = 0f;
                    return;
                }

                // Shadows
                if (_effect == ColorGradeEffect.SMHShadows && defaultShadowsMidtonesHighlights.shadows.overrideState)
                {
                    _defaultValue = defaultShadowsMidtonesHighlights.shadows.value.w;
                }
                // Midtones
                else if (_effect == ColorGradeEffect.SMHMidtones && defaultShadowsMidtonesHighlights.midtones.overrideState)
                {
                    _defaultValue = defaultShadowsMidtonesHighlights.midtones.value.w;
                }
                // Highlights
                else if (_effect == ColorGradeEffect.SMHHighlights && defaultShadowsMidtonesHighlights.highlights.overrideState)
                {
                    _defaultValue = defaultShadowsMidtonesHighlights.highlights.value.w;
                }
            }
        }

        public override float Get()
        {
            // If this is called during EDIT Mode then the Instance will be null.
            if (IsLiftGammaGain(_effect))
            {
                if (_liftGammaGain == null || _liftGammaGain.active == false)
                    return _defaultValue;

                // Lift
                if (_effect == ColorGradeEffect.Lift && _liftGammaGain.lift.overrideState)
                {
                    return _liftGammaGain.lift.value.w;
                }
                // Gamma
                else if (_effect == ColorGradeEffect.Gamma && _liftGammaGain.gamma.overrideState)
                {
                    return _liftGammaGain.gamma.value.w;
                }
                // Gain
                else if (_effect == ColorGradeEffect.Gain && _liftGammaGain.gain.overrideState)
                {
                    return _liftGammaGain.gain.value.w;
                }
            }
            else if (IsColorAdjustment(_effect))
            {
                if (_colorAdjustment == null || _colorAdjustment.active == false)
                    return _defaultValue;

                // Post Exposure
                if (_effect == ColorGradeEffect.PostExposure && _colorAdjustment.postExposure.overrideState)
                {
                    return _colorAdjustment.postExposure.value;
                }
                // Saturation
                else if (_effect == ColorGradeEffect.Saturation && _colorAdjustment.saturation.overrideState)
                {
                    return _colorAdjustment.saturation.value;
                }
                // ColorFilter <- not supported
                // Contrast
                else if (_effect == ColorGradeEffect.Contrast && _colorAdjustment.contrast.overrideState)
                {
                    return _colorAdjustment.contrast.value;
                }
                // HueShift
                else if (_effect == ColorGradeEffect.HueShift && _colorAdjustment.hueShift.overrideState)
                {
                    return _colorAdjustment.hueShift.value;
                }
            }
            else if (IsChannelMixer(_effect))
            {
                if (_channelMixer == null || _channelMixer.active == false)
                {
                    return _defaultValue;
                }

                // Mixer BlueOutBlueIn
                if (_effect == ColorGradeEffect.MixerBlueOutBlueIn && _channelMixer.blueOutBlueIn.overrideState)
                {
                    return _channelMixer.blueOutBlueIn.value;
                }
                // Mixer BlueOutGreenIn
                else if (_effect == ColorGradeEffect.MixerBlueOutGreenIn && _channelMixer.blueOutGreenIn.overrideState)
                {
                    return _channelMixer.blueOutGreenIn.value;
                }
                // Mixer BlueOutRedIn
                else if (_effect == ColorGradeEffect.MixerBlueOutRedIn && _channelMixer.blueOutRedIn.overrideState)
                {
                    return _channelMixer.blueOutRedIn.value;
                }
                //
                // Mixer GreenOutBlueIn
                else if (_effect == ColorGradeEffect.MixerGreenOutBlueIn && _channelMixer.greenOutBlueIn.overrideState)
                {
                    return _channelMixer.greenOutBlueIn.value;
                }
                // Mixer GreenOutGreenIn
                else if (_effect == ColorGradeEffect.MixerGreenOutGreenIn && _channelMixer.greenOutGreenIn.overrideState)
                {
                    return _channelMixer.greenOutGreenIn.value;
                }
                // Mixer GreenOutRedIn
                else if (_effect == ColorGradeEffect.MixerGreenOutRedIn && _channelMixer.greenOutRedIn.overrideState)
                {
                    return _channelMixer.greenOutRedIn.value;
                }
                //
                // Mixer RedOutBlueIn
                else if (_effect == ColorGradeEffect.MixerRedOutBlueIn && _channelMixer.redOutBlueIn.overrideState)
                {
                    return _channelMixer.redOutBlueIn.value;
                }
                // Mixer RedOutGreenIn
                else if (_effect == ColorGradeEffect.MixerRedOutGreenIn && _channelMixer.redOutGreenIn.overrideState)
                {
                    return _channelMixer.redOutGreenIn.value;
                }
                // Mixer RedOutRedIn
                else if (_effect == ColorGradeEffect.MixerRedOutRedIn && _channelMixer.redOutRedIn.overrideState)
                {
                    return _channelMixer.redOutRedIn.value;
                }
            }
            else if (IsWhiteBalance(_effect))
            {
                if (_whiteBalance == null || _whiteBalance.active == false)
                    return _defaultValue;

                // Temperature
                if (_effect == ColorGradeEffect.Temperature && _whiteBalance.temperature.overrideState)
                {
                    return _whiteBalance.temperature.value;
                }
                // Tint
                else if (_effect == ColorGradeEffect.Tint && _whiteBalance.tint.overrideState)
                {
                    return _whiteBalance.tint.value;
                }
            }
            else if (IsShadowsMidtonesHighlights(_effect))
            {
                if (_shadowsMidtonesHighlights == null || _shadowsMidtonesHighlights.active == false)
                    return _defaultValue;

                // Shadows
                if (_effect == ColorGradeEffect.SMHShadows && _shadowsMidtonesHighlights.shadows.overrideState)
                {
                    return _shadowsMidtonesHighlights.shadows.value.w;
                }
                // Midtones
                else if (_effect == ColorGradeEffect.SMHMidtones && _shadowsMidtonesHighlights.midtones.overrideState)
                {
                    return _shadowsMidtonesHighlights.midtones.value.w;
                }
                // Highlights
                else if (_effect == ColorGradeEffect.SMHHighlights && _shadowsMidtonesHighlights.highlights.overrideState)
                {
                    return _shadowsMidtonesHighlights.highlights.value.w;
                }
            }

            return _defaultValue;
        }

        public override void Set(float value)
        {
            if (IsLiftGammaGain(_effect))
            {
                // If this is called during EDIT Mode then the Instance will be null.
                if (_liftGammaGain == null)
                    return;

                _liftGammaGain.active = true;

                var newValue = new Vector4(1f, 1f, 1f, _defaultValue);
                newValue.w = value;

                // Lift
                if (_effect == ColorGradeEffect.Lift)
                {
                    _liftGammaGain.lift.Override(newValue);

                }
                // Gamma
                else if (_effect == ColorGradeEffect.Gamma)
                {
                    _liftGammaGain.gamma.Override(newValue);
                }
                // Gain
                else if (_effect == ColorGradeEffect.Gain)
                {
                    _liftGammaGain.gain.Override(newValue);
                }
            }
            else if (IsColorAdjustment(_effect))
            {
                // If this is called during EDIT Mode then the Instance will be null.
                if (_colorAdjustment == null)
                    return;

                _colorAdjustment.active = true;

                // Post Exposure
                if (_effect == ColorGradeEffect.PostExposure)
                {
                    _colorAdjustment.postExposure.Override(value);
                }
                // Saturation
                else if (_effect == ColorGradeEffect.Saturation)
                {
                    _colorAdjustment.saturation.Override(value);
                }
                // ColorFilter <- not supported
                // Contrast
                else if (_effect == ColorGradeEffect.Contrast)
                {
                    _colorAdjustment.contrast.Override(value);
                }
                // HueShift
                else if (_effect == ColorGradeEffect.HueShift)
                {
                    _colorAdjustment.hueShift.Override(value);
                }
            }
            else if (IsChannelMixer(_effect))
            {
                // If this is called during EDIT Mode then the Instance will be null.
                if (_channelMixer == null)
                    return;

                _channelMixer.active = true;

                // Mixer BlueOutBlueIn
                if (_effect == ColorGradeEffect.MixerBlueOutBlueIn)
                {
                    _channelMixer.blueOutBlueIn.Override(value);
                }
                // Mixer BlueOutGreenIn
                else if (_effect == ColorGradeEffect.MixerBlueOutGreenIn)
                {
                    _channelMixer.blueOutGreenIn.Override(value);
                }
                // Mixer BlueOutRedIn
                else if (_effect == ColorGradeEffect.MixerBlueOutRedIn)
                {
                    _channelMixer.blueOutRedIn.Override(value);
                }
                //
                // Mixer GreenOutBlueIn
                else if (_effect == ColorGradeEffect.MixerGreenOutBlueIn)
                {
                    _channelMixer.greenOutBlueIn.Override(value);
                }
                // Mixer GreenOutGreenIn
                else if (_effect == ColorGradeEffect.MixerGreenOutGreenIn)
                {
                    _channelMixer.greenOutGreenIn.Override(value);
                }
                // Mixer GreenOutRedIn
                else if (_effect == ColorGradeEffect.MixerGreenOutRedIn)
                {
                    _channelMixer.greenOutRedIn.Override(value);
                }
                //
                // Mixer RedOutBlueIn
                else if (_effect == ColorGradeEffect.MixerRedOutBlueIn)
                {
                    _channelMixer.redOutBlueIn.Override(value);
                }
                // Mixer RedOutGreenIn
                else if (_effect == ColorGradeEffect.MixerRedOutGreenIn)
                {
                    _channelMixer.redOutGreenIn.Override(value);
                }
                // Mixer RedOutRedIn
                else if (_effect == ColorGradeEffect.MixerRedOutRedIn)
                {
                    _channelMixer.redOutRedIn.Override(value);
                }
            }
            else if (IsWhiteBalance(_effect))
            {
                // If this is called during EDIT Mode then the Instance will be null.
                if (_whiteBalance == null)
                    return;

                _whiteBalance.active = true;

                // Temperature
                if (_effect == ColorGradeEffect.Temperature)
                {
                    _whiteBalance.temperature.Override(value);
                }
                // Tint
                else if (_effect == ColorGradeEffect.Tint)
                {
                    _whiteBalance.tint.Override(value);
                }
            }
            else if (IsShadowsMidtonesHighlights(_effect))
            {
                if (_shadowsMidtonesHighlights == null)
                    return;

                _shadowsMidtonesHighlights.active = true;

                var newValue = new Vector4(1f, 1f, 1f, _defaultValue);
                newValue.w = value;

                // Shadows
                if (_effect == ColorGradeEffect.SMHShadows)
                {
                    _shadowsMidtonesHighlights.shadows.Override(newValue);
                }
                // Midtones
                else if (_effect == ColorGradeEffect.SMHMidtones)
                {
                    _shadowsMidtonesHighlights.midtones.Override(newValue);
                }
                // Highlights
                else if (_effect == ColorGradeEffect.SMHHighlights)
                {
                    _shadowsMidtonesHighlights.highlights.Override(newValue);
                }
            }

            NotifyListenersIfChanged(value);
        }
    }
}

#endif