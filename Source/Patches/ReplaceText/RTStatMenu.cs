using HarmonyLib;
using Localyssation.Util;
using System.Collections.Generic;

namespace Localyssation.Patches.ReplaceText
{
    internal static partial class RTReplacer
    {
        // stats menu
        [HarmonyPatch(typeof(StatsMenuCell), nameof(StatsMenuCell.Cell_OnAwake))]
        [HarmonyPostfix]
        public static void StatsMenuCell_Cell_OnAwake(StatsMenuCell __instance)
        {
            RTUtil.RemapAllTextUnderObject(__instance.gameObject, new Dictionary<string, string>()
            {
                { "_text_statsHeader", "TAB_MENU_CELL_STATS_HEADER" },
                { "_tag_attributePoints", "TAB_MENU_CELL_STATS_ATTRIBUTE_POINT_COUNTER" },
                { "_text_attributePointCounter", "TAB_MENU_CELL_STATS_BUTTON_APPLY_ATTRIBUTE_POINTS" },
            });
            RTUtil.RemapChildTextsByPath(__instance.transform, new Dictionary<string, string>()
            {
                { "_statsCell_infoStatPanel/_statInfoCell_nickName/Image_01/Text", "TAB_MENU_CELL_STATS_INFO_CELL_NICK_NAME" },
                { "_statsCell_infoStatPanel/_statInfoCell_raceName/Image_01/Text", "TAB_MENU_CELL_STATS_INFO_CELL_RACE_NAME" },
                { "_statsCell_infoStatPanel/_statInfoCell_className/Image_01/Text", "TAB_MENU_CELL_STATS_INFO_CELL_CLASS_NAME" },
                { "_statsCell_infoStatPanel/_statInfoCell_levelCounter/Image_01/Text", "TAB_MENU_CELL_STATS_INFO_CELL_LEVEL_COUNTER" },
                { "_statsCell_infoStatPanel/_statInfoCell_experience/Image_01/Text", "TAB_MENU_CELL_STATS_INFO_CELL_EXPERIENCE" },

                { "_statsCell_infoStatPanel/_statInfoCell_maxHealth/Image_01/Text", "TAB_MENU_CELL_STATS_INFO_CELL_MAX_HEALTH" },
                { "_statsCell_infoStatPanel/_statInfoCell_maxMana/Image_01/Text", "TAB_MENU_CELL_STATS_INFO_CELL_MAX_MANA" },
                { "_statsCell_infoStatPanel/_statInfoCell_maxStamina/Image_01/Text", "TAB_MENU_CELL_STATS_INFO_CELL_MAX_STAMINA" },

                { "_statsCell_infoStatPanel/_statInfoCell_attack/Image_01/Text", "TAB_MENU_CELL_STATS_INFO_CELL_ATTACK" },
                { "_statsCell_infoStatPanel/_statInfoCell_rangedPower/Image_01/Text", "TAB_MENU_CELL_STATS_INFO_CELL_RANGED_POWER" },
                { "_statsCell_infoStatPanel/_statInfoCell_physCritical/Image_01/Text", "TAB_MENU_CELL_STATS_INFO_CELL_PHYS_CRITICAL" },

                { "_statsCell_infoStatPanel/_statInfoCell_magicPow/Image_01/Text", "TAB_MENU_CELL_STATS_INFO_CELL_MAGIC_POW" },
                { "_statsCell_infoStatPanel/_statInfoCell_magicCrit/Image_01/Text", "TAB_MENU_CELL_STATS_INFO_CELL_MAGIC_CRIT" },

                { "_statsCell_infoStatPanel/_statInfoCell_defense/Image_01/Text", "TAB_MENU_CELL_STATS_INFO_CELL_DEFENSE" },
                { "_statsCell_infoStatPanel/_statInfoCell_magicDef/Image_01/Text", "TAB_MENU_CELL_STATS_INFO_CELL_MAGIC_DEF" },

                { "_statsCell_infoStatPanel/_statInfoCell_evasion/Image_01/Text", "TAB_MENU_CELL_STATS_INFO_CELL_EVASION" },
                { "_statsCell_infoStatPanel/_statInfoCell_moveSpd/Image_01/Text", "TAB_MENU_CELL_STATS_INFO_CELL_MOVE_SPD" },
            });
        }

        [HarmonyPatch(typeof(StatsMenuCell), nameof(StatsMenuCell.Apply_StatsCellData))]
        [HarmonyPostfix]
        public static void StatsMenuCell_Apply_StatsCellData(StatsMenuCell __instance)
        {
            if (!TabMenu._current._isOpen && !__instance._mainPlayer._bufferingStatus) return;

            if (!string.IsNullOrEmpty(__instance._mainPlayer._pVisual._playerAppearanceStruct._setRaceTag))
            {
                var race = GameManager._current.Locate_PlayerRace(__instance._mainPlayer._pVisual._playerAppearanceStruct._setRaceTag);
                if (race)
                {
                    __instance._statsCell_raceTag.text = Localyssation.GetString($"{KeyUtil.GetForAsset(race)}_NAME", __instance._statsCell_raceTag.text, __instance._statsCell_raceTag.fontSize);
                }
            }

            if (__instance._mainPlayer._pStats._currentLevel >= GameManager._current._statLogics._maxMainLevel)
                __instance._statsCell_experience.text = Localyssation.GetString("EXP_COUNTER_MAX", __instance._statsCell_experience.text, __instance._statsCell_experience.fontSize);

            var classFontSize = __instance._statsCell_baseClassTag.fontSize;
            string classText;
            if (__instance._mainPlayer._pStats._class)
                classText = Localyssation.GetString($"{KeyUtil.GetForAsset(__instance._mainPlayer._pStats._class)}_NAME", __instance._mainPlayer._pStats._class._className, classFontSize);
            else
                classText = Localyssation.GetString("PLAYER_CLASS_EMPTY_NAME", GameManager._current._statLogics._emptyClassName, classFontSize);
            __instance._statsCell_baseClassTag.text = classText;
        }

        [HarmonyPatch(typeof(StatsMenuCell), nameof(StatsMenuCell.ToolTip_DisplayBaseStat))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> StatsMenuCell_ToolTip_DisplayBaseStat_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.SimpleStringReplaceTranspiler(instructions, new Dictionary<string, string>() {
                { "<b>Base Stat:</b> <i>", "TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_BEGIN" },
                { "%</i> (Critical %)", "TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_END_CRIT" },
                { "%</i> (Evasion %)", "TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_END_EVASION" },
                { "{0}</i> (Attack Power)", "TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_ATTACK_POW" },
                { "{0}</i> (Max Mana)", "TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_MAX_MP" },
                { "{0}</i> (Max Health)", "TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_MAX_HP" },
                { "{0}</i> (Dex Power)", "TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_RANGE_POW" },
                { "%</i> (Magic Critical %)", "TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_END_MAGIC_CRIT" },
                { "{0}</i> (Magic Defense)", "TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_MAGIC_DEF" },
                { "{0}</i> (Defense)", "TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_DEFENSE" },
                { "{0}</i> (Magic Power)", "TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_MAGIC_POW" },
                { "{0}</i> (Max Stamina)", "TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_MAX_STAM" },
            });
        }

        [HarmonyPatch(typeof(AttributeListDataEntry), nameof(AttributeListDataEntry.Handle_AttributeData))]
        [HarmonyPostfix]
        public static void AttributeListDataEntry_Handle_AttributeData(AttributeListDataEntry __instance)
        {
            if (!GameManager._current ||
                !Player._mainPlayer ||
                string.IsNullOrEmpty(__instance._pStats._playerAttributes[__instance._dataID]._attributeName))
                return;

            var key = KeyUtil.GetForAsset(__instance._gm._statLogics._statAttributes[__instance._dataID]);
            __instance._dataNameText.text = Localyssation.GetString($"{key}_NAME", __instance._dataNameText.text, __instance._dataNameText.fontSize);
        }

        [HarmonyPatch(typeof(AttributeListDataEntry), nameof(AttributeListDataEntry.Init_TooltipInfo))]
        [HarmonyPostfix]
        public static void AttributeListDataEntry_Init_TooltipInfo(AttributeListDataEntry __instance)
        {
            if (string.IsNullOrEmpty(__instance._scriptableAttribute._attributeDescriptor)) return;

            var key = KeyUtil.GetForAsset(__instance._scriptableAttribute);
            ToolTipManager._current.Apply_GenericToolTip(Localyssation.GetString($"{key}_DESCRIPTOR", __instance._scriptableAttribute._attributeDescriptor));
        }

    }
}
