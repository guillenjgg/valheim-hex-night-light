using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using HexNightLight.Assets.AssetBundles;
using System.Reflection;
using UnityEngine;

namespace HexNightLight
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private const string PluginGuid = "com.hex.nightlight";
        private const string PluginName = "HexNightLight";
        private const string PluginVersion = "1.0.0";

        private Harmony _harmonyInstance;

        internal static ManualLogSource Log;
        internal static Plugin Instance;

        internal static ConfigEntry<float> LanternLightRange;
        internal static ConfigEntry<float> LanternLightIntensity;
        internal static ConfigEntry<LightShadows> LanternLightShadows;

        private void Awake()
        {
            Instance = this;
            Log = Logger;

            LanternLightRange = Config.Bind("Lantern", "LightRange", 20f, "The range of the lantern's light.");
            LanternLightIntensity = Config.Bind("Lantern", "LightIntensity", 2f, "The intensity of the lantern's light.");
            LanternLightShadows = Config.Bind("Lantern", "LightShadows", LightShadows.None, "The lantern's shadow type.");

            AssetBundleHelper.Init();

            _harmonyInstance = new Harmony(PluginGuid);
            _harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
            
            Log.LogInfo($"{PluginName} v{PluginVersion} loaded.");
        }

        private void OnDestroy()
        {
            Log.LogInfo($"{PluginName} v{PluginVersion} unloaded.");

            _harmonyInstance?.UnpatchSelf();
            _harmonyInstance = null;
            Instance = null;
            Log = null;
        }
    }
}
