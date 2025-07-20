
namespace Localyssation
{
    internal static partial class I18nKeys
    {
        internal static class Feedback
        {
            internal static void Init() { }
            private static string Create(string key, string defaultString)
            {
                if (!key.StartsWith("FEEDBACK_"))
                {
                    key = "FEEDBACK_" + key;
                }
                I18nKeys.Create(key, defaultString);
                return key;
            }
            public static readonly string DROP_ITEM_FORMAT
                = Create("DROP_ITEM_FORMAT", "Dropped {0}. (-{1})");
        }
    }
}