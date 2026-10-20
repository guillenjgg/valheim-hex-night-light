using HarmonyLib;
using System.Collections.Generic;
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
                Plugin.Log.LogError("HexNightLight prefab is null.");
                return;
            }

            var itemDrop = lanternPrefab.GetComponent<ItemDrop>();

            if (itemDrop == null || itemDrop.m_itemData == null || lanternPrefab.GetComponent<ZNetView>() == null)
            {
                Plugin.Log.LogError("HexNightLight prefab is missing required components.");
                return;
            }

            lanternPrefab.name = "HexNightLight";

            itemDrop.m_itemData.m_dropPrefab = lanternPrefab;
            itemDrop.m_itemData.m_shared.m_itemType = LanternEquipmentManager.LanternItemType;

            _lanternPrefab = lanternPrefab;

            foreach (Transform child in lanternPrefab.transform)
                Plugin.Log.LogInfo($"Lantern prefab child: {child.name}");

            Plugin.Log.LogInfo("HexNightLight registry initialized.");
        }


        internal static void RegisterPrefabs(ZNetScene scene)
        {
            if (_lanternPrefab == null || scene == null)
                return;

            var prefabs = MPrefabs?.GetValue(scene) as List<GameObject>;
            var namedPrefabs = MNamedPrefabs?.GetValue(scene) as Dictionary<int, GameObject>;

            if (prefabs == null || namedPrefabs == null)
            {
                Plugin.Log.LogError("ZNetScene prefab collections are unavailable.");
                return;
            }

            var hash = _lanternPrefab.name.GetStableHashCode();

            if (namedPrefabs.TryGetValue(hash, out var existing))
            {
                if (existing != _lanternPrefab)
                {
                    Plugin.Log.LogError($"Prefab hash collision: {_lanternPrefab.name} ({existing.name})");
                    return;
                }

                if (!prefabs.Contains(_lanternPrefab))
                    prefabs.Add(_lanternPrefab);

                return;
            }

            if (!prefabs.Contains(_lanternPrefab))
                prefabs.Add(_lanternPrefab);

            namedPrefabs.Add(hash, _lanternPrefab);

            Plugin.Log.LogInfo($"Registered network prefab: {_lanternPrefab.name}");
        }


        internal static void RegisterItems(ObjectDB database)
        {
            if (_lanternPrefab == null || database == null) return;

            var itemDrop = _lanternPrefab.GetComponent<ItemDrop>();
            itemDrop.m_itemData.m_dropPrefab = _lanternPrefab;

            if (!database.m_items.Contains(_lanternPrefab))
            {
                database.m_items.RemoveAll(item => item != null && item.name == _lanternPrefab.name);
                database.m_items.Add(_lanternPrefab);
            }

            database.UpdateRegisters();

            Plugin.Log.LogInfo($"Registered item: {_lanternPrefab.name}");
        }
    }
}
