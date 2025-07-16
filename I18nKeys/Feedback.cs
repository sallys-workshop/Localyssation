
namespace Localyssation
{
    internal static partial class I18nKeys
    {
        internal static class Feedback
        {
            internal static void init() { }
            private static string create(string key, string defaultString)
            {
                if (!key.StartsWith("FEEDBACK_"))
                {
                    key = "FEEDBACK_" + key;
                }
                I18nKeys.create(key, defaultString);
                return key;
            }
            public static readonly string DROP_ITEM_FORMAT
                = create("DROP_ITEM_FORMAT", "Dropped {0}. (-{1})");
        }
    }
}