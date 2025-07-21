using HarmonyLib;
using Localyssation.Util;
using Mirror;
using System.Collections.Generic;

namespace Localyssation.Patches.ReplaceText
{
    internal static partial class RTReplacer
    {
        [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.InteractQueue_Portal))]
        [HarmonyPostfix]
        public static void PlayerInteract__InteractQueue_Portal__Postfix(PlayerInteract __instance, Portal _foundPortal)
        {
            if (!NetworkClient.active)
                return;
            string key = "";
            if (_foundPortal != null)
            {
                if (_foundPortal._scenePortal != null)
                {
                    if (_foundPortal._scenePortal._portalCaptionTitle != null)
                    {
                        key = _foundPortal._scenePortal._portalCaptionTitle;
                    }
                }
            }
            InGameUI._current.PortalCaptionPrompt(
                !string.IsNullOrEmpty(key) ?
                Localyssation.GetString(KeyUtil.GetForMapName(key)) : ""
            );
        }

        [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.InteractQueue_RecallPortal))]
        [HarmonyPostfix]
        public static void PlayerInteract__InteractQueue_RecallPortal__Postfix(PlayerInteract __instance, RecallPortal _foundPortal)
        {
            if (!NetworkClient.active)
                return;
            string key = "";
            if (_foundPortal != null)
            {
                if (_foundPortal._scenePortal != null)
                {
                    if (_foundPortal._scenePortal._portalCaptionTitle != null)
                    {
                        key = _foundPortal._scenePortal._portalCaptionTitle;
                    }
                }
            }
            InGameUI._current.PortalCaptionPrompt(
                !string.IsNullOrEmpty(key) ?
                Localyssation.GetString(KeyUtil.GetForMapName(key)) : ""
            );
        }

        [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.Handle_InteractControl))]
        [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.InteractQueue_RevivePlayer))]
        [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.InteractQueue_DialogTrigger))]
        [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.InteractQueue_QuestTrigger))]
        [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.InteractQueue_Pushblock))]
        [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.InteractQueue_NetTrigger))]
        [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.InteractQueue_Portal))]
        [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.InteractQueue_RecallPortal))]
        [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.InteractQueue_ItemChestEntity))]
        [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.InteractQueue_ItemObject))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> PlayerInteract_General__Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.SimpleStringReplaceTranspiler(instructions, new[] {
                I18nKeys.Lore.INTERACT_REEL,
                I18nKeys.Lore.INTERACT_REVIVE,
                I18nKeys.Lore.INTERACT_INTERACT,
                I18nKeys.Lore.INTERACT_HOLD,
                I18nKeys.Lore.INTERACT_TELEPORT,
                I18nKeys.Lore.INTERACT_OPEN,
                I18nKeys.Lore.INTERACT_PICK_UP
            }, supressNotfoundWarnings: true);
        }

    }
}
