using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// A Singleton that check the screen size every frame and offers an OnScreenSizeChanged event to register to.
    /// </summary>
    public class ScreenSizeObserver : MonoBehaviour
    {
        private static ScreenSizeObserver _instance;
        public static ScreenSizeObserver Instance
        {
            get
            {
                if (!_instance)
                {
#if UNITY_EDITOR
                    // Keep the instance null outside of play mode to avoid leaking
                    // instances into the scene.
                    if (!UnityEditor.EditorApplication.isPlaying)
                    {
                        return null;
                    }
#endif
                    _instance = new GameObject().AddComponent<ScreenSizeObserver>();
                    _instance.name = _instance.GetType().ToString();
#if UNITY_EDITOR
                    _instance.hideFlags = HideFlags.DontSave;
                    if (UnityEditor.EditorApplication.isPlaying)
                    {
#endif
                        DontDestroyOnLoad(_instance.gameObject);
#if UNITY_EDITOR
                    }
#endif
                }
                return _instance;
            }
        }

        public delegate void OnScreenSizeChangedDelegate(Resolution resolution);
        public OnScreenSizeChangedDelegate OnScreenSizeChanged;

        private int _lastScreenWidth;
        private int _lastScreenHeight;

        public void OnEnable()
        {
            // Don't trigger the event but update memory.
            _lastScreenWidth = Screen.width;
            _lastScreenHeight = Screen.height;
        }

        public void Update()
        {
            if (_lastScreenWidth != Screen.width || _lastScreenHeight != Screen.height)
            {
                _lastScreenWidth = Screen.width;
                _lastScreenHeight = Screen.height;

                var resolution = new Resolution();
                // We can not use Screen.currentResolution because in windowed mode that is
                // always the FULLSCREEN resolution not the window resolution.
                resolution.width = Screen.width;
                resolution.height = Screen.height;
#if UNITY_2022_2_OR_NEWER
                resolution.refreshRateRatio = Screen.currentResolution.refreshRateRatio;
#else
                resolution.refreshRate = Screen.currentResolution.refreshRate;
#endif
                OnScreenSizeChanged?.Invoke(resolution);
            }
        }
    }
}
