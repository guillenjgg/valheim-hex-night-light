using HexNightLight.Core.Managers;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace HexNightLight.Assets.AssetBundles
{
    internal static class AssetBundleHelper
    {
        public const string AssetBundleName = "hexnightlight";
        public const string LanternPrefabPath = "Assets/_CustomItems/HexNightLight/HexNightLight.prefab";

        public static AssetBundle LanternAssetBundle;
        public static GameObject LanternPrefab;

        public static void Init()
        {
            LanternAssetBundle = GetAssetBundleFromResources(AssetBundleName);

            if (LanternAssetBundle == null)
            {
                Plugin.Log.LogError($"Failed to load AssetBundle: {AssetBundleName}");
                return;
            }

            LanternPrefab = LanternAssetBundle.LoadAsset<GameObject>(LanternPrefabPath);

            if (LanternPrefab == null)
            {
                Plugin.Log.LogError($"Failed to load prefab: {LanternPrefabPath}");
                return;
            }

            CustomItemRegistry.Initialize(LanternPrefab);
        }

        private static AssetBundle GetAssetBundleFromResources(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var resourceName = assembly.GetManifestResourceNames()
                .SingleOrDefault(name => name.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));

            if (resourceName == null)
            {
                Plugin.Log.LogError($"Embedded resource not found: {fileName}");
                return null;
            }

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new Exception($"Failed to open embedded resource: {resourceName}");
                }

                return AssetBundle.LoadFromStream(stream);
            }
        }
    }
}