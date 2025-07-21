using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

#if TND_SGSR
using TND.SGSR;
#endif

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// While the name of this class is AlteregoFSR2Connection it also covers FSR3.
    /// The name remained the same for backwards compatibility reasons.
    /// </summary>
    public class NakedDevSGSRConnection : ConnectionWithOptions<string>
    {
        protected List<string> _labels;

        public NakedDevSGSRConnection()
        {
#if TND_SGSR
            if (!IsSupported())
            {
                Logger.LogWarning("NakedDevSGSRConnection: SGSR is not supported. Please contact The Naked Dev for more info and support. https://docs.google.com/document/d/1s8tQYdpSMZRLf1gndRSekam-t9FYGE_e9QLgVJAbeH8");
            }
#else
            Logger.LogWarning("NakedDevSGSRConnection: SGSR is not yet set up. Please consult The Naked Dev Games Manual for more info and support. https://docs.google.com/document/d/1s8tQYdpSMZRLf1gndRSekam-t9FYGE_e9QLgVJAbeH8");
#endif
        }

#if TND_SGSR
        public SGSR_BASE GetUtils()
        {
            SGSR_BASE utils = null;
            
            var cam = Camera.main;
            if (cam != null)
                utils = cam.GetComponent<SGSR_BASE>();
            if (utils != null)
                return utils;

            cam = Camera.current;
            if (cam != null)
                utils = cam.GetComponent<SGSR_BASE>();

            if (utils == null)
            {
                Logger.LogWarning("NakedDevSGSRConnection: Could not find the SGSR component on the current camera. Please make sure you have the SGSR component added to your camera (ignoring all SGSR settings for now).");
            }

            return utils;
        }

#endif

        public bool IsSupported()
        {
#if TND_SGSR
            var utils = GetUtils();
            if (utils == null)
                return false;

            return true; // We assume it is always supported.
#else
            return false;
#endif
        }

        public override List<string> GetOptionLabels()
        {
#if TND_SGSR
            if (!IsSupported())
            {
                if(_labels == null || _labels.Count == 0)
                    _labels = new List<string>() { "Not Supported" };
                return _labels;
            }

            if (_labels == null)
            {
                _labels = new List<string>();
                var qualityNames = System.Enum.GetNames(typeof(SGSR_Quality));
                foreach (var name in qualityNames)
                {
                    _labels.Add(name);
                }
            }
#else
            Logger.LogWarning("NakedDevSGSRConnection: The Naked Dev SGSR is not yet set up. Please consult the Naked Dev Manual for more info and support. https://docs.google.com/document/d/1s8tQYdpSMZRLf1gndRSekam-t9FYGE_e9QLgVJAbeH8");
#endif
            return _labels;
        }

        public override void SetOptionLabels(List<string> optionLabels)
        {
            if (optionLabels == null)
            {
                Debug.LogError("Invalid new labels.");
                return;
            }

            _labels = optionLabels;
        }

        public override void RefreshOptionLabels()
        {
            _labels = null;
            GetOptionLabels();
        }

        public override int Get()
        {
#if TND_SGSR
            var utils = GetUtils();
            if (utils != null)
            {
                return (int)utils.qualityLevel;
            }
            else
            {
                return 0;
            }
#else
            Logger.LogWarning("NakedDevSGSRConnection: The Naked Dev SGSR is not yet set up. Please consult the Alterego Games Manual for more info and support. https://docs.google.com/document/d/1s8tQYdpSMZRLf1gndRSekam-t9FYGE_e9QLgVJAbeH8");
            return 0;
#endif
        }

        public override void Set(int index)
        {
#if TND_SGSR
            var utils = GetUtils();
            if (utils != null)
            {
                utils.qualityLevel = (SGSR_Quality)index;
            }
#endif
            NotifyListenersIfChanged(index);
        }
    }
}