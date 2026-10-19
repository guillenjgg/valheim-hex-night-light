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
                throw new Exception($"Failed to load AssetBundle: {AssetBundleName}");
            }

            LanternPrefab = LanternAssetBundle.LoadAsset<GameObject>(LanternPrefabPath);

            if (LanternPrefab == null)
            {
                throw new Exception($"Failed to load prefab: {LanternPrefabPath}");
            }

            foreach (var assetName in LanternAssetBundle.GetAllAssetNames())
            {
                Plugin.Log.LogInfo($"AssetBundle asset: {assetName}");
            }

            LogPrefabDiagnostics(LanternPrefab);

            Plugin.Log.LogInfo("HexNightLight AssetBundle loaded.");
        }

        private static AssetBundle GetAssetBundleFromResources(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var resourceName = assembly.GetManifestResourceNames()
                .SingleOrDefault(name => name.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));

            if (resourceName == null)
            {
                throw new Exception($"Embedded resource not found: {fileName}");
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

        private static void LogPrefabDiagnostics(GameObject prefab)
        {
            Plugin.Log.LogInfo($"Prefab: {prefab.name}");

            foreach (var renderer in prefab.GetComponentsInChildren<Renderer>(true))
            {
                var path = GetTransformPath(renderer.transform, prefab.transform);

                foreach (var material in renderer.sharedMaterials)
                {
                    if (material == null)
                    {
                        Plugin.Log.LogWarning($"Missing material | Object: {path}");
                        continue;
                    }

                    var shader = material.shader;
                    var shaderName = shader != null ? shader.name : "NULL";
                    var shaderSupported = shader != null && shader.isSupported;

                    Plugin.Log.LogInfo(
                        $"Material | Object: {path} | " +
                        $"Name: {material.name} | " +
                        $"Shader: {shaderName} | " +
                        $"Supported: {shaderSupported}");

                    if (shader == null || !shaderSupported)
                    {
                        Plugin.Log.LogWarning(
                            $"Shader issue | Object: {path} | Material: {material.name}");
                    }

                    var textureProperties = material.GetTexturePropertyNames();

                    foreach (var propertyName in textureProperties)
                    {
                        var texture = material.GetTexture(propertyName);

                        if (texture == null)
                        {
                            continue;
                        }

                        Plugin.Log.LogInfo(
                            $"Texture | Object: {path} | " +
                            $"Material: {material.name} | " +
                            $"Property: {propertyName} | " +
                            $"Name: {texture.name} | " +
                            $"Size: {texture.width}x{texture.height}");
                    }
                }
            }
        }

        private static string GetTransformPath(Transform transform, Transform root)
        {
            var path = transform.name;

            while (transform != root && transform.parent != null)
            {
                transform = transform.parent;
                path = transform.name + "/" + path;
            }

            return path;
        }
    }
}