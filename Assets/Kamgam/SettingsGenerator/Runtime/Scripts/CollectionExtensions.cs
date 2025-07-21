using System.Collections.Generic;
using System.Collections;
using System.Data.Common;

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// Some really basic collection extension to shorten all sorts of checks.
    /// </summary>
    public static class CollectionExtensions
    {
        public static bool IsNull(this string text) // Okay, this is not a collection.
        {
            return string.IsNullOrEmpty(text);
        }

        public static bool IsNull(this ICollection list)
        {
            return list == null;
        }

        public static bool IsNullOrEmpty(this ICollection list)
        {
            return list == null || list.Count == 0;
        }

        public static bool IsNullOrEmpty(this IEnumerable source)
        {
            if (source != null)
                foreach (var _ in source) // IEnumerable has no count but this does the trick.
                    return false;

            return true;
        }

        public static bool HasValuesThatAreNotNull(this IEnumerable source)
        {
            if (source.IsNullOrEmpty())
                return false;

            foreach (var val in source)
            {
                if (val != null)
                    return true;
            }

            return false;
        }

        public static bool IsIndexOutOfBounds(this ICollection list, int index)
        {
            return index < 0 || index >= list.Count;
        }
        
        public static void RemoveRange(this IList list, IEnumerable collection)
        {
            for (int i = list.Count-1; i >= 0; i--)
            {
                foreach (var item in collection)
                {
                    if (item == list[i])
                    {
                        list.RemoveAt(i);
                        break;
                    }
                }
            }
        }
        
        public static void AddIfNotContained(this IList list, IEnumerable collection)
        {
            foreach (var item in collection)
            {
                if (!list.Contains(item))
                {
                    list.Add(item);
                }
            }
        }
        
        public static void AddIfNotContained<T>(this IList<T> list, T item)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
            }
        }
    }
}
