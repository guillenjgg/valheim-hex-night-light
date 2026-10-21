using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace HexNightLight.Patches
{
    [HarmonyPatch]
    internal static class MaterialManDebugPatch
    {
        private static readonly FieldInfo PropertyBlockField =
            AccessTools.Field(typeof(MaterialMan), "m_propertyBlock");

        private static readonly FieldInfo ContainersField =
            AccessTools.Field(typeof(MaterialMan), "m_blocks");

        [HarmonyPatch(typeof(MaterialMan), "Update")]
        [HarmonyPrefix]
        private static void UpdatePrefix(MaterialMan __instance)
        {
            if (PropertyBlockField == null)
            {
                return;
            }

            var block = PropertyBlockField.GetValue(__instance) as MaterialPropertyBlock;

            if (block == null)
            {
                Plugin.Log.LogWarning("MaterialMan has a NULL MaterialPropertyBlock.");
            }
        }
    }
}