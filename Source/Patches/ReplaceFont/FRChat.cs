using HarmonyLib;
using Localyssation.LanguageModule;

namespace Localyssation.Patches.ReplaceFont
{
    internal static class FRChat
    {



        [HarmonyPatch(typeof(ChatBehaviour), nameof(ChatBehaviour.UserCode_Rpc_RecieveChatMessage__String__Boolean__ChatChannel))]
        [HarmonyPostfix]
        public static void FixChatFont(ChatBehaviour __instance, string message, bool _isEmoteMessage, ChatBehaviour.ChatChannel _chatChannel)
        {
            var text = __instance._chatTextMesh;
            var replacementFontLookupInfo = LanguageManager.CurrentLanguage.info.chatFont;

            FRUtil.ReplaceTmpFont(text, replacementFontLookupInfo);
        }

        [HarmonyPatch(typeof(ChatBehaviourAssets), nameof(ChatBehaviourAssets.Update))]
        [HarmonyPostfix]
        public static void FixChatBehaviourAssets(ChatBehaviourAssets __instance)
        {
            var replacementFontLookupInfo = LanguageManager.CurrentLanguage.info.chatFont;

            FRUtil.ReplaceTmpFont(__instance._chatText, replacementFontLookupInfo);
            if (__instance._chatInput != null)
            {
                FRUtil.ReplaceUiFont(__instance._chatInput.textComponent, replacementFontLookupInfo);
            }
            FRUtil.ReplaceUiFont(__instance.inputPlaceHolderText, replacementFontLookupInfo);
        }
    }
}
