using HarmonyLib;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Localyssation.Patches.ReplaceText
{
    internal static class RTTabMenu
    {
        [HarmonyPatch(typeof(TabMenu), nameof(TabMenu.Handle_TabMenuControl))]
        [HarmonyPostfix]
        public static void TabMenu_Handle_TabMenuControl_Postfix(TabMenu __instance)
        {
            if (__instance._currentCellSelection > 0)
            {
                string tag = __instance._menuCells[__instance._currentCellSelection - 1]._menuCell_tag;
                __instance._button_previousCell.GetComponentInChildren<Text>().text = "<< " 
                    + Localyssation.GetString($"TAB_MENU_CELL_{KeyUtil.Normalize(tag)}_HEADER");
            }

            if (__instance._currentCellSelection < __instance._menuCells.Length - 1)
            {
                string tag = __instance._menuCells[__instance._currentCellSelection + 1]._menuCell_tag;
                __instance._button_nextCell.GetComponentInChildren<Text>().text =
                    Localyssation.GetString($"TAB_MENU_CELL_{KeyUtil.Normalize(tag)}_HEADER") + " >>";
            }
        }

        [HarmonyPatch(typeof(ItemMenuCell), nameof(ItemMenuCell.Cell_OnAwake))]
        [HarmonyPostfix]
        public static void ItemsMenu_Cell_OnAwake_Postfix(ItemMenuCell __instance)
        {
            RTUtil.RemapChildTextsByPath(__instance.transform, new Dictionary<string, string>() {
                { "_text_itemHeader", I18nKeys.TabMenu.CELL_ITEMS_HEADER }
            });
        }

        [HarmonyPatch(typeof(ItemMenuCell), nameof(ItemMenuCell.Handle_CellUpdate))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> ItemMenuCell_Handle_CellUpdate_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.SimpleStringReplaceTranspiler(instructions, new List<string>()
            {
                I18nKeys.TabMenu.CELL_ITEMS_EQUIP_TAB_HEADER_EQUIPMENT,
                I18nKeys.TabMenu.CELL_ITEMS_EQUIP_TAB_HEADER_VANITY
            });
        }
        [HarmonyPatch(typeof(ItemMenuCell), nameof(ItemMenuCell.Init_ItemTabToolTip))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> ItemMenuCell_Init_ItemTabToolTip_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.SimpleStringReplaceTranspiler(instructions, new List<string>()
            {
                I18nKeys.TabMenu.CELL_ITEMS_EQUIP_TAB_HEADER_EQUIPMENT,
                I18nKeys.TabMenu.CELL_ITEMS_EQUIP_TAB_HEADER_VANITY,
                I18nKeys.TabMenu.CELL_ITEMS_EQUIP_TAB_HEADER_STAT
            });
        }
    }
}
