using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator.Examples
{
    // This class contains a LOT of comments to explain the code API.
    //
    // Many things shown here could be written in shorter code. It is
    // written in a lengthy style on purpose to keep things as simple
    // to understand as possible.
    //
    // NOTICE: While the ScriptAPI can do anything the ScriptableObject
    // based workflow can I encourage you to use the ScriptableObjects
    // instead. The reasons are:
    // 1) Iterating settings in code is rather slow (re-compile).
    // 2) The Settings Asset has a neat Editor UI which visualizes
    //    data nicely (colors are hard to interpret if seen in code).
    // 3) Assets can be changed by none programmers too (be kind
    //    to your artists).

    public class SettingsFromCodeDemo : MonoBehaviour
    {
        /// <summary>
        /// The SettingsProvider ScriptableObject is always needed. It serves as the
        /// access point for anything related to the settings. Simply drag in your
        /// SettingsProvider asset.
        /// </summary>
        public SettingsProvider Provider;

        public void Awake()
        {
            // A call to GetOrCreateRuntimeSettingsAsset() will automatically
            // create a runtime Settings object, either a copy or a new one. In this case
            // we have no Settings set in the provider. To check open the "SettingsProvider (FromCode).asset"
            // and you'll notice the "Settings Asset" is left empty. Hence we get a new one here.
            //
            // NOTICE: The settings are NOT YET LOADED here. You are editing the settings list before
            // it is loaded. Any call to Provider.Settings will synchronously load the settings. Usually
            // that's done automatically by the SettingsGeneratorSettings class after all Awake() calls
            // but before any Start() call via RuntimeInitializeLoadType.AfterSceneLoad.

            var settings = Provider.GetOrCreateRuntimeSettingsAsset();

            // At this point the settings are already loaded. A setting will be created
            // automatically for each SettingData entry found in the loaded data. Yet these
            // settings still only are pure data. They need to be hooked up with the code
            // which reacts to setting changes.

            // Look into these methods. They contain detailed descriptions.
            
            // # Boss Fights
            addEnableBossFightsBoolean(settings);
            
            // # Health regeneration
            addHealthRegenerationPercentage(settings);

            // # Difficulty
            // An options setting (with a little shorter code this time).
            var difficulties = new List<string>() { "Easy", "Normal", "Hard" };
            int defaultSelectedIndex = 1; // Normal is the default setting.
            var difficultySetting = settings.GetOrCreateOption("difficulty", defaultSelectedIndex, options: difficulties);
            // Instead of connections let's use a listener this time.
            difficultySetting.AddChangeListener((selectedIndex) => Debug.Log($"Selected difficulty is {difficulties[selectedIndex]}."));

            // # Opponent color
            var opponentsColors = new List<Color> { Color.red, Color.green, Color.blue };
            var opponentColor = settings.GetOrCreateColorOption("opponentColor", 0, options: opponentsColors);
            opponentColor.AddChangeListener((selectedColor) => Debug.Log($"Opponent color is {opponentColor.GetOptionLabels()[selectedColor]}."));
            
            // # Team color picker
            var teamColorSetting = settings.GetOrCreateColorOption("teamColor", 0);
            // You may wonder why there are not options defined.
            // Take a look at the UI in the "SettingsFromCodeDemo UI (loaded additive)" scene.
            // There you will find that there are some "ColorPickerButtonUGUI" buttons within
            // the "ColorPickerButtonUGUI" button.
            // If no options are defined in the setting data then the SettingResolver will try to
            // derive options from the UI (ColorPickerButtonUGUI buttons in this case).
            // CAVEAT: Doing it this way will mean during the initial load (which happens after Awake but before Start)
            // the color options (which are derived in Start() of the UI) are not yet available. My recommendation: define the colors in the settings. 
            teamColorSetting.AddChangeListener((selectedColor) =>
            {
                if (teamColorSetting.GetOptionLabels() != null)
                {
                    Debug.Log($"Selected team color is {teamColorSetting.GetOptionLabels()[selectedColor]}.");    
                }
                else
                {
                    Debug.LogWarning("Team color options are not yet initialized (one of the caveats of defining the colors in the UI is the system has to wait for the UI to load before the options are available).");
                }
            });

            // # Audio enable/disable (start as disabled)
            // This is an example of using a predefined connection (there are many, they are listed in the manual).
            // If you want to implement your own connection just take a look into the existing ones.
            // NOTICE: Some connections have public properties or parameters to configure them.
            settings.GetOrCreateBool("audioEnabled", connection: new AudioPausedConnection());
        }

        public void Start()
        {
            Debug.Log("Settings loaded: " + Provider.InitialLoadDone);
            
            // Thus far we have defined some new settings but nothing has happened yet. That is because the settings
            // have not yet been loaded and applied. Usually that's done automatically by the SettingsGeneratorSettings
            // class after all Awake() calls but before any Start() call via RuntimeInitializeLoadType.AfterSceneLoad.
            
            // You may notice that with this setup only the "health" setting will log in the beginning. That is because
            // it has a CONNECTION and connections are automatically applied (change is triggered) upon initial load.
            // The change event on all the other settings are not triggered because the system assumes the initial default
            // value is the current value (because there is no connection to check against).
            // To trigger the changed events on all settings that do not have a connection you can manually call Apply()
            // with "triggerChangeEvents" set to true:
            Provider.Settings.Apply(changedOnly: true, triggerChangeEvents: true);
            // You may ask why that is not done automatically like the automatic Apply() on all connections after loading.
            // The answer is backwards compatibility.
        }

        private static void addEnableBossFightsBoolean(Settings settings)
        {
            // First let's add the simplest setting possible (a boolean that does nothing).
            settings.GetOrCreateBool(
                // Each setting needs an ID. This ID is used in variouos places to
                // find the setting. Most importantly it is used in the UI (SettingResolver).
                id: "enableBossFights",

                // This is the default value this setting will reset to if setting.ResetToDefault()
                // is called. This is also the initial value of the setting at first boot.
                defaultValue: true
                );

            // That's it. If you look in Update() you see we now can log the current value of this setting.

            // As a seasoned programmer you are most likely not thrilled by the idea of having to
            // pull the setting values. The next setting ("healthRegeneration") will use some
            // callback to react to changes instead.
        }



        #region Health Regeneration

        /// <summary>
        /// A simple int for a slider from 0 to 100 (the range is defined in the UI).
        /// It defines a percentage of how strong the health regeneration should be.
        /// </summary>
        protected int _healthRegeneration;

        protected void addHealthRegenerationPercentage(Settings settings)
        {
            // This setting will react to changes in the setting and propagate those
            // to a local field "_healthRegeneration".
            //
            // To connect a setting (data) with some logic we use Connection objects.
            // These are very simple. They have a Get() and a Set(value) method.
            //
            // Get() means getting a value from the connection and saving it within the setting (pull).
            // Set(value) means sending a new value to the connection (push).
            // There are many specialized predefined Connections (like the "FrameRateConnection").
            //
            // For this example we use a simple generic GetSetConnection<T> connection.
            // This connection does nothing except forward Get() and Set(value) calls to
            // some other methods (getter and setter).

            var connection = new GetSetConnection<int>(
                getter: getHealthRegeneration, // executed if connection.Get() is called.
                setter: setHealthRegeneration  // executed if connection.Set(value) is called.
            );

            // Now that we have our connection we need to hook it up with our setting.
            // In fact at first boot we also have to create our setting. Luckily there
            // is a handy GetOrCreateInt() method so we don't have to worry about that.
            var healthSetting = settings.GetOrCreateInt(

                // Each setting needs an ID.
                id: "healthRegeneration",

                // The default value is the fallback default value used if no connection is
                // set. In this case we are using a connection and the default value is pulled
                // initially from that connection. Therefore we actually don't need to specify it.
                // defaultValue: false

                // We want to use a connection to get/set the values.
                connection: connection
            );

            // If all you need is to listen for changes in a setting then you may not even need a
            // Connection object. There is a healthSetting.AddChangeListener() method which you
            // can use for that.
        }

        // This simply returns the current state of "_healthRegeneration".
        // This getter is called at the very first use of the setting
        // and the return value will be stored as the default value (used
        // if you call setting.ResetToDefault()).
        //
        // It may also be called at any time by the settings system and
        // should return the current state of the value in your game.
        //
        // If this value is changed from outside the settings system, then
        // you need to call setting.PullFromConnection() to update the interal
        // value of the setting.
        //
        // "Pull" in this context is meant as from the view point of the setting.
        // I.e. "pull the value from the connection into the setting and update the UI".
        // During this pull process connection.Get() is called which calls
        // this getter.
        //
        // There is also a setting.PushToConnection() method which does to opposite.
        // It pushes the value from the setting into the connection (connection.Set())
        protected int getHealthRegeneration()
        {
            return _healthRegeneration;
        }

        // This simply sets the local field and logs the new value.
        protected void setHealthRegeneration(int value)
        {
            _healthRegeneration = value;
            Debug.Log("Health regeneration has been set to: " + value);
        }
        #endregion


        #region logging
        float _logTimer;

        public void Update()
        {
            // Execute once every two seconds
            _logTimer += Time.deltaTime;
            if(_logTimer > 2f)
            {
                _logTimer = 0f;

                // Log enableBossFights
                SettingBool enableBossFightsSetting = Provider.Settings.GetBool("enableBossFights");
                bool enableBossFights = enableBossFightsSetting.GetValue();
                Debug.Log("Enable Boss Fights is: " + enableBossFights + " (time: " + Time.realtimeSinceStartup + ")");
            }
        }
        #endregion
    }
}
