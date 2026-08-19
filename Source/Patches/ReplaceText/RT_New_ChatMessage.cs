
using HarmonyLib;
using System.Collections.Generic;
using System;
using System.Reflection.Emit;
using Localyssation.Util;
using UnityEngine;

namespace Localyssation.Patches.ReplaceText
{
    internal static partial class RTReplacer
    {
        [HarmonyPatch(typeof(ChatBehaviourAssets), nameof(ChatBehaviourAssets.Update))]
        [HarmonyPostfix]
        private static void ChatBehaviourAssets__Update__Postfix(ChatBehaviourAssets __instance)
        {
            var placeholder = __instance.inputPlaceHolderText;
            if (placeholder != null)
            {
                if (placeholder.text == I18nKeys.ChatBehaviour.INPUT_PLACEHOLDER.DefaultString())
                    placeholder.text = I18nKeys.ChatBehaviour.INPUT_PLACEHOLDER.Localize();
                else if (placeholder.text == I18nKeys.ChatBehaviour.GLOBAL_INPUT_PLACEHOLDER.DefaultString())
                    placeholder.text = I18nKeys.ChatBehaviour.GLOBAL_INPUT_PLACEHOLDER.Localize();
                else if (placeholder.text == I18nKeys.ChatBehaviour.PARTY_INPUT_PLACEHOLDER.DefaultString())
                    placeholder.text = I18nKeys.ChatBehaviour.PARTY_INPUT_PLACEHOLDER.Localize();
                else if (placeholder.text == I18nKeys.ChatBehaviour.ZONE_INPUT_PLACEHOLDER.DefaultString())
                    placeholder.text = I18nKeys.ChatBehaviour.ZONE_INPUT_PLACEHOLDER.Localize();
            }

            var chatText = __instance._chatText;
            if (chatText != null)
            {
                var vanillaWelcome = "<color=#a7fc00>Welcome to ATLYSS (version: " + Application.version + ")</color>";
                if (chatText.text.Contains(vanillaWelcome))
                {
                    chatText.text = chatText.text.Replace(
                        vanillaWelcome,
                        I18nKeys.ChatBehaviour.WELCOME_MESSAGE_FORMAT.Format(Application.version));
                }
            }
        }

        [HarmonyPatch(typeof(PatternInstanceManager), nameof(PatternInstanceManager.On_DungeonKeyChange))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> PatternInstanceManager__On_DungeonKeyChange__Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.Wrap(instructions)
                .ReplaceStrings(new[]
                {
                    I18nKeys.ChatMessage.DUNGEON_KEY_DISSIPATES,
                    I18nKeys.ChatMessage.RECIEVE_DUNGEON_KEY
                })
                .Unwrap();
        }

        [HarmonyPatch(typeof(ChatBehaviour), nameof(ChatBehaviour.OnClick_GlobalChannel))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> ChatBehaviour__OnClick_GlobalChannel__Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.Wrap(instructions)
                .ReplaceStrings(new[] {
                    I18nKeys.ChatBehaviour.DISABLE_GLOBAL_CHANNEL_MESSAGE,
                    I18nKeys.ChatBehaviour.ENABLE_GLOBAL_CHANNEL_MESSAGE
                })
                .Unwrap();
        }

        [HarmonyPatch(typeof(ChatBehaviour), nameof(ChatBehaviour.OnClick_PartyChannel))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> ChatBehaviour__OnClick_PartyChannel__Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.Wrap(instructions)
                .ReplaceStrings(new[] {
                    I18nKeys.ChatBehaviour.DISABLE_PARTY_CHANNEL_MESSAGE,
                    I18nKeys.ChatBehaviour.ENABLE_PARTY_CHANNEL_MESSAGE
                })
                .Unwrap();
        }

        [HarmonyPatch(typeof(ChatBehaviour), nameof(ChatBehaviour.OnClick_ZoneChannel))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> ChatBehaviour__OnClick_ZoneChannel__Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.Wrap(instructions)
                .ReplaceStrings(new[] {
                    I18nKeys.ChatBehaviour.DISABLE_ROOM_CHANNEL_MESSAGE,
                    I18nKeys.ChatBehaviour.ENABLE_ROOM_CHANNEL_MESSAGE
                })
                .Unwrap();
        }

        /// Might conflict with command libs
        [HarmonyPatch(typeof(ChatBehaviour), nameof(ChatBehaviour.Send_ChatMessage))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> ChatBehaviour__Send_ChatMessage__Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.Wrap(instructions)
                .ReplaceStrings(new[] {
                    I18nKeys.ChatBehaviour.GLOBAL_CHANNEL_DISABLED,
                    I18nKeys.ChatBehaviour.PARTY_CHANNEL_DISABLED,
                    I18nKeys.ChatBehaviour.ROOM_CHANNEL_DISABLED
                }).Unwrap();
        }

        [HarmonyPatch(typeof(ItemMenuCell), nameof(ItemMenuCell.PromptCmd_DropItem))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> ItemMenuCell__PromptCmd_DropItem__Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);
            TranspilerHelper.RemoveMethodCallParamsStackForward(matcher, MessageCallbacks.New_ChatMessage, 5);
            matcher.InsertAndAdvance(new[]
            {
                new CodeInstruction(OpCodes.Ldloc_0),
                Transpilers.EmitDelegate<Func<ScriptableItem, string>>(item =>
                {
                    var key = KeyUtil.GetForAsset(item._scriptableQuest);
                    return I18nKeys.TabMenu.DROP_ITEM_ABANDON_QUEST_FORMAT.Format(key.Localize());
                })
            });
            return matcher.InstructionEnumeration();
        }

        [HarmonyPatch(typeof(WhoMenuCell), nameof(WhoMenuCell.Init_MutePeer))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> WhoMenuCell__Init_MutePeer__Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);
            TranspilerHelper.RemoveMethodCallParamsStackForward(matcher, MessageCallbacks.New_ChatMessage, 7);
            matcher.InsertAndAdvance(new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                Transpilers.EmitDelegate<Func<WhoMenuCell, string>>(cell =>
                {
                    return I18nKeys.ChatMessage.UNMUTE_PLAYER_FORMAT.Format(cell._selectedDataEntry._player._nickname);
                })
            });
            matcher.Advance(1);
            TranspilerHelper.RemoveMethodCallParamsStackForward(matcher, MessageCallbacks.New_ChatMessage, 7);
            matcher.InsertAndAdvance(new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                Transpilers.EmitDelegate<Func<WhoMenuCell, string>>(cell =>
                {
                    return I18nKeys.ChatMessage.MUTE_PLAYER_FORMAT.Format(cell._selectedDataEntry._player._nickname);
                })
            });
            return matcher.InstructionEnumeration();
        }
    }

    [HarmonyPatch(typeof(ChatBehaviour), nameof(ChatBehaviour.UserCode_Cmd_SendChatMessage__String__ChatChannel))]
    internal static class ChatBehaviour_AllowUnicodeChatMessage
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var containsUnicodeCharacter = AccessTools.Method(
                typeof(GameManager),
                nameof(GameManager.ContainsUnicodeCharacter));
            var allowUnicodeChatMessage = AccessTools.Method(
                typeof(ChatBehaviour_AllowUnicodeChatMessage),
                nameof(AllowUnicodeChatMessage));

            var matcher = new CodeMatcher(instructions)
                .MatchForward(false, new CodeMatch(instruction => instruction.Calls(containsUnicodeCharacter)));
            if (matcher.IsInvalid)
            {
                throw new InvalidOperationException("Could not find the chat Unicode validation call.");
            }

            return matcher
                .SetInstruction(new CodeInstruction(OpCodes.Call, allowUnicodeChatMessage))
                .InstructionEnumeration();
        }

        private static bool AllowUnicodeChatMessage(GameManager gameManager, string message)
        {
            return false;
        }
    }
}
