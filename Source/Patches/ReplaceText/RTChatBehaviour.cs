using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace Localyssation.Patches.ReplaceText
{
    internal static partial class RTReplacer
    {
        [HarmonyPatch(typeof(ChatBehaviour), nameof(ChatBehaviour.OnClick_GlobalChannel))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> ChatBehaviour__OnClick_GlobalChannel__Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.Wrap(instructions)
                .ReplaceStrings( new[] {
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
                .ReplaceStrings( new[] {
                    I18nKeys.ChatBehaviour.DISABLE_PARTY_CHANNEL_MESSAGE,
                    I18nKeys.ChatBehaviour.ENABLE_PARTY_CHANNEL_MESSAGE
                })
                .Unwrap();
        }

        [HarmonyPatch(typeof(ChatBehaviour), nameof(ChatBehaviour.OnClick_RoomChannel))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> ChatBehaviour__OnClick_RoomChannel__Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.Wrap(instructions)
                .ReplaceStrings( new[] {
                    I18nKeys.ChatBehaviour.DISABLE_ROOM_CHANNEL_MESSAGE,
                    I18nKeys.ChatBehaviour.ENABLE_ROOM_CHANNEL_MESSAGE
                })
                .Unwrap();
        }

        [HarmonyPatch(typeof(ChatBehaviour), nameof(ChatBehaviour.On_ChannelSwitch))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> ChatBehaviour__On_ChannelSwitch__Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);
            TranspilerHelper.RemoveMethodCallParamsStackForward(matcher, MessageCallbacks.New_ChatMessage, 7, OpCodes.Call);
            matcher.InsertAndAdvance(new CodeInstruction[] {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_2),
                Transpilers.EmitDelegate<Func<ChatBehaviour, string, string>>((cb, _new) =>
                {
                    return I18nKeys.ChatBehaviour.CHANNEL_SWTICH_MESSAGE_FORMAT.Format(cb._player._nickname, _new);
                })
            });
            return matcher.InstructionEnumeration();
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
                    I18nKeys.ChatBehaviour.ROOM_CHANNEL_DISABLED,
                    I18nKeys.ChatBehaviour.ENTER_A_ROOM_HINT
                }).Unwrap();
        }
    }
}
