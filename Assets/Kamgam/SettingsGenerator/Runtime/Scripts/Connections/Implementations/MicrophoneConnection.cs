using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public class MicrophoneConnection : ConnectionWithOptions<string>, IConnectionWithSettingsAccess
    {
        protected float _pollIntervalInSec = -1f;

        /// <summary>
        /// If > 0 then every # seconds the connection will check for new microphones and update the options list.
        /// </summary>
        public float PollIntervalInSec
        {
            get => _pollIntervalInSec;
            set
            {
                if (Mathf.Approximately(_pollIntervalInSec, value))
                    return;
                
                _pollIntervalInSec = value;
                startStopPolling();
            }
        }

        async void startStopPolling()
        {
            while (_pollIntervalInSec > 0.01f
#if UNITY_EDITOR
                   && UnityEditor.EditorApplication.isPlaying
#endif
                   )
            {
                await Task.Delay((int) (_pollIntervalInSec * 1000));

                if (_pollIntervalInSec <= 0.01f)
                    break;

                var newDevices = Microphone.devices;
                if (newDevices.Length != _values.Count - 1)
                {
                    onDeviceListChanged();
                }
                else
                {
                    foreach (var device in newDevices)
                    {
                        if (!_values.Contains(device))
                        {
                            onDeviceListChanged();
                            break;
                        }
                    }
                }
                    
            }
        }

        void onDeviceListChanged()
        {
            GetOptionLabels();
            GetSettings().RefreshRegisteredResolversWithConnection<MicrophoneConnection>();
        }
        
        protected List<string> _values;
        protected List<string> _labels;

        public MicrophoneConnection(float pollIntervalInSec = -1f)
        {
            PollIntervalInSec = pollIntervalInSec;
        }

        public override List<string> GetOptionLabels()
        {
            getDeviceNames();

            if (_labels == null)
                _labels = new List<string>();
            else
                _labels.Clear();

            foreach (var deviceName in _values)
            {
                if (deviceName == null)
                    _labels.Add("Default");
                else
                    _labels.Add(deviceName);
            }

            return _labels;
        }

        public override void SetOptionLabels(List<string> optionLabels)
        {
            var values = getDeviceNames();
            if (optionLabels == null || optionLabels.Count != values.Count)
            {
                Debug.LogError("Invalid new labels. Need to be " + values.Count + ".");
                return;
            }

            _labels = new List<string>(optionLabels);
        }

        public override void RefreshOptionLabels()
        {
            _labels = null;
            GetOptionLabels();
        }

        protected List<string> getDeviceNames()
        {
            if (_values == null)
                _values = new List<string>();
            else
                _values.Clear();
                
            // As per documentation: NULL = the system default
            // See "If you pass a null or empty string for the device name then the default microphone is used."
            // https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Microphone.Start.html
            _values.Add(null);
            
            var devices = Microphone.devices;
            foreach (var deviceName in devices)
            {
                _values.Add(deviceName);
            }
            
            return _values;
        }

        protected Settings _settings;
        protected int _selectedDeviceIndex = 0;

        /// <summary>
        /// Returns the selected index.
        /// </summary>
        /// <returns></returns>
        public override int Get()
        {
            return _selectedDeviceIndex;
        }

        public override void Set(int index)
        {
            _selectedDeviceIndex = index;
            NotifyListenersIfChanged(index);
        }

        public string GetSelectedDeviceName()
        {
            return _values[_selectedDeviceIndex];
        }

        /// <summary>
        /// Start Recording with currently selected device.
        /// </summary>
        /// <param name="loop">Indicates whether the recording should continue recording if lengthSec is reached, and wrap around and record from the beginning of the AudioClip.</param>
        /// <param name="lengthSec">Is the length of the AudioClip produced by the recording.</param>
        /// <param name="frequency">The sample rate of the AudioClip produced by the recording.</param>
        /// <returns>
        ///   <para>The function returns null if the recording fails to start.</para>
        /// </returns>
        public AudioClip StartRecording(bool loop, int lengthSec, int frequency)
        {
            return Microphone.Start(GetSelectedDeviceName(), loop, lengthSec, frequency);
        }
        
        public void EndRecording()
        {
            Microphone.End(GetSelectedDeviceName());
        }
        
        public bool IsRecording()
        {
            return Microphone.IsRecording(GetSelectedDeviceName());
        }
        
        public int GetPosition()
        {
            return Microphone.GetPosition(GetSelectedDeviceName());
        }
        
        public void GetDeviceCaps(out int minFreq, out int maxFreq)
        {
            Microphone.GetDeviceCaps(GetSelectedDeviceName(), out minFreq, out maxFreq);
        }

        public void SetSettings(Settings settings)
        {
            _settings = settings;
        }

        public Settings GetSettings()
        {
            return _settings;
        }
    }
}
