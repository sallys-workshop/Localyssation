using Localyssation.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localyssation
{
    internal static partial class I18nKeys
    {
        public static class DeathPrompt
        {
            internal static void Init() { }


            public static readonly TranslationKey USER_TIER_PROMPT_FORMAT
                = I18nKeys.Create(nameof(USER_TIER_PROMPT_FORMAT), "Use Tear (x{0})");
            public static readonly TranslationKey DEATH_PROMPT_HEADER 
                = I18nKeys.Create(nameof(DEATH_PROMPT_HEADER), "You died.");
            public static readonly TranslationKey DEATH_PROMPT_RELEASE_SOUL_BUTTON
                = I18nKeys.Create(nameof(DEATH_PROMPT_RELEASE_SOUL_BUTTON), "Release Soul");
        }
    }
}
