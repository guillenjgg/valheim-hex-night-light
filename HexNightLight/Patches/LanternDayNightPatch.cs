using HarmonyLib;
using HexNightLight.Core.Managers;

namespace HexNightLight.Patches
{
    [HarmonyPatch(typeof(EnvMan), nameof(EnvMan.CalculateNight))]
    internal static class LanternDayNightPatch
    {
        // Prevents unnecessary ZDO writes when the day/night state hasn't changed.
        private static bool? _nightTimeCacheState;
        private static Player _lastPlayer;

        [HarmonyPostfix]
        private static void Postfix(bool __result)
        {
            if (!Plugin.LanternAutoToggle.Value)
            {
                _nightTimeCacheState = null;
                _lastPlayer = null;
                return;
            }

            var player = Player.m_localPlayer;

            if (player == null || LanternEquipmentManager.GetEquippedLantern(player) == null)
            {
                _nightTimeCacheState = null;
                _lastPlayer = null;
                return;
            }

            if (_lastPlayer == player && _nightTimeCacheState == __result)
            {
                return;
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

            zdo.Set(LanternVisualPatch.LanternLightEnabledZdoKey, __result);

            _nightTimeCacheState = __result;
            _lastPlayer = player;
        }
    }
}