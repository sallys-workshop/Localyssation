using HarmonyLib;
using System.Collections.Generic;

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
                { buttonPath("quitGame"), I18nKeys.TabMenu.CELL_OPTIONS_BUTTON_SAVE_AND_QUIT },
                { "_dolly_confirmQuit/_backdrop_confirmQuit/_confirmQuit_header", I18nKeys.TabMenu.CELL_OPTIONS_CONFIRM_QUIT_HEADER },
                { "_dolly_confirmQuit/_backdrop_confirmQuit/_button_confirmSaveQuit/Text", I18nKeys.TabMenu.CELL_OPTIONS_CONFIRM_QUIT_CONFIRM },
                { "_dolly_confirmQuit/_backdrop_confirmQuit/_button_cancelSaveQuit/Text", I18nKeys.TabMenu.CELL_OPTIONS_CONFIRM_QUIT_CANCEL }
            });
        }

        [HarmonyPatch(typeof(OptionsMenuCell), nameof(OptionsMenuCell.Handle_CellUpdate))]
        [HarmonyPostfix]
        public static void OptionsMenuCell_Handle_CellUpdate_Postfix(OptionsMenuCell __instance)
        {
            __instance._quitGameButtonText.text = Localyssation.GetString(I18nKeys.TabMenu.CELL_OPTIONS_BUTTON_SAVE_AND_QUIT);
        }

    }
}
