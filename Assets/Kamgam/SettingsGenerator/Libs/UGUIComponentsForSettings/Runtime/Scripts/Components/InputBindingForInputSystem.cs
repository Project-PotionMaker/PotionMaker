using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.Events;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
#endif

namespace Kamgam.UGUIComponentsForSettings
{
    /// <summary>
    /// Implements the input binding interface for the new Unity InputSystem package:
    /// https://docs.unity3d.com/Packages/com.unity.inputsystem@1.5
    ///<br />
    /// It handles composite Bindings by concatenating multiple binding paths with a special character
    /// (CompositeControlSeparator). This path is then returned as the binding path. The InputBindingConnection
    /// Get()/Set() methods know how to interpret these.
    /// </summary>
    [System.Serializable]
    public class InputBindingForInputSystem : IInputBindingForGUI
    {
        public enum LocalConfigBehaviours { OverrideGlobalIfLocalExists, AppendLocalToGlobal };

        /// <summary>
        /// Character that separates binding paths in key combinations.
        /// </summary>
        public static char CompositeControlSeparator = '+';
        
        /// <summary>
        /// Time the input waits for another key in a key combo.
        /// </summary>
        public static float WaitForKeyComboDuration = 0.3f;

        /// <summary>
        /// Used if no local ignore paths are defined.
        /// </summary>
        public static string[] GlobalIgnoreControlPaths = new string[] {
             "<Pointer>/position"           // Don't bind to mouse position
            ,"<Pointer>/delta"              // Don't bind to mouse movement deltas
            ,"<Pointer>/{PrimaryAction}"    // Don't bind to controls such as leftButton and taps.
            ,"<Mouse>/clickCount"           // Don't bind to mouse click count envents
        };

        /// <summary>
        /// Used if no local ignore paths are defined.
        /// </summary>
        public static string[] GlobalAbortControlPaths = new string[] {
            "<Keyboard>/escape",
            "<Gamepad>/start"
        };
        
        /// <summary>
        /// Used if no local ignore paths are defined.
        /// </summary>
        public static string GlobalControlsHavingToMatchPath;
        
        [Tooltip("Defines how the paths configs should be combined with the GLOBAL path configs. See IputBindingUGUI.cs")]
        public LocalConfigBehaviours LocalConfigBehaviour = LocalConfigBehaviours.AppendLocalToGlobal;

        /// <summary>
        /// Local ignore control paths. These are handed to RebindingOperation.WithControlsExcluding().
        /// See: https://docs.unity3d.com/Packages/com.unity.inputsystem@1.5/api/UnityEngine.InputSystem.InputActionRebindingExtensions.RebindingOperation.html#methods
        /// </summary>
        [Tooltip("Local ignore control paths. These are handed to RebindingOperation.WithControlsExcluding().")]
        public string[] IgnoreControlPaths = new string[] { };

        /// <summary>
        /// Local abort control paths. These are handed to RebindingOperation.WithCancelingThrough().<br />
        /// See: https://docs.unity3d.com/Packages/com.unity.inputsystem@1.5/api/UnityEngine.InputSystem.InputActionRebindingExtensions.RebindingOperation.html#methods
        /// </summary>
        [Tooltip("Local abort control paths. These are handed to RebindingOperation.WithCancelingThrough().")]
        public string[] AbortControlPaths = new string[] { };

        /// <summary>
        /// Used to limit the controls which react to this input.<br />
        /// Example: Set to "&lt;Keyboard&gt;" to limit to keyboard inputs only.<br />
        /// DO NOT USE: This will be removed in the next major upgrade! Use the MatchControlPaths list instead.
        /// </summary>
        // TODO (in next major release): Remove because we are now using MatchControlPaths list instead.
        public string ControlsHavingToMatchPath;

        [Tooltip("Local match paths. Used to limit the controls which react to this input.\n" +
            "Example: Set to <Keyboard>/* to limit to keyboard inputs only or <Gamepad>/<Button> for gamepad buttons.")]
        public string[] MatchControlPaths = new string[] { };

        [Tooltip("At runtime this is the selected binding path.")]
        [SerializeField]
        protected string _bindingPath = "<Keyboard>/space";

        [System.NonSerialized]
        public bool AllowComposite = false;

        public delegate bool CheckBindingPathDelegate(string previousPath, string path);
        /// <summary>
        /// A function that will be called before the binding path is finally assigned. Use this to check binding paths for duplicates. Return false if you want to cancel the binding change.
        /// </summary>
        public CheckBindingPathDelegate CheckBindingPathFunc = null;

        public void CopyFrom(InputBindingForInputSystem other)
        {
            LocalConfigBehaviour = other.LocalConfigBehaviour;
            Array.Copy(other.IgnoreControlPaths, IgnoreControlPaths, other.IgnoreControlPaths.Length);
            Array.Copy(other.AbortControlPaths, AbortControlPaths, other.AbortControlPaths.Length);
            Array.Copy(other.AbortControlPaths, AbortControlPaths, other.AbortControlPaths.Length);
            Array.Copy(other.MatchControlPaths, MatchControlPaths, other.MatchControlPaths.Length);
            ControlsHavingToMatchPath = other.ControlsHavingToMatchPath;
            _bindingPath = other._bindingPath;
            AllowComposite = other.AllowComposite;
            CheckBindingPathFunc = other.CheckBindingPathFunc;
                
#if ENABLE_INPUT_SYSTEM
            OnBeforeRebindStart = other.OnBeforeRebindStart;
            OnComplete = other.OnComplete;
            OnCanceled = other.OnCanceled;
#endif
        }

#if ENABLE_INPUT_SYSTEM
        public string GetBindingPath()
        {
            return _bindingPath;
        }

        public void SetBindingPath(string path)
        {
            if (CheckBindingPathFunc != null && path != _bindingPath)
            {
                if (CheckBindingPathFunc.Invoke(_bindingPath, path))
                {
                    _bindingPath = path;
                }
            }
            else
            {
                _bindingPath = path;
            }
        }

        public delegate System.Action OnBeforeRebindStartDelegate(InputActionRebindingExtensions.RebindingOperation rebindingOperation);

        /// <summary>
        /// Use this to change the configuration (excludes/includes) of the rebind.<br />
        /// Is called on the RebindingOperation right before it is started.
        /// </summary>
        public OnBeforeRebindStartDelegate OnBeforeRebindStart;

        /// <summary>
        /// Is called after a new input was bound.
        /// </summary>
        public event System.Action OnComplete;

        public void AddOnCompleteCallback(System.Action callback)
        {
            OnComplete += callback;
        }

        public void RemoveOnCompleteCallback(System.Action callback)
        {
            OnComplete -= callback;
        }

        /// <summary>
        /// Is called after a new input was bound.
        /// </summary>
        public event System.Action OnCanceled;

        public void AddOnCanceledCallback(System.Action callback)
        {
            OnCanceled += callback;
        }

        public void RemoveOnCanceledCallback(System.Action callback)
        {
            OnCanceled -= callback;
        }

        protected InputActionRebindingExtensions.RebindingOperation _rebindingOperation;

        public void StartListening()
        {
            // See: https://forum.unity.com/threads/after-alt-tabbing-the-left-alt-key-state-is-faulty.1367766/
            InputUtils.ResetStuckKeyStates();

            _rebindingOperation = new InputActionRebindingExtensions.RebindingOperation();

            // Limit possible inputs
            var ignoreControlPaths = resolveConfigStrings(GlobalIgnoreControlPaths, IgnoreControlPaths);
            foreach (var ignore in ignoreControlPaths)
            {
                _rebindingOperation.WithControlsExcluding(ignore);
            }

            var abortControlPaths = resolveConfigStrings(GlobalAbortControlPaths, AbortControlPaths);
            _rebindingOperation.OnPotentialMatch(operation =>
            {
                if (InputSystem.version.CompareTo(System.Version.Parse("1.4.1")) < 0)
                {
                    // We need a workaround for InputSystem < 1.4.1 due to this bug
                    // https://forum.unity.com/threads/withcancelingthrough-keyboard-escape-also-cancels-with-keyboard-e.1233400/
                    foreach (var cancelPath in abortControlPaths)
                    {
                        // Convert paths like "<Keyboard>/Escape" to "keyboard/escape".
                        string path = cancelPath;
                        if (path[0] == '<') path = "/" + path;
                        path = Regex.Replace(path, "[><{}*]+", "");
                        path = path.ToLower();

                        string controlPath = operation.selectedControl.path.ToLower();

                        // We'll only fix the diverging endings here.
                        // All other cases may still fail but the users should upgrade
                        // to InputSystem 1.4 anyways.
                        // If path ends with a long word (like ../escape) but controlPath
                        // ends short (like .../e) then ignore the comparison.
                        int controlPathIndex = controlPath.IndexOf("/");
                        int controlPathDelta = controlPath.Length - controlPathIndex;
                        int pathIndex = path.IndexOf("/");
                        int pathDelta = path.Length - pathIndex;

                        if (Mathf.Abs(controlPathDelta - pathDelta) > 1)
                        {
                            continue;
                        }

                        if (InputControlPath.Matches(cancelPath, operation.selectedControl))
                        {
                            _rebindingOperation.Cancel();
                            break;
                        }
                    }
                }
                else
                {
                    foreach (var cancelPath in abortControlPaths)
                    {
                        if (InputControlPath.Matches(cancelPath, operation.selectedControl))
                        {
                            _rebindingOperation.Cancel();
                            break;
                        }
                    }
                }
            });

            foreach (var matchPath in MatchControlPaths)
            {
                if (!string.IsNullOrEmpty(matchPath))
                {
                    _rebindingOperation.WithControlsHavingToMatchPath(matchPath);
                }
            }

            // TODO (in next major release): Remove ControlsHavingToMatchPath because we are now using MatchControlPaths list instead.
            var controlsMatch = resolveConfigString(GlobalControlsHavingToMatchPath, ControlsHavingToMatchPath);
            if (!string.IsNullOrEmpty(controlsMatch))
            {
                _rebindingOperation.WithControlsHavingToMatchPath(controlsMatch);
            }

            // How long to wait for other (modifier) key to be pressed.
            _rebindingOperation.OnMatchWaitForAnother(AllowComposite ? WaitForKeyComboDuration : 0.1f);

            // register complete action
            _rebindingOperation.OnApplyBinding((rebindingOp, path) =>
            {
                if (AllowComposite)
                {
                    // If more than one single input is pressed then construct a composition path.
                    string multiplePaths = "";
                    // Glue the paths of all the pressed inputs together.
                    int numOfPressedInputs = 0;
                    foreach (var candidate in rebindingOp.candidates)
                    {
                        // Skip all controls except KeyControls
                        // TODO: Expand this for other input types (currently only keyboard is supported).
                        if (candidate is AnyKeyControl || (!(candidate is KeyControl) &&
                                                           !(candidate is ButtonControl &&
                                                             candidate.path.Contains("Mouse"))))
                            continue;

                        numOfPressedInputs++;

                        // Prepare path
                        // The paths are listed as "/Keyboard/a" or similar but the official format is "<Keyboard>/a".
                        // While it works with both we still convert it here to make translations easier because the
                        // paths are used by the localization and different paths would make translations cumbersome.
                        string candidatePath = candidate.path.Replace("/Keyboard", "<Keyboard>");
                        candidatePath = candidatePath.Replace("/Mouse", "<Mouse>");

                        // Append to paths.
                        if (string.IsNullOrEmpty(multiplePaths))
                        {
                            multiplePaths += candidatePath;
                        }
                        else
                        {
                            multiplePaths += CompositeControlSeparator + candidatePath;
                        }
                    }

                    // Ensure working event if candidates check fails (happened once during testing).
                    if (string.IsNullOrEmpty(multiplePaths))
                        multiplePaths = path;
                    
                    path = numOfPressedInputs > 1 ? multiplePaths : path;
                }

                rebindingOp.Dispose();
                _rebindingOperation = null;

                SetBindingPath(path);
                OnComplete?.Invoke();
            });

            _rebindingOperation.OnCancel((rebindingOp) =>
            {
                rebindingOp.Dispose();
                _rebindingOperation = null;

                OnCanceled?.Invoke();
            });

            OnBeforeRebindStart?.Invoke(_rebindingOperation);

            // start the rebind
            _rebindingOperation.Start();
        }

        protected string[] resolveConfigStrings(string[] globals, string[] locals)
        {
            if (LocalConfigBehaviour == LocalConfigBehaviours.OverrideGlobalIfLocalExists)
            {
                return (locals == null || locals.Length == 0) ? globals : locals;
            }
            else if (LocalConfigBehaviour == LocalConfigBehaviours.AppendLocalToGlobal)
            {
                if (globals == null)
                    return locals;

                var configs = new List<string>(globals);
                if (locals != null && locals.Length > 0)
                    configs.AddRange(locals);
                return configs.ToArray();
            }

            return new string[] { };
        }

        protected string resolveConfigString(string global, string local)
        {
            if (LocalConfigBehaviour == LocalConfigBehaviours.OverrideGlobalIfLocalExists)
            {
                return string.IsNullOrEmpty(local) ? global : local;
            }
            else if (LocalConfigBehaviour == LocalConfigBehaviours.AppendLocalToGlobal)
            {
                if (global == null)
                    return local;

                var config = global;
                if (!string.IsNullOrEmpty(local))
                    config += local;

                return config;
            }

            return null;
        }

        public void OnEnable()
        {
        }

        public void OnDisable()
        {
            if (_rebindingOperation != null)
            {
                _rebindingOperation.Cancel();
                if (_rebindingOperation != null)
                {
                    _rebindingOperation.Dispose();
                    _rebindingOperation = null;
                }
            }
        }
#else
        public string GetBindingPath()
        {
            return _bindingPath;
        }

        public void SetBindingPath(string path)
        {
            _bindingPath = path;
        }

        public void AddOnCompleteCallback(System.Action callback)
        {
        }

        public void RemoveOnCompleteCallback(System.Action callback)
        {
        }

        public void AddOnCanceledCallback(System.Action callback)
        {
        }

        public void RemoveOnCanceledCallback(System.Action callback)
        {
        }

        public void StartListening()
        {
            Debug.LogWarning("Please install the InputSystem package. See: https://docs.unity3d.com/Packages/com.unity.inputsystem@1.5");
        }

        public void OnEnable()
        {
            Debug.LogWarning("Please install the InputSystem package. See: https://docs.unity3d.com/Packages/com.unity.inputsystem@1.5");
        }

        public void OnDisable()
        {
            Debug.LogWarning("Please install the InputSystem package. See: https://docs.unity3d.com/Packages/com.unity.inputsystem@1.5");
        }
#endif
    }
}
