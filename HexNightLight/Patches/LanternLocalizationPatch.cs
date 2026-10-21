using HarmonyLib;
using HexNightLight.Core.Managers;

namespace HexNightLight.Core.Patches
{
    [HarmonyPatch]
    internal static class LanternLocalizationPatch
    {
        [HarmonyPatch(typeof(Localization), nameof(Localization.SetupLanguage))]
        [HarmonyPostfix]
        private static void SetupLanguagePostfix(Localization __instance)
        {
            LanternLocalizationManager.Register(__instance);
        }

        [HarmonyPatch(typeof(FejdStartup), nameof(FejdStartup.SetupGui))]
        [HarmonyPostfix]
        private static void SetupGuiPostfix()
        {
            LanternLocalizationManager.Register(Localization.instance);
        }
    }
}