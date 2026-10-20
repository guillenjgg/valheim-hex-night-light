
//using HarmonyLib;
//using System.Diagnostics;
//using UnityEngine;

//namespace HexNightLight.Patches
//{
//    internal static class EquipmentTrace
//    {
//        internal static void Log(string method, string details = "", bool includeStack = false)
//        {
//            Plugin.Log.LogInfo($"[EQUIPMENT TRACE] Frame {Time.frameCount} | {method} | {details}");

//            if (includeStack)
//            {
//                Plugin.Log.LogInfo(new StackTrace(2, true).ToString());
//            }
//        }
//    }

//    [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.EquipItem))]
//    internal static class EquipItemTracePatch
//    {
//        private static void Prefix(Humanoid __instance, ItemDrop.ItemData item)
//        {
//            if (__instance != Player.m_localPlayer)
//            {
//                return;
//            }

//            EquipmentTrace.Log("EquipItem ENTER", item?.m_shared?.m_name, true);
//        }

//        private static void Postfix(Humanoid __instance, ItemDrop.ItemData item, bool __result)
//        {
//            if (__instance != Player.m_localPlayer)
//            {
//                return;
//            }

//            EquipmentTrace.Log("EquipItem EXIT", $"Item: {item?.m_shared?.m_name} | Result: {__result}");
//        }
//    }

//    [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.UnequipItem))]
//    internal static class UnequipItemTracePatch
//    {
//        private static void Prefix(Humanoid __instance, ItemDrop.ItemData item)
//        {
//            if (__instance != Player.m_localPlayer)
//            {
//                return;
//            }

//            EquipmentTrace.Log("UnequipItem ENTER", item?.m_shared?.m_name, true);
//        }

//        private static void Postfix(Humanoid __instance, ItemDrop.ItemData item)
//        {
//            if (__instance != Player.m_localPlayer)
//            {
//                return;
//            }

//            EquipmentTrace.Log("UnequipItem EXIT", item?.m_shared?.m_name);
//        }
//    }

//    [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.SetupVisEquipment))]
//    internal static class SetupVisEquipmentTracePatch
//    {
//        private static void Prefix(Humanoid __instance)
//        {
//            if (__instance != Player.m_localPlayer)
//            {
//                return;
//            }

//            EquipmentTrace.Log("SetupVisEquipment ENTER", includeStack: true);
//        }

//        private static void Postfix(Humanoid __instance)
//        {
//            if (__instance != Player.m_localPlayer)
//            {
//                return;
//            }

//            EquipmentTrace.Log("SetupVisEquipment EXIT");
//        }
//    }

//    [HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.UpdateEquipmentVisuals))]
//    internal static class UpdateEquipmentVisualsTracePatch
//    {
//        private static void Prefix(VisEquipment __instance)
//        {
//            if (__instance.GetComponent<Player>() != Player.m_localPlayer)
//            {
//                return;
//            }
//        }

//        private static void Postfix(VisEquipment __instance)
//        {
//            if (__instance.GetComponent<Player>() != Player.m_localPlayer)
//            {
//                return;
//            }

//            EquipmentTrace.Log("UpdateEquipmentVisuals EXIT");
//        }
//    }
//}
