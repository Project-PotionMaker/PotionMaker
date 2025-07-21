using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public class GameObjectInspector
    {
        private readonly Dictionary<string, object> _components = new();
        private readonly Dictionary<string, (object obj, PropertyInfo)> _properties = new();
        private readonly Dictionary<string, (object obj, FieldInfo)> _fields = new();
        private readonly Dictionary<string, (object obj, MethodInfo)> _getMethods = new();
        private readonly Dictionary<string, (object obj, MethodInfo)> _setMethods = new();
        
        static BindingFlags BindingFlags = BindingFlags.Instance | BindingFlags.Public;

        public GameObject Target;
        
        public GameObjectInspector(GameObject go)
        {
            Target = go;
        }

        public System.Type GetTypeOfPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;
            
            if (   !_properties.TryGetValue(path, out _) 
                && !_fields.TryGetValue(path, out _) 
                && !_getMethods.TryGetValue(path, out _) 
                && !_setMethods.TryGetValue(path, out _))
            {
                GetAndCacheObjectAtPath(path);
            }
            
            if (_properties.TryGetValue(path, out var objAndProp))
            {
                return objAndProp.Item2.PropertyType;
            }
            
            if (_fields.TryGetValue(path, out var objAndField))
            {
                return objAndField.Item2.FieldType;
            }
            
            if (_getMethods.TryGetValue(path, out var objAndGetMethod))
            {
                return objAndGetMethod.Item2.ReturnType;
            }
            
            if (_setMethods.TryGetValue(path, out var objAndSetMethod))
            {
                return objAndSetMethod.Item2.GetParameters()[0].ParameterType;
            }

            return null;
        }

        public void Clear()
        {
            _fields.Clear();
            _properties.Clear();
            _getMethods.Clear();
            _setMethods.Clear();
            _components.Clear();
            Target = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="includeMethods"></param>
        /// <param name="getOrSetMethods">If true then only GET methods are returned. If false then only SET methods are returned.</param>
        /// <param name="compatibleTypes">Used to filter fields and methods based on the types. If null then no type filtering will be done.</param>
        /// <param name="results"></param>
        /// <returns></returns>
        public List<string> GetPaths(string path, bool includeMethods, bool getOrSetMethods, List<Type> compatibleTypes, List<string> results = null)
        {
            try
            {
                if (results == null)
                    results = new List<string>();

                if (string.IsNullOrEmpty(path))
                {
                    return GetComponentPaths(results);
                }
                else
                {
                    if (path.EndsWith(")"))
                        return results;
                    
                    var obj = Get<object>(path);
                    if (obj != null)
                    {
                        return GetMemberPaths(obj, path, includePropsAndFields : true, includeMethods, getOrSetMethods, compatibleTypes, results);
                    }
                    else
                    {
                        return results;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return results;
            }
        }
        

        /// <summary>
        /// Generates a path for each component's members (fields, properties and methods).
        /// </summary>
        /// <param name="results">The paths. Will create a new list if null.</param>
        /// <returns></returns>
        public List<string> GetComponentPaths(List<string> results = null)
        {
            if (Target == null)
                return results;
            
            if (results == null)
                results = new List<string>();
            
            results.Add("gameObject");

            var allComponents = Target.GetComponents<Component>();
            var componentCounts = new Dictionary<Type, int>();
            foreach (var component in allComponents)
            {
                if (component.GetType() == typeof(SettingReceiverGenericConnector))
                    continue;
                
                Type componentType = component.GetType();
                string componentTypeName = componentType.Name;

                // Initialize or increment the component count for this type.
                if (!componentCounts.ContainsKey(componentType))
                    componentCounts[componentType] = 0;
                else
                    componentCounts[componentType]++;

                int componentIndex = componentCounts[componentType];

                string compPath = "";
                if (componentIndex == 0)
                    compPath = $"{componentTypeName}";
                else
                    compPath = $"{componentTypeName}[{componentIndex}]";
                results.Add(compPath);
                
                // Cache
                _components.TryAdd(compPath, component);
            }

            return results;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="path"></param>
        /// <param name="includePropsAndFields"></param>
        /// <param name="includeMethods"></param>
        /// <param name="getOrSetMethods">If true then only GET methods are returned. If false then only SET methods are returned.</param>
        /// <param name="compatibleTypes">Used to filter fields and methods based on the type.</param>
        /// <param name="results"></param>
        /// <returns></returns>
        private List<string> GetMemberPaths(object obj, string path, bool includePropsAndFields, bool includeMethods, bool getOrSetMethods, List<Type> compatibleTypes, List<string> results = null)
        {
            if (results == null)
                results = new List<string>();
            
            Type componentType = obj.GetType();
            
            // Get all fields, properties, and methods of the component.
            var fields = componentType.GetFields(BindingFlags);
            var properties = componentType.GetProperties(BindingFlags);
            var methods = componentType.GetMethods(BindingFlags);

            if (includePropsAndFields)
            {
                // Fields
                // Stop if primitive type as those can not be inspected anymore.
                if (!isInspectableType(obj.GetType()))
                    return results;
                
                foreach (var field in fields)
                {
                    if (Attribute.IsDefined(field, typeof(ObsoleteAttribute)))
                        continue;

                    string fieldPath = $"{path}.{field.Name}";
                    results.Add(fieldPath);

                    _fields.TryAdd(fieldPath, (obj, field)); 
                }

                // Properties
                foreach (var prop in properties)
                {
                    if (Attribute.IsDefined(prop, typeof(ObsoleteAttribute)))
                        continue;

                    if (!prop.CanRead && !prop.CanWrite)
                        continue;

                    string propPath = $"{path}.{prop.Name}";
                    results.Add(propPath);

                    _properties.TryAdd(propPath, (obj, prop));
                }
            }

            // Methods
            if (includeMethods)
            {
                // Stop if method as those can not be inspected anymore.
                if (path.EndsWith(")"))
                    return results;
                
                foreach (var method in methods)
                {
                    if (method.IsSpecialName)
                        continue;

                    if (Attribute.IsDefined(method, typeof(ObsoleteAttribute)))
                        continue;

                    var parameters = method.GetParameters();


                    if (getOrSetMethods)
                    {
                        // GET methods only
                        if (method.GetParameters().Length == 0 && compatibleTypes.Contains(method.ReturnType) || compatibleTypes == null )
                        {
                            string methodPath = $"{path}.{method.Name}()";
                            results.Add(methodPath);

                            _getMethods.TryAdd(methodPath, (obj, method));
                        }
                    }
                    else
                    {
                        // SET methods only
                        if (method.GetParameters().Length == 1)
                        {
                            var paramType = parameters[0].ParameterType;
                            if (compatibleTypes.Contains(paramType))
                            {
                                string methodPath = $"{path}.{method.Name}({paramType.Name})";
                                results.Add(methodPath);

                                _setMethods.TryAdd(methodPath, (obj, method));
                            }
                        }
                    }
                }
            }

            return results;
        }
        
        private static readonly Regex componentIndexRegex = new(@"^(?<type>[a-zA-Z0-9_]+)(\[(?<index>\d+)\])?$");

        public object GetAndCacheObjectAtPath(string path)
        {
            if (string.IsNullOrEmpty(path) || Target == null)
                return null;

            string[] segments = path.Split('.');
            if (segments.Length == 0)
                return null;

            object current = null;
            
            if (path == "gameObject")
            {
                _components.TryAdd(path, Target);
                return Target;
            }
            else if (path.StartsWith("gameObject"))
            {
                current = Target;
            }
            else
            {
                // Handle first segment as component (e.g., MyComponent[0])
                var match = componentIndexRegex.Match(segments[0]);
                if (!match.Success)
                {
                    Debug.LogWarning($"Invalid component segment: '{segments[0]}'");
                    return null;
                }

                int componentIndex = 0; // Default to 0 if index is not provided
                if (match.Groups["index"].Success)
                    componentIndex = int.Parse(match.Groups["index"].Value);

                // Find the component type
                string typeName = match.Groups["type"].Value;
                Type componentType = null;
                var allComponents = Target.GetComponents<Component>();
                foreach (var comp in allComponents)
                {
                    if (comp.GetType().Name == typeName)
                    {
                        componentType = comp.GetType();
                        break;
                    }
                }

                if (componentType == null)
                    return null;

                // Get the components of the correct type and pick by index.
                var components = Target.GetComponents(componentType);

                if (components.Length > componentIndex)
                    current = components[componentIndex];
                else
                    return current;
            }


            // Cache
            if (segments.Length == 1)
            {
                _components.TryAdd(path, current);
                return current;
            }

            // Traverse remaining path segments
            for (int i = 1; i < segments.Length; i++)
            {
                string segment = segments[i];
                Type type = current.GetType();

                // Handle method call
                if (segment.EndsWith(")"))
                {
                    string methodName = segment.Substring(0, segment.LastIndexOf("(")); // Remove the "(...)" part. 
                    if (segment.EndsWith("()"))
                    {
                        MethodInfo method = type.GetMethods(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(
                            m =>
                            {
                                return m.Name == methodName && m.GetParameters().Length == 0;
                            });
                        if (method != null)
                        {
                            // Cache
                            _getMethods.TryAdd(path, (current, method));
                            return current;
                        }
                    }
                    else
                    {
                        string parameterTypeName = segment.Substring(segment.LastIndexOf("(")+1).Trim(')'); // Extract the (TypeName) part.
                        MethodInfo method = type.GetMethods(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(
                            m =>
                            {
                                return m.Name == methodName && m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType.Name == parameterTypeName;
                            });
                        if (method != null)
                        {
                            // Cache
                            _setMethods.TryAdd(path, (current, method));
                            return current;
                        }
                    }
                }
                else
                {
                    // Try property first
                    PropertyInfo prop = type.GetProperty(segment, BindingFlags.Public | BindingFlags.Instance);
                    if (prop != null && prop.CanRead)
                    {
                        // Cache
                        if (i == segments.Length - 1)
                        {
                            // Cache
                            _properties.TryAdd(path, (current, prop));
                            return current;
                        }
                        
                        current = prop.GetValue(current);
                    }
                    else
                    {
                        // Try field
                        FieldInfo field = type.GetField(segment, BindingFlags.Public | BindingFlags.Instance);
                        if (field != null)
                        {
                            // Cache
                            if (i == segments.Length - 1)
                            {
                                // Cache
                                _fields.TryAdd(path, (current, field));
                                return current;
                            }
                            
                            current = field.GetValue(current);
                        }
                    }
                }

                if (current == null)
                    return null;
            }

            return current;
        }
        
                
        public T Get<T>(string path)
        {
            var valueType = typeof(T);
            
            if (path.EndsWith(")"))
            {
                if (!_getMethods.TryGetValue(path, out _))
                {
                    GetAndCacheObjectAtPath(path);
                }

                if (_getMethods.TryGetValue(path, out var objAndMethod))
                {
                    var obj = objAndMethod.Item1;
                    var method = objAndMethod.Item2;
                    return (T) method.Invoke(obj, null);
                }
                else
                {
                    Logger.Log($"No method found at path '{path}'.");   
                }
            }
            else
            {
                if (!_components.TryGetValue(path, out _) && !_properties.TryGetValue(path, out _) && !_fields.TryGetValue(path, out _))
                {
                    GetAndCacheObjectAtPath(path);
                }
                
                if (_components.TryGetValue(path, out var comp))
                {
                    return (T) comp;
                }
                else if (_properties.TryGetValue(path, out var objAndProp))
                {
                    var obj = objAndProp.Item1;
                    var prop = objAndProp.Item2;
                    return (T) prop.GetValue(obj);
                }
                else if (_fields.TryGetValue(path, out var objAndField))
                {
                    var obj = objAndField.Item1;
                    var field = objAndField.Item2;
                    return (T) field.GetValue(obj);
                }
                else
                {
                    Logger.Log($"No field or property found at path '{path}'.");
                }
            }

            return default(T);
        }


        public void Set<T>(string path, T value)
        {
            Type valueType = typeof(T);

            if (path.EndsWith(")"))
            {
                if (!_setMethods.TryGetValue(path, out _))
                {
                    GetAndCacheObjectAtPath(path);
                }

                if (_setMethods.TryGetValue(path, out var objAndMethod))
                {
                    var obj = objAndMethod.Item1;
                    var method = objAndMethod.Item2;
                    method.Invoke(obj, new object[] { value });
                }
                else
                {
                    Logger.Log($"No method found at path '{path}' with parameter type {valueType.Name}");   
                }
            }
            else
            {
                if (!_components.TryGetValue(path, out _) && !_properties.TryGetValue(path, out _) && !_fields.TryGetValue(path, out _))
                {
                    GetAndCacheObjectAtPath(path);
                }
                
                if (_properties.TryGetValue(path, out var objAndProp))
                {
                    var obj = objAndProp.Item1;
                    var prop = objAndProp.Item2;

                    prop.SetValue(obj, value);
                    
                    // If it is a struct the also re-assign the struct to the parent.
                    if (isStruct(obj))
                    {
                        var parentPath = getParentPath(path);
                        Set(parentPath, obj);
                    }
                }
                else if (_fields.TryGetValue(path, out var objAndField))
                {
                    var obj = objAndField.Item1;
                    var field = objAndField.Item2;
                    
                    field.SetValue(obj, value);
                    
                    // If it is a struct the also re-assign the struct to the parent.
                    if (isStruct(obj))
                    {
                        var parentPath = getParentPath(path);
                        Set(parentPath, obj);
                    }
                }
                else
                {
                    Logger.Log($"No field or property found at path '{path}' for type {valueType.Name}");
                }
            }
        }

        private string getParentPath(string path)
        {
            return path.Substring(0, path.LastIndexOf("."));
        }

        private bool isInspectableType(Type type)
        {
            return !type.IsPrimitive && (type.IsClass || (type.IsValueType && !type.IsEnum && type != typeof(string)));
        }
        
        private bool isStruct(object obj)
        {
            var type = obj.GetType();
            return !type.IsPrimitive && !type.IsClass && type.IsValueType && !type.IsEnum && type != typeof(string);
        }
        
        public bool IsSettingCompatibleWithPath(SettingsProvider provider, string settingId, string path, bool defaultResult = true)
        {
            if (provider == null || string.IsNullOrEmpty(path) || string.IsNullOrEmpty(settingId))
                return defaultResult;
            
            var settings = provider.GetSettingsAssetOrRuntimeCopy();
            if (settings == null)
                return defaultResult;
            
            var setting = settings.GetSetting(settingId);
            if (setting == null)
                return defaultResult;
            
            var dataType = setting.GetDataType();
            var compatibleTypes = SettingData.CompatibleTypes[dataType];
            var pathType = GetTypeOfPath(path);

            // This check is redundant for methods but valid for fields and properties.
            return compatibleTypes.Contains(pathType);
        }
    }
}