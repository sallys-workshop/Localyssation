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
                = Create(nameof(DISABLE_PARTY_CHANNEL_MESSAGE), "<color=#B2EC5D>Disabled #Party Chat Channel.</color>");
            public static readonly TranslationKey ENABLE_PARTY_CHANNEL_MESSAGE
                = Create(nameof(ENABLE_PARTY_CHANNEL_MESSAGE), "<color=#B2EC5D>Enabled #Party Chat Channel.</color>");

            public static readonly TranslationKey DISABLE_ROOM_CHANNEL_MESSAGE
                = Create(nameof(DISABLE_ROOM_CHANNEL_MESSAGE), "<color=#FF8A90>Disabled #Zone Chat Channel.</color>");
            public static readonly TranslationKey ENABLE_ROOM_CHANNEL_MESSAGE
                = Create(nameof(ENABLE_ROOM_CHANNEL_MESSAGE), "<color=#FF8A90>Enabled #Zone Chat Channel.</color>");

            public static readonly TranslationKey CHANNEL_SWTICH_MESSAGE_FORMAT
                = Create(nameof(CHANNEL_SWTICH_MESSAGE_FORMAT), "{0} Entered #{1}.");

            public static readonly TranslationKey GLOBAL_CHANNEL_DISABLED
                = Create(nameof(GLOBAL_CHANNEL_DISABLED), "Global chat is disabled.");
            public static readonly TranslationKey PARTY_CHANNEL_DISABLED
                = Create(nameof(PARTY_CHANNEL_DISABLED), "Party chat is disabled.");
            public static readonly TranslationKey ROOM_CHANNEL_DISABLED
                = Create(nameof(ROOM_CHANNEL_DISABLED), "Zone chat is disabled.");

            public static readonly TranslationKey ENTER_A_ROOM_HINT
                = Create(nameof(ENTER_A_ROOM_HINT), "Enter a room to send messages to a room channel.");

            public static readonly TranslationKey INPUT_PLACEHOLDER
                = Create(nameof(INPUT_PLACEHOLDER), "Enter text... (/g, /p, /z)");
            public static readonly TranslationKey GLOBAL_INPUT_PLACEHOLDER
                = Create(nameof(GLOBAL_INPUT_PLACEHOLDER), "#Global Chat... (>/g, /p, /z)");
            public static readonly TranslationKey PARTY_INPUT_PLACEHOLDER
                = Create(nameof(PARTY_INPUT_PLACEHOLDER), "#Party Chat... (/g, >/p, /z)");
            public static readonly TranslationKey ZONE_INPUT_PLACEHOLDER
                = Create(nameof(ZONE_INPUT_PLACEHOLDER), "#Zone Chat... (/g, /p, >/z)");

            public static readonly TranslationKey WELCOME_MESSAGE_FORMAT
                = Create(nameof(WELCOME_MESSAGE_FORMAT), "<color=#a7fc00>Welcome to ATLYSS (version: {0})</color>");
        }
    }
}
