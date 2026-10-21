using HarmonyLib;
using System.Reflection;

namespace HexNightLight.Core.Managers
{
    internal static class LanternLocalizationManager
    {
        private static readonly MethodInfo AddWord = AccessTools.Method(typeof(Localization), "AddWord");

        internal static void Register(Localization localization)
        {
            if (localization == null || AddWord == null) return;

            AddWord.Invoke(localization, new object[] { "item_hexnightlight", "Lantern" });
            AddWord.Invoke(localization, new object[] { "item_hexnightlight_desc", "Shines brightest when you need it the most." });
        }
    }
}