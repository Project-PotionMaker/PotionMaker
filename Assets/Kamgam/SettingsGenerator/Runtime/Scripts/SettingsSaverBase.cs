using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public abstract class SettingsSaverBase : ScriptableObject
    {
        public abstract void LoadInto(string key, Settings settings);

        public abstract void Save(string key, Settings settings);

        public abstract void Delete(string key);
    }
}
