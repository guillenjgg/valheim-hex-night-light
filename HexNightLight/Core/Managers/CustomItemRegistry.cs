using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace HexNightLight.Core.Managers
{
    internal static class CustomItemRegistry
    {
        private static GameObject _lanternPrefab;

        private static readonly FieldInfo MPrefabs = AccessTools.Field(typeof(ZNetScene), "m_prefabs");
        private static readonly FieldInfo MNamedPrefabs = AccessTools.Field(typeof(ZNetScene), "m_namedPrefabs");

        internal static void Initialize(GameObject lanternPrefab)
        {
            if (lanternPrefab == null)
            {
                return;
            }

            var itemDrop = lanternPrefab.GetComponent<ItemDrop>();

            if (itemDrop == null || itemDrop.m_itemData == null || lanternPrefab.GetComponent<ZNetView>() == null)
            {
                return;
            }

            lanternPrefab.name = "HexNightLight";

            itemDrop.m_itemData.m_dropPrefab = lanternPrefab;
            itemDrop.m_itemData.m_shared.m_itemType = LanternEquipmentManager.LanternItemType;
            itemDrop.m_itemData.m_shared.m_weight = 1f;
            itemDrop.m_itemData.m_shared.m_movementModifier = 0f;

            _lanternPrefab = lanternPrefab;
        }

        internal static void RegisterPrefabs(ZNetScene scene)
        {
            if (_lanternPrefab == null || scene == null)
            {
                return;
            }

            var prefabs = MPrefabs?.GetValue(scene) as List<GameObject>;
            var namedPrefabs = MNamedPrefabs?.GetValue(scene) as Dictionary<int, GameObject>;

            if (prefabs == null || namedPrefabs == null)
            {
                return;
            }

            if (prefabs.Any(x => x != null && x.name == _lanternPrefab.name && x != _lanternPrefab))
            {
                Plugin.Log.LogError($"Prefab name conflict: {_lanternPrefab.name}");
                return;
            }

            var hash = _lanternPrefab.name.GetStableHashCode();

            if (namedPrefabs.TryGetValue(hash, out var existing) && existing != _lanternPrefab)
            {
                Plugin.Log.LogError($"Prefab hash conflict: {_lanternPrefab.name} ({existing.name})");
                return;
            }

            if (!prefabs.Contains(_lanternPrefab))
            {
                prefabs.Add(_lanternPrefab);
            }

            if (!namedPrefabs.ContainsKey(hash))
            {
                namedPrefabs.Add(hash, _lanternPrefab);
            }
        }

        internal static void RegisterItems(ObjectDB database)
        {
            if (_lanternPrefab == null || database == null) return;

            var itemDrop = _lanternPrefab.GetComponent<ItemDrop>();
            itemDrop.m_itemData.m_dropPrefab = _lanternPrefab;

            if (database.m_items.Any(x => x != null && x.name == _lanternPrefab.name && x != _lanternPrefab))
            {
                Plugin.Log.LogError($"Item registration conflict: {_lanternPrefab.name}");
                return;
            }

            if (!database.m_items.Contains(_lanternPrefab))
            {
                database.m_items.Add(_lanternPrefab);
            }
        }

        internal static void RegisterRecipe(ObjectDB database)
        {
            if (database == null || _lanternPrefab == null) return;

            LanternRecipeRegistry.Register(database, _lanternPrefab);
        }
    }
}
