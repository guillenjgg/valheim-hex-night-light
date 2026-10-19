using HarmonyLib;
using HexNightLight.Core.Managers;

namespace HexNightLight.Patches
{
    [HarmonyPatch(typeof(ObjectDB), nameof(ObjectDB.CopyOtherDB))]
    internal static class ObjectDBCopyOtherDBPatch
    {
        private static void Prefix(ObjectDB other)
        {
            CustomItemRegistry.RegisterItems(other);
        }
    }
}