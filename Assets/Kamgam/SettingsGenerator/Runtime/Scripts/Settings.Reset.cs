using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public partial class Settings : ScriptableObject
    {
        /// <summary>
        /// List of input elements which have been shown at least once.
        /// We use this list to update the UIs after a reset.
        /// Input elements register once they are made visible for the first time.
        /// </summary>
        [System.NonSerialized]
        public List<ISettingResolver> RegisteredResolvers = new List<ISettingResolver>();

        /// <summary>
        /// The number of resolvers
        /// </summary>
        [System.NonSerialized]
        public int ActiveResolverCount = 0;
        
        public void RegisterResolver(ISettingResolver resolver)
        {
            if (resolver == null)
                return;
            
            // Ignore elements which are not part of this settings object.
            if (!HasID(resolver.GetID()))
                return;

            RegisteredResolvers.Add(resolver);
            DefragRegisteredResolvers();
        }

        public void UnregisterResolver(ISettingResolver input)
        {
            if (input == null)
                return;

            RegisteredResolvers.Remove(input);
            DefragRegisteredResolvers();
        }

        public void DefragRegisteredResolvers()
        {
            for (int i = RegisteredResolvers.Count-1; i >= 0; i--)
            {
                if (RegisteredResolvers[i] == null)
                    RegisteredResolvers.RemoveAt(i);
            }
        }

        /// <summary>
        /// Triggers all registered resolvers to update themselves.
        /// </summary>
        public void RefreshRegisteredResolvers()
        {
            if (RegisteredResolvers == null || RegisteredResolvers.Count == 0)
                return;

            DefragRegisteredResolvers();

            foreach (var resolver in RegisteredResolvers)
            {
                resolver.Refresh();
            }
        }

        /// <summary>
        /// Refreshes resolvers registered to the given ID.
        /// </summary>
        /// <param name="id"></param>
        public void RefreshRegisteredResolvers(string id)
        {
            if (RegisteredResolvers == null || RegisteredResolvers.Count == 0)
                return;

            DefragRegisteredResolvers();

            foreach (var resolver in RegisteredResolvers)
            {
                if(resolver.GetID() == id)
                    resolver.Refresh();
            }
        }

        /// <summary>
        /// Refreshes resolvers registered to the given setting.
        /// </summary>
        /// <param name="setting"></param>
        public void RefreshRegisteredResolvers(ISetting setting)
        {
            RefreshRegisteredResolvers(setting.GetID());
        }

        /// <summary>
        /// Refreshes resolvers that are connected to a setting with the given Connection type T.
        /// </summary>
        public void RefreshRegisteredResolversWithConnection<T>() where T : IConnection
        {
            if (RegisteredResolvers == null || RegisteredResolvers.Count == 0)
                return;

            DefragRegisteredResolvers();

            foreach (var resolver in RegisteredResolvers)
            {
                var id = resolver.GetID();
                if (!string.IsNullOrEmpty(id))
                {
                    var setting = GetSetting(id);
                    if (setting != null && setting.HasConnection() && setting.GetConnectionInterface() is T)
                    {
                        resolver.Refresh();
                    }
                }
            }
        }

        /// <summary>
        /// Refreshes resolvers that are connected to a setting with the given connection object.
        /// </summary>
        public void RefreshRegisteredResolversWithConnection(IConnection connection)
        {
            if (RegisteredResolvers == null || RegisteredResolvers.Count == 0)
                return;

            DefragRegisteredResolvers();

            foreach (var resolver in RegisteredResolvers)
            {
                var id = resolver.GetID();
                if (!string.IsNullOrEmpty(id))
                {
                    var setting = GetSetting(id);
                    if (setting != null && setting.HasConnection() && setting.GetConnectionInterface() == connection)
                    {
                        resolver.Refresh();
                    }
                }
            }
        }

        /// <summary>
        /// Resets all settings to their default value.
        /// </summary>
        public void Reset()
        {
            // Reset all
            foreach (var setting in _settingsCache)
            {
                // Ignore inactive
                if (!setting.IsActive)
                    continue;

                setting.ResetToDefault();
            }

            // Notify inputs
            DefragRegisteredResolvers();
            foreach (var input in RegisteredResolvers)
            {
                input.Refresh();
            }
        }

        /// <summary>
        /// Resets all settings (ids) to their default value.
        /// </summary>
        public void Reset(params string[] ids)
        {
            if (ids == null || ids.Length == 0)
                return;

            // Reset fields
            foreach (var setting in _settingsCache)
            {
                // Ignore inactive
                if (!setting.IsActive)
                    continue;

                // Ignore all which do not match the paths
                if (!ids.Contains(setting.GetID()))
                    continue;

                setting.ResetToDefault();
            }

            // Notify resolvers
            DefragRegisteredResolvers();
            foreach (var resolver in RegisteredResolvers)
            {
                resolver.Refresh();
            }
        }

        public void ResetGroups(params string[] groups)
        {
            if (groups == null || groups.Length == 0)
                return;

            // Reset fields
            foreach (var setting in _settingsCache)
            {
                // Ignore inactive
                if (!setting.IsActive)
                    continue;

                // Ignore all which do not match the tags
                if (!setting.MatchesAnyGroup(groups))
                    continue;

                setting.ResetToDefault();
            }

            // Notify inputs
            DefragRegisteredResolvers();
            foreach (var input in RegisteredResolvers)
            {
                input.Refresh();
            }
        }

        public void ResetToUnappliedValues()
        {
            ResetToUnappliedValues(true);
        }

        /// <summary>
        /// Resets all settings to their unapplied values (if they have any)
        /// </summary>
        public void ResetToUnappliedValues(bool propagateChange)
        {
            foreach (var setting in _settingsCache)
            {
                if (!setting.IsActive || !setting.HasUnappliedChanges() || !setting.HasConnection())
                    continue;
                
                setting.ResetToUnappliedValue(propagateChange);
            }

            // Notify resolvers
            DefragRegisteredResolvers();
            foreach (var resolver in RegisteredResolvers)
            {
                resolver.Refresh();
            }
        }

        /// <summary>
        /// Resets all settings (ids) to their unapplied values (if they have any)
        /// </summary>
        public void ResetToUnappliedValues(params string[] ids)
        {
            ResetToUnappliedValues(propagateChange: true, ids);
        }

        /// <summary>
        /// Resets all settings (ids) to their unapplied values (if they have any)
        /// </summary>
        public void ResetToUnappliedValues(bool propagateChange, params string[] ids)
        {
            if (ids == null || ids.Length == 0)
                return;

            // Reset fields
            foreach (var setting in _settingsCache)
            {
                // Ignore
                if (!setting.IsActive || !setting.HasUnappliedChanges() || !setting.HasConnection())
                    continue;

                // Ignore all which do not match the paths
                if (!ids.Contains(setting.GetID()))
                    continue;

                setting.ResetToUnappliedValue(propagateChange);
            }

            // Notify resolvers
            DefragRegisteredResolvers();
            foreach (var resolver in RegisteredResolvers)
            {
                resolver.Refresh();
            }
        }
    }
}
