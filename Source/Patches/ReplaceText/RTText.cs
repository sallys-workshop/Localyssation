using HarmonyLib;
using Localyssation.LangAdjutable;
using Localyssation.LanguageModule;
using Localyssation.Util;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YamlDotNet.Core.Tokens;

namespace Localyssation.Patches.ReplaceText
{
    internal static partial class RTReplacer
    {
        private static List<string> USED_FONT_NAME = new List<string>();
        [HarmonyPatch(typeof(Text), nameof(Text.text), MethodType.Setter)]
        [HarmonyPrefix]
        public static void Text_set_text(Text __instance, ref string value)
        {
            // if fontSize is not provided for a <scale> tag,
            // it gets auto-replaced with a <scalefallback> tag that gets parsed here instead
            if (__instance != null && value != null && value.Contains("scalefallback"))
            {
                value = Localyssation.ApplyTextEditTags(value, __instance.fontSize, RTUtil.GetFallbackTextEditTags());
            }
        }

        [HarmonyPatch(typeof(Text), nameof(Text.OnEnable))]
        [HarmonyPostfix]
        public static void Text_OnEnable(Text __instance)
        {
            if (LanguageManager.CurrentLanguage != null && __instance != null && __instance.font != null)
            {
                LangAdjustables.RegisterText(__instance);
            }
#if DEBUG
            if (__instance.font != null && !USED_FONT_NAME.Contains(__instance.font.name))
            {
                USED_FONT_NAME.Add(__instance.font.name);
                Localyssation.logger.LogDebug($"Using font: `{__instance.font.name}`");
            }
#endif
        }

        [HarmonyPatch(typeof(TMP_Text), nameof(TMP_Text.font), MethodType.Setter)]
        [HarmonyPrefix]
        static void TMP_Text_set_font(TMP_Text __instance, TMP_FontAsset value)
        {
            AddUnifontFallback(value);
        }

        [HarmonyPatch(typeof(TMP_Text), nameof(TMP_Text.text), MethodType.Setter)]
        [HarmonyPostfix]
        static void TMP_Text_set_text(TMP_Text __instance)
        {
            AddUnifontFallback(__instance.font);
        }

        private static void AddUnifontFallback(TMP_FontAsset value)
        {
            if (FontManager.UnifontLoaded && !(value is null))
            {
                if (value != null)
                {
                    if (value.fallbackFontAssetTable is null || value.fallbackFontAssetTable == null)
                    {
                        value.fallbackFontAssetTable = new List<TMP_FontAsset>();
                    }
                    if (!value.fallbackFontAssetTable.Contains(FontManager.UNIFONT_SDF))
                    {
                        value.fallbackFontAssetTable.Add(FontManager.UNIFONT_SDF);
                    }
                }
            }
        }

    }

}