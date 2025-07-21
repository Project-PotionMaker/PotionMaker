using System;
using System.Collections.Generic;
using Kamgam.UGUIComponentsForSettings;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [System.Serializable]
    public class SettingData
    {
        /// <summary>
        /// Thou SHALL NOT CHANGE these values.
        /// The int values of this enum are used for serialization and therefore
        /// you should not change them unless you have some very good reason.
        /// </summary>
        public enum DataType {
            Unknown = 0,
            Int = 1,
            Float = 2,
            Bool = 3,
            String = 4,
            Color = 5,
            KeyCombination = 6,
            Option = 7,
            ColorOption = 8
        }
        
        /// <summary>
        /// Conversion table from settings data type to System.Type.
        /// </summary>
        public static Dictionary<DataType, Type> Types = new Dictionary<DataType, Type>()
        {
            { DataType.Unknown, null },
            { DataType.Int, typeof(int) },
            { DataType.Float, typeof(float) },
            { DataType.Bool, typeof(bool) },
            { DataType.String, typeof(string) },
            { DataType.Color, typeof(Color) },
            { DataType.KeyCombination, typeof(KeyCombination) },
            { DataType.Option, typeof(int) },
            { DataType.ColorOption, typeof(int) }
        };

        /// <summary>
        /// All types per setting data type that can be used via implicit casting.
        /// </summary>
        public static Dictionary<DataType, List<Type>> CompatibleTypes = new Dictionary<DataType, List<Type>>()
        {
            { DataType.Unknown, new List<Type> { } },
            { DataType.Int, new List<Type> { typeof(int), typeof(float) } },
            { DataType.Float, new List<Type> { typeof(float) } },
            { DataType.Bool, new List<Type> { typeof(bool) } },
            { DataType.String, new List<Type> { typeof(string) } },
            { DataType.Color, new List<Type> { typeof(Color), typeof(Color32) } },
            { DataType.KeyCombination, new List<Type> { typeof(KeyCombination) } },
            { DataType.Option, new List<Type> { typeof(int) } },
            { DataType.ColorOption, new List<Type> { typeof(int) } }
        };

        public string ID;
        public DataType Type;

        [SerializeField]
        public int[] IntValues;

        [SerializeField]
        public float[] FloatValues;

        [SerializeField]
        public string[] StringValues;

        public SettingData(string path, DataType type, int[] intValues, float[] floatValues, string[] stringValues) : this(path, type)
        {
            IntValues = intValues;
            FloatValues = floatValues;
            StringValues = stringValues;
        }

        public SettingData(string path, DataType type)
        {
            ID = path;
            Type = type;
        }
    }
}
