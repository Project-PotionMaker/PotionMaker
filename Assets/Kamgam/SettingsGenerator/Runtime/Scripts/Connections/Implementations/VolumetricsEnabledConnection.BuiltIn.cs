// If it's neither URP nor HDRP then it is the old BuiltIn renderer
// If both HDRP and URP are set then we also assume BuiltIn until this ambiguity is resolved by the AssemblyDefinitionUpdater
#if (!KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP) || (KAMGAM_RENDER_PIPELINE_URP && KAMGAM_RENDER_PIPELINE_HDRP)

using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public partial class VolumetricsEnabledConnection
    {
        public override bool Get()
        {
            Logger.LogWarning("Volumetrics no supported in Built In render pipeline.");
            return true;
        }

        public override void Set(bool enable)
        {
            Logger.LogWarning("Volumetrics no supported in Built In render pipeline.");
        }
    }
}
#endif
