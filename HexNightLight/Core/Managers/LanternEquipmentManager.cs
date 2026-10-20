using System.Runtime.CompilerServices;

namespace HexNightLight.Core.Managers
{
    internal static class LanternEquipmentManager
    {
        internal const ItemDrop.ItemData.ItemType LanternItemType = (ItemDrop.ItemData.ItemType)56;

        private sealed class EquipmentState
        {
            internal ItemDrop.ItemData EquippedLantern;
        }

        private static readonly ConditionalWeakTable<Humanoid, EquipmentState> EquipmentStates = new ConditionalWeakTable<Humanoid, EquipmentState>();

        internal static bool IsLantern(ItemDrop.ItemData item)
        {
            return item != null && item.m_shared.m_itemType == LanternItemType;
        }

        internal static ItemDrop.ItemData GetEquippedLantern(Humanoid humanoid)
        {
            return EquipmentStates.TryGetValue(humanoid, out var state)
                ? state.EquippedLantern
                : null;
        }

        internal static void SetEquippedLantern(Humanoid humanoid, ItemDrop.ItemData item)
        {
            EquipmentStates.GetOrCreateValue(humanoid).EquippedLantern = item;
        }
    }
}