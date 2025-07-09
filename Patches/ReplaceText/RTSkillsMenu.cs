using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UI;

namespace Localyssation.Patches.ReplaceText
{
    internal static class RTSkillsMenu
    {
        // skills menu
        [HarmonyPatch(typeof(SkillsMenuCell), nameof(SkillsMenuCell.Cell_OnAwake))]
        [HarmonyPostfix]
        public static void SkillsMenuCell_Cell_OnAwake(SkillsMenuCell __instance)
        {
            RTUtil.RemapAllTextUnderObject(__instance.gameObject, new Dictionary<string, string>()
            {
                { "_text_skillsHeader", "TAB_MENU_CELL_SKILLS_HEADER" },
            });
            RTUtil.RemapChildTextsByPath(__instance.transform, new Dictionary<string, string>()
            {
                { "_backdrop_skillPoints/_text_skillPointsTag", "TAB_MENU_CELL_SKILLS_SKILL_POINT_COUNTER" },
                { "Content_noviceSkills/_skillsCell_skillListObject_recall/_text_skillRank", "SKILL_RANK_SOULBOUND" },
            }, (transform, key) =>
            {
                if (key == "TAB_MENU_CELL_SKILLS_SKILL_POINT_COUNTER")
                {
                    var text = transform.GetComponent<Text>();
                    if (text) text.alignment = TextAnchor.MiddleLeft;
                }
            });
        }

        [HarmonyPatch(typeof(SkillsMenuCell), nameof(SkillsMenuCell.Init_ClassTabTooltip))]
        [HarmonyPostfix]
        public static void SkillsMenuCell_Init_ClassTabTooltip(SkillsMenuCell __instance, int _tabValue)
        {
            switch (_tabValue)
            {
                case 0:
                    ToolTipManager._current.Apply_GenericToolTip(Localyssation.GetString("TAB_MENU_CELL_SKILLS_CLASS_TAB_TOOLTIP_NOVICE"));
                    break;
                case 1:
                    var playerClass = Player._mainPlayer._pStats._class;
                    if (!playerClass) return;

                    var classKey = $"{KeyUtil.GetForAsset(playerClass)}_NAME";
                    if (Localyssation.currentLanguage.strings.ContainsKey(classKey + "_VARIANT_OF"))
                        classKey += "_VARIANT_OF";

                    ToolTipManager._current.Apply_GenericToolTip(string.Format(
                        Localyssation.GetString("TAB_MENU_CELL_SKILLS_CLASS_TAB_TOOLTIP"),
                        Localyssation.GetString(classKey, playerClass._className)));
                    break;
            }
        }

        [HarmonyPatch(typeof(SkillsMenuCell), nameof(SkillsMenuCell.Handle_CellUpdate))]
        [HarmonyPostfix]
        public static void SkillsMenuCell_Handle_CellUpdate(SkillsMenuCell __instance)
        {
            if (!TabMenu._current._isOpen || !Player._mainPlayer) return;

            var txt = __instance._skillsCell_classHeader.text;
            var fontSize = __instance._skillsCell_classHeader.fontSize;
            switch (__instance._currentSkillTab)
            {
                case SkillTier.NOVICE:
                    txt = Localyssation.GetString("TAB_MENU_CELL_SKILLS_CLASS_HEADER_NOVICE", txt, fontSize);
                    break;
                case SkillTier.CLASS:
                    var classKey = $"{KeyUtil.GetForAsset(Player._mainPlayer._pStats._class)}_NAME";
                    if (Localyssation.currentLanguage.strings.ContainsKey(classKey + "_VARIANT_OF"))
                        classKey += "_VARIANT_OF";
                    txt = string.Format(
                        Localyssation.GetString("TAB_MENU_CELL_SKILLS_CLASS_HEADER", fontSize: fontSize),
                        Localyssation.GetString(classKey, Player._mainPlayer._pStats._class._className, fontSize));
                    break;
                case SkillTier.SUBCLASS:
                    // TODO
                    break;
            }
            __instance._skillsCell_classHeader.text = txt;
        }

        // TODO: cannot decide which one to apply, rank text partially copied from `Update` method
        //[HarmonyPatch(typeof(SkillListDataEntry), nameof(SkillListDataEntry.Apply_SkillData))]
        [HarmonyPatch(typeof(SkillListDataEntry), nameof(SkillListDataEntry.Update))]
        [HarmonyPostfix]
        public static void SkillListDataEntry_Handle_SkillData(SkillListDataEntry __instance)
        {
            if (!Player._mainPlayer || Player._mainPlayer._bufferingStatus || !__instance._scriptSkill) return;

            __instance._skillNameText.text = Localyssation.GetString(
                $"{KeyUtil.GetForAsset(__instance._scriptSkill)}_NAME",
                __instance._skillNameText.text,
                __instance._skillNameText.fontSize
            );

            if (__instance._skillRankText)
            {
                // "rank" now is skill type
                if (__instance._scriptSkill._skillControlType == SkillControlType.Passive)
                {
                    __instance._skillRankText.text = Localyssation.GetString(KeyUtil.GetForAsset(__instance._scriptSkill._skillControlType));
                }
                else
                {
                    __instance._skillRankText.text = Localyssation.GetString(KeyUtil.GetForAsset(__instance._scriptSkill._skillUtilityType));
                }
            }
        }

        [HarmonyPatch(typeof(SkillToolTip), nameof(SkillToolTip.Apply_SkillStats))]
        [HarmonyPostfix]
        public static void SkillToolTip_Apply_SkillStats(SkillToolTip __instance)
        {
            if (!Player._mainPlayer || !__instance._scriptSkill) return;

            var key = KeyUtil.GetForAsset(__instance._scriptSkill);

            __instance._toolTipName.text = Localyssation.GetString($"{key}_NAME", __instance._toolTipName.text, __instance._toolTipName.fontSize);
            __instance._toolTipSubName.text = string.Format(
                Localyssation.GetString("FORMAT_SKILL_RANK", __instance._toolTipSubName.text, __instance._toolTipSubName.fontSize),
                __instance._skillStruct._rank,
                __instance._scriptSkill._skillRanks.Length);
            __instance._scaleTypeText.text = string.Format(
                Localyssation.GetString("FORMAT_SKILL_TOOLTIP_DAMAGE_TYPE", __instance._scaleTypeText.text, __instance._scaleTypeText.fontSize),
                Localyssation.GetString(KeyUtil.GetForAsset(__instance._scriptSkill._skillDamageType), __instance._scriptSkill._skillDamageType.ToString(), __instance._scaleTypeText.fontSize));
            if (!string.IsNullOrEmpty(__instance._scriptSkill._skillDescription))
                __instance._toolTipDescription.text = Localyssation.GetString($"{key}_DESCRIPTION", __instance._toolTipDescription.text, __instance._toolTipDescription.fontSize);

            var rankIndex = 0;
            if (__instance._skillStruct._rank > 0) rankIndex = __instance._skillStruct._rank - 1;

            if (__instance._scriptSkill._skillControlType != SkillControlType.Passive)
            {
                var requiredItem = __instance._scriptSkill._skillRanks[rankIndex]._requiredItem;
                if (requiredItem)
                {
                    __instance._itemCost.text = string.Format(
                        Localyssation.GetString("FORMAT_SKILL_TOOLTIP_ITEM_COST", __instance._itemCost.text, __instance._itemCost.fontSize),
                        __instance._scriptSkill._skillRanks[rankIndex]._requiredItemQuantity,
                        Localyssation.GetString($"{KeyUtil.GetForAsset(requiredItem)}_NAME", requiredItem._itemName, __instance._itemCost.fontSize));
                }
            }

            var passiveSkillText = __instance._passiveSkillTabObject.transform.Find("_passiveSkill_text").GetComponent<Text>();
            passiveSkillText.text = Localyssation.GetString("SKILL_TOOLTIP_PASSIVE", passiveSkillText.text, passiveSkillText.fontSize);
        }

        [HarmonyPatch(typeof(SkillToolTip), nameof(SkillToolTip.Apply_SkillStats))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> SkillToolTip_Apply_SkillStats_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.SimpleStringReplaceTranspiler(instructions, new Dictionary<string, string>() {
                { "{0} Mana", "FORMAT_SKILL_TOOLTIP_MANA_COST" },
                { "{0} Health", "FORMAT_SKILL_TOOLTIP_HEALTH_COST" },
                { "{0} Stamina", "FORMAT_SKILL_TOOLTIP_STAMINA_COST" },
                { "Instant Cast", "SKILL_TOOLTIP_CAST_TIME_INSTANT" },
                { "{0} sec Cast", "FORMAT_SKILL_TOOLTIP_CAST_TIME" },
                { "{0} sec Cooldown", "FORMAT_SKILL_TOOLTIP_COOLDOWN" },
            });
        }

        [HarmonyPatch(typeof(SkillToolTip), nameof(SkillToolTip.Apply_SkillDecriptorInfo))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> SkillToolTip_Apply_SkillDecriptorInfo_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.SimpleStringReplaceTranspiler(instructions, new Dictionary<string, string>() {
                { "\n<color=white><i>[Next Rank]</i></color>", "SKILL_TOOLTIP_RANK_DESCRIPTOR_NEXT_RANK" },
                { "\n<color=white><i>[Rank {0}]</i></color>", "FORMAT_SKILL_TOOLTIP_RANK_DESCRIPTOR_CURRENT_RANK" },
                { "<color=red>\n(Requires Lv. {0})</color>", "FORMAT_SKILL_TOOLTIP_RANK_DESCRIPTOR_REQUIRED_LEVEL" },
                { "<color=yellow>{0} sec cooldown.</color>", "FORMAT_SKILL_TOOLTIP_RANK_DESCRIPTOR_COOLDOWN" },
                { "<color=yellow>{0} sec cast time.</color>", "FORMAT_SKILL_TOOLTIP_RANK_DESCRIPTOR_CAST_TIME" },
                { "<color=yellow>instant cast time.</color>", "SKILL_TOOLTIP_RANK_DESCRIPTOR_CAST_TIME_INSTANT" },
            });
        }

        [HarmonyPatch(typeof(SkillToolTip), nameof(SkillToolTip.Apply_SkillDecriptorInfo))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> SkillToolTip_Apply_SkillDecriptorInfo_Transpiler2(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions)
                .MatchForward(true,
                    new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(SkillRanking), nameof(SkillRanking._rankDescriptor))));
            matcher.Advance(1);
            matcher.MatchForward(true,
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(SkillRanking), nameof(SkillRanking._rankDescriptor))));
            matcher.MatchForward(true,
                new CodeMatch(x => x.IsStloc()));
            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg, 0),
                new CodeInstruction(OpCodes.Ldarg, 1),
                Transpilers.EmitDelegate<Func<string, SkillToolTip, int, string>>((oldString, __instance, _rank) =>
                {
                    var key = KeyUtil.GetForAsset(__instance._scriptSkill);
                    return Localyssation.GetString($"{key}_RANK_{_rank + 1}_DESCRIPTOR", oldString);
                }));
            return matcher.InstructionEnumeration();
        }

        [HarmonyPatch(typeof(SkillToolTip), nameof(SkillToolTip.Apply_ConditionRankInfo))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> SkillToolTip_Apply_ConditionRankInfo_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.SimpleStringReplaceTranspiler(instructions, new Dictionary<string, string>() {
                { " <color=yellow>Cancels if hit.</color>", "SKILL_TOOLTIP_RANK_DESCRIPTOR_CONDITION_CANCEL_ON_HIT" },
                { " <color=yellow>Permanent.</color>", "SKILL_TOOLTIP_RANK_DESCRIPTOR_CONDITION_IS_PERMANENT" },
                { " <color=yellow>Lasts for {0} seconds.</color>", "FORMAT_SKILL_TOOLTIP_RANK_DESCRIPTOR_CONDITION_DURATION" },
                { " <color=yellow>Stackable.</color>", "SKILL_TOOLTIP_RANK_DESCRIPTOR_CONDITION_IS_STACKABLE" },
                { " <color=yellow>Refreshes when re-applied.</color>", "SKILL_TOOLTIP_RANK_DESCRIPTOR_CONDITION_IS_REFRESHABLE" },
            });
        }

        [HarmonyPatch(typeof(SkillToolTip), nameof(SkillToolTip.Apply_ConditionRankInfo))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> SkillToolTip_Apply_ConditionRankInfo_Transpiler2(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions)
                .MatchForward(true,
                    new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(ScriptableCondition), nameof(ScriptableCondition._conditionDescription))));
            matcher.Advance(1);
            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg, 0),
                new CodeInstruction(OpCodes.Ldarg, 1),
                Transpilers.EmitDelegate<Func<string, SkillToolTip, int, string>>((oldString, __instance, _rank) =>
                {
                    var condition = __instance._scriptSkill._skillRanks[_rank]._selfConditionOutput;
                    var key = KeyUtil.GetForAsset(condition);
                    return Localyssation.GetString($"{key}_{condition._conditionRank}_DESCRIPTION", oldString);
                }));
            return matcher.InstructionEnumeration();
        }

    }
}
