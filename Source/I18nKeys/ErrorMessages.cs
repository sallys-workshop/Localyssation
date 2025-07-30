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
        internal static class ErrorMessages
        {
            internal static void Init() { }

            private static TranslationKey Create(string key, string english)
            {
                return I18nKeys.Create($"ERROR_MESSAGE_{key}", english);
            }

            public static readonly TranslationKey QUEST_LOG_FULL
                = Create("QUEST_LOG_FULL", "Quest Log Full");
            public static readonly TranslationKey QUEST_ALREADY_IN_LOG
                = Create("ALREADY_ON_THIS_QUEST", "Already on this Quest");
        }
    }
    
}
