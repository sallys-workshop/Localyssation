using HarmonyLib;
using Localyssation.Util;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine.UI;

namespace Localyssation.Patches.ReplaceText
{
    internal static partial class RTReplacer
    {
        [HarmonyPatch(typeof(TabMenu), nameof(TabMenu.Handle_TabMenuControl))]
        [HarmonyPostfix]
        public static void TabMenu__Handle_TabMenuControl__Postfix(TabMenu __instance)
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

        [HarmonyPatch(typeof(TabMenu), nameof(TabMenu.Awake))]
        [HarmonyPostfix]
        public static void TabMenu__Awake__Postfix(TabMenu __instance)
        {
            RTUtil.RemapAllTextUnderObject(__instance._pointsAvailDolly_L, new Dictionary<string, string>
            {
                { "Text", I18nKeys.TabMenu.POINTS_AVAILABLE }
            });
            RTUtil.RemapAllTextUnderObject(__instance._pointsAvailDolly_R, new Dictionary<string, string>
            {
                { "Text", I18nKeys.TabMenu.POINTS_AVAILABLE }
            });
        }

        [HarmonyPatch(typeof(ItemMenuCell), nameof(ItemMenuCell.Cell_OnAwake))]
        [HarmonyPostfix]
        public static void ItemsMenu__Cell_OnAwake__Postfix(ItemMenuCell __instance)
        {
            RTUtil.RemapChildTextsByPath(__instance.transform, new Dictionary<string, string>() {
                { "_text_itemHeader", I18nKeys.TabMenu.CELL_ITEMS_HEADER }
            });
        }

        [HarmonyPatch(typeof(ItemMenuCell), nameof(ItemMenuCell.Init_ItemPromptWindow))]
        [HarmonyPostfix]
        public static void ItemMenuCell__Init_ItemPromptWindow__Postfix(ItemMenuCell __instance, ItemListDataEntry _listEntry)
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
        public static IEnumerable<CodeInstruction> ItemMenuCell__Handle_CellUpdate__Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.SimpleStringReplaceTranspiler(instructions, new List<string>()
            {
                I18nKeys.TabMenu.CELL_ITEMS_EQUIP_TAB_HEADER_EQUIPMENT,
                I18nKeys.TabMenu.CELL_ITEMS_EQUIP_TAB_HEADER_VANITY
            });
        }
        [HarmonyPatch(typeof(ItemMenuCell), nameof(ItemMenuCell.Init_ItemTabToolTip))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> ItemMenuCell__Init_ItemTabToolTip__Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.SimpleStringReplaceTranspiler(instructions, new List<string>()
            {
                I18nKeys.TabMenu.CELL_ITEMS_EQUIP_TAB_HEADER_EQUIPMENT,
                I18nKeys.TabMenu.CELL_ITEMS_EQUIP_TAB_HEADER_VANITY,
                I18nKeys.TabMenu.CELL_ITEMS_EQUIP_TAB_HEADER_STAT
            });
        }

        [HarmonyPatch(typeof(ItemMenuCell), nameof(ItemMenuCell.Init_InventoryTooltip))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> ItemMenuCell__Init_InventoryTooltip__Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.SimpleStringReplaceTranspiler(instructions, new List<string>()
            {
                I18nKeys.TabMenu.CELL_ITEMS_INVENTORY_SORT_ITEMS,
                I18nKeys.TabMenu.CELL_ITEMS_INVENTORY_TYPE_CONSUMABLE,
                I18nKeys.TabMenu.CELL_ITEMS_INVENTORY_TYPE_EQUIPMENT,
                I18nKeys.TabMenu.CELL_ITEMS_INVENTORY_TYPE_TRADE_ITEM
            });
        }
    }
}
