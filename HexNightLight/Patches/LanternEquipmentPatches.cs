using HarmonyLib;
using HexNightLight.Core.Managers;
using System.Reflection;

namespace HexNightLight.Patches
{
    internal static class LanternEquipmentHelper
    {
        private static readonly MethodInfo SetupEquipmentMethod = AccessTools.Method(typeof(Humanoid), "SetupEquipment");

        internal static void RefreshEquipment(Humanoid humanoid)
        {
            if (SetupEquipmentMethod == null)
            {
                Plugin.Log.LogError("Could not resolve SetupEquipment.");
                return;
            }

            SetupEquipmentMethod.Invoke(humanoid, null);
        }
    }

    [HarmonyPatch(typeof(ItemDrop.ItemData), nameof(ItemDrop.ItemData.IsEquipable))]
    internal static class LanternIsEquipablePatch
    {
        private static void Postfix(ItemDrop.ItemData __instance, ref bool __result)
        {
            if (LanternEquipmentManager.IsLantern(__instance))
                __result = true;
        }
    }

    [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.EquipItem))]
    internal static class LanternEquipPatch
    {
        private static bool Prefix(Humanoid __instance, ItemDrop.ItemData item, ref bool __result)
        {
            if (!LanternEquipmentManager.IsLantern(item))
                return true;

            Plugin.Log.LogInfo($"Lantern equip requested | Item: {item.m_shared.m_name} | Prefab: {item.m_dropPrefab?.name ?? "NULL"}");

            var previous = LanternEquipmentManager.GetEquippedLantern(__instance);

            if (previous == item)
            {
                item.m_equipped = true;
                LanternEquipmentHelper.RefreshEquipment(__instance);
                __result = true;
                return false;
            }

            if (previous != null)
                __instance.UnequipItem(previous, false);

            LanternEquipmentManager.SetEquippedLantern(__instance, item);
            item.m_equipped = true;

            Plugin.Log.LogInfo("Lantern equipment state assigned.");

            LanternEquipmentHelper.RefreshEquipment(__instance);

            __result = true;
            return false;
        }
    }

    [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.IsItemEquiped))]
    internal static class LanternIsEquippedPatch
    {
        private static void Postfix(Humanoid __instance, ItemDrop.ItemData item, ref bool __result)
        {
            if (LanternEquipmentManager.IsLantern(item))
                __result = LanternEquipmentManager.GetEquippedLantern(__instance) == item;
        }
    }

    [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.UnequipItem))]
    internal static class LanternUnequipPatch
    {
        private static bool Prefix(Humanoid __instance, ItemDrop.ItemData item)
        {
            if (!LanternEquipmentManager.IsLantern(item))
                return true;

            if (LanternEquipmentManager.GetEquippedLantern(__instance) == item)
            {
                LanternEquipmentManager.SetEquippedLantern(__instance, null);
                item.m_equipped = false;

                Plugin.Log.LogInfo("Lantern unequipped.");

                LanternEquipmentHelper.RefreshEquipment(__instance);

                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.UnequipAllItems))]
    internal static class LanternUnequipAllPatch
    {
        private static void Postfix(Humanoid __instance)
        {
            var lantern = LanternEquipmentManager.GetEquippedLantern(__instance);

            if (lantern != null)
                __instance.UnequipItem(lantern, false);
        }
    }

    [HarmonyPatch(typeof(Inventory), nameof(Inventory.Changed))]
    internal static class LanternInventoryChangedPatch
    {
        private static void Postfix(Inventory __instance)
        {
            var player = Player.m_localPlayer;

            if (player == null || __instance != player.GetInventory())
                return;

            var lantern = LanternEquipmentManager.GetEquippedLantern(player);

            if (lantern != null && !__instance.ContainsItem(lantern))
            {
                Plugin.Log.LogWarning("Equipped lantern removed from inventory.");

                player.UnequipItem(lantern, false);
            }
        }
    }
}
