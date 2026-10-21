using HarmonyLib;
using HexNightLight.Core;
using HexNightLight.Core.Managers;
using System.Collections;
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
        internal const string LanternLightEnabledZdoKey = "HexNightLight_LightEnabled";

        private static readonly AccessTools.FieldRef<VisEquipment, ZNetView> NViewField = AccessTools.FieldRefAccess<VisEquipment, ZNetView>("m_nview");
        private static readonly AccessTools.FieldRef<Character, ZNetView> CharacterNViewField = AccessTools.FieldRefAccess<Character, ZNetView>("m_nview");

        private static readonly MethodInfo AttachArmorMethod = AccessTools.Method(typeof(VisEquipment), "AttachArmor");
        private static readonly MethodInfo UpdateLodgroupMethod = AccessTools.Method(typeof(VisEquipment), "UpdateLodgroup");
        private static readonly AccessTools.FieldRef<MaterialMan, MaterialPropertyBlock> MaterialPropertyBlockField =
    AccessTools.FieldRefAccess<MaterialMan, MaterialPropertyBlock>("m_propertyBlock");

        private sealed class VisualState
        {
            internal int CurrentHash;
            internal List<GameObject> Attachments;
            internal LanternLightComponent LightComponent;
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
                if (state.LightComponent != null)
                {
                    state.LightComponent.RefreshLightState();
                }

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
                state.LightComponent = null;
            }

            if (hash == 0)
            {
                state.CurrentHash = 0;
                UpdateLodgroupMethod?.Invoke(__instance, null);
                return;
            }

            var attachments = AttachArmorMethod?.Invoke(__instance, new object[] { hash, -1, 0 }) as List<GameObject>;

            if (attachments == null || attachments.Count == 0)
            {
                return;
            }

            state.Attachments = attachments;
            state.LightComponent = null;
            state.CurrentHash = hash;
            
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

                state.LightComponent = lightTransform.gameObject.AddComponent<LanternLightComponent>();
                state.LightComponent.Initialize(nview);
            }

            UpdateLodgroupMethod?.Invoke(__instance, null);
        }

        [HarmonyPatch(typeof(FejdStartup), nameof(FejdStartup.SetupCharacterPreview))]
        [HarmonyPostfix]
        private static void SetupCharacterPreviewPostfix(FejdStartup __instance)
        {
            var player = __instance.GetPreviewPlayer();
            if (player == null) return;

            __instance.StartCoroutine(AttachCharacterPreviewLantern(__instance, player));
        }

        private static IEnumerator AttachCharacterPreviewLantern(FejdStartup startup, Player player)
        {
            while (player != null && startup.GetPreviewPlayer() == player)
            {
                var materialMan = Object.FindFirstObjectByType<MaterialMan>();

                if (materialMan != null && MaterialPropertyBlockField(materialMan) != null)
                {
                    break;
                }

                yield return null;
            }

            if (player == null || startup.GetPreviewPlayer() != player)
            {
                yield break;
            }

            var lantern = LanternEquipmentManager.GetEquippedLantern(player);
            if (lantern?.m_dropPrefab == null) yield break;

            var visEquipment = player.GetComponentInChildren<VisEquipment>();
            if (visEquipment == null) yield break;

            var hash = lantern.m_dropPrefab.name.GetStableHashCode();
            var attachments = AttachArmorMethod?.Invoke(visEquipment, new object[] { hash, -1, 0 }) as List<GameObject>;

            if (attachments == null || attachments.Count == 0)
            {
                yield break;
            }

            UpdateLodgroupMethod?.Invoke(visEquipment, null);
        }
    }
}
