#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Kamgam.LocalizationForSettings;
using Kamgam.UGUIComponentsForSettings;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Kamgam.SettingsGenerator
{
    public static class SettingsGeneratorPrefabStyleConverter
    {
        static Selectable[] _selectables = new Selectable[10];

        // Here we have a list of all Settings UGUI Prefabs (styles).
        public static string OptionsButtonPrefabName = "OptionsButtonUGUI (Setting)";
        public static string OptionsButtonConsolePrefabName = "OptionsButtonConsoleUGUI (Setting)";
        public static string DropDownPrefabName = "DropdownUGUI (Setting)";
        public static string DropDownWithLabelPrefabName = "DropdownUGUIWithLabel (Setting)";

        public static string SliderPrefabName = "SliderUGUI (Setting)";
        public static string SliderConsolePrefabName = "SliderConsoleUGUI (Setting)";
        
        public static string TogglePrefabName = "ToggleUGUI (Setting)";
        public static string ToggleConsolePrefabName = "ToggleConsoleUGUI (Setting)";
        
        public static string InputKeyPrefabName = "InputKeyUGUI (Setting)";
        public static string InputKeyConsolePrefabName = "InputKeyConsoleUGUI (Setting)";

        public static string StepperPrefabName = "StepperUGUI (Setting)";
        // public static string StepperConsolePrefabName = "StepperConsoleUGUI (Setting)"; // Currently we have no extra stepper for console
        public static string StepperTwoButtonsConsolePrefabName = "StepperTwoButtonsConsoleUGUI (Setting)";
        public static string StepperOneButtonConsolePrefabName = "StepperOneButtonConsoleUGUI (Setting)";
        public static string StepperOneButtonStepsConsolePrefabName = "StepperOneButtonStepsConsoleUGUI (Setting)";

        public static string TextfieldPrefabName = "TextfieldUGUI (Setting)";
        public static string TextfieldConsoleUGUIPrefabName = "TextfieldConsoleUGUI (Setting)";

        public static string ColorPickerPrefabName = "ColorPickerUGUI (Setting)";
        public static string ColorPickerConsolePrefabName = "ColorPickerConsoleUGUI (Setting)";
        
        /// <summary>
        /// Information on what conversions are doable (used to populate style choices). Bidirectional.
        /// </summary>
        public static List<(string, string)> ValidConversions = new List<(string, string)>()
        {
            ( OptionsButtonPrefabName, OptionsButtonConsolePrefabName ),
            ( OptionsButtonPrefabName, DropDownPrefabName ),
            ( OptionsButtonPrefabName, DropDownWithLabelPrefabName ),
            //
            ( OptionsButtonConsolePrefabName, DropDownPrefabName),
            ( OptionsButtonConsolePrefabName, DropDownWithLabelPrefabName),
            //
            ( DropDownPrefabName, DropDownWithLabelPrefabName),
            
            ( SliderPrefabName, SliderConsolePrefabName ),
            
            ( TogglePrefabName, ToggleConsolePrefabName ),
            
            ( InputKeyPrefabName, InputKeyConsolePrefabName ),
            
            // ( StepperPrefabName, StepperConsolePrefabName ),
            ( StepperPrefabName, StepperTwoButtonsConsolePrefabName ),
            ( StepperPrefabName, StepperOneButtonConsolePrefabName ),
            ( StepperPrefabName, StepperOneButtonStepsConsolePrefabName ),
            //
            ( StepperTwoButtonsConsolePrefabName, StepperOneButtonConsolePrefabName ),
            ( StepperTwoButtonsConsolePrefabName, StepperOneButtonStepsConsolePrefabName ),
            //
            ( StepperOneButtonConsolePrefabName, StepperOneButtonStepsConsolePrefabName ),
            
            ( TextfieldPrefabName, TextfieldConsoleUGUIPrefabName ),
            
            ( ColorPickerPrefabName, ColorPickerConsolePrefabName )
        };
        
        /// <summary>
        /// Add Prefab path and the conversion method to it.
        /// </summary>
        public static Dictionary<string, System.Action<GameObject>> ConversionMethods = new Dictionary<string, Action<GameObject>>()
        {
            { OptionsButtonPrefabName, ReplaceObjWithOptionsButton},
            { OptionsButtonConsolePrefabName, ReplaceObjWithOptionsButtonConsole},
            { DropDownPrefabName, ReplaceObjWithDropDown},
            { DropDownWithLabelPrefabName, ReplaceObjWithDropDownWithLabel},

            { SliderPrefabName, ReplaceObjWithSlider},
            { SliderConsolePrefabName, ReplaceObjWithSliderConsole},

            { TogglePrefabName, ReplaceObjWithToggle},
            { ToggleConsolePrefabName, ReplaceObjWithToggleConsole},

            { InputKeyPrefabName, ReplaceObjWithInputKey},
            { InputKeyConsolePrefabName, ReplaceObjWithInputKeyConsole},

            { StepperPrefabName, (go) => ReplaceObjWithStepper(go, StepperPrefabName)},
            // { StepperConsolePrefabName, (go) => ReplaceObjWithStepper(go, StepperConsolePrefabName)},
            { StepperTwoButtonsConsolePrefabName, (go) => ReplaceObjWithStepper(go, StepperTwoButtonsConsolePrefabName)},
            { StepperOneButtonConsolePrefabName, (go) => ReplaceObjWithStepper(go, StepperOneButtonConsolePrefabName)},
            { StepperOneButtonStepsConsolePrefabName, (go) => ReplaceObjWithStepper(go, StepperOneButtonStepsConsolePrefabName)},

            { TextfieldPrefabName, (go) => ReplaceObjWithTextfield(go, TextfieldPrefabName)},
            { TextfieldConsoleUGUIPrefabName, (go) => ReplaceObjWithTextfield(go, TextfieldConsoleUGUIPrefabName) },

            { ColorPickerPrefabName, (go) => ReplaceObjWithColorPicker(go, ColorPickerPrefabName)},
            { ColorPickerConsolePrefabName, (go) => ReplaceObjWithColorPicker(go, ColorPickerConsolePrefabName)}
        };

        /// <summary>
        /// Add your custom prefabs here.
        /// Even if the data type matches a setting may not be compatible with every prefab (option settings for
        /// example are INT compatible but really should only be shown by prefabs that show the option
        /// labels, not a slider or similar).
        /// </summary>
        public static List<(string, List<SettingData.DataType>)> ValidPrefabsTypes =
            new List<(string, List<SettingData.DataType>)>()
            {
                (OptionsButtonPrefabName, new List<SettingData.DataType>() { SettingData.DataType.Option }),
                (OptionsButtonConsolePrefabName, new List<SettingData.DataType>() { SettingData.DataType.Option }),
                (DropDownPrefabName, new List<SettingData.DataType>() { SettingData.DataType.Option }),
                (DropDownWithLabelPrefabName, new List<SettingData.DataType>() { SettingData.DataType.Option }),

                (SliderPrefabName,
                    new List<SettingData.DataType>() { SettingData.DataType.Int, SettingData.DataType.Float }),
                (SliderConsolePrefabName,
                    new List<SettingData.DataType>() { SettingData.DataType.Int, SettingData.DataType.Float }),

                (TogglePrefabName, new List<SettingData.DataType>() { SettingData.DataType.Bool }),
                (ToggleConsolePrefabName, new List<SettingData.DataType>() { SettingData.DataType.Bool }),

                (InputKeyPrefabName, new List<SettingData.DataType>() { SettingData.DataType.KeyCombination }),
                (InputKeyConsolePrefabName, new List<SettingData.DataType>() { SettingData.DataType.KeyCombination }),

                (StepperPrefabName,
                    new List<SettingData.DataType>() { SettingData.DataType.Int, SettingData.DataType.Option }),
                // ( StepperConsolePrefabName, new List<SettingData.DataType>(){SettingData.DataType.Int, SettingData.DataType.Option} ),
                (StepperTwoButtonsConsolePrefabName,
                    new List<SettingData.DataType>() { SettingData.DataType.Int, SettingData.DataType.Option }),
                (StepperOneButtonConsolePrefabName,
                    new List<SettingData.DataType>() { SettingData.DataType.Int, SettingData.DataType.Option }),
                (StepperOneButtonStepsConsolePrefabName,
                    new List<SettingData.DataType>() { SettingData.DataType.Int, SettingData.DataType.Option }),

                (TextfieldPrefabName, new List<SettingData.DataType>() { SettingData.DataType.String }),
                (TextfieldConsoleUGUIPrefabName, new List<SettingData.DataType>() { SettingData.DataType.String }),

                (ColorPickerPrefabName,
                    new List<SettingData.DataType>() { SettingData.DataType.ColorOption, SettingData.DataType.Color }),
                (ColorPickerConsolePrefabName,
                    new List<SettingData.DataType>() { SettingData.DataType.ColorOption, SettingData.DataType.Color })
            };

        public static Dictionary<string, string> SettingIdToLabel = new Dictionary<string, string>()
        {
            {"fullscreen", "Full Screen"},
            {"vSync", "V-Sync"},
            {"shadows", "Shadows"},
            {"audioEnabled", "Audio"},
            {"ambientOcclusionEnabled", "Ambient Occlusion"},
            {"bloomEnabled", "Bloom"},
            {"motionBlurEnabled", "Motion Blur"},
            {"vignetteEnabled", "Vignette"},
            {"depthOfFieldEnabled", "Depth of field"},
            {"fog", "Fog"},
            {"volumetricsEnabled", "Volumetrics"},
            {"resolution", "Resolution"},
            {"windowMode", "Window"},
            {"refreshRate", "Frequency"},
            {"frameRate", "FPS"},
            {"antiAliasing", "Anti Aliasing"},
            {"gfxQuality", "Quality"},
            {"shadowDistance", "Shadow Distance"},
            {"shadowResolution", "Shadow Resolution"},
            {"textureResolution", "Texture Resolution"},
            {"dlss", "DLSS"},
            {"fsr", "FSR"},
            {"msaa", "MSAA"},
            {"microphone", "Microphone"},
            {"monitor", "Monitor"},
            {"ambientLight", "Ambient Occlusion"},
            {"audioMasterVolume", "Audio"},
            {"gamma", "Gamma"},
            {"fieldOfView", "Field Of View"},
            {"renderScale", "Render Scale"},
            {"renderDistance", "Render Distance"},
            {"colorGradeBrightness", "Color Grading"},
            {"lift", "Lift"},
            {"gain", "Gain"},
            {"postExposure", "Post Exposure"},
            {"contrast", "Contrast"},
            {"hueShift", "Hue Shift"},
            {"saturation", "Saturation"},
            {"temperature", "Temperature"},
            {"tint", "Tint"},
            {"smhShadows", "Shadows"},
            {"smhMidtones", "Midtones"},
            {"smhHighlights", "Highlights"},
            {"mixerBlueBlue", "Mixer Blue > Blue"},
            {"mixerBlueGreen", "Mixer Blue > Green"},
            {"mixerBlueRed", "Mixer Blue > Red"},
            {"mixerGreenBlue", "Mixer Green > Blue"},
            {"mixerGreenGreen", "Mixer Green > Green"},
            {"mixerGreenRed", "Mixer Green > Red"},
            {"mixerRedBlue", "Mixer Red > Blue"},
            {"mixerRedGreen", "Mixer Red > Green"},
            {"mixerRedRed", "Mixer Red > Red"},
            {"keyboard.jump", "Jump"}
        };
        
        public static bool CanConvertPath(string fromPrefabPath, string toPrefabPath)
        {
            var fromPrefabName = System.IO.Path.GetFileNameWithoutExtension(fromPrefabPath);
            var toPrefabName = System.IO.Path.GetFileNameWithoutExtension(toPrefabPath);

            if (fromPrefabName == toPrefabName)
                return true;
            
            foreach (var link in ValidConversions)
            {
                if ((link.Item1 == fromPrefabName && link.Item2 == toPrefabName)
                    || (link.Item1 == toPrefabName && link.Item2 == fromPrefabName))
                {
                    return true;
                }
            }

            return false;
        }
        
        /// <summary>
        /// This does check if the prefab matches the data type. However it does NOT use the SupportedDataTypes of the
        /// resolvers but predefined list (ValidPrefabsTypes) because even if the data type matches it may not be compatible
        /// (option settings for example are INT compatible but really should only be shown by prefabs that show the option
        /// labels, not a slider or similar).
        /// </summary>
        /// <param name="prefabPath"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public static bool PrefabSupportsDataType(string prefabPath, SettingData.DataType dataType)
        {
            if (prefabPath.IsNullOrEmpty() || dataType == SettingData.DataType.Unknown)
                return false;
            
            var prefabName = System.IO.Path.GetFileNameWithoutExtension(prefabPath);
            
            foreach (var link in ValidPrefabsTypes)
            {
                if (link.Item1 != prefabName)
                    continue;

                if (link.Item2.Contains(dataType))
                    return true;
            }

            return false;
        }

        public static System.Action<GameObject> GetConversionMethod(string toPrefabPath)
        {
            var toPrefabName = System.IO.Path.GetFileNameWithoutExtension(toPrefabPath);
            if (ConversionMethods.TryGetValue(toPrefabName, out var method))
                return method;

            return null;
        }

        static void getBasicInfo(GameObject go, out string id, out SettingsProvider settingsProvider, out SettingResolver resolver, out GameObject root, out bool isPrefab)
        {
            resolver = go.GetComponentInChildren<SettingResolver>();
            // Search in parent too
            if (resolver == null)
            {
                resolver = go.GetComponentInParent<SettingResolver>();
                if (resolver != null)
                    go = resolver.gameObject;
            }
            isPrefab = PrefabUtility.GetPrefabInstanceStatus(go) != PrefabInstanceStatus.NotAPrefab;
            // We assume the resolver or the prefab is the root of the UI element.
            root = isPrefab ? PrefabUtility.GetNearestPrefabInstanceRoot(go) : resolver.gameObject;
            id = resolver.GetID();
            settingsProvider = resolver.SettingsProvider;
        }

        static GameObject createUI<T>(string id, SettingsProvider settingsProvider, GameObject oldUI, string prefabName, bool undo = true)
            where T : SettingResolver
        {
            // Instantiate new ui
            var prefab = loadPrefab(prefabName);
            var newUI = PrefabUtility.InstantiatePrefab(prefab, oldUI.transform.parent) as GameObject;
            var oldRect = (oldUI.transform as RectTransform);
            var newRect = (newUI.transform as RectTransform);
            newRect.pivot = oldRect.pivot;
            newRect.anchorMin = oldRect.anchorMin;
            newRect.anchorMax = oldRect.anchorMax;
            newRect.anchoredPosition = oldRect.anchoredPosition;
            newRect.sizeDelta = oldRect.sizeDelta;

            // Replace "(Setting)" with any existing text inside brackets like "(Quality)".
            if (newUI.name.Contains("(Setting)"))
            {
                var matchesInOldName = Regex.Matches(oldUI.name, @"\([^()]*\)");
                var matchesInNewName = Regex.Matches(newUI.name, @"\([^()]*\)");
                if (matchesInOldName.Count > 0)
                {
                    Match lastOldMatch = matchesInOldName[matchesInNewName.Count - 1];
                    if (matchesInNewName.Count > 0)
                    {
                        Match lastNewMatch = matchesInNewName[matchesInNewName.Count - 1];
                        newUI.name = newUI.name.Substring(0, lastNewMatch.Index) + lastOldMatch.Value + newUI.name.Substring(lastNewMatch.Index + lastNewMatch.Length);
                    }
                    else
                    {
                        newUI.name = newUI.name + " " + lastOldMatch.Value;
                    }
                }
            }

            if (undo)
            {
                Undo.RegisterCreatedObjectUndo(newUI, "SettingsGeneratorPrefabStyleConverter.Create");
                Undo.RegisterFullObjectHierarchyUndo(newUI, "SettingsGeneratorPrefabStyleConverter.Modify");
            }

            // Assign resolver
            var newResolver = newUI.GetComponent<T>();
            newResolver.SettingsProvider = settingsProvider;
            newResolver.ID = id;

            // Copy label
            var oldLabel = findLabel(oldUI);
            var newLabel = findLabel(newUI);
            if (oldLabel != null && newLabel != null)
            {
                newLabel.text = oldLabel.text;
            }

            // Copy Navigation Overrides
            var oldNavigation = oldUI.GetComponentInChildren<AutoNavigationOverrides>();
            var newNavigation = newUI.GetComponentInChildren<AutoNavigationOverrides>();
            if (oldNavigation != null && newNavigation != null)
            {
                newNavigation.BlockUp = oldNavigation.BlockUp;
                newNavigation.BlockDown = oldNavigation.BlockDown;
                newNavigation.BlockLeft = oldNavigation.BlockLeft;
                newNavigation.BlockRight = oldNavigation.BlockRight;

                if(oldNavigation.SelectOnUpOverride != null && !oldNavigation.SelectOnUpOverride.transform.IsChildOf(oldNavigation.transform))
                    newNavigation.SelectOnUpOverride = oldNavigation.SelectOnUpOverride;

                if (oldNavigation.SelectOnDownOverride != null && !oldNavigation.SelectOnDownOverride.transform.IsChildOf(oldNavigation.transform))
                    newNavigation.SelectOnDownOverride = oldNavigation.SelectOnDownOverride;

                if (oldNavigation.SelectOnLeftOverride != null && !oldNavigation.SelectOnLeftOverride.transform.IsChildOf(oldNavigation.transform))
                    newNavigation.SelectOnLeftOverride = oldNavigation.SelectOnLeftOverride;

                if (oldNavigation.SelectOnRightOverride != null && !oldNavigation.SelectOnRightOverride.transform.IsChildOf(oldNavigation.transform))
                    newNavigation.SelectOnRightOverride = oldNavigation.SelectOnRightOverride;

                newNavigation.DisableOnAwakeIfNotNeeded = oldNavigation.DisableOnAwakeIfNotNeeded;
            }

            // Patch other navigations and overrides that point to this selectable
            if (Selectable.allSelectableCount > _selectables.Length)
            {
                _selectables = new Selectable[Selectable.allSelectableCount];
            }
            for (int i = 0; i < _selectables.Length; i++)
            {
                _selectables[i] = null;
            }
            Selectable.AllSelectablesNoAlloc(_selectables);
            for (int i = 0; i < Selectable.allSelectableCount; i++)
            {
                var nav = _selectables[i].gameObject.GetComponent<AutoNavigationOverrides>();
                if (nav == null)
                    continue;

                if (nav.SelectOnUpOverride == oldUI.GetComponentInChildren<Selectable>())
                {
                    Undo.RegisterCompleteObjectUndo(nav, "Assigned new selectables");
                    nav.SelectOnUpOverride = newUI.GetComponentInChildren<Selectable>();
                    EditorUtility.SetDirty(nav);
                }

                if (nav.SelectOnDownOverride == oldUI.GetComponentInChildren<Selectable>())
                {
                    Undo.RegisterCompleteObjectUndo(nav, "Assigned new selectables");
                    nav.SelectOnDownOverride = newUI.GetComponentInChildren<Selectable>();
                    EditorUtility.SetDirty(nav);
                }

                if (nav.SelectOnLeftOverride == oldUI.GetComponentInChildren<Selectable>())
                {
                    Undo.RegisterCompleteObjectUndo(nav, "Assigned new selectables");
                    nav.SelectOnLeftOverride = newUI.GetComponentInChildren<Selectable>();
                    EditorUtility.SetDirty(nav);
                }

                if (nav.SelectOnRightOverride == oldUI.GetComponentInChildren<Selectable>())
                {
                    Undo.RegisterCompleteObjectUndo(nav, "Assigned new selectables");
                    nav.SelectOnRightOverride = newUI.GetComponentInChildren<Selectable>();
                    EditorUtility.SetDirty(nav);
                }
            }

            // Move to position of old ui
            newUI.transform.SetSiblingIndex(oldUI.transform.GetSiblingIndex());

            Selection.objects = new GameObject[] { newUI };

            return newUI;
        }

        static GameObject loadPrefab(string prefabName)
        {
            var guids = AssetDatabase.FindAssets("t:Prefab " + prefabName);
            if (guids.Length > 0)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
            return null;
        }

        static TextMeshProUGUI findLabel(GameObject go)
        {
            var loc = go.GetComponentInChildren<LocalizeTMPro>();
            if (loc != null)
                return loc.Textfield;
            return null;
        }

        [MenuItem("GameObject/UI/Settings Generator/Convert To/OptionsButton", priority = 2001)]
        public static void ReplaceWithOptionsButton()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var objects = Selection.gameObjects;
            foreach (var go in objects)
            {
                ReplaceObjWithOptionsButton(go);
            }
        }

        [MenuItem("GameObject/UI/Settings Generator/Convert To/OptionsButton Console", priority = 2001)]
        public static void ReplaceWithConsoleOptionsButtonConsole()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var objects = Selection.gameObjects;
            foreach (var go in objects)
            {
                ReplaceObjWithOptionsButtonConsole(go);
            }
        }

        public static void ReplaceObjWithOptionsButtonConsole(GameObject target)
        {
            getBasicInfo(target, out string id, out SettingsProvider settingsProvider, out SettingResolver resolver, out GameObject root, out bool isPrefab);

            // Can we convert?
            var optionsButtonUGUIResolver = resolver as OptionsButtonUGUIResolver;
            var dropDownUGUIResolver = resolver as DropDownUGUIResolver;

            if (optionsButtonUGUIResolver == null && dropDownUGUIResolver == null)
            {
                Logger.LogWarning("Conversion not possible.");
            }

            if (dropDownUGUIResolver != null)
            {
                GameObject newGo = createUI<OptionsButtonUGUIResolver>(id, settingsProvider, root, OptionsButtonConsolePrefabName, undo: true);

                // Copy data
                var newGUI = newGo.GetComponent<OptionsButtonUGUI>();
                var dropDown = dropDownUGUIResolver.GetComponent<TMP_Dropdown>();
                var options = dropDown.options;
                newGUI.SetOptions(options.Select(o => o.text).ToList());
                Undo.DestroyObjectImmediate(root);
            }
            else if (optionsButtonUGUIResolver != null)
            {
                GameObject newGo = createUI<OptionsButtonUGUIResolver>(id, settingsProvider, root, OptionsButtonConsolePrefabName, undo: true);

                // Copy data
                var newGUI = newGo.GetComponent<OptionsButtonUGUI>();
                var oldGUI = optionsButtonUGUIResolver.gameObject.GetComponent<OptionsButtonUGUI>();
                newGUI.SetOptions(oldGUI.GetOptions());
                Undo.DestroyObjectImmediate(root);
            }
        }

        public static void ReplaceObjWithOptionsButton(GameObject target)
        {
            getBasicInfo(target, out string id, out SettingsProvider settingsProvider, out SettingResolver resolver, out GameObject root, out bool isPrefab);

            // Can we convert?
            var optionsButtonUGUIResolver = resolver as OptionsButtonUGUIResolver;
            var dropDownUGUIResolver = resolver as DropDownUGUIResolver;

            if (optionsButtonUGUIResolver == null && dropDownUGUIResolver == null)
            {
                Logger.LogWarning("Conversion not possible.");
                return;
            }

            if (dropDownUGUIResolver != null)
            {
                GameObject newGo = createUI<OptionsButtonUGUIResolver>(id, settingsProvider, root, OptionsButtonPrefabName, undo: true);

                // Copy data
                var newGUI = newGo.GetComponent<OptionsButtonUGUI>();
                var dropDown = dropDownUGUIResolver.GetComponent<TMP_Dropdown>();
                var options = dropDown.options;
                // var oldGUI = dropDownUGUIResolver.gameObject.GetComponent<DropDownUGUI>();
                newGUI.SetOptions(options.Select(o => o.text).ToList());
                newGUI.Loop = newGUI.name.Contains("Console");
                newGUI.EnableButtonControls = newGUI.name.Contains("Console");
                Undo.DestroyObjectImmediate(root);
            }
            else if (optionsButtonUGUIResolver != null)
            {
                GameObject newGo = createUI<OptionsButtonUGUIResolver>(id, settingsProvider, root, OptionsButtonPrefabName, undo: true);

                // Copy data
                var newGUI = newGo.GetComponent<OptionsButtonUGUI>();
                var oldGUI = optionsButtonUGUIResolver.gameObject.GetComponent<OptionsButtonUGUI>();
                newGUI.SetOptions(oldGUI.GetOptions());
                newGUI.Loop = oldGUI.Loop;
                newGUI.EnableButtonControls = oldGUI.EnableButtonControls;
                Undo.DestroyObjectImmediate(root);
            }
        }

        [MenuItem("GameObject/UI/Settings Generator/Convert To/DropDown", priority = 2001)]
        public static void ReplaceWithDropDown()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var objects = Selection.gameObjects;
            foreach (var go in objects)
            {
                ReplaceObjWithDropDown(go);
            }
        }

        [MenuItem("GameObject/UI/Settings Generator/Convert To/DropDownWithLabel", priority = 2001)]
        public static void ReplaceWithDropDownWithLabel()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var objects = Selection.gameObjects;
            foreach (var go in objects)
            {
                ReplaceObjWithDropDownWithLabel(go);
            }
        }

        public static void ReplaceObjWithDropDown(GameObject target)
        {
            replaceObjWithDropDown(target, DropDownPrefabName);
        }
        
        public static void ReplaceObjWithDropDownWithLabel(GameObject target)
        {
            replaceObjWithDropDown(target, DropDownWithLabelPrefabName);
        }

        private static void replaceObjWithDropDown(GameObject target, string prefabName)
        {
            getBasicInfo(target, out string id, out SettingsProvider settingsProvider, out SettingResolver resolver, out GameObject root, out bool isPrefab);

            var optionsButtonUGUIResolver = resolver as OptionsButtonUGUIResolver;
            var dropDownUGUIResolver = resolver as DropDownUGUIResolver;
            
            // Can we convert?
            if (optionsButtonUGUIResolver != null)
            {
                GameObject newGo = createUI<DropDownUGUIResolver>(id, settingsProvider, root, prefabName, undo: true);

                // Copy data
                var newGUI = newGo.GetComponent<DropDownUGUI>();
                var oldGUI = optionsButtonUGUIResolver.gameObject.GetComponent<OptionsButtonUGUI>();
                newGUI.SetOptions(oldGUI.GetOptions());
                Undo.DestroyObjectImmediate(root);
            }
            else if (dropDownUGUIResolver != null)
            {
                GameObject newGo = createUI<DropDownUGUIResolver>(id, settingsProvider, root, prefabName, undo: true);

                // Copy data
                var newGUI = newGo.GetComponent<DropDownUGUI>();
                var oldGUI = dropDownUGUIResolver.gameObject.GetComponent<DropDownUGUI>();
                newGUI.SetOptions(oldGUI.GetOptions());
                Undo.DestroyObjectImmediate(root);
            }
            else
            {
                Logger.LogWarning("Conversion not possible.");
            }
        }

        [MenuItem("GameObject/UI/Settings Generator/Convert To/Slider", priority = 2001)]
        public static void ReplaceWithSlider()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var objects = Selection.gameObjects;
            foreach (var go in objects)
            {
                ReplaceObjWithSlider(go);
            }
        }

        [MenuItem("GameObject/UI/Settings Generator/Convert To/Slider Console", priority = 2001)]
        public static void ReplaceWithSliderConsole()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var objects = Selection.gameObjects;
            foreach (var go in objects)
            {
                ReplaceObjWithSliderConsole(go);
            }
        }

        public static void ReplaceObjWithSlider(GameObject target)
        {
            replaceObjWithSlider(target, SliderPrefabName);
        }
        
        public static void ReplaceObjWithSliderConsole(GameObject target)
        {
            replaceObjWithSlider(target, SliderConsolePrefabName);
        }

        private static void replaceObjWithSlider(GameObject target, string prefabName)
        {
            getBasicInfo(target, out string id, out SettingsProvider settingsProvider, out SettingResolver resolver, out GameObject root, out bool isPrefab);

            // Can we convert?
            var sliderResolver = resolver as SliderUGUIResolver;

            if (sliderResolver == null)
            {
                Logger.LogWarning("Conversion not possible.");
                return;
            }

            GameObject newGo = createUI<SliderUGUIResolver>(id, settingsProvider, root, prefabName, undo: true);

            // Copy data
            var newGUI = newGo.GetComponent<SliderUGUI>();
            var oldGUI = sliderResolver.gameObject.GetComponent<SliderUGUI>();
            newGUI.WholeNumbers = oldGUI.WholeNumbers;
            newGUI.MinValue = oldGUI.MinValue;
            newGUI.MaxValue = oldGUI.MaxValue;
            newGUI.Value = oldGUI.Value;
            Undo.DestroyObjectImmediate(root);
        }

        [MenuItem("GameObject/UI/Settings Generator/Convert To/Toggle Console", priority = 2001)]
        public static void ReplaceWithToggleConsole()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var objects = Selection.gameObjects;
            foreach (var go in objects)
            {
                ReplaceObjWithToggleConsole(go);
            }
        }

        [MenuItem("GameObject/UI/Settings Generator/Convert To/Toggle", priority = 2001)]
        public static void ReplaceWithToggle()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var objects = Selection.gameObjects;
            foreach (var go in objects)
            {
                ReplaceObjWithToggle(go);
            }
        }

        public static void ReplaceObjWithToggle(GameObject target)
        {
            replaceObjWithToggle(target, TogglePrefabName);
        }

        public static void ReplaceObjWithToggleConsole(GameObject target)
        {
            replaceObjWithToggle(target, ToggleConsolePrefabName);
        }

        private static void replaceObjWithToggle(GameObject target, string prefabName)
        {
            getBasicInfo(target, out string id, out SettingsProvider settingsProvider, out SettingResolver resolver, out GameObject root, out bool isPrefab);

            // Can we convert?
            var toggleResolver = resolver as ToggleUGUIResolver;

            if (toggleResolver == null)
            {
                Logger.LogWarning("Conversion not possible.");
            }

            if (toggleResolver != null)
            {
                GameObject newGo = createUI<ToggleUGUIResolver>(id, settingsProvider, root, prefabName, undo: true);

                // Copy data
                var newGUI = newGo.GetComponent<ToggleUGUI>();
                var oldGUI = toggleResolver.gameObject.GetComponent<ToggleUGUI>();
                newGUI.Value = oldGUI.Value;
                Undo.DestroyObjectImmediate(root);
            }
        }

        [MenuItem("GameObject/UI/Settings Generator/Convert To/InputKey Console", priority = 2001)]
        public static void ReplaceWithInputBindingConsole()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var objects = Selection.gameObjects;
            foreach (var go in objects)
            {
                ReplaceObjWithInputKeyConsole(go);
            }
        }

        [MenuItem("GameObject/UI/Settings Generator/Convert To/InputKey", priority = 2001)]
        public static void ReplaceWithInputKey()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var objects = Selection.gameObjects;
            foreach (var go in objects)
            {
                ReplaceObjWithInputKey(go);
            }
        }

        public static void ReplaceObjWithInputKey(GameObject target)
        {
            replaceObjWithInputKey(target, InputKeyPrefabName);
        }
        
        public static void ReplaceObjWithInputKeyConsole(GameObject target)
        {
            replaceObjWithInputKey(target, InputKeyConsolePrefabName);
        }

        private static void replaceObjWithInputKey(GameObject target, string prefabName)
        {
            getBasicInfo(target, out string id, out SettingsProvider settingsProvider, out SettingResolver resolver, out GameObject root, out bool isPrefab);

            // Can we convert?
            var toggleResolver = resolver as InputKeyUGUIResolver;

            if (toggleResolver == null)
            {
                Logger.LogWarning("Conversion not possible.");
            }

            if (toggleResolver != null)
            {
                GameObject newGo = createUI<InputKeyUGUIResolver>(id, settingsProvider, root, prefabName, undo: true);

                // Copy data
                var newGUI = newGo.GetComponent<InputKeyUGUI> ();
                var oldGUI = toggleResolver.gameObject.GetComponent<InputKeyUGUI>();
                newGUI.Key = oldGUI.Key;
                newGUI.ModifierKey = oldGUI.ModifierKey;
                Undo.DestroyObjectImmediate(root);
            }
        }

        [MenuItem("GameObject/UI/Settings Generator/Convert To/Stepper", priority = 2001)]
        public static void ReplaceWithStepper()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var objects = Selection.gameObjects;
            foreach (var go in objects)
            {
                ReplaceObjWithStepper(go, StepperPrefabName);
            }
        }

        [MenuItem("GameObject/UI/Settings Generator/Convert To/StepperTwoButtons Console", priority = 2001)]
        public static void ReplaceWithStepperTwoButtonsConsole()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var objects = Selection.gameObjects;
            foreach (var go in objects)
            {
                ReplaceObjWithStepper(go, StepperTwoButtonsConsolePrefabName);
            }
        }

        [MenuItem("GameObject/UI/Settings Generator/Convert To/StepperOneButton Console", priority = 2001)]
        public static void ReplaceWithStepperOneButtonConsole()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var objects = Selection.gameObjects;
            foreach (var go in objects)
            {
                ReplaceObjWithStepper(go, StepperOneButtonConsolePrefabName);
            }
        }

        [MenuItem("GameObject/UI/Settings Generator/Convert To/StepperOneButtonSteps Console", priority = 2001)]
        public static void StepperOneButtonStepsConsole()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var objects = Selection.gameObjects;
            foreach (var go in objects)
            {
                ReplaceObjWithStepper(go, StepperOneButtonStepsConsolePrefabName);
            }
        }

        public static void ReplaceObjWithStepper(GameObject target, string prefabName)
        {
            getBasicInfo(target, out string id, out SettingsProvider settingsProvider, out SettingResolver resolver, out GameObject root, out bool isPrefab);

            // Can we convert?
            var stepperResolver = resolver as StepperUGUIResolver;

            if (stepperResolver == null)
            {
                Logger.LogWarning("Conversion not possible.");
            }

            if (stepperResolver != null)
            {
                GameObject newGo = createUI<StepperUGUIResolver>(id, settingsProvider, root, prefabName, undo: true);

                // Copy data
                var newGUI = newGo.GetComponent<StepperUGUI>();
                var oldGUI = stepperResolver.gameObject.GetComponent<StepperUGUI>();
                newGUI.DisableButtons = oldGUI.DisableButtons;
                newGUI.EnableButtonControls = oldGUI.EnableButtonControls;
                newGUI.ValueFormat = oldGUI.ValueFormat;
                newGUI.WholeNumbers = oldGUI.WholeNumbers;
                newGUI.StepSize = oldGUI.StepSize;
                newGUI.MinValue = oldGUI.MinValue;
                newGUI.MaxValue = oldGUI.MaxValue;
                newGUI.Value = oldGUI.Value;
                newGUI.Refresh();
                Undo.DestroyObjectImmediate(root);
            }
        }


        [MenuItem("GameObject/UI/Settings Generator/Convert To/Texfield", priority = 2001)]
        public static void ReplaceWithTextfield()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var objects = Selection.gameObjects;
            foreach (var go in objects)
            {
                ReplaceObjWithTextfield(go, TextfieldPrefabName);
            }
        }

        [MenuItem("GameObject/UI/Settings Generator/Convert To/Textfield Console", priority = 2001)]
        public static void ReplaceWithTextfieldConsole()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var objects = Selection.gameObjects;
            foreach (var go in objects)
            {
                ReplaceObjWithTextfield(go, TextfieldConsoleUGUIPrefabName);
            }
        }

        public static void ReplaceObjWithTextfield(GameObject target, string prefabName)
        {
            getBasicInfo(target, out string id, out SettingsProvider settingsProvider, out SettingResolver resolver, out GameObject root, out bool isPrefab);

            // Can we convert?
            var textfieldResolver = resolver as TextfieldUGUIResolver;

            if (textfieldResolver == null)
            {
                Logger.LogWarning("Conversion not possible.");
            }

            if (textfieldResolver != null)
            {
                GameObject newGo = createUI<TextfieldUGUIResolver>(id, settingsProvider, root, prefabName, undo: true);

                // Copy data
                var newGUI = newGo.GetComponent<TextfieldUGUI>();
                var oldGUI = textfieldResolver.gameObject.GetComponent<TextfieldUGUI>();
                newGUI.Text = oldGUI.Text;
                Undo.DestroyObjectImmediate(root);
            }
        }


        [MenuItem("GameObject/UI/Settings Generator/Convert To/ColorPicker", priority = 2001)]
        public static void ReplaceWithColorPicker()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var objects = Selection.gameObjects;
            foreach (var go in objects)
            {
                ReplaceObjWithColorPicker(go, ColorPickerPrefabName);
            }
        }

        [MenuItem("GameObject/UI/Settings Generator/Convert To/ColorPicker Console", priority = 2001)]
        public static void ReplaceWithColorPickerConsole()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var objects = Selection.gameObjects;
            foreach (var go in objects)
            {
                ReplaceObjWithColorPicker(go, ColorPickerConsolePrefabName);
            }
        }

        public static void ReplaceObjWithColorPicker(GameObject target, string prefabName)
        {
            getBasicInfo(target, out string id, out SettingsProvider settingsProvider, out SettingResolver resolver, out GameObject root, out bool isPrefab);

            // Can we convert?
            var textfieldResolver = resolver as ColorPickerUGUIResolver;

            if (textfieldResolver == null)
            {
                Logger.LogWarning("Conversion not possible.");
            }

            if (textfieldResolver != null)
            {
                GameObject newGo = createUI<ColorPickerUGUIResolver>(id, settingsProvider, root, prefabName, undo: true);

                // Copy data
                var newGUI = newGo.GetComponent<ColorPickerUGUI>();
                var oldGUI = textfieldResolver.gameObject.GetComponent<ColorPickerUGUI>();
                newGUI.SelectedIndex = oldGUI.SelectedIndex;
                var oldColorButtons = oldGUI.GetComponentsInChildren<ColorPickerButtonUGUI>(includeInactive: true);
                var newColorButtons = newGUI.GetComponentsInChildren<ColorPickerButtonUGUI>(includeInactive: true);
                int range = Mathf.Max(oldColorButtons.Length, newColorButtons.Length);
                var template = newColorButtons[0];
                var prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(template);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                for (int i = 0; i < range; i++)
                {
                    if (i < oldColorButtons.Length && i < newColorButtons.Length)
                    {
                        // Copy color
                        Undo.RegisterCompleteObjectUndo(newColorButtons[i], "SettingsGeneratorPrefabStyleConverter.Create.CopyColor");
                        newColorButtons[i].Color = oldColorButtons[i].Color;
                    }
                    else if(i < oldColorButtons.Length && i >= newColorButtons.Length)
                    {
                        // Add new instance
                        var newColorObject = PrefabUtility.InstantiatePrefab(prefab, template.transform.parent) as GameObject;
                        PrefabUtility.SetPropertyModifications(newColorObject, PrefabUtility.GetPropertyModifications(template));
                        var newColorButton = newColorObject.GetComponent<ColorPickerButtonUGUI>();
                        newColorButton.Color = oldColorButtons[i].Color;

                        Undo.RegisterCreatedObjectUndo(newColorObject, "SettingsGeneratorPrefabStyleConverter.Create.ColorButton");
                        Undo.RegisterFullObjectHierarchyUndo(newColorObject, "SettingsGeneratorPrefabStyleConverter.Modify.ColorButton");
                    }
                }

                Undo.DestroyObjectImmediate(root);
            }
        }

    }
}
#endif