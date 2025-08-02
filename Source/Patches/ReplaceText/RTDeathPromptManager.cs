using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Localyssation.Util;

namespace Localyssation.Patches.ReplaceText
{
    internal static partial class RTReplacer
    {
        [HarmonyPatch(typeof(DeathPromptManager), nameof(DeathPromptManager.Handle_DeathPromptWindow))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> DeathPromptManager__Handle_DeathPromptWindow__Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RTUtil.Wrap(instructions)
                .ReplaceStrings(new[] { 
                    I18nKeys.DeathPrompt.USER_TIER_PROMPT_FORMAT 
                })
                .Unwrap();
        }

        [HarmonyPatch(typeof(DeathPromptManager), nameof(DeathPromptManager.Start))]
        [HarmonyPostfix]
        private static void DeathPromptManager__Start__Postfix(DeathPromptManager __instance)
        {
            RTUtil.RemapChildTextsByPath(__instance.transform, new Dictionary<string, TranslationKey>()
            {
                { "Canvas_DeathPrompt/Dolly_deathPromptWindow/_deathPromptHeader", I18nKeys.DeathPrompt.DEATH_PROMPT_HEADER },
                {"Canvas_DeathPrompt/Dolly_deathPromptWindow/_deathPromptBackdrop/_button_releaseDeathPrompt/_text_releaseSoul", I18nKeys.DeathPrompt.DEATH_PROMPT_RELEASE_SOUL_BUTTON}
            });
        }
    }
}
