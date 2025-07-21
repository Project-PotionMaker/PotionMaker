using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public static class PrefabUtils
    {
        public static List<GameObject> GetPrefabVariantParents(GameObject prefab)
        {
            List<GameObject> hierarchy = new List<GameObject>();

            GameObject current = prefab;
            while (true)
            {
                var basePrefab = PrefabUtility.GetCorrespondingObjectFromSource(current);

                if (basePrefab == null || basePrefab == current)
                    break;

                hierarchy.Add(basePrefab);
                current = basePrefab;
            }

            return hierarchy;
        }
    }
}