using HarmonyLib;
using Localyssation.LanguageModule;
using Localyssation.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using YamlDotNet.Core.Tokens;

namespace Localyssation.Patches.ReplaceText
{
    internal static partial class RTReplacer
    {
        [HarmonyPatch(typeof(QuestMenuCell), nameof(QuestMenuCell.Cell_OnAwake))]
        [HarmonyPostfix]
        public static void QuestMenu_Cell_OnAwake_Postfix(QuestMenuCell __instance)
        {
            RTUtil.RemapChildTextsByPath(__instance.transform, new Dictionary<string, string>() {
                { "_text_questsHeader", I18nKeys.TabMenu.CELL_QUESTS_HEADER },
                { "_questListPanel/_dolly_questCellList/_abandonQuestPanel/_button_abandonQuest/_buttonText_abandonQuest", I18nKeys.TabMenu.CELL_QUESTS_BUTTON_ABANDON }
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
                    KeyUtil.GetForAsset(__instance._scriptableQuest._questSubType),
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

            var rewardHeaderText = FindGameObjectTextChild(__instance._rewardsPanelObject, "_text_questRewardHeader"); 
            rewardHeaderText.text = Localyssation.GetString("QUEST_MENU_CELL_REWARD_HEADER", rewardHeaderText.text, rewardHeaderText.fontSize);

            var objectiveItemText = FindGameObjectTextChild(__instance._objectiveItemPanel, "_text_objectiveItemHeader");
            objectiveItemText.text = Localyssation.GetString("QUEST_MENU_CELL_OBJECTIVE_ITEM_HEADER", objectiveItemText.text, objectiveItemText.fontSize);
        }
        
        private static Text FindGameObjectTextChild(GameObject obj, String componentName)
        { 
            Transform t = obj.transform.Find(componentName);
            if (t != null)
            {
                return t.GetComponent<Text>();
            }
            else
            {
                Debug.LogError("找不到对象: " + componentName);
                return null;
            }
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


        [HarmonyPatch(typeof(QuestSelectionManager), nameof(QuestSelectionManager.Handle_QuestSelector))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> QuestSelectionManager_Handle_QuestSelector_Transpiler(IEnumerable<CodeInstruction> instructions)
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
                if (LanguageManager.CurrentLanguage.ContainsKey($"{creepKey}_VARIANT_{requirement}"))
                    creepKey = $"{creepKey}_VARIANT_{requirement}";
                else if (LanguageManager.CurrentLanguage.ContainsKey($"{creepKey}_PLURAL"))
                    creepKey = $"{creepKey}_PLURAL";

                if (LanguageManager.CurrentLanguage.ContainsKey($"{formatKey}_VARIANT_{requirement}"))
                    formatKey = $"{formatKey}_VARIANT_{requirement}";
                else if (LanguageManager.CurrentLanguage.ContainsKey($"{formatKey}_PLURAL"))
                    formatKey = $"{formatKey}_PLURAL";
            }
            if (LanguageManager.CurrentLanguage.ContainsKey($"{creepKey}_VARIANT_QUEST_KILLED"))
                creepKey = $"{creepKey}_VARIANT_QUEST_KILLED";

            var defaultCreepName = requirement > 1 ? creep._creepName + "s" : creep._creepName;
            return string.Format(
                Localyssation.GetString(formatKey, fontSize: fontSize),
                Localyssation.GetString(creepKey, defaultCreepName, fontSize));
        }

        internal static string GetQuestTriggerRequirementKey(QuestTriggerRequirement requirement)
        {
            return $"QUEST_TRIGGER_REQUIREMENT_{KeyUtil.Normalize(requirement._prefix)}_{KeyUtil.Normalize(requirement._suffix)}";
        }

        internal static string GetQuestTriggerRequirementText(QuestTriggerRequirement requirement, int fontSize = -1)
        {
            return Localyssation.GetString(
                GetQuestTriggerRequirementKey(requirement),
                $"{requirement._prefix} {requirement._suffix}",
                fontSize);
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
                    ReplaceTrackElementText(GetQuestTriggerRequirementText(questTriggerRequirement, fontSize), questProgressData._triggerProgressValues[i], questTriggerRequirement._triggerEmitsNeeded);
                }

                __instance._trackElementText.text = string.Join("\n", trackElementText);
            }
        }

        [HarmonyPatch(typeof(QuestSelectionManager), nameof(QuestSelectionManager.OnClick_QuestAcceptButton))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> QuestSelectionManager__OnClick_QuestAcceptButton__Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.Wrap(instructions)
                .ReplaceStrings(new Dictionary<string, string>() {
                    { "Quest Log Full", I18nKeys.ErrorMessages.QUEST_LOG_FULL }
                })
                .Unwrap();
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
            var formatMethod = AccessTools.Method(typeof(PlayerQuesting), nameof(PlayerQuesting.Apply_QuestProgressNote), new[] { typeof(string), typeof(int) });
            if (formatMethod == null) throw new InvalidOperationException();

            var matcher = new CodeMatcher(instructions)
                .MatchForward(true,
                    new CodeMatch(OpCodes.Newarr),
                    new CodeMatch(x => x.IsStloc()));
            if (matcher.IsInvalid) throw new InvalidOperationException();
            var acquiredItemsArray_pos = RTUtil.GetIntOperand(matcher);

            matcher.MatchForward(true,
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(ScriptableQuest), nameof(ScriptableQuest._questObjective))),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(QuestObjective), nameof(QuestObjective._questItemRequirements))),
                new CodeMatch(),
                new CodeMatch(),
                new CodeMatch(x => x.IsStloc()));
            if (matcher.IsInvalid) throw new InvalidOperationException();
            var questItemRequirement_pos = RTUtil.GetIntOperand(matcher);

            matcher.MatchForward(true, new CodeMatch(OpCodes.Ldstr, "{0}: ({1} / {2})"));
            if (matcher.IsInvalid) throw new InvalidOperationException();
            int startPos = matcher.Pos;
            if (startPos > 0 && matcher.InstructionAt(-1).opcode == OpCodes.Ldarg_0) startPos--;

            matcher.MatchForward(true, new CodeMatch(OpCodes.Call, formatMethod));
            if (matcher.IsInvalid) throw new InvalidOperationException();
            int endPos = matcher.Pos;
            int count = endPos - startPos + 1;

            matcher.Advance(startPos - matcher.Pos);
            matcher.RemoveInstructions(count);

            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldloc, questItemRequirement_pos),
                new CodeInstruction(OpCodes.Ldloc, acquiredItemsArray_pos),
                Transpilers.EmitDelegate<Func<ScriptableQuest, QuestItemRequirement, int[], string>>((quest, questItemRequirement, acquiredItemsArray) =>
                {
                    var questItemRequirementIndex = Array.IndexOf(quest._questObjective._questItemRequirements, questItemRequirement);
                    return string.Format(
                        Localyssation.GetString(
                            //"FORMAT_QUEST_PROGRESS",
                            I18nKeys.Quest.FORMAT_PROGRESS,
                            questItemRequirement._questItem._itemName),
                        Localyssation.GetString(
                            KeyUtil.GetForAsset(questItemRequirement._questItem) + "_NAME",
                            questItemRequirement._questItem._itemName),
                        acquiredItemsArray[questItemRequirementIndex],
                        questItemRequirement._itemsNeeded);
                }));
            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Call, formatMethod));

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
            var formatMethod = AccessTools.Method(typeof(PlayerQuesting), nameof(PlayerQuesting.Apply_QuestProgressNote), new[] { typeof(string), typeof(int) });
            if (formatMethod == null) throw new InvalidOperationException();

            var matcher = new CodeMatcher(instructions)
                .MatchForward(true,
                    new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(ScriptableQuest), nameof(ScriptableQuest._questObjective))),
                    new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(QuestObjective), nameof(QuestObjective._questTriggerRequirements))),
                    new CodeMatch(),
                    new CodeMatch(),
                    new CodeMatch(x => x.IsStloc()));
            if (matcher.IsInvalid) throw new InvalidOperationException();
            var questTriggerRequirement_pos = RTUtil.GetIntOperand(matcher);

            matcher.MatchForward(true,
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(QuestProgressStruct), nameof(QuestProgressStruct._triggerProgressValues))),
                new CodeMatch(x => x.IsStloc()));
            if (matcher.IsInvalid) throw new InvalidOperationException();
            var triggerProgressValues_pos = RTUtil.GetIntOperand(matcher);

            matcher.MatchForward(true, new CodeMatch(OpCodes.Ldstr, "{0} {1}: ({2} / {3})"));
            if (matcher.IsInvalid) throw new InvalidOperationException();
            int startPos = matcher.Pos;
            if (startPos > 0 && matcher.InstructionAt(-1).opcode == OpCodes.Ldarg_0) startPos--;

            matcher.MatchForward(true, new CodeMatch(OpCodes.Call, formatMethod));
            if (matcher.IsInvalid) throw new InvalidOperationException();
            int endPos = matcher.Pos;
            int count = endPos - startPos + 1;

            matcher.Advance(startPos - matcher.Pos);
            matcher.RemoveInstructions(count);

            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldloc, questTriggerRequirement_pos),
                new CodeInstruction(OpCodes.Ldloc, triggerProgressValues_pos),
                Transpilers.EmitDelegate<Func<ScriptableQuest, QuestTriggerRequirement, int[], string>>((quest, questTriggerRequirement, triggerProgressValues) =>
                {
                    var questTriggerRequirementIndex = Array.IndexOf(quest._questObjective._questTriggerRequirements, questTriggerRequirement);
                    return string.Format(
                        Localyssation.GetString(
                            I18nKeys.Quest.FORMAT_PROGRESS,
                            $"{questTriggerRequirement._prefix} {questTriggerRequirement._suffix}"),
                        RTReplacer.GetQuestTriggerRequirementText(questTriggerRequirement),
                        triggerProgressValues[questTriggerRequirementIndex],
                        questTriggerRequirement._triggerEmitsNeeded);
                }));
            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Call, formatMethod));

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
            var formatMethod = AccessTools.Method(typeof(PlayerQuesting), nameof(PlayerQuesting.Apply_QuestProgressNote), new[] { typeof(string), typeof(int) });
            if (formatMethod == null) throw new InvalidOperationException();

            var matcher = new CodeMatcher(instructions)
                .MatchForward(true,
                    new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(ScriptableQuest), nameof(ScriptableQuest._questObjective))),
                    new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(QuestObjective), nameof(QuestObjective._questCreepRequirements))),
                    new CodeMatch(),
                    new CodeMatch(),
                    new CodeMatch(x => x.IsStloc()));
            if (matcher.IsInvalid) throw new InvalidOperationException();
            var questCreepRequirement_pos = RTUtil.GetIntOperand(matcher);

            matcher.MatchForward(true, new CodeMatch(OpCodes.Ldstr, "{0} slain: ({1} / {2})"));
            if (matcher.IsInvalid) throw new InvalidOperationException();
            int startPos = matcher.Pos;
            if (startPos > 0 && matcher.InstructionAt(-1).opcode == OpCodes.Ldarg_0) startPos--;

            matcher.MatchForward(true, new CodeMatch(OpCodes.Call, formatMethod));
            if (matcher.IsInvalid) throw new InvalidOperationException();
            int endPos = matcher.Pos;
            int count = endPos - startPos + 1;

            matcher.Advance(startPos - matcher.Pos);
            matcher.RemoveInstructions(count);

            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Ldloc, questCreepRequirement_pos),
                Transpilers.EmitDelegate<Func<ScriptableQuest, int, QuestCreepRequirement, string>>((quest, questIndex, questCreepRequirement) =>
                {
                    var questCreepRequirementIndex = Array.IndexOf(quest._questObjective._questCreepRequirements, questCreepRequirement);
                    int[] creepKillProgressValues = Player._mainPlayer._pQuest._questProgressData[questIndex]._creepKillProgressValues;
                    return string.Format(
                        Localyssation.GetString(
                            //"FORMAT_QUEST_PROGRESS",
                            I18nKeys.Quest.FORMAT_PROGRESS,
                            RTReplacer.GetCreepKillRequirementText(questCreepRequirement._questCreep, questCreepRequirement._creepsKilled)
                        ),
                        Localyssation.GetString(
                            KeyUtil.GetForAsset(questCreepRequirement._questCreep) + "_NAME",
                            questCreepRequirement._questCreep._creepName),
                        Math.Min(
                            creepKillProgressValues[questCreepRequirementIndex] + 1,
                            questCreepRequirement._creepsKilled
                        ),
                        questCreepRequirement._creepsKilled);
                }));
            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Call, formatMethod));

            return matcher.InstructionEnumeration();
        }
    }

    [HarmonyPatch]
    class QuestSelectionManager__Handle_QuestSelector
    {
        private static readonly TargetInnerMethod __TARGET = new TargetInnerMethod()
        {
            Type = typeof(QuestSelectionManager),
            ParentMethodName = nameof(QuestSelectionManager.Handle_QuestSelector),
            InnerMethodName = "Handle_Expbar"
        };

        private static readonly string[] REPLACEMENT = new string[] {
            I18nKeys.Lore.EXP_COUNTER_MAX,
        };

        public static MethodBase TargetMethod() => TranspilerHelper.GenerateTargetMethod(__TARGET);
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) => RTUtil.SimpleStringReplaceTranspiler(instructions, REPLACEMENT);
    }

    [HarmonyPatch]
    class PlayerQuesting__Client_CompleteQuest__Transpiler
    {
        private static readonly TargetInnerMethod __TARGET = new TargetInnerMethod()
        {
            Type = typeof(PlayerQuesting),
            ParentMethodName = nameof(PlayerQuesting.Client_CompleteQuest),
            InnerMethodName = "Finish_Quest"
        };
        public static MethodBase TargetMethod() => TranspilerHelper.GenerateTargetMethod(__TARGET);

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            TranspilerHelper.RemoveMethodCallParamsStackForward(
                matcher,
                MessageCallbacks.Start_QuickSentence, 
                11
            );
            matcher.Insert(new[]
            {
                Transpilers.EmitDelegate<Func<string>>(() =>
                {
                    var key = KeyUtil.GetForAsset(DialogManager._current._scriptableDialog);
                    int index = UnityEngine.Random.Range(0, DialogManager._current._scriptableDialog._questCompleteResponses.Length);
                    return Localyssation.GetString(
                        $"{key}_QUEST_COMPLETE_RESPONSE_{index}",
                        DialogManager._current._scriptableDialog._questCompleteResponses[index]
                    );
                })
            });

            return matcher.Instructions();
        }
    }

    [HarmonyPatch]
    class PlayerQuesting__Accept_Quest__Transpiler
    {
        //private static readonly TargetInnerMethod
        public static MethodBase TargetMethod()
        {
            return typeof(PlayerQuesting).GetMethod(nameof(PlayerQuesting.Accept_Quest), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        }

        

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = RTUtil.Wrap(instructions)
                .ReplaceStrings(new[]
                {
                    I18nKeys.ErrorMessages.QUEST_LOG_FULL,
                    I18nKeys.ErrorMessages.QUEST_ALREADY_IN_LOG
                })
                .Matcher()
                .MatchForward(false, new CodeMatch(OpCodes.Ldstr, "Retrieved Quest Objective Item: "));

            TranspilerHelper
                .RemoveMethodCallParamsStackForward(matcher, MessageCallbacks.New_ChatMessage, 7)
                .InsertAndAdvance(new[] {
                    new CodeInstruction(OpCodes.Ldarg_1),
                    Transpilers.EmitDelegate<Func<ScriptableQuest, string>>( quest =>
                        I18nKeys.Quest.RETRIEVED_QUEST_OBJECTIVE_ITEM_FORMAT
                            .Format(
                                KeyUtil.GetForAsset(quest._questObjectiveItem._scriptItem).Name.Localize(
                                    quest._questObjectiveItem._scriptItem._itemName)
                                )
                    )
                    });

            TranspilerHelper
                .RemoveMethodCallParamsStackForward(matcher, MessageCallbacks.Init_GameLogicMessage, 5)
                .InsertAndAdvance(new[]
                {
                    new CodeInstruction(OpCodes.Ldarg_1),
                    Transpilers.EmitDelegate<Func<ScriptableQuest, string>>( quest =>
                        I18nKeys.Quest.RETRIEVED_QUEST_OBJECTIVE_ITEM_FORMAT
                        .Format(
                            KeyUtil.GetForAsset(quest).Name.Localize(quest._questName)
                            )
                        )
                });
            TranspilerHelper
                .RemoveMethodCallParamsStackForward(matcher, MessageCallbacks.Start_QuickSentence, 11)
                .InsertAndAdvance(new[]
                {
                    Transpilers.EmitDelegate<Func<string>>(() =>
                    {
                        var key = KeyUtil.GetForAsset(DialogManager._current._scriptableDialog);
                        int index = UnityEngine.Random.Range(0, DialogManager._current._scriptableDialog._questCompleteResponses.Length);
                        return Localyssation.GetString(
                            $"{key}_QUEST_ACCEPT_RESPONSE_{index}",
                            DialogManager._current._scriptableDialog._questCompleteResponses[index]
                        );
                    })
                });
            return matcher.InstructionEnumeration();
        }
    }
}
