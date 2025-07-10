using HarmonyLib;
using UnityEngine.UI;

namespace Localyssation.Patches.ReplaceFont
{
    internal static class FRChat
    {
        


        [HarmonyPatch(typeof(ChatBehaviour), nameof(ChatBehaviour.UserCode_Rpc_RecieveChatMessage__String__Boolean__ChatChannel))]
        [HarmonyPostfix]
        public static void FixChatFont(ChatBehaviour __instance, string message, bool _isEmoteMessage, ChatBehaviour.ChatChannel _chatChannel)
        {
            var text = __instance._chatTextMesh;
            var replacementFontLookupInfo = Localyssation.currentLanguage.info.fontReplacementLibrationSans;

            FRUtil.replaceTmpFont(text, replacementFontLookupInfo);
        }

        [HarmonyPatch(typeof(ChatBehaviourAssets), nameof(ChatBehaviourAssets.Update))]
        [HarmonyPostfix]
        public static void FixChatBehaviourAssets(ChatBehaviourAssets __instance)
        {
            FRUtil.replaceTmpFont(__instance._chatText, Localyssation.currentLanguage.info.fontReplacementLibrationSans);
        }
    }
}
