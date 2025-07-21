using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public static class CompatibilityUtils
    {
        public static T FindObjectOfType<T>(bool includeInactive = false) where T : UnityEngine.Object
        {
#if UNITY_2023_1_OR_NEWER
            return GameObject.FindFirstObjectByType<T>(includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude);
#else
            return GameObject.FindObjectOfType<T>(includeInactive);
#endif
        }
        
        public static T[] FindObjectsOfType<T>(bool includeInactive = false) where T : UnityEngine.Object
        {
#if UNITY_2023_1_OR_NEWER
            var include = includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude;
            return GameObject.FindObjectsByType<T>(include, FindObjectsSortMode.None);
#else
            return GameObject.FindObjectsOfType<T>(includeInactive);
#endif
        }
    }
}