using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using HexNightLight.Assets.AssetBundles;
using HexNightLight.Core;
using HexNightLight.Core.Managers;
using HexNightLight.Patches;
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
        internal static ConfigEntry<KeyboardShortcut> LanternLightToggleShortcut;
        internal static bool LanternLightEnabled = true;

        private void Awake()
        {
            Instance = this;
            Log = Logger;

            LanternLightRange = Config.Bind(
                "Lantern",
                "LightRange",
                20f,
                new ConfigDescription(
                    "The range of the lantern's light.",
                    new AcceptableValueRange<float>(5f, 50f)));

            LanternLightIntensity = Config.Bind(
                "Lantern",
                "LightIntensity",
                2f,
                new ConfigDescription(
                    "The intensity of the lantern's light.",
                    new AcceptableValueRange<float>(0f, 20f)));

            LanternLightShadows = Config.Bind("Lantern", "LightShadows", LightShadows.None, "The lantern's shadow type.");

            LanternLightToggleShortcut = Config.Bind(
                "Lantern",
                "LightToggleShortcut",
                new KeyboardShortcut(KeyCode.L),
                "Hotkey to toggle the equipped lantern's light.");

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

        private void Update()
        {
            var player = Player.m_localPlayer;

            if (player == null || LanternEquipmentManager.GetEquippedLantern(player) == null)
            {
                return;
            }

            var shortcut = LanternLightToggleShortcut.Value;

            if (!ZInput.GetKeyDown(shortcut.MainKey))
            {
                return;
            }

            foreach (var modifier in shortcut.Modifiers)
            {
                if (!ZInput.GetKey(modifier))
                {
                    return;
                }
            }

            var nview = player.GetComponent<ZNetView>();

            if (nview == null || !nview.IsOwner())
            {
                return;
            }

            var zdo = nview.GetZDO();

            if (zdo == null)
            {
                return;
            }

            LanternLightEnabled = !zdo.GetBool(LanternVisualPatch.LanternLightEnabledZdoKey, true);
            zdo.Set(LanternVisualPatch.LanternLightEnabledZdoKey, LanternLightEnabled);
        }
    }
}
