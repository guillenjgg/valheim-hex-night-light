using HarmonyLib;
using HexNightLight.Core.Managers;

namespace HexNightLight.Patches
{
    [HarmonyPatch(typeof(ZNetScene), "Awake")]
    internal static class ZNetSceneAwakePatch
    {
        private static void Postfix(ZNetScene __instance)
        {
            CustomItemRegistry.RegisterPrefabs(__instance);
        }
    }
}
