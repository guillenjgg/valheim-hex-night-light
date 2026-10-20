
using HarmonyLib;
using HexNightLight.Core.Managers;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace HexNightLight.Patches
{
    [HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.UpdateEquipmentVisuals))]
    internal static class LanternVisualPatch
    {
        private sealed class VisualState
        {
            internal GameObject Attachment;
        }

        private static readonly ConditionalWeakTable<VisEquipment, VisualState> States = new ConditionalWeakTable<VisEquipment, VisualState>();

        private static void Postfix(VisEquipment __instance)
        {
            var player = __instance.GetComponent<Player>();

            if (player == null)
                return;

            var state = States.GetOrCreateValue(__instance);
            var lantern = LanternEquipmentManager.GetEquippedLantern(player);

            if (lantern == null)
            {
                if (state.Attachment != null)
                    Object.Destroy(state.Attachment);

                state.Attachment = null;
                return;
            }

            if (state.Attachment != null)
                return;

            var hips = __instance.transform.Find("Visual/Armature/Hips");

            if (hips == null)
            {
                Plugin.Log.LogError("Lantern attachment failed: Hips not found.");
                return;
            }

            var prefab = ObjectDB.instance.GetItemPrefab("HexNightLight");
            var attachmentPrefab = prefab != null ? prefab.transform.Find("attach_back/default") : null;

            if (attachmentPrefab == null)
            {
                Plugin.Log.LogError("Lantern attachment failed: attach_back/default not found.");
                return;
            }

            state.Attachment = Object.Instantiate(attachmentPrefab.gameObject, hips, false);
            state.Attachment.transform.localPosition = Vector3.zero;
            state.Attachment.transform.localRotation = Quaternion.identity;
            state.Attachment.SetActive(true);

            Plugin.Log.LogInfo("Lantern attached to Hips.");
        }
    }
}
