
//using HarmonyLib;
//using HexNightLight.Assets.AssetBundles;
//using System.Collections.Generic;
//using UnityEngine;

//namespace HexNightLight.Patches
//{
//    [HarmonyPatch]
//    internal static class LanternShaderDebugPatch
//    {
//        private static readonly HashSet<int> InspectedPrefabs = new HashSet<int>();

//        [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
//        [HarmonyPostfix]
//        [HarmonyPriority(Priority.Last)]
//        private static void ZNetSceneAwakePostfix()
//        {
//            var prefab = ZNetScene.instance?.GetPrefab("HexNightLight");

//            if (prefab == null || !InspectedPrefabs.Add(prefab.GetInstanceID()))
//            {
//                return;
//            }

//            var bundle = AssetBundleHelper.LanternAssetBundle;

//            if (bundle == null)
//            {
//                Plugin.Log.LogWarning("HexNightLight AssetBundle is NULL.");
//                return;
//            }

//            Plugin.Log.LogInfo("========== HexNightLight Shader Diagnostics ==========");

//            InspectPrefab(prefab);
//            InspectBundleMaterials(bundle);
//            InspectBundleShaders(bundle);

//            Plugin.Log.LogInfo("========== End Shader Diagnostics ==========");
//        }

//        private static void InspectPrefab(GameObject prefab)
//        {
//            Plugin.Log.LogInfo("=== Prefab Renderer Materials ===");

//            var shaderNames = new HashSet<string>();
//            var materialCount = 0;

//            foreach (var renderer in prefab.GetComponentsInChildren<Renderer>(true))
//            {
//                var path = GetPath(renderer.transform, prefab.transform);
//                var materials = renderer.sharedMaterials;

//                for (var i = 0; i < materials.Length; i++)
//                {
//                    var material = materials[i];

//                    if (material == null)
//                    {
//                        Plugin.Log.LogWarning($"Renderer: {path} | Slot: {i} | NULL material");
//                        continue;
//                    }

//                    materialCount++;

//                    var shader = material.shader;
//                    var shaderName = shader != null ? shader.name : "NULL";

//                    shaderNames.Add(shaderName);

//                    Plugin.Log.LogInfo(
//                        $"Renderer: {path} | " +
//                        $"Slot: {i} | " +
//                        $"Material: {material.name} | " +
//                        $"Shader: {shaderName} | " +
//                        $"Supported: {(shader != null && shader.isSupported)} | " +
//                        $"MaterialID: {material.GetInstanceID()} | " +
//                        $"ShaderID: {(shader != null ? shader.GetInstanceID() : 0)}");
//                }
//            }

//            Plugin.Log.LogInfo(
//                $"Prefab Summary | Material References: {materialCount} | " +
//                $"Unique Shaders: {shaderNames.Count}");
//        }

//        private static void InspectBundleMaterials(AssetBundle bundle)
//        {
//            Plugin.Log.LogInfo("=== AssetBundle Materials ===");

//            var materials = bundle.LoadAllAssets<Material>();
//            var shaderNames = new HashSet<string>();

//            foreach (var material in materials)
//            {
//                if (material == null)
//                {
//                    continue;
//                }

//                var shader = material.shader;
//                var shaderName = shader != null ? shader.name : "NULL";

//                shaderNames.Add(shaderName);

//                Plugin.Log.LogInfo(
//                    $"Material: {material.name} | " +
//                    $"Shader: {shaderName} | " +
//                    $"Supported: {(shader != null && shader.isSupported)} | " +
//                    $"MaterialID: {material.GetInstanceID()} | " +
//                    $"ShaderID: {(shader != null ? shader.GetInstanceID() : 0)}");
//            }

//            Plugin.Log.LogInfo(
//                $"AssetBundle Materials: {materials.Length} | " +
//                $"Unique Shaders: {shaderNames.Count}");
//        }

//        private static void InspectBundleShaders(AssetBundle bundle)
//        {
//            Plugin.Log.LogInfo("=== AssetBundle Shader Assets ===");

//            var shaders = bundle.LoadAllAssets<Shader>();

//            foreach (var shader in shaders)
//            {
//                if (shader == null)
//                {
//                    continue;
//                }

//                Plugin.Log.LogInfo(
//                    $"Shader: {shader.name} | " +
//                    $"Supported: {shader.isSupported} | " +
//                    $"InstanceID: {shader.GetInstanceID()} | " +
//                    $"RenderQueue: {shader.renderQueue} | " +
//                    $"PassCount: {shader.passCount}");
//            }

//            Plugin.Log.LogInfo($"AssetBundle Shader Assets: {shaders.Length}");

//            Plugin.Log.LogInfo("=== Shader Mock Candidates ===");

//            var candidates = new[]
//            {
//                "Particles/Standard Surface2",
//                "Particles/Standard Unlit2",
//                "Lux Lit Particles/ Bumped"
//            };

//            foreach (var candidate in candidates)
//            {
//                var found = false;

//                foreach (var shader in shaders)
//                {
//                    if (shader != null && shader.name == candidate)
//                    {
//                        found = true;
//                        break;
//                    }
//                }

//                Plugin.Log.LogInfo(
//                    $"Candidate: {candidate} | " +
//                    $"Standalone Shader Asset Found: {found}");
//            }
//        }

//        private static string GetPath(Transform transform, Transform root)
//        {
//            var path = transform.name;

//            while (transform != root && transform.parent != null)
//            {
//                transform = transform.parent;
//                path = transform.name + "/" + path;
//            }

//            return path;
//        }
//    }
//}
