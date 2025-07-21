using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// Base class for every Setting. Settings are serializable objects
    /// within the Settings Scriptable Object.
    /// </summary>
    [System.Serializable]
    public abstract class SettingWithValue<TValue> : ISettingWithValue<TValue>
    {
        /// <summary>
        /// Was this setting filled with some saved and loaded user data?
        /// </summary>
        [System.NonSerialized]
        protected bool _hasUserData = false;

        [Tooltip("If a settings is disabled then the settings system will ignore it.\n" +
            "This is useful if you want to keep a setting in the list but disable it.\n" +
            "You should not change this at runtime.")]
        [SerializeField]
        protected bool _isActive = true;
        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
#if UNITY_EDITOR
                if (UnityEditor.EditorApplication.isPlaying)
                {
#endif
                    Logger.LogWarning("Changing the IsActive state of settings at runtime is not recommended.");
#if UNITY_EDITOR
                }
#endif
            }
        }

        public string ID;

        public const string _IdFieldName = "ID";

        /// <summary>
        /// If true then any changes made are immediately sent to the connection.<br />
        /// This means you do not have to call Apply() to "apply" the change.<br />
        /// NOTICE: If you disable this on a setting without connection then you may want to use the applied listeners or the OnSettingApplied event to listen for changes.
        /// </summary>
        [Tooltip("If true then any changes made are immediately sent to the connection. If false then you need call Apply() to push the value to the connection. If no connection is set then this does nothing.\n\n" +
                 "NOTICE: If you disable this on a setting without connection then you may want to use the applied listeners or the OnSettingApplied event to listen for changes.")]
        public bool ApplyImmediately = true;

        public virtual ConnectionSO GetConnectionSO()
        {
            return null;
        }
        
        public virtual void SetConnectionSO(ConnectionSO connectionSO) {}

        public virtual SettingData.DataType GetConnectionSettingDataType()
        {
            return SettingData.DataType.Unknown;
        }

        /// <summary>
        /// Settings can be part of groups. This makes it easier to reset only some of them.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("Groups")]
        protected List<string> _groups;

        [SerializeField]
        [DisableIf(propertyName: "ConnectionObject", comparedValue: null, propertyName2: "IgnoreConnectionDefaults", comparedValue2: true, invertBehaviour: true)]
        [Tooltip("The default value which will be used if no user setting data was found (first boot) and if no connection is set." +
            "\n\nNOTICE: If a connection is set then this will be ignored as the value will come from the connection (unless 'IgnoreConnectionDefaults' is enabled).")]
        protected TValue _defaultValue;

        
        /// <summary>
        /// A flag that tells external code if the default value has been initialized. Useful to init the default just once externally.
        /// </summary>
        [System.NonSerialized]
        public bool HasDefaultValue = false;

        [Tooltip("If enabled then the default value will NOT be fetched from the connection but will be set to the default value configured here on this setting.")]
        [DisableIf("ConnectionObject", null, invertBehaviour: false)]
        public bool IgnoreConnectionDefaults = false;

        protected bool _hasChanged = false;
        protected System.Func<string, string> _translateFunc;

        protected List<Action<TValue>> _applyListeners;
        protected List<Action<TValue>> _changeListeners;
        protected List<Action<TValue>> _pulledFromConnectionListeners;
        protected List<Action> _genericPulledFromConnectionListeners;

        /// <summary>
        /// Called whenever the setting values changes, not matter what ApplyImmediately is set to.
        /// </summary>
        public event Action<ISetting> OnSettingChanged;
        
        /// <summary>
        /// If ApplyImmediately is true then this does the same as OnSettingChanged (it's called immediately).<br />
        /// If ApplyImmediately is false then this will be called once the setting is applied via Apply().<br />
        /// Use this if you want to take ApplyImmediately into account for settings without connections.
        /// </summary>
        public event Action<ISetting> OnSettingApplied;

        public SettingWithValue(SettingData data, List<string> groups)
        {
            // Be aware that these constructors are only called if 
            // a new setting is created via script!
            //
            // Usually it will be part of the Settings SO and
            // thus only the default constructor is used.
            // If saved user settings are loaded then only
            // DeserializeValueFromData() is called.

            ID = data.ID;
            DeserializeValueFromData(data);
            _groups = groups;
            ApplyImmediately = true;
        }

        public SettingWithValue(string id, List<string> groups)
        {
            // Be aware that these constructors are only called if 
            // a new setting is created via script (see above)!

            ID = id;
            _groups = groups;
            ApplyImmediately = true;
        }

        public virtual void OnBeforeSerialize(){}

        public virtual void OnAfterDeserialize()
        {
            ID = ID.Trim();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual void InitializeConnection()
        {
            if (HasConnection())
            {
                // Pull the default value from the connection (updates _defaultValue).
                SetDefaultFromConnection((IConnection<TValue>)GetConnectionInterface());

                // If there is no saved user data then load the default value
                // as the initial value of the setting. NOTICE: If IgnoreConnectionDefaults
                // is true then the _defaultValue will still be the default set in the setting
                // itself.
                if (!_hasUserData)
                {
                    SetValue(_defaultValue);
                }
            }
        }

        public string GetID()
        {
            return ID;
        }

        public void SetHasUserData(bool loaded)
        {
            _hasUserData = loaded;
        }

        public bool HasUserData()
        {
            return _hasUserData;
        }

        public abstract SettingData.DataType GetDataType();

        public abstract TValue GetValue();
        public abstract void SetValue(TValue value, bool propagateChange = true);

        public abstract void SetValueFromObject(object value, bool propagateChange = true);

        public bool MatchesID(string id)
        {
            if (string.IsNullOrEmpty(ID) || string.IsNullOrEmpty(id))
                return false;

            return ID == id;
        }

        public virtual void SetDefault(TValue defaultValue)
        {
            _defaultValue = defaultValue;
            HasDefaultValue = true;
        }

        public virtual void SetDefaultFromConnection(IConnection<TValue> connection)
        {
            if (HasConnection() && !IgnoreConnectionDefaults)
            {
                SetDefault(connection.GetDefault());
            }
        }

        public abstract void ResetToDefault();

        public void ResetToUnappliedValue(bool propagateChange = true)
        {
            if (!ApplyImmediately && HasConnection())
            {
                PullFromConnection(propagateChange);
                MarkAsUnchanged();
            }
            else
            {
                Logger.LogWarning("Can not reset to unapplied value if ApplyImmediate is FALSE or if the settings has no connection.");
            }
        }

        public bool MatchesAnyGroup(string[] groups)
        {
            if (groups == null || groups.Length == 0 || _groups == null || _groups.Count == 0)
                return false;

            foreach (var tag in groups)
            {
                foreach (var myTag in _groups)
                {
                    if (myTag == tag)
                        return true;
                }
            }
            return false;
        }

        public List<string> GetGroups()
        {
            return _groups;
        }

        public void SetGroups(List<string> groups)
        {
            _groups = groups;
        }

        protected bool checkDataType(SettingData.DataType serializedDataType, SettingData.DataType dataType)
        {
            if (serializedDataType != dataType)
            {
                Debug.LogError("SGSettings: The serialized data type is '" + serializedDataType + "' instead of the expected '" + dataType + "' for settings path '" + ID + "'.");
                return false;
            }

            return true;
        }

        public bool MatchesAnyDataType(IList<SettingData.DataType> dataTypes)
        {
            if (dataTypes == null)
                return false;

            int count = dataTypes.Count;
            for (int i = 0; i < count; i++)
            {
                if (dataTypes[i] == GetDataType())
                    return true;
            }
            return false;
        }

        public abstract SettingData SerializeValueToData();
        public abstract void DeserializeValueFromData(SettingData data);

        public void AddChangeListener(Action<TValue> onChanged)
        {
            if (_changeListeners == null)
                _changeListeners = new List<Action<TValue>>();

            if (!_changeListeners.Contains(onChanged))
            {
                _changeListeners.Add(onChanged);
            }
        }

        public void RemoveChangeListener(Action<TValue> onChanged)
        {
            if (_changeListeners == null)
                return;

            _changeListeners.Remove(onChanged);
        }

        /// <summary>
        /// Notice that apply listeners are ONLY called if Apply() was called on a setting with a connection.
        /// They are NOT called if the value changes.<br />
        /// If you want to be notified of a value changes at the time of application whether it was triggered by
        /// Apply() or a normal value change then use the OnSettingApplied event instead.
        /// </summary>
        /// <param name="onApplied"></param>
        public void AddApplyListener(Action<TValue> onApplied)
        {
            if (_applyListeners == null)
                _applyListeners = new List<Action<TValue>>();

            if (!_applyListeners.Contains(onApplied))
            {
                _applyListeners.Add(onApplied);
            }
        }

        public void RemoveApplyListener(Action<TValue> onApplied)
        {
            if (_applyListeners == null)
                return;

            _applyListeners.Remove(onApplied);
        }

        protected void invokeApplyListeners()
        {
            if (_applyListeners != null)
            {
                foreach (var listener in _applyListeners)
                {
                    if (listener != null)
                        listener?.Invoke(GetValue());
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Apply()
        {
            _hasChanged = false;

            if (HasConnection())
            {
                PushToConnection();
                PullFromConnection();
            }
            
            triggerOnSettingApplied();
            invokeApplyListeners();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void OnChanged()
        {
            MarkAsChanged();

            triggerOnSettingChanged();
            
            if (ApplyImmediately)
                triggerOnSettingApplied();

            if (_changeListeners != null)
            {
                foreach (var listener in _changeListeners)
                {
                    listener?.Invoke(GetValue());
                }
            }
        }

        protected virtual void triggerOnSettingChanged()
        {
            OnSettingChanged?.Invoke(this);
        }
        
        protected virtual void triggerOnSettingApplied()
        {
            OnSettingApplied?.Invoke(this);
        }
        

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void MarkAsChanged()
        {
            _hasChanged = true;
        }

        public void MarkAsUnchanged()
        {
            _hasChanged = false;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool HasUnappliedChanges()
        {
            return _hasChanged;
        }

        public void AddPulledFromConnectionListener(Action<TValue> onApply)
        {
            if (_pulledFromConnectionListeners == null)
                _pulledFromConnectionListeners = new List<Action<TValue>>();

            if (!_pulledFromConnectionListeners.Contains(onApply))
            {
                _pulledFromConnectionListeners.Add(onApply);
            }
        }

        public void RemovePulledFromConnectionListener(Action<TValue> onApply)
        {
            if (_pulledFromConnectionListeners == null)
                return;

            _pulledFromConnectionListeners.Remove(onApply);
        }

        protected void invokePulledFromConnectionListeners()
        {
            if (HasConnection() && _pulledFromConnectionListeners != null)
            {
                foreach (var listener in _pulledFromConnectionListeners)
                {
                    if (listener != null)
                        listener?.Invoke(GetValue());
                }
            }

            invokeGenericPulledFromConnectionListeners();
        }

        public void AddPulledFromConnectionListener(Action onApply)
        {
            if (_genericPulledFromConnectionListeners == null)
                _genericPulledFromConnectionListeners = new List<Action>();

            if (!_genericPulledFromConnectionListeners.Contains(onApply))
            {
                _genericPulledFromConnectionListeners.Add(onApply);
            }
        }

        public void RemovePulledFromConnectionListener(Action onApply)
        {
            if (_genericPulledFromConnectionListeners == null)
                return;

            _genericPulledFromConnectionListeners.Remove(onApply);
        }

        protected void invokeGenericPulledFromConnectionListeners()
        {
            if (HasConnection() && _genericPulledFromConnectionListeners != null)
            {
                foreach (var listener in _genericPulledFromConnectionListeners)
                {
                    if (listener != null)
                        listener?.Invoke();
                }
            }
        }

        public void RemoveAllListeners()
        {
            _changeListeners?.Clear();
            _pulledFromConnectionListeners?.Clear();
            _genericPulledFromConnectionListeners?.Clear();
            _applyListeners?.Clear();
        }

        public abstract object GetValueAsObject();
        public abstract bool HasConnection();
        public abstract bool HasConnectionObject();
        public abstract void PullFromConnection();
        public abstract void PullFromConnection(bool propagateChange);
        public abstract IConnection GetConnectionInterface();

        public virtual void PushToConnection()
        {
            _hasChanged = false;
        }

        public abstract int GetConnectionOrder();

        public abstract void OnQualityChanged(int qualityLevel);
    }
}
