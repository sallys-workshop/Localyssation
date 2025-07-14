using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UI;

namespace Localyssation.Patches.ReplaceText
{
    internal static partial class RTReplacer
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
                { "_skillContentGroups/Content_generalSkills/_skillsCell_skillListObject_recall/_text_skillRank", "SKILL_RANK_SOULBOUND" },
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
            var pStats = Player._mainPlayer._pStats;
            switch ((SkillTier)(byte)_tabValue)
            {
                case SkillTier.NOVICE:
                    ToolTipManager._current.Apply_GenericToolTip(Localyssation.GetString("TAB_MENU_CELL_SKILLS_CLASS_TAB_TOOLTIP_NOVICE"));
                    break;
                case SkillTier.CLASS:
                    var playerClass = Player._mainPlayer._pStats._class;
                    if (!playerClass) return;

                    var classKey = $"{KeyUtil.GetForAsset(playerClass)}_NAME";
                    if (Localyssation.currentLanguage.strings.ContainsKey(classKey + "_VARIANT_OF"))
                        classKey += "_VARIANT_OF";

                    ToolTipManager._current.Apply_GenericToolTip(string.Format(
                        Localyssation.GetString("TAB_MENU_CELL_SKILLS_CLASS_TAB_TOOLTIP"),
                        Localyssation.GetString(classKey, playerClass._className)));
                    break;
                case SkillTier.SUBCLASS:
                    if (!pStats._class || pStats._syncClassTier <= 0)
                    {
                        return;
                    }
                    ToolTipManager._current.Apply_GenericToolTip(
                        string.Format(
                            Localyssation.GetString("TAB_MENU_CELL_SKILLS_CLASS_TAB_TOOLTIP"),
                            KeyUtil.GetForAsset(pStats._class._playerClassTiers[pStats._syncClassTier - 1])
                        )
                    );
                        //pStats._class._playerClassTiers[pStats._syncClassTier - 1]._classTierName + " Skills");
                    break;
            }
        }

        [HarmonyPatch(typeof(SkillsMenuCell), nameof(SkillsMenuCell.Handle_CellUpdate))]
        [HarmonyPostfix]
        public static void SkillsMenuCell_Handle_CellUpdate(SkillsMenuCell __instance)
        {
            if (!TabMenu._current._isOpen || !Player._mainPlayer) return;

            var pStats = Player._mainPlayer._pStats;
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
                    if (!pStats._class || pStats._syncClassTier <= 0)
                    {
                        return;
                    }
                    ToolTipManager._current.Apply_GenericToolTip(
                        string.Format(
                            Localyssation.GetString("TAB_MENU_CELL_SKILLS_CLASS_CLASS_HEADER"),
                            KeyUtil.GetForAsset(pStats._class._playerClassTiers[pStats._syncClassTier - 1])
                        )
                    );
                    break;
            }
            __instance._skillsCell_classHeader.text = txt;
        }

        [HarmonyPatch(typeof(SkillListDataEntry), nameof(SkillListDataEntry.Update))]
        [HarmonyPostfix]
        public static void SkillListDataEntry_Handle_SkillData(SkillListDataEntry __instance)
        {
            if (!__instance) return;
            if (!Player._mainPlayer || Player._mainPlayer._bufferingStatus || !__instance._scriptSkill) return;
            ScriptableSkill skill = __instance._scriptSkill;
            __instance._skillNameText.text = Localyssation.GetString(
                $"{KeyUtil.GetForAsset(skill)}_NAME",
                __instance._skillNameText.text,
                __instance._skillNameText.fontSize
            );
            SkillStruct _skillStruct = __instance._skillStruct;
            
            if (_skillStruct._skillUnlocked)
            {
                if (__instance._skillRankText)
                {
                    // "rank" now is skill type
                    if (skill._skillControlType == SkillControlType.Passive)
                    {
                        __instance._skillRankText.text = Localyssation.GetString(KeyUtil.GetForAsset(skill._skillControlType));
                    }
                    else
                    {
                        __instance._skillRankText.text = Localyssation.GetString(KeyUtil.GetForAsset(skill._skillUtilityType));
                    }

                }
            }
            else
            {
                //__instance._skillRankText.text = $"Point Cost: {__instance._scriptSkill._skillRankParams._skillPointCost}";
                if (__instance._skillRankText)
                {
                    __instance._skillRankText.text = string.Format(
                        Localyssation.GetString(I18nKeys.SkillMenu.SKILL_POINT_COST_FORMAT),
                        skill._skillRankParams._skillPointCost
                    );
                }
                
            }

        }

        [HarmonyPatch(typeof(SkillToolTip), nameof(SkillToolTip.Apply_SkillStats))]
        [HarmonyPostfix]
        public static void SkillToolTip_Apply_SkillStats(SkillToolTip __instance)
        {
            if (!Player._mainPlayer || !__instance._scriptSkill) return;
            var skill = __instance._scriptSkill;
            var key = KeyUtil.GetForAsset(__instance._scriptSkill);
            __instance._toolTipName.text = Localyssation.GetString($"{key}_NAME",fontSize: __instance._toolTipName.fontSize);


            if (skill._skillControlType != SkillControlType.Passive)
            {
                NonPassiveSkillsTooltip();
            }
            else
            {
                __instance._toolTipSubName.text = Localyssation.GetString(KeyUtil.GetForAsset(SkillControlType.Passive));
            }

            

            void NonPassiveSkillsTooltip()
            {
                __instance._toolTipSubName.text = Localyssation.GetString(KeyUtil.GetForAsset(skill._skillUtilityType));
                __instance._scaleTypeText.text = Localyssation.GetString(KeyUtil.GetForAsset(skill._skillDamageType));
            }

        }

        [HarmonyPatch(typeof(SkillToolTip), nameof(SkillToolTip.Apply_SkillStats))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> SkillToolTip_Apply_SkillStats_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.SimpleStringReplaceTranspiler(instructions, ImmutableArray.Create(
                I18nKeys.SkillMenu.TOOLTIP_MANA_COST,
                I18nKeys.SkillMenu.TOOLTIP_HEALTH_COST,
                I18nKeys.SkillMenu.TOOLTIP_STAMINA_COST,
                I18nKeys.SkillMenu.TOOLTIP_CAST_TIME,
                I18nKeys.SkillMenu.TOOLTIP_ITEM_COST,
                I18nKeys.SkillMenu.TOOLTIP_CAST_TIME_INSTANT,
                I18nKeys.SkillMenu.TOOLTIP_COOLDOWN
            ));
        }




        [HarmonyPatch(typeof(SkillToolTip), nameof(SkillToolTip.Apply_SkillDescriptorInfo))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> SkillToolTip_Apply_SkillDecriptorInfo_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            //return RTUtil.SimpleStringReplaceTranspiler(instructions, new Dictionary<string, string>() {
            //    //{ "\n<color=white><i>[Next Rank]</i></color>", "SKILL_TOOLTIP_RANK_DESCRIPTOR_NEXT_RANK" },
            //    //{ "\n<color=white><i>[Rank {0}]</i></color>", "FORMAT_SKILL_TOOLTIP_RANK_DESCRIPTOR_CURRENT_RANK" },
            //    //{ "<color=red>\n(Requires Lv. {0})</color>", "FORMAT_SKILL_TOOLTIP_RANK_DESCRIPTOR_REQUIRED_LEVEL" },
            //    { "<color=yellow>{0} sec cooldown.</color>", I18nKeys.SkillMenu.TOOLTIP_RANK_DESCRIPTOR_COOLDOWN }, //有用
            //    { "<color=yellow>{0} sec cast time.</color>", "FORMAT_SKILL_TOOLTIP_RANK_DESCRIPTOR_CAST_TIME" },
            //    { "<color=yellow>instant cast time.</color>", "SKILL_TOOLTIP_RANK_DESCRIPTOR_CAST_TIME_INSTANT" },
            //});
            return RTUtil.SimpleStringReplaceTranspiler(RTUtil.SimpleStringReplaceTranspiler(instructions, ImmutableList.Create(
                I18nKeys.SkillMenu.TOOLTIP_DESCRIPTOR_COOLDOWN,
                I18nKeys.SkillMenu.TOOLTIP_DESCRIPTOR_CAST_TIME,
                I18nKeys.SkillMenu.TOOLTIP_DESCRIPTOR_CAST_TIME_INSTANT,
                I18nKeys.SkillMenu.TOOLTIP_DESCRIPTOR_MANACOST,
                I18nKeys.SkillMenu.TOOLTIP_DESCRIPTOR_HEALTHCOST,
                I18nKeys.SkillMenu.TOOLTIP_DESCRIPTOR_STAMINACOST,

                I18nKeys.SkillMenu.TOOLTIP_REQUIRE_SHIELD
            )), typeof(SkillToolTipRequirement).GetEnumValues().Cast<SkillToolTipRequirement>().ToDictionary(
                x => string.Format(Localyssation.GetDefaultString(I18nKeys.SkillMenu.TOOLTIP_REQUIEMENT_FORMAT), x.ToString().ToLower()), 
                x => string.Format(Localyssation.GetString(I18nKeys.SkillMenu.TOOLTIP_REQUIEMENT_FORMAT), Localyssation.GetString(KeyUtil.GetForAsset(x)))
            ));
        }

        [HarmonyPatch(typeof(SkillToolTip), nameof(SkillToolTip.Apply_SkillDescriptorInfo))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> SkillToolTip_Apply_SkillDecriptorInfo_Transpiler2(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions)
                .MatchForward(true,
                    new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(ScriptableSkill), nameof(ScriptableSkill._skillDescription))));
            matcher.Advance(1);
            matcher.MatchForward(true,
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(ScriptableSkill), nameof(ScriptableSkill._skillDescription))));
            matcher.MatchForward(true,
                new CodeMatch(x => x.IsStloc()));
            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg, 0),
                new CodeInstruction(OpCodes.Ldarg, 1),
                Transpilers.EmitDelegate<Func<string, SkillToolTip, int, string>>((oldString, __instance, _rank) =>
                {
                    var key = KeyUtil.GetForAsset(__instance._scriptSkill);
                    return Localyssation.GetString($"{key}_DESCRIPTOR", oldString);
                }));
            return matcher.InstructionEnumeration();
        }

        [HarmonyPatch(typeof(ScriptableStatusCondition), nameof(ScriptableStatusCondition.Generate_ConditionDescriptor))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> ScriptableStatusCondition_Generate_ConditionDescriptor_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.SimpleStringReplaceTranspiler(instructions, ImmutableList.Create(
                I18nKeys.ScriptableStatusCondition.RATE_FORMAT,
                I18nKeys.ScriptableStatusCondition.DURATION_FORMAT
            ));
        }




        //[HarmonyPatch(typeof(SkillToolTip), nameof(SkillToolTip.Apply_ConditionRankInfo))]
        //[HarmonyTranspiler]
        //public static IEnumerable<CodeInstruction> SkillToolTip_Apply_ConditionRankInfo_Transpiler(IEnumerable<CodeInstruction> instructions)
        //{
        //    return RTUtil.SimpleStringReplaceTranspiler(instructions, new Dictionary<string, string>() {
        //        { " <color=yellow>Cancels if hit.</color>", "SKILL_TOOLTIP_RANK_DESCRIPTOR_CONDITION_CANCEL_ON_HIT" },
        //        { " <color=yellow>Permanent.</color>", "SKILL_TOOLTIP_RANK_DESCRIPTOR_CONDITION_IS_PERMANENT" },
        //        { " <color=yellow>Lasts for {0} seconds.</color>", "FORMAT_SKILL_TOOLTIP_RANK_DESCRIPTOR_CONDITION_DURATION" },
        //        { " <color=yellow>Stackable.</color>", "SKILL_TOOLTIP_RANK_DESCRIPTOR_CONDITION_IS_STACKABLE" },
        //        { " <color=yellow>Refreshes when re-applied.</color>", "SKILL_TOOLTIP_RANK_DESCRIPTOR_CONDITION_IS_REFRESHABLE" },
        //    });
        //}

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
                    var condition = __instance._scriptSkill._skillRankParams._selfConditionOutput;
                    var key = KeyUtil.GetForAsset(condition);
                    return Localyssation.GetString($"{key}_DESCRIPTION", oldString);
                }));
            return matcher.InstructionEnumeration();
            //return instructions; //TODO

        }

    }
}
