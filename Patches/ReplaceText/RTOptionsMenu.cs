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
        [HarmonyPatch(typeof(OptionsMenuCell), nameof(OptionsMenuCell.Cell_OnAwake))]
        [HarmonyPostfix]
        public static void OptionsMenuCell_Cell_OnAwake(OptionsMenuCell __instance)
        {
            string buttonPath(string name)
            {
                return $"_dolly_escapeMenu/_backdrop_escapeMenu/_button_{name}/Text";
            }

            RTUtil.RemapChildTextsByPath(__instance.transform, new Dictionary<string, string>() {
                { "_headerIcon/_text_skillsHeader", I18nKeys.TabMenu.CELL_OPTIONS_HEADER },
                { buttonPath("settings"), I18nKeys.TabMenu.CELL_OPTIONS_BUTTON_SETTINGS },
                { buttonPath("saveFile"), I18nKeys.TabMenu.CELL_OPTIONS_BUTTON_SAVE_FILE },
                { buttonPath("invite"), I18nKeys.TabMenu.CELL_OPTIONS_BUTTON_INVITE_TO_LOBBY },
                { buttonPath("hostConsole"), I18nKeys.TabMenu.CELL_OPTIONS_BUTTON_HOST_CONSOLE },
                { buttonPath("quitGame"), I18nKeys.TabMenu.CELL_OPTIONS_BUTTON_SAVE_AND_QUIT }
            });
        }

    }
}
