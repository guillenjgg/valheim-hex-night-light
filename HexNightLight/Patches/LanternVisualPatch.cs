using HarmonyLib;
using HexNightLight.Core;
using HexNightLight.Core.Managers;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace HexNightLight.Patches
{
    [HarmonyPatch]
    internal static class LanternVisualPatch
    {
        private const string LanternZdoKey = "HexNightLight_Lantern";

        private static readonly AccessTools.FieldRef<VisEquipment, ZNetView> NViewField = AccessTools.FieldRefAccess<VisEquipment, ZNetView>("m_nview");
        private static readonly AccessTools.FieldRef<Character, ZNetView> CharacterNViewField = AccessTools.FieldRefAccess<Character, ZNetView>("m_nview");

        private static readonly MethodInfo AttachArmorMethod = AccessTools.Method(typeof(VisEquipment), "AttachArmor");
        private static readonly MethodInfo UpdateLodgroupMethod = AccessTools.Method(typeof(VisEquipment), "UpdateLodgroup");

        private sealed class VisualState
        {
            internal int CurrentHash;
            internal List<GameObject> Attachments;
        }

        private static readonly ConditionalWeakTable<VisEquipment, VisualState> States = new ConditionalWeakTable<VisEquipment, VisualState>();

        [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.SetupVisEquipment))]
        [HarmonyPostfix]
        private static void SetupVisEquipmentPostfix(Humanoid __instance, VisEquipment visEq, bool isRagdoll)
        {
            if (isRagdoll || !__instance.IsPlayer() || visEq == null)
            {
                return;
            }

            var nview = CharacterNViewField(__instance);
            var zdo = nview != null ? nview.GetZDO() : null;

            if (zdo == null || !nview.IsOwner())
            {
                return;
            }

            var lantern = LanternEquipmentManager.GetEquippedLantern(__instance);
            var hash = lantern?.m_dropPrefab != null
                ? lantern.m_dropPrefab.name.GetStableHashCode()
                : 0;

            zdo.Set(LanternZdoKey, hash);
        }

        [HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.UpdateEquipmentVisuals))]
        [HarmonyPostfix]
        private static void UpdateEquipmentVisualsPostfix(VisEquipment __instance)
        {
            if (!__instance.m_isPlayer)
            {
                return;
            }

            var nview = NViewField(__instance);
            var zdo = nview != null ? nview.GetZDO() : null;
            var hash = zdo?.GetInt(LanternZdoKey, 0) ?? 0;
            var state = States.GetOrCreateValue(__instance);

            if (state.CurrentHash == hash)
            {
                return;
            }

            if (state.Attachments != null)
            {
                foreach (var attachment in state.Attachments)
                {
                    if (attachment != null)
                    {
                        Object.Destroy(attachment);
                    }
                }

                state.Attachments = null;
            }

            state.CurrentHash = hash;

            if (hash != 0)
            {
                state.Attachments = AttachArmorMethod?.Invoke(__instance, new object[] { hash, -1, 0 }) as List<GameObject>;

                if (state.Attachments == null || state.Attachments.Count == 0)
                {
                    Plugin.Log.LogWarning("Lantern visual attachment failed.");
                }
                else
                {
                    foreach (var attachment in state.Attachments)
                    {
                        if (attachment == null)
                        {
                            continue;
                        }

                        var lightTransform = attachment.transform.Find("default/Point Light");

                        if (lightTransform == null)
                        {
                            continue;
                        }

                        lightTransform.gameObject.AddComponent<LanternLightComponent>();
                    }
                }
            }

            UpdateLodgroupMethod?.Invoke(__instance, null);
        }
    }
}
