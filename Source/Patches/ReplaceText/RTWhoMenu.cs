using HarmonyLib;
using Localyssation.Util;
using System.Collections.Generic;

namespace Localyssation.Patches.ReplaceText
{
    internal static partial class RTReplacer
    {
        [HarmonyPatch(typeof(WhoMenuCell), nameof(WhoMenuCell.Cell_OnAwake))]
        [HarmonyPostfix]
        public static void WhoMenu_Cell_OnAwake_Postfix(WhoMenuCell __instance)
        {
            RTUtil.RemapChildTextsByPath(__instance.transform, new Dictionary<string, string>() {
                { "_text_whoHeader", I18nKeys.TabMenu.CELL_WHO_HEADER }
            });
        }

        [HarmonyPatch(typeof(WhoMenuCell), nameof(WhoMenuCell.Init_StringGenericToolTip))]
        [HarmonyPrefix]
        public static void WhoMenuCell_Init_StringGenericToolTip_Prefix(ref string _string)
        {
            var tooltipKeys = new Dictionary<string, TranslationKey>()
            {
                { "Refresh List", I18nKeys.TabMenu.CELL_WHO_BUTTON_REFRESH_LIST },
                { "Mute / Unmute Player", I18nKeys.TabMenu.CELL_WHO_BUTTON_MUTE_PEER },
                { "Party Invite", I18nKeys.TabMenu.CELL_WHO_BUTTON_INVITE_TO_PARTY },
                { "Give Leadership", I18nKeys.TabMenu.CELL_WHO_BUTTON_GIVE_LEADERSHIP },
                { "Teleport to Leader", I18nKeys.TabMenu.CELL_WHO_BUTTON_TELEPORT_TO_LEADER },
                { "Leave Party", I18nKeys.TabMenu.CELL_WHO_BUTTON_LEAVE_PARTY }
            };

            if (_string != null && tooltipKeys.TryGetValue(_string, out var key))
                _string = key.Localize(_string);
        }
    }
}
