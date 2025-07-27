using HarmonyLib;
using Localyssation.LangAdjutable;
using Localyssation.LanguageModule;
using System.Collections.Generic;
using UnityEngine.UI;

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

    }

}