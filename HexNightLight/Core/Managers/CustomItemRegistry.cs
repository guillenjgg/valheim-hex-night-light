
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

            if (itemDrop == null)
            {
                Plugin.Log.LogError("HexNightLight prefab is missing ItemDrop.");
                return;
            }

            if (lanternPrefab.GetComponent<ZNetView>() == null)
            {
                Plugin.Log.LogError("HexNightLight prefab is missing ZNetView.");
                return;
            }

            if (itemDrop.m_itemData == null)
            {
                Plugin.Log.LogError("HexNightLight ItemDrop has no item data.");
                return;
            }

            itemDrop.m_itemData.m_dropPrefab = lanternPrefab;

            _lanternPrefab = lanternPrefab;

            Plugin.Log.LogInfo("HexNightLight registry initialized.");
        }

        internal static void RegisterPrefabs(ZNetScene scene)
        {
            if (_lanternPrefab == null || scene == null)
                return;

            if (MPrefabs == null || MNamedPrefabs == null)
            {
                Plugin.Log.LogError("Could not resolve ZNetScene prefab fields.");
                return;
            }

            var prefabs = MPrefabs.GetValue(scene) as List<GameObject>;
            var namedPrefabs = MNamedPrefabs.GetValue(scene) as Dictionary<int, GameObject>;

            if (prefabs == null || namedPrefabs == null)
            {
                Plugin.Log.LogError("ZNetScene prefab collections are unavailable.");
                return;
            }

            var hash = _lanternPrefab.name.GetStableHashCode();

            if (namedPrefabs.TryGetValue(hash, out var existing))
            {
                if (existing != _lanternPrefab)
                    Plugin.Log.LogWarning($"Prefab hash collision: {_lanternPrefab.name} ({existing.name})");

                return;
            }

            if (!prefabs.Contains(_lanternPrefab))
                prefabs.Add(_lanternPrefab);

            namedPrefabs.Add(hash, _lanternPrefab);

            Plugin.Log.LogInfo($"Registered network prefab: {_lanternPrefab.name}");
        }

        internal static void RegisterItems(ObjectDB database)
        {
            if (_lanternPrefab == null || database == null)
                return;

            foreach (var prefab in database.m_items)
            {
                if (prefab == null)
                    continue;

                if (prefab.name == _lanternPrefab.name)
                {
                    if (prefab != _lanternPrefab)
                        Plugin.Log.LogWarning($"Item name already registered: {_lanternPrefab.name}");

                    return;
                }
            }

            database.m_items.Add(_lanternPrefab);

            Plugin.Log.LogInfo($"Registered item: {_lanternPrefab.name}");
        }
    }
}
