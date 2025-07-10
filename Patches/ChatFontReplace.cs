using HarmonyLib;
using UnityEngine.UI;

namespace Localyssation.Patches
{
    internal static class ChatFontReplace
    {

        private static void replaceTmpFont(TMPro.TMP_Text text, Language.BundledFontLookupInfo replacementFontLookupInfo)
        {
            if (
                        replacementFontLookupInfo != null &&
                        Localyssation.fontBundles.TryGetValue(replacementFontLookupInfo.bundleName, out var fontBundle) &&
                        fontBundle.loadedFonts.TryGetValue(replacementFontLookupInfo.fontName, out var loadedFont))
            {
                if (text.font != loadedFont.tmpFont)
                {
                    float orig_fontSize = text.fontSize;
                    float orig_lineSpacing = text.lineSpacing;
                    text.font = loadedFont.tmpFont;
                    text.fontSize = (int)(orig_fontSize * loadedFont.info.sizeMultiplier);
                    text.lineSpacing = orig_lineSpacing * loadedFont.info.sizeMultiplier;

                }
            }
        }


        [HarmonyPatch(typeof(ChatBehaviour), nameof(ChatBehaviour.UserCode_Rpc_RecieveChatMessage__String__Boolean__ChatChannel))]
        [HarmonyPostfix]
        public static void FixChatFont(ChatBehaviour __instance, string message, bool _isEmoteMessage, ChatBehaviour.ChatChannel _chatChannel)
        {
            var text = __instance._chatTextMesh;
            var replacementFontLookupInfo = Localyssation.currentLanguage.info.fontReplacementLibrationSans;

            replaceTmpFont(text, replacementFontLookupInfo);
        }

        [HarmonyPatch(typeof(ChatBehaviourAssets), nameof(ChatBehaviourAssets.Update))]
        [HarmonyPostfix]
        public static void FixChatBehaviourAssets(ChatBehaviourAssets __instance)
        {
            replaceTmpFont(__instance._chatText, Localyssation.currentLanguage.info.fontReplacementLibrationSans);
        }
    }
}
