using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

namespace Localyssation.Patches.ReplaceText
{
    internal static class RTQuest
    {
        [HarmonyPatch(typeof(QuestMenuCell), nameof(QuestMenuCell.Cell_OnAwake))]
        [HarmonyPostfix]
        public static void QuestMenu_Cell_OnAwake_Postfix(QuestMenuCell __instance)
        {
            RTUtil.RemapChildTextsByPath(__instance.transform, new Dictionary<string, string>() {
                { "_text_questsHeader", I18nKeys.TabMenu.CELL_QUESTS_HEADER }
            });
        }
        // quests
        [HarmonyPatch(typeof(QuestListDataEntry), nameof(QuestListDataEntry.Update))]
        [HarmonyPostfix]
        public static void QuestListDataEntry_Update(QuestListDataEntry __instance)
        {
            var key = KeyUtil.GetForAsset(__instance._scriptableQuest);

            var formattedQuestString = Localyssation.GetString($"{key}_NAME", __instance._scriptableQuest._questName, __instance._dataNameText.fontSize);

            if (__instance._scriptableQuest._questSubType == QuestSubType.NONE ||
                __instance._scriptableQuest._questSubType == QuestSubType.MAIN_QUEST)
            {
                formattedQuestString += " " + string.Format(
                    Localyssation.GetString("FORMAT_QUEST_REQUIRED_LEVEL", fontSize: __instance._dataNameText.fontSize),
                    __instance._scriptableQuest._questLevel);

                //var styleTag = __instance._dataNameText.text.Substring(0, __instance._dataNameText.text.IndexOf(">") + 1);
                Match styleTag = Regex.Match(__instance._dataNameText.text, @"<(\w*)=([^>]*)>");
                if (styleTag.Success)
                {
                    __instance._dataNameText.text = $"<{styleTag.Groups[1]}={styleTag.Groups[2]}>" + formattedQuestString + $"</{styleTag.Groups[1]}>";
                }
                else
                {
                    __instance._dataNameText.text = formattedQuestString;
                }
            }
            else
            {
                formattedQuestString += " " + Localyssation.GetString(
                    $"QUEST_TYPE_{__instance._scriptableQuest._questSubType.ToString().ToUpper()}",
                    fontSize: __instance._dataNameText.fontSize);

                __instance._dataNameText.text = $"<color=#f7e98e>{formattedQuestString}</color>";
            }
        }

        [HarmonyPatch(typeof(QuestMenuCell), nameof(QuestMenuCell.Handle_CellUpdate))]
        [HarmonyPostfix]
        public static void QuestMenuCell_Handle_CellUpdate(QuestMenuCell __instance)
        {
            if (!Player._mainPlayer) return;

            PlayerQuesting _pQuest = Player._mainPlayer._pQuest;

            __instance._questLogCounterText.text = string.Format(
                Localyssation.GetString("FORMAT_QUEST_MENU_CELL_QUEST_LOG_COUNTER", __instance._questLogCounterText.text, __instance._questLogCounterText.fontSize),
                _pQuest._questProgressData.Count,
                _pQuest._questLogLimit);

            var finishedQuestCount = 0;
            if (ProfileDataManager._current._characterFile._questProgressProfile._finishedQuests != null)
                finishedQuestCount = ProfileDataManager._current._characterFile._questProgressProfile._finishedQuests.Length;
            __instance._finishedQuestCounterText.text = string.Format(
                Localyssation.GetString("FORMAT_QUEST_MENU_CELL_FINISHED_QUEST_COUNTER", __instance._finishedQuestCounterText.text, __instance._finishedQuestCounterText.fontSize),
                finishedQuestCount);

            var errandsStr = "";
            if (_pQuest._questProgressData.Count > 0 && __instance._selectedQuest)
            {
                var acceptedQuestIndex = 0;
                while (acceptedQuestIndex < _pQuest._questProgressData.Count && !QuestTrackerManager._current._refreshingElements)
                {
                    var questProgress = _pQuest._questProgressData[acceptedQuestIndex];
                    if (questProgress._questTag == __instance._selectedQuest._questName)
                    {
                        if (questProgress._questComplete)
                        {
                            var key = KeyUtil.GetForAsset(__instance._selectedQuest);
                            var local = Localyssation.GetString($"{key}_COMPLETE_RETURN_MESSAGE", __instance._selectedQuest._questCompleteReturnMessage, __instance._questErrandsText.fontSize);
                            errandsStr = errandsStr.Insert(0, $"<color=yellow>{local}</color>\n\n");
                        }
                        errandsStr += QuestTrackerManager._current._questTrackElements[acceptedQuestIndex]._trackElementText.text;
                    }
                    acceptedQuestIndex++;
                }
            }
            __instance._questErrandsText.text = errandsStr;
        }

        [HarmonyPatch(typeof(QuestMenuCell), nameof(QuestMenuCell.Apply_QuestInfo))]
        [HarmonyPostfix]
        public static void QuestMenuCell_Select_QuestSlot(QuestMenuCell __instance, ScriptableQuest _scriptQuest)
        {
            var key = KeyUtil.GetForAsset(_scriptQuest);

            __instance._questHeaderText.text = Localyssation.GetString($"{key}_NAME", __instance._questHeaderText.text, __instance._questHeaderText.fontSize)
                + " " + string.Format(
                    Localyssation.GetString("FORMAT_QUEST_REQUIRED_LEVEL", fontSize: __instance._questHeaderText.fontSize),
                    _scriptQuest._questLevel);

            __instance._questSummaryText.text = Localyssation.GetString($"{key}_DESCRIPTION", __instance._questSummaryText.text, __instance._questSummaryText.fontSize);

            int expReward = (int)((int)GameManager._current._statLogics._experienceCurve.Evaluate(_scriptQuest._questLevel) * _scriptQuest._questExperiencePercentage);
            if (expReward > 0)
            {
                __instance._rewardsPanelText_experience.text = string.Format(
                    Localyssation.GetString("FORMAT_QUEST_MENU_CELL_REWARD_EXP", __instance._rewardsPanelText_experience.text, __instance._rewardsPanelText_experience.fontSize),
                    expReward);
            }
            if (_scriptQuest._questCurrencyReward > 0)
            {
                __instance._rewardsPanelText_currency.text = string.Format(
                    Localyssation.GetString("FORMAT_QUEST_MENU_CELL_REWARD_CURRENCY", __instance._rewardsPanelText_currency.text, __instance._rewardsPanelText_currency.fontSize),
                    expReward);
            }
        }

        [HarmonyPatch(typeof(QuestMenuCell), nameof(QuestMenuCell.Clear_DisplayQuestData))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> QuestMenuCell_Clear_DisplayQuestData_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.SimpleStringReplaceTranspiler(instructions, new Dictionary<string, string>() {
                { "No Quests in Quest Log.", "QUEST_MENU_SUMMARY_NO_QUESTS" },
                { "Select a Quest.", "QUEST_MENU_HEADER_UNSELECTED" },
            });
        }

        [HarmonyPatch(typeof(QuestMenuCellSlot), nameof(QuestMenuCellSlot.Update))]
        [HarmonyPostfix]
        public static void QuestMenuCellSlot_Update(QuestMenuCellSlot __instance)
        {
            if (__instance._scriptQuest)
            {
                var fontSize = __instance._slotTag.fontSize;
                var questName = Localyssation.GetString($"{KeyUtil.GetForAsset(__instance._scriptQuest)}_NAME", __instance._scriptQuest._questName, fontSize);
                var levelRequirementStr = string.Format(
                    Localyssation.GetString("FORMAT_QUEST_REQUIRED_LEVEL", fontSize: fontSize),
                    __instance._scriptQuest._questLevel);

                __instance._slotTag.text = $"{questName}\n{levelRequirementStr}";
                switch (__instance._scriptQuest._questSubType)
                {
                    case QuestSubType.MAIN_QUEST:
                        __instance._slotTag.text = $"<color=cyan>{questName}</color>\n<color=cyan>{levelRequirementStr}</color>";
                        break;
                    case QuestSubType.CLASS:
                        __instance._slotTag.text = $"<color=#f7e98e>{questName}</color>\n<color=#f7e98e>{Localyssation.GetString("QUEST_TYPE_CLASS", null, fontSize)}</color>";
                        break;
                    // No more QuestSubType.MASTERY
                    //case QuestSubType.MASTERY:
                    //    __instance._slotTag.text = $"<color=#f7e98e>{questName}</color>\n<color=#f7e98e>{Localyssation.GetString("QUEST_TYPE_MASTERY", null, fontSize)}</color>";
                    //    break;
                }
            }
            else
            {
                __instance._slotTag.text = Localyssation.GetString("QUEST_MENU_CELL_SLOT_EMPTY", __instance._slotTag.text, __instance._slotTag.fontSize);
            }
        }

        [HarmonyPatch(typeof(QuestSelectionManager), nameof(QuestSelectionManager.Update))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> QuestSelectionManager_Handle_Expbar_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.SimpleStringReplaceTranspiler(instructions, new Dictionary<string, string>() {
                { "MAX", "EXP_COUNTER_MAX" },
            });
        }

        [HarmonyPatch(typeof(QuestSelectionManager), nameof(QuestSelectionManager.Handle_QuestSelectionConditions))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> QuestSelectionManager_Handle_QuestSelectionConditions_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.SimpleStringReplaceTranspiler(instructions, new Dictionary<string, string>() {
                { "Quest Incomplete", "QUEST_SELECTION_MANAGER_QUEST_ACCEPT_BUTTON_INCOMPLETE" },
                { "Complete Quest", "QUEST_SELECTION_MANAGER_QUEST_ACCEPT_BUTTON_TURN_IN" },
                { "Select a Quest", "QUEST_SELECTION_MANAGER_QUEST_ACCEPT_BUTTON_UNSELECTED" },
            });
        }

        [HarmonyPatch(typeof(QuestSelectionManager), nameof(QuestSelectionManager.Select_QuestEntry))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> QuestSelectionManager_Select_QuestEntry_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.SimpleStringReplaceTranspiler(instructions, new Dictionary<string, string>() {
                { "Accept Quest", "QUEST_SELECTION_MANAGER_QUEST_ACCEPT_BUTTON_ACCEPT" },
                { "Quest Locked", "QUEST_SELECTION_MANAGER_QUEST_ACCEPT_BUTTON_LOCKED" },
            });
        }

        internal static string GetCreepKillRequirementText(ScriptableCreep creep, int requirement, int fontSize = -1)
        {
            var formatKey = "FORMAT_QUEST_PROGRESS_CREEPS_KILLED";
            var creepKey = $"{KeyUtil.GetForAsset(creep)}_NAME";
            if (requirement > 1)
            {
                if (Localyssation.currentLanguage.strings.ContainsKey($"{creepKey}_VARIANT_{requirement}"))
                    creepKey = $"{creepKey}_VARIANT_{requirement}";
                else if (Localyssation.currentLanguage.strings.ContainsKey($"{creepKey}_VARIANT_MANY"))
                    creepKey = $"{creepKey}_VARIANT_MANY";

                if (Localyssation.currentLanguage.strings.ContainsKey($"{formatKey}_VARIANT_{requirement}"))
                    formatKey = $"{formatKey}_VARIANT_{requirement}";
                else if (Localyssation.currentLanguage.strings.ContainsKey($"{formatKey}_VARIANT_MANY"))
                    formatKey = $"{formatKey}_VARIANT_MANY";
            }
            if (Localyssation.currentLanguage.strings.ContainsKey($"{creepKey}_VARIANT_QUEST_KILLED"))
                creepKey = $"{creepKey}_VARIANT_QUEST_KILLED";

            return string.Format(
                Localyssation.GetString(formatKey, fontSize: fontSize),
                Localyssation.GetString(creepKey, fontSize: fontSize));
        }
        [HarmonyPatch(typeof(QuestTrackElement), nameof(QuestTrackElement.Update_QuestTrackElement))]
        [HarmonyPostfix]
        public static void QuestTrackElement_Handle_QuestTrackInfo(QuestTrackElement __instance)
        {
            var key = KeyUtil.GetForAsset(__instance._scriptQuest);
            if (!string.IsNullOrEmpty(__instance._scriptQuest._questName))
                __instance._trackQuestNameText.text = __instance._trackQuestNameText.text.Replace(__instance._scriptQuest._questName, Localyssation.GetString($"{key}_NAME", __instance._scriptQuest._questName, __instance._trackQuestNameText.fontSize));

            var playerQuesting = Player._mainPlayer.GetComponent<PlayerQuesting>();
            if (playerQuesting._questProgressData.Count > 0)
            {
                var questProgressData = playerQuesting._questProgressData[__instance._questIndex];

                var trackElementText = __instance._trackElementText.text.Split(new string[] { "\n" }, StringSplitOptions.None);
                var c = 0;
                var fontSize = __instance._trackElementText.fontSize;
                void ReplaceTrackElementText(string newText, int progressCurrent, int progressMax)
                {
                    var styleTag = trackElementText[c].Substring(0, trackElementText[c].IndexOf(">") + 1);
                    var formattedQuestString = string.Format(
                        Localyssation.GetString("FORMAT_QUEST_PROGRESS", fontSize: fontSize),
                        newText, progressCurrent, progressMax);
                    trackElementText[c] = styleTag + formattedQuestString + "</color>";
                    c++;
                }

                for (var i = 0; i < __instance._scriptQuest._questObjective._questItemRequirements.Length; i++)
                {
                    var questItemRequirement = __instance._scriptQuest._questObjective._questItemRequirements[i];
                    var itemKey = $"{KeyUtil.GetForAsset(questItemRequirement._questItem)}_NAME";
                    ReplaceTrackElementText(Localyssation.GetString(itemKey, questItemRequirement._questItem._itemName, fontSize), questProgressData._itemProgressValues[i], questItemRequirement._itemsNeeded);
                }
                for (var i = 0; i < __instance._scriptQuest._questObjective._questCreepRequirements.Length; i++)
                {
                    var questCreepRequirement = __instance._scriptQuest._questObjective._questCreepRequirements[i];
                    ReplaceTrackElementText(GetCreepKillRequirementText(questCreepRequirement._questCreep, questCreepRequirement._creepsKilled, fontSize), questProgressData._creepKillProgressValues[i], questCreepRequirement._creepsKilled);
                }
                for (var i = 0; i < __instance._scriptQuest._questObjective._questTriggerRequirements.Length; i++)
                {
                    var questTriggerRequirement = __instance._scriptQuest._questObjective._questTriggerRequirements[i];
                    var triggerKey = KeyUtil.GetForAsset(questTriggerRequirement);
                    ReplaceTrackElementText($"{Localyssation.GetString(triggerKey + "_PREFIX", questTriggerRequirement._prefix, fontSize)} {Localyssation.GetString(triggerKey + "_SUFFIX", questTriggerRequirement._suffix, fontSize)}", questProgressData._triggerProgressValues[i], questTriggerRequirement._triggerEmitsNeeded);
                }

                __instance._trackElementText.text = string.Join("\n", trackElementText);
            }
        }

    }


    [HarmonyPatch]
    class PlayerQuestingPatch_Apply_QuestItemProgress
    {
        [HarmonyTargetMethod]
        public static MethodBase TargetMethod()
        {
            return AccessTools.GetDeclaredMethods(typeof(PlayerQuesting))
                .Where(methodInfo => methodInfo.Name.Contains($"<{nameof(PlayerQuesting.Apply_QuestItemProgress)}>g__"))
                .Cast<MethodBase>()
                .FirstOrDefault();
        }

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions)
                .MatchForward(true,
                    new CodeMatch(OpCodes.Newarr),
                    new CodeMatch(x => x.IsStloc()));

            var acquiredItemsArray_pos = RTUtil.GetIntOperand(matcher);

            matcher.MatchForward(true,
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(ScriptableQuest), nameof(ScriptableQuest._questObjective))),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(QuestObjective), nameof(QuestObjective._questItemRequirements))),
                new CodeMatch(),
                new CodeMatch(),
                new CodeMatch(x => x.IsStloc()));

            var questItemRequirement_pos = RTUtil.GetIntOperand(matcher);

            matcher.MatchForward(true,
                new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(PlayerQuesting), nameof(PlayerQuesting.Apply_QuestProgressNote))));

            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Ldloc, questItemRequirement_pos),
                new CodeInstruction(OpCodes.Ldloc, acquiredItemsArray_pos),
                Transpilers.EmitDelegate<Func<string, PlayerQuesting, ScriptableQuest, int, QuestItemRequirement, int[], string>>((oldString, __instance, quest, questIndex, questItemRequirement, acquiredItemsArray) =>
                {
                    var questItemRequirementIndex = Array.IndexOf(quest._questObjective._questItemRequirements, questItemRequirement);
                    return string.Format(
                        Localyssation.GetString(
                            //"FORMAT_QUEST_PROGRESS",
                            I18nKeys.Quest.FORMAT_PROGRESS,
                            Localyssation.GetString($"{KeyUtil.GetForAsset(questItemRequirement._questItem)}_NAME")),
                        Localyssation.GetString(KeyUtil.GetForAsset(questItemRequirement._questItem) + "_NAME"),
                        acquiredItemsArray[questItemRequirementIndex],
                        questItemRequirement._itemsNeeded);
                }));

            return matcher.InstructionEnumeration();
        }
    }

    [HarmonyPatch]
    class PlayerQuestingPatch_Apply_QuestTriggerProgress
    {
        [HarmonyTargetMethod]
        public static MethodBase TargetMethod()
        {
            return AccessTools.GetDeclaredMethods(typeof(PlayerQuesting))
                .Where(methodInfo => methodInfo.Name.Contains($"<{nameof(PlayerQuesting.Apply_QuestTriggerProgress)}>g__"))
                .Cast<MethodBase>()
                .FirstOrDefault();
        }

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions)
                .MatchForward(true,
                    new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(ScriptableQuest), nameof(ScriptableQuest._questObjective))),
                    new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(QuestObjective), nameof(QuestObjective._questTriggerRequirements))),
                    new CodeMatch(),
                    new CodeMatch(),
                    new CodeMatch(x => x.IsStloc()));

            var questTriggerRequirement_pos = RTUtil.GetIntOperand(matcher);

            matcher.MatchForward(true,
                new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(PlayerQuesting), nameof(PlayerQuesting.Apply_QuestProgressNote))));

            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Ldloc, questTriggerRequirement_pos),
                Transpilers.EmitDelegate<Func<string, PlayerQuesting, ScriptableQuest, int, QuestTriggerRequirement, string>>((oldString, __instance, quest, questIndex, questTriggerRequirement) =>
                {
                    var questTriggerRequirementIndex = Array.IndexOf(quest._questObjective._questTriggerRequirements, questTriggerRequirement);
                    return string.Format(
                        Localyssation.GetString(
                            //"FORMAT_QUEST_PROGRESS",
                            I18nKeys.Quest.FORMAT_PROGRESS,
                            $"{questTriggerRequirement._prefix} {questTriggerRequirement._suffix}"),
                        $"{questTriggerRequirement._prefix} {questTriggerRequirement._suffix}",
                        __instance._questProgressData[questIndex]._triggerProgressValues[questTriggerRequirementIndex],
                        questTriggerRequirement._triggerEmitsNeeded);
                }));

            return matcher.InstructionEnumeration();
        }
    }

    [HarmonyPatch]
    class PlayerQuestingPatch_Target_Query_CreepKillProgress
    {
        [HarmonyTargetMethod]
        public static MethodBase TargetMethod()
        {
            return AccessTools.GetDeclaredMethods(typeof(PlayerQuesting))
                .Where(methodInfo => methodInfo.Name.Contains($"<{nameof(PlayerQuesting.Target_Query_CreepKillProgress)}>g__"))
                .Cast<MethodBase>()
                .FirstOrDefault();
        }

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions)
                .MatchForward(true,
                    new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(ScriptableQuest), nameof(ScriptableQuest._questObjective))),
                    new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(QuestObjective), nameof(QuestObjective._questCreepRequirements))),
                    new CodeMatch(),
                    new CodeMatch(),
                    new CodeMatch(x => x.IsStloc()));

            var questCreepRequirement_pos = RTUtil.GetIntOperand(matcher);

            matcher.MatchForward(true,
                new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(PlayerQuesting), nameof(PlayerQuesting.Apply_QuestProgressNote))));

            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Ldloc, questCreepRequirement_pos),
                Transpilers.EmitDelegate<Func<string, PlayerQuesting, ScriptableQuest, int, QuestCreepRequirement, string>>((oldString, __instance, quest, questIndex, questCreepRequirement) =>
                {
                    var questCreepRequirementIndex = Array.IndexOf(quest._questObjective._questCreepRequirements, questCreepRequirement);
                    return string.Format(
                        Localyssation.GetString(
                            //"FORMAT_QUEST_PROGRESS",
                            I18nKeys.Quest.FORMAT_PROGRESS,
                            RTQuest.GetCreepKillRequirementText(questCreepRequirement._questCreep, questCreepRequirement._creepsKilled)
                        ),
                        Localyssation.GetString(KeyUtil.GetForAsset(questCreepRequirement._questCreep) + "_NAME"),
                        Math.Min(
                            __instance._questProgressData[questIndex]._creepKillProgressValues[questCreepRequirementIndex] + 1, 
                            questCreepRequirement._creepsKilled
                        ),
                        questCreepRequirement._creepsKilled);
                }));

            return matcher.InstructionEnumeration();
        }
    }
}
