using HarmonyLib;
using Localyssation;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localyssation.Patches.ReplaceFont
{
    internal static class FRUtil
    {
        public static void replaceTmpFont(TMPro.TMP_Text text, Language.BundledFontLookupInfo replacementFontLookupInfo)
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

        private static readonly ImmutableList<Type> PATCH_CLASSES = ImmutableList.Create(
            typeof(FRChat),
            typeof(FRItemObjectVisual),
            typeof(FRPlayerNickname)
        );

        public static void PatchAll(Harmony harmony)
        {
            foreach (var patchClass in PATCH_CLASSES)
            {
                harmony.PatchAll(patchClass);
            }
        }
    }
}
