using System.Reflection;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;

namespace PinkCoinProgression;

/// <summary>
/// Loader entry point. Game hooks must be verified against the installed sts2.dll
/// before enabling gameplay patches. This intentionally does not call PatchAll.
/// </summary>
[ModInitializer("Initialize")]
public static class ModEntry
{
    public static void Initialize()
    {
        Log.Info("[PinkCoinProgression] Initialized core; gameplay hooks pending version verification.");
    }
}
