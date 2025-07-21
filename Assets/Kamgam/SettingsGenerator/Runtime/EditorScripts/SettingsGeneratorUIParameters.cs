#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Kamgam.LocalizationForSettings;
using Kamgam.UGUIComponentsForSettings;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// This class is used to define the default UI parameters of new ui instances (like the min/max values of sliders).
    /// </summary>
    public class SettingsGeneratorUIParameters
    {
        public static Dictionary<string, SettingsGeneratorUIParameters> SettingIdToUIParameters = new Dictionary<string, SettingsGeneratorUIParameters>()
        {
            {"fullscreen", null },
            {"vSync", null},
            {"shadows", null},
            {"audioEnabled", null},
            {"ambientOcclusionEnabled", null},
            {"bloomEnabled", null},
            {"motionBlurEnabled", null},
            {"vignetteEnabled", null},
            {"depthOfFieldEnabled", null},
            {"fog", null},
            {"volumetricsEnabled", null},
            {"resolution", new SettingsGeneratorUIParameters(prefillOptions: true) },
            {"windowMode", new SettingsGeneratorUIParameters(prefillOptions: true) },
            {"refreshRate", new SettingsGeneratorUIParameters(prefillOptions: true) },
            {"frameRate", new SettingsGeneratorUIParameters(prefillOptions: true) },
            {"antiAliasing", new SettingsGeneratorUIParameters(prefillOptions: true) },
            {"gfxQuality", new SettingsGeneratorUIParameters(prefillOptions: true) },
            {"shadowDistance", new SettingsGeneratorUIParameters(prefillOptions: true) },
            {"shadowResolution", new SettingsGeneratorUIParameters(prefillOptions: true) },
            {"textureResolution", new SettingsGeneratorUIParameters(prefillOptions: true) },
            {"dlss", new SettingsGeneratorUIParameters(prefillOptions: true) },
            {"fsr", new SettingsGeneratorUIParameters(prefillOptions: true) },
            {"msaa", new SettingsGeneratorUIParameters(prefillOptions: true) },
            {"microphone", new SettingsGeneratorUIParameters(prefillOptions: true) },
            {"monitor", new SettingsGeneratorUIParameters(prefillOptions: true) },
            {"ambientLight", new SettingsGeneratorUIParameters(sliderMin: 0f, sliderDefault: 0, sliderMax: 100f, sliderStepSize: 5f, sliderWholeNumbers: true, sliderFormat: SliderFormatWholeNumbers) },
            {"audioMasterVolume", new SettingsGeneratorUIParameters(sliderMin: 0f, sliderDefault: 100f, sliderMax: 200f, sliderStepSize: 5f, sliderWholeNumbers: true, sliderFormat: SliderFormatWholeNumbers) },
            {"gamma", new SettingsGeneratorUIParameters(sliderMin: -1f, sliderDefault: 0f, sliderMax: 1f, sliderStepSize: 0.1f, sliderWholeNumbers: false, sliderFormat: SliderFormatOneDigit) },
            {"fieldOfView", new SettingsGeneratorUIParameters(sliderMin: 30f, sliderDefault: 60f, sliderMax: 120f, sliderStepSize: 1f, sliderWholeNumbers: true, sliderFormat: SliderFormatWholeNumbers) },
            {"renderScale", new SettingsGeneratorUIParameters(sliderMin: 0.1f, sliderDefault: 1f, sliderMax: 2f, sliderStepSize: 0.1f, sliderWholeNumbers: false, sliderFormat: SliderFormatOneDigit) },
            {"renderDistance", new SettingsGeneratorUIParameters(sliderMin: 1f, sliderDefault: 1000f, sliderMax: 10_000f, sliderStepSize: 1f, sliderWholeNumbers: true, sliderFormat: SliderFormatWholeNumbers)},
            {"colorGradeBrightness", new SettingsGeneratorUIParameters(sliderMin: -100f, sliderDefault: 0f, sliderMax: 100f, sliderStepSize: 1f, sliderWholeNumbers: true, sliderFormat: SliderFormatWholeNumbers)},
            {"lift", new SettingsGeneratorUIParameters(sliderMin: -1f, sliderDefault: 0f, sliderMax: 1f, sliderStepSize: 0.1f, sliderWholeNumbers: false, sliderFormat: SliderFormatOneDigit)},
            {"gain", new SettingsGeneratorUIParameters(sliderMin: -1f, sliderDefault: 0f, sliderMax: 1f, sliderStepSize: 0.1f, sliderWholeNumbers: false, sliderFormat: SliderFormatOneDigit)},
            {"postExposure", new SettingsGeneratorUIParameters(sliderMin: -1f, sliderDefault: 0f, sliderMax: 1f, sliderStepSize: 0.1f, sliderWholeNumbers: false, sliderFormat: SliderFormatOneDigit)},
            {"contrast", new SettingsGeneratorUIParameters(sliderMin: -1f, sliderDefault: 0f, sliderMax: 1f, sliderStepSize: 0.1f, sliderWholeNumbers: false, sliderFormat: SliderFormatOneDigit)},
            {"hueShift", new SettingsGeneratorUIParameters(sliderMin: -1f, sliderDefault: 0f, sliderMax: 1f, sliderStepSize: 0.1f, sliderWholeNumbers: false, sliderFormat: SliderFormatOneDigit)},
            {"saturation", new SettingsGeneratorUIParameters(sliderMin: -1f, sliderDefault: 0f, sliderMax: 1f, sliderStepSize: 0.1f, sliderWholeNumbers: false, sliderFormat: SliderFormatOneDigit)},
            {"temperature", new SettingsGeneratorUIParameters(sliderMin: -1f, sliderDefault: 0f, sliderMax: 1f, sliderStepSize: 0.1f, sliderWholeNumbers: false, sliderFormat: SliderFormatOneDigit)},
            {"tint", new SettingsGeneratorUIParameters(sliderMin: -1f, sliderDefault: 0f, sliderMax: 1f, sliderStepSize: 0.1f, sliderWholeNumbers: false, sliderFormat: SliderFormatOneDigit)},
            {"smhShadows", new SettingsGeneratorUIParameters(sliderMin: -1f, sliderDefault: 0f, sliderMax: 1f, sliderStepSize: 0.1f, sliderWholeNumbers: false, sliderFormat: SliderFormatOneDigit)},
            {"smhMidtones", new SettingsGeneratorUIParameters(sliderMin: -1f, sliderDefault: 0f, sliderMax: 1f, sliderStepSize: 0.1f, sliderWholeNumbers: false, sliderFormat: SliderFormatOneDigit)},
            {"smhHighlights", new SettingsGeneratorUIParameters(sliderMin: -1f, sliderDefault: 0f, sliderMax: 1f, sliderStepSize: 0.1f, sliderWholeNumbers: false, sliderFormat: SliderFormatOneDigit)},
            {"mixerBlueBlue", new SettingsGeneratorUIParameters(sliderMin: -200f, sliderDefault: 100f, sliderMax: 200f, sliderStepSize: 1f, sliderWholeNumbers: true, sliderFormat: SliderFormatWholeNumbers)},
            {"mixerBlueGreen", new SettingsGeneratorUIParameters(sliderMin: -200f, sliderDefault: 0f, sliderMax: 200f, sliderStepSize: 1f, sliderWholeNumbers: true, sliderFormat: SliderFormatWholeNumbers)},
            {"mixerBlueRed", new SettingsGeneratorUIParameters(sliderMin: -200f, sliderDefault: 0f, sliderMax: 200f, sliderStepSize: 1f, sliderWholeNumbers: true, sliderFormat: SliderFormatWholeNumbers)},
            {"mixerGreenBlue", new SettingsGeneratorUIParameters(sliderMin: -200f, sliderDefault: 0f, sliderMax: 200f, sliderStepSize: 1f, sliderWholeNumbers: true, sliderFormat: SliderFormatWholeNumbers)},
            {"mixerGreenGreen", new SettingsGeneratorUIParameters(sliderMin: -200f, sliderDefault: 100f, sliderMax: 200f, sliderStepSize: 1f, sliderWholeNumbers: true, sliderFormat: SliderFormatWholeNumbers)},
            {"mixerGreenRed", new SettingsGeneratorUIParameters(sliderMin: -200f, sliderDefault: 0f, sliderMax: 200f, sliderStepSize: 1f, sliderWholeNumbers: true, sliderFormat: SliderFormatWholeNumbers)},
            {"mixerRedBlue", new SettingsGeneratorUIParameters(sliderMin: -200f, sliderDefault: 0f, sliderMax: 200f, sliderStepSize: 1f, sliderWholeNumbers: true, sliderFormat: SliderFormatWholeNumbers)},
            {"mixerRedGreen", new SettingsGeneratorUIParameters(sliderMin: -200f, sliderDefault: 0f, sliderMax: 200f, sliderStepSize: 1f, sliderWholeNumbers: true, sliderFormat: SliderFormatWholeNumbers)},
            {"mixerRedRed", new SettingsGeneratorUIParameters(sliderMin: -200f, sliderDefault: 100f, sliderMax: 200f, sliderStepSize: 1f, sliderWholeNumbers: true, sliderFormat: SliderFormatWholeNumbers)},
            {"keyboard.jump", null}
        };

        public SettingsGeneratorUIParameters(float sliderMin, float sliderDefault, float sliderMax, float sliderStepSize, bool sliderWholeNumbers, string sliderFormat)
        {
            UsesSlider = true;
            SliderMin = sliderMin;
            SliderDefault = sliderDefault;
            SliderMax = sliderMax;
            SliderStepSize = sliderStepSize;
            SliderWholeNumbers = sliderWholeNumbers;
            SliderFormat = sliderFormat;
            PrefillOptions = false;
        }
        
        public SettingsGeneratorUIParameters(bool prefillOptions)
        {
            UsesSlider = false;
            PrefillOptions = prefillOptions;
        }

        public static void ApplyUIParametersToUGUI(SettingResolver resolver)
        {
            if (resolver.SettingsProvider.SettingsAsset == null)
                return;

            var setting = resolver.SettingsProvider.SettingsAsset.GetSetting(resolver.GetID());
            if (setting == null)
                return;

            if (SettingIdToUIParameters.TryGetValue(resolver.GetID(), out var parameters))
            {
                if (parameters != null)
                {
                    if (parameters.UsesSlider)
                    {
                        var slider = resolver.gameObject.GetComponent<SliderUGUI>();
                        if (slider == null)
                            return;

                        slider.ValueFormat = parameters.SliderFormat;
                        slider.WholeNumbers = parameters.SliderWholeNumbers;
                        slider.StepSize = parameters.SliderStepSize;
                        slider.MinValue = parameters.SliderMin;
                        slider.MaxValue = parameters.SliderMax;
                        slider.Value = parameters.SliderMin; // Set 3 times to force a UI update due to value change.
                        slider.Value = parameters.SliderMax;
                        slider.Value = parameters.SliderDefault;
                    }

                    if (parameters.PrefillOptions)
                    {
                        var optionSetting = setting as SettingOption;
                        if (optionSetting == null)
                            return;

                        var options = new List<string>(); 
                        options.AddRange(optionSetting.GetOptionLabels());
                        if (optionSetting.HasConnectionObject())
                        {
                            var optionConnectionSO = optionSetting.GetConnectionSO() as OptionConnectionSO;
                            if (optionConnectionSO != null)
                            {
                                var optionsFromConnection = optionConnectionSO.GetConnection().GetOptionLabels();
                                if (!optionsFromConnection.IsNullOrEmpty())
                                {
                                    options.Clear();
                                    options.AddRange(optionsFromConnection);
                                }
                            }
                        }
                        
                        OptionsButtonUGUI optionsButton = resolver.gameObject.GetComponent<OptionsButtonUGUI>();
                        if (optionsButton != null)
                            optionsButton.SetOptions(options);
                        
                        DropDownUGUI dropDown = resolver.gameObject.GetComponent<DropDownUGUI>();
                        if (dropDown != null)
                            dropDown.SetOptions(options);
                    }
                }
            }
        }
        
        public static void ApplyUIParametersToUIToolkit(SettingResolver resolver)
        {
            if (resolver.SettingsProvider.SettingsAsset == null)
                return;

            var setting = resolver.SettingsProvider.SettingsAsset.GetSetting(resolver.GetID());
            if (setting == null)
                return;

            throw new NotImplementedException("Not yet implemented for UI Toolkit");
        }
        
        public const string SliderFormatWholeNumbers = "{0:N0}";
        public const string SliderFormatOneDigit = "{0:N1}";

        /// <summary>
        /// If true then the slider properties are applied.
        /// </summary>
        public bool UsesSlider;
        public float SliderMin;
        public float SliderDefault;
        public float SliderMax;
        public float SliderStepSize;
        public bool SliderWholeNumbers;
        /// <summary>
        /// The number format:
        /// '{0:N0}' = whole number without commas ('1')
        /// '{0:N1}' = one digit after the comma ('1.2')
        /// You can learn more here: https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings")]
        /// </summary>
        public string SliderFormat;

        /// <summary>
        /// If true then the options list will be prefilled with the defaults (or the defaults of the connection).
        /// </summary>
        public bool PrefillOptions;
    }
}
#endif