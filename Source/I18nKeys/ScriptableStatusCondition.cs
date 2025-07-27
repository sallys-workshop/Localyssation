
namespace Localyssation
{
    internal static partial class I18nKeys
    {
        internal static class ScriptableStatusCondition
        {
            internal static void Init() { }
            public static readonly string DURATION_FORMAT
                = Create("SCRIPTABLE_STATUS_CONDITION_DURATION_FORMAT", "<color=yellow>Lasts for {0} sec</color>.");
            public static readonly string RATE_FORMAT
                = Create("SCRIPTABLE_STATUS_CONDITION_RATE_FORMAT", "<color=yellow>every {0} sec</color>.");
        }
    }
}