using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public class FrameRateConnection : ConnectionWithOptions<string>
    {
        public bool RemoveUnlimited = false;
        
        /// <summary>
        /// If specified then these frame rates will be used. Common frame rates are: 30, 60, 120, 144, 165, 200, 240. HINT: Use -1 for unlimited frame rate.
        /// </summary>
        public List<int> CustomFrameRates = null;

        public List<int> _values;
        public List<string> _labels;

        protected List<int> getFrameRates()
        {
            if (_values == null)
            {
                _values = new List<int>();
                if (!CustomFrameRates.IsNullOrEmpty())
                {
                    _values.AddRange(CustomFrameRates);
                }
                else
                {
                    _values.Add(-1);
                    _values.Add(30);
                    _values.Add(60);
                    _values.Add(120);
                    _values.Add(144);
                    _values.Add(165);
                    _values.Add(200);
                    _values.Add(240);   
                }

                if (RemoveUnlimited)
                    _values.Remove(-1);
            }

            return _values;
        }

        public override List<string> GetOptionLabels()
        {
            if (_labels == null)
            {
                _labels = new List<string>();

                var frameRates = getFrameRates();
                for (int i = 0; i < frameRates.Count; i++)
                {
                    if (frameRates[i] < 0)
                    {
                        string name = "Unlimited";
                        _labels.Add(name);
                    }
                    else
                    {
                        string name = frameRates[i].ToString() + " fps";
                        _labels.Add(name);
                    }
                }
            }

            return _labels;
        }

        public override void SetOptionLabels(List<string> optionLabels)
        {
            var frameRates = getFrameRates();
            if (optionLabels == null || optionLabels.Count != frameRates.Count)
            {
                Debug.LogError("Invalid new labels. Need to be " + frameRates.Count + ".");
                return;
            }

            _labels = new List<string>(optionLabels);
        }

        public override void RefreshOptionLabels()
        {
            _labels = null;
            GetOptionLabels();
        }

        public override int Get()
        {
            var frameRates = getFrameRates();
            for (int i = 0; i < frameRates.Count; i++)
            {
                if (frameRates[i] == Application.targetFrameRate)
                {
                    return i;
                }
            }

            return 0;
        }

        /// <summary>
        /// Keep in mind that VR platforms ignore both QualitySettings.vSyncCount and Application.targetFrameRate. Instead, the VR SDK controls the frame rate.
        /// </summary>
        /// <param name="index"></param>
        public override void Set(int index)
        {
            var frameRates = getFrameRates();
            index = Mathf.Clamp(index, 0, frameRates.Count - 1);
            var rate = frameRates[index];

            Application.targetFrameRate = rate;

            NotifyListenersIfChanged(index);
        }
    }
}
