using Localyssation.Util;

namespace Localyssation
{
    internal static partial class I18nKeys
    {
        public static class ChatBehaviour
        {
            private static TranslationKey Create(string key, string english)
            {
                return I18nKeys.Create($"CHAT_BEHAVIOUR_{key}", english);
            }
            public static void Init() { }

            public static readonly TranslationKey DISABLE_GLOBAL_CHANNEL_MESSAGE
                = Create(nameof(DISABLE_GLOBAL_CHANNEL_MESSAGE), "<color=yellow>Disabled #Global Chat Channel.</color>");
            public static readonly TranslationKey ENABLE_GLOBAL_CHANNEL_MESSAGE
                = Create(nameof(ENABLE_GLOBAL_CHANNEL_MESSAGE), "<color=yellow>Enabled #Global Chat Channel.</color>");

            public static readonly TranslationKey DISABLE_PARTY_CHANNEL_MESSAGE
                = Create(nameof(DISABLE_PARTY_CHANNEL_MESSAGE), "<color=yellow>Disabled #Party Chat Channel.</color>");
            public static readonly TranslationKey ENABLE_PARTY_CHANNEL_MESSAGE
                = Create(nameof(ENABLE_PARTY_CHANNEL_MESSAGE), "<color=yellow>Enabled #Party Chat Channel.</color>");

            public static readonly TranslationKey DISABLE_ROOM_CHANNEL_MESSAGE
                = Create(nameof(DISABLE_ROOM_CHANNEL_MESSAGE), "<color=yellow>Disabled #Room Chat Channel.</color>");
            public static readonly TranslationKey ENABLE_ROOM_CHANNEL_MESSAGE
                = Create(nameof(ENABLE_ROOM_CHANNEL_MESSAGE), "<color=yellow>Enabled #Room Chat Channel.</color>");

            public static readonly TranslationKey CHANNEL_SWTICH_MESSAGE_FORMAT
                = Create(nameof(CHANNEL_SWTICH_MESSAGE_FORMAT), "{0} Entered #{1}.");

            public static readonly TranslationKey GLOBAL_CHANNEL_DISABLED
                = Create(nameof(GLOBAL_CHANNEL_DISABLED), "#Global chat is disabled.");
            public static readonly TranslationKey PARTY_CHANNEL_DISABLED
                = Create(nameof(PARTY_CHANNEL_DISABLED), "#Party chat is disabled.");
            public static readonly TranslationKey ROOM_CHANNEL_DISABLED
                = Create(nameof(ROOM_CHANNEL_DISABLED), "#Room chat is disabled.");

            public static readonly TranslationKey ENTER_A_ROOM_HINT
                = Create(nameof(ENTER_A_ROOM_HINT), "Enter a room to send messages to a room channel.");
        }
    }
}