using HarmonyLib;
using Localyssation.LanguageModule;
using Localyssation.Util;
using System;
using System.Collections.Generic;

namespace Localyssation.Patches.ReplaceFont
{
    internal static class FRUtil
    {
        public static void ReplaceTmpFont(TMPro.TMP_Text text, Language.BundledFontLookupInfo replacementFontLookupInfo)
        {
            if (
                replacementFontLookupInfo != null &&
                FontManager.TMPfonts.TryGetValue(replacementFontLookupInfo.fontName, out var loadedFont))
            {
                if (text.font != loadedFont)
                {
                    float orig_fontSize = text.fontSize;
                    float orig_lineSpacing = text.lineSpacing;
                    text.font = loadedFont;
                    text.fontSize = (int)(orig_fontSize * replacementFontLookupInfo.fontScale);
                    text.lineSpacing = orig_lineSpacing * replacementFontLookupInfo.fontScale;

                }
            }
        }

        private static readonly List<Type> PATCH_CLASSES = new List<Type>()
        {
            typeof(FRChat),
            typeof(FRItemObjectVisual),
            typeof(FRPlayerNickname)
        };

        public static void PatchAll(Harmony harmony)
        {
            foreach (var patchClass in PATCH_CLASSES)
            {
                harmony.PatchAll(patchClass);
            }
        }
    }
}
