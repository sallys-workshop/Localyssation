using HarmonyLib;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using UnityEngine.UI;

namespace Localyssation.Patches.ReplaceText
{
    internal static partial class RTReplacer
    {
        [HarmonyPatch(typeof(TabMenu), nameof(TabMenu.Handle_TabMenuControl))]
        [HarmonyPostfix]
        public static void TabMenu_Handle_TabMenuControl_Postfix(TabMenu __instance)
        {
            if (__instance._currentCellSelection > 0)
            {
                string tag = __instance._menuCells[__instance._currentCellSelection - 1]._menuCell_tag;
                string key = $"TAB_MENU_CELL_{KeyUtil.Normalize(tag)}_HEADER";
                __instance._button_previousCell.GetComponentInChildren<Text>().text = "<< " 
                    + Localyssation.GetString(key, I18nKeys.TR_KEYS[key]);
            }

            if (__instance._currentCellSelection < __instance._menuCells.Length - 1)
            {
                string tag = __instance._menuCells[__instance._currentCellSelection + 1]._menuCell_tag;
                string key = $"TAB_MENU_CELL_{KeyUtil.Normalize(tag)}_HEADER";
                __instance._button_nextCell.GetComponentInChildren<Text>().text =
                    Localyssation.GetString(key, I18nKeys.TR_KEYS[key]) + " >>";
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

        [HarmonyPatch(typeof(ItemMenuCell), nameof(ItemMenuCell.Init_ItemPromptWindow))]
        [HarmonyPostfix]
        public static void ItemMenuCell_Init_ItemPromptWindow(ItemMenuCell __instance, ItemListDataEntry _listEntry)
        {
            
            
            foreach (var kvPair in I18nKeys.TabMenu.CELL_ITEMS_PROMPT_BUTTONS)
            {
                string key = $"_{kvPair.Key}Button";
                Localyssation.logger.LogDebug(key);
                key = key.Replace("transmogrify", "transmog");
                var info = typeof(ItemMenuCell).GetField(key, BindingFlags.NonPublic | BindingFlags.Instance);
                Button button = (Button)(info.GetValue(__instance));
                button.GetComponentInChildren<Text>().text = Localyssation.GetString(kvPair.Value);
            }
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
