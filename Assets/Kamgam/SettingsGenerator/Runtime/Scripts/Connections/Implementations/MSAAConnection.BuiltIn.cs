// If it's neither URP nor HDRP then it is the old BuiltIn renderer
// If both HDRP and URP are set then we also assume BuiltIn until this ambiguity is resolved by the AssemblyDefinitionUpdater
#if (!KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP) || (KAMGAM_RENDER_PIPELINE_URP && KAMGAM_RENDER_PIPELINE_HDRP)

using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public partial class MSAAConnection
    {
        // MSAA in Built-In is already implemented via the AntiAliasingConnection.
    }
}
#endif
