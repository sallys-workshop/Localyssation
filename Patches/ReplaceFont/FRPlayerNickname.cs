using System;
using HarmonyLib;

namespace Localyssation.Patches.ReplaceFont
{
    internal static class FRPlayerNickname
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

        [HarmonyPatch(typeof(Player), nameof(Player.Handle_ClientParameters))]
        [HarmonyPostfix]
        public static void Player_Handle_ClientParameter_Postfix(Player __instance)
        {
            if (__instance._nicknameTextMesh.enabled)
                replaceTmpFont(__instance._nicknameTextMesh, Localyssation.currentLanguage.info.fontReplacementLibrationSans);
            if (__instance._globalNicknameTextMesh.enabled)
                replaceTmpFont(__instance._globalNicknameTextMesh, Localyssation.currentLanguage.info.fontReplacementLibrationSans);
        }
    }
}
