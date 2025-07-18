using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localyssation.Patches.ReplaceText
{
    internal static partial class RTReplacer
    {
        [HarmonyPatch(typeof(WhoMenuCell), nameof(WhoMenuCell.Cell_OnAwake))]
        [HarmonyPostfix]
        public static void WhoMenu_Cell_OnAwake_Postfix(WhoMenuCell __instance)
        {
            string actionPath(string action)
            {
                return $"_panel_actionList/_button_{action}/_text_{action}Button";
            }
            RTUtil.RemapChildTextsByPath(__instance.transform, new Dictionary<string, string>() {
                { "_text_whoHeader", I18nKeys.TabMenu.CELL_WHO_HEADER },
                { actionPath("inviteToParty"), I18nKeys.TabMenu.CELL_WHO_BUTTON_INVITE_TO_PARTY },
                { actionPath("leaveParty"), I18nKeys.TabMenu.CELL_WHO_BUTTON_LEAVE_PARTY },
                { actionPath("mutePeer"), I18nKeys.TabMenu.CELL_WHO_BUTTON_MUTE_PEER },
                { actionPath("refreshList"), I18nKeys.TabMenu.CELL_WHO_BUTTON_REFRESH_LIST }
            });
        }
    }
}
