using System.Collections.Generic;
using UnityEngine;

#if TND_XeSS
using TND.XeSS;
#endif

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// While the name of this class is AlteregoFSR2Connection it also covers FSR3.
    /// The name remained the same for backwards compatibility reasons.
    /// </summary>
    public class NakedDevXeSSConnection : ConnectionWithOptions<string>
    {
        protected List<string> _labels;

        public NakedDevXeSSConnection()
        {
#if TND_XeSS
            if (!IsSupported())
            {
                Logger.LogWarning("NakedDevXeSSConnection: XeSS is not supported. Please contact The Naked Dev for more info and support. https://docs.google.com/document/d/1nb1cdNNc9zzmvbDbwPERKm21g_Cp8o9V2sjVV1JsQNM");
            }
#else
            Logger.LogWarning("NakedDevXeSSConnection: XeSS is not yet set up. Please consult The Naked Dev Games Manual for more info and support. https://docs.google.com/document/d/1nb1cdNNc9zzmvbDbwPERKm21g_Cp8o9V2sjVV1JsQNM");
#endif
        }

#if TND_XeSS
        public XeSS_Base GetUtils()
        {
            XeSS_Base utils = null;
            
            var cam = Camera.main;
            if (cam != null)
                utils = cam.GetComponent<XeSS_Base>();
            if (utils != null)
                return utils;

            cam = Camera.current;
            if (cam != null)
                utils = cam.GetComponent<XeSS_Base>();

            if (utils == null)
            {
                Logger.LogWarning("NakedDevXeSSConnection: Could not find the XeSS component on the current camera. Please make sure you have the SGSR component added to your camera (ignoring all SGSR settings for now).");
            }

            return utils;
        }
#endif

#if TND_XeSS && (!KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP)
        public UnityEngine.Rendering.PostProcessing.PostProcessLayer GetLayer()
        {
            var cam = Camera.main;
            if (cam == null)
                return null;

            return cam.GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>();
        }
#endif

        public bool IsSupported()
        {
#if TND_XeSS
#if !KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP
            // BiRP
            var layer = GetLayer();
            if (layer != null && layer.xess != null)
            {
                return layer.xess.IsSupported();
            }

            return false;
#else
            // URP & HDRP
            var utils = GetUtils();
            if (utils == null)
            {
#if UNITY_EDITOR
                Logger.LogWarning("NakedDevXeSSConnection: XeSS_Base was not found. Did you forget to add it to your camera? See: https://docs.google.com/document/d/1nb1cdNNc9zzmvbDbwPERKm21g_Cp8o9V2sjVV1JsQNM");
#endif
                return false;
            }

            bool supported = utils.IsSupported(out XeSSResult result);
            if(!supported)
            {
                Logger.LogWarning("NakedDevXeSSConnection: XeSS is not supported, reason: " + result.ToString());
            }
            return supported;
#endif
#else
            return false;
#endif
        }

        public override List<string> GetOptionLabels()
        {
#if TND_XeSS
            if (!IsSupported())
            {
                if(_labels == null || _labels.Count == 0)
                    _labels = new List<string>() { "Not Supported" };
                return _labels;
            }

            if (_labels == null)
            {
                _labels = new List<string>();
                var qualityNames = System.Enum.GetNames(typeof(XeSS_Quality));
                foreach (var name in qualityNames)
                {
                    _labels.Add(name);
                }
            }
#else
            Logger.LogWarning("NakedDevXeSSConnection: The Naked Dev XeSS is not yet set up. Please consult the Naked Dev Manual for more info and support. https://docs.google.com/document/d/1nb1cdNNc9zzmvbDbwPERKm21g_Cp8o9V2sjVV1JsQNM");
#endif
            return _labels;
        }

        public override void SetOptionLabels(List<string> optionLabels)
        {
            if (optionLabels == null)
            {
                Debug.LogError("Invalid new labels. Need to be four.");
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
#if TND_XeSS
#if !KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP
            // BiRP
            var layer = GetLayer();
            if (layer != null && layer.xess != null)
            {
                return (int)layer.xess.qualityMode;
            }
            else
            {
                return 0;
            }
#else
            // URP & HDRP
            var utils = GetUtils();
            if (utils != null)
            {
                return (int)utils.OnGetQuality();
            }
            else
            {
                return 0;
            }
#endif
#else
            Logger.LogWarning("NakedDevXeSSConnection: The Naked Dev XeSS is not yet set up. Please consult the The Naked Dev Manual for more info and support. https://docs.google.com/document/d/1nb1cdNNc9zzmvbDbwPERKm21g_Cp8o9V2sjVV1JsQNM");
            return 0;
#endif
        }

        public override void Set(int index)
        {
#if TND_XeSS
#if !KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP
            // BiRP
            var layer = GetLayer();
            if (layer != null && layer.xess != null)
            {
                layer.xess.qualityMode = (XeSS_Quality)index;
            }
#else
            // URP & HDRP
            var utils = GetUtils();
            if (utils != null)
            {
                utils.OnSetQuality((XeSS_Quality)index);
            }
#endif
#endif
            NotifyListenersIfChanged(index);
        }
    }
}