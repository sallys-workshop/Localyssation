using HarmonyLib;
using System.Reflection;
using System.Reflection.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace Localyssation.Patches.ReplaceText
{
    internal static partial class RTReplacer
    {
        // dialog
        [HarmonyPatch(typeof(DialogManager), nameof(DialogManager.Start_Dialog))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> DialogManager_Start_Dialog_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions)
                .MatchForward(true,
                    new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Dialog), nameof(Dialog._dialogInput))))
                .MatchBack(true,
                    new CodeMatch(x => x.IsLdloc()));

            var dialog_pos = RTUtil.GetIntOperand(matcher);

            matcher.MatchForward(true,
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(DialogManager), nameof(DialogManager._dialogSentences))))
            .MatchForward(true,
                new CodeMatch(x => (x.opcode == OpCodes.Call || x.opcode == OpCodes.Callvirt) && ((MethodInfo)x.operand).Name == "Enqueue"));

            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldloc, dialog_pos),
                Transpilers.EmitDelegate<Func<string, DialogManager, DialogBranch, Dialog, string>>((oldString, __instance, dialogBranch, dialog) =>
                {
                    if (__instance._scriptableDialog)
                    {
                        var key = KeyUtil.GetForAsset(__instance._scriptableDialog);

                        string GetInputKey(string keySuffixBranch)
                        {
                            var dialogIndex = Array.IndexOf(dialogBranch.dialogs, dialog);
                            var inputKey = $"{key}_{keySuffixBranch}_DIALOG_{dialogIndex}_INPUT";

                            if (dialog._altInputs != null && dialog._altInputs.Length != 0)
                                inputKey += $"_ALT_{UnityEngine.Random.Range(0, dialog._altInputs.Length)}";

                            return inputKey;
                        }

                        var dialogTrigger = __instance._currentDialogTrigger;
                        if (dialogTrigger && dialogTrigger._useLocalDialogBranch)
                        {
                            return Localyssation.GetString(
                                GetInputKey(
                                    KeyUtil.Normalize(
                                        $"LOCAL_BRANCH_{dialogTrigger.gameObject.scene.name}_{Util.GetChildTransformPath(dialogTrigger.transform, 2)}"
                                        )
                                    ), oldString);
                        }
                        else
                        {
                            var branchTypes = new Dictionary<DialogBranch[], string>()
                            {
                                { __instance._scriptableDialog._dialogBranches, "BRANCH" },
                                { __instance._scriptableDialog._introductionBranches, "INTRODUCTION_BRANCH" }
                            };
                            foreach (var branchType in branchTypes)
                            {
                                var branchArray = branchType.Key;
                                var branchTypeName = branchType.Value;
                                if (branchArray.Contains(dialogBranch))
                                {
                                    var branchIndex = Array.IndexOf(branchArray, dialogBranch);
                                    return Localyssation.GetString(GetInputKey($"{branchTypeName}_{branchIndex}"), oldString);
                                }
                            }
                        }
                    }
                    return oldString;
                }));

            return matcher.InstructionEnumeration();
        }

        [HarmonyPatch(typeof(DialogManager), nameof(DialogManager.Display_NextSentence))]
        [HarmonyPostfix]
        public static void DialogManager_Display_NextSentence(DialogManager __instance)
        {
            if (!__instance._scriptableDialog) return;

            var key = KeyUtil.GetForAsset(__instance._scriptableDialog);

            if (__instance._characterNameText.IsActive())
                __instance._characterNameText.text = Localyssation.GetString($"{key}_NAME_TAG", __instance._characterNameText.text, __instance._characterNameText.fontSize);
        }

        [HarmonyPatch(typeof(DialogManager), nameof(DialogManager.Create_DialogSelectionButton))]
        [HarmonyPostfix]
        public static void DialogManager_Create_DialogSelectionButton(DialogManager __instance, DialogSelection _dialogSelection)
        {
            if (__instance._selectionButtons.Count <= 0 || !__instance._scriptableDialog) return;

            var key = KeyUtil.GetForAsset(__instance._scriptableDialog);

            var newButton = __instance._selectionButtons[__instance._selectionButtons.Count - 1];
            var buttonText = newButton.GetComponentInChildren<Text>();
            if (buttonText)
            {
                var branchTypes = new Dictionary<DialogBranch[], string>()
                {
                    { __instance._scriptableDialog._dialogBranches, "BRANCH" },
                    { __instance._scriptableDialog._introductionBranches, "INTRODUCTION_BRANCH" }
                };
                foreach (var branchType in branchTypes)
                {
                    var branchArray = branchType.Key;
                    var branchTypeName = branchType.Value;
                    for (var branchIndex = 0; branchIndex < branchArray.Length; branchIndex++)
                    {
                        var branch = branchArray[branchIndex];
                        for (var dialogIndex = 0; dialogIndex < branch.dialogs.Length; dialogIndex++)
                        {
                            var dialog = branch.dialogs[dialogIndex];
                            for (var selectionIndex = 0; selectionIndex < dialog._dialogSelections.Length; selectionIndex++)
                            {
                                var selection = dialog._dialogSelections[selectionIndex];
                                if (selection == _dialogSelection)
                                {
                                    buttonText.text = Localyssation.GetString($"{key}_{branchTypeName}_{branchIndex}_DIALOG_{dialogIndex}_SELECTION_{selectionIndex}", buttonText.text, buttonText.fontSize);
                                }
                            }
                        }
                    }
                }
            }
        }

        internal static Dictionary<string, string> dialogManagerQuickSentencesHack = new Dictionary<string, string>();
        [HarmonyPatch(typeof(DialogManager), nameof(DialogManager.Start_QuickSentence))]
        [HarmonyPostfix]
        public static void DialogManager_Start_QuickSentence(DialogManager __instance, ref string _sentence)
        {
            // most likely shouldn't do this, because two different dialogue entries might share the same string
            // let's hope that this never actually happens

            if (dialogManagerQuickSentencesHack.TryGetValue(_sentence, out var key))
            {
                _sentence = Localyssation.GetString(key, _sentence);
            }
        }
    }
}
