using HarmonyLib;
using Localyssation.LanguageModule;
using Localyssation.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

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

        public static void ReplaceUiFont(UnityEngine.UI.Text text, Language.BundledFontLookupInfo replacementFontLookupInfo)
        {
            if (text == null || replacementFontLookupInfo == null)
            {
                return;
            }

            Font loadedFont = null;
            if (!FontManager.Fonts.TryGetValue(replacementFontLookupInfo.fontName, out loadedFont)
                && FontManager.TMPfonts.TryGetValue(replacementFontLookupInfo.fontName, out var loadedTmpFont))
            {
                // TMP assets in a font bundle normally reference the matching legacy Font.
                // The chat history uses TMP, while ATLYSS's chat input still uses UI.Text.
                loadedFont = loadedTmpFont.sourceFontFile;
            }

            if (loadedFont != null && text.font != loadedFont)
            {
                text.font = loadedFont;
                text.fontSize = (int)(text.fontSize * replacementFontLookupInfo.fontScale);
                text.lineSpacing *= replacementFontLookupInfo.fontScale;
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
