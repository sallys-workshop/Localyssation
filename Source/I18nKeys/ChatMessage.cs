

using Localyssation.Util;
using YamlDotNet.Core.Tokens;

namespace Localyssation
{
    internal partial class I18nKeys
    {
        internal static class ChatMessage
        {
            internal static void Init() { }
            private static TranslationKey Create(string key, string defaultValue) => I18nKeys.Create("CHAT_MESSAGE_" + key, defaultValue);

            public static readonly TranslationKey RECIEVE_DUNGEON_KEY = 
                Create(nameof(RECIEVE_DUNGEON_KEY), "You received a dungeon key.");
            public static readonly TranslationKey DUNGEON_KEY_DISSIPATES =
                Create(nameof(DUNGEON_KEY_DISSIPATES), "A dungeon key dissipates...");

            public static readonly TranslationKey UNMUTE_PLAYER_FORMAT = 
                Create(nameof(UNMUTE_PLAYER_FORMAT), "Unmuted {0}.");
            public static readonly TranslationKey MUTE_PLAYER_FORMAT =
                Create(nameof(MUTE_PLAYER_FORMAT), "Muted {0}.");
        }
    }
}
