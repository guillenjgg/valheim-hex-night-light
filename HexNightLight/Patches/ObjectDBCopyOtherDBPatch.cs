using HarmonyLib;
using HexNightLight.Core.Managers;

namespace HexNightLight.Patches
{
    [HarmonyPatch(typeof(ObjectDB), nameof(ObjectDB.Awake))]
    internal static class ObjectDBAwakePatch
    {
        private static void Postfix(ObjectDB __instance)
        {
            CustomItemRegistry.RegisterItems(__instance);
            AccessTools.Method(typeof(ObjectDB), "UpdateRegisters")?.Invoke(__instance, null);
        }
    }

    [HarmonyPatch(typeof(ObjectDB), nameof(ObjectDB.CopyOtherDB))]
    internal static class ObjectDBCopyOtherDBPatch
    {
        private static void Postfix(ObjectDB __instance)
        {
            CustomItemRegistry.RegisterItems(__instance);
            AccessTools.Method(typeof(ObjectDB), "UpdateRegisters")?.Invoke(__instance, null);
        }
    }
}
