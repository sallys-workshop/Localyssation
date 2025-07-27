
using Localyssation.Util;

namespace Localyssation
{
    internal static partial class I18nKeys
    {
        public static class SteamLobby
        {
            internal static void Init() { }
            private static TranslationKey Create(string key, string defaultValue = "")
            {
                return I18nKeys.Create($"STEAM_LOBBY_{key}", defaultValue);
            }

            public readonly static TranslationKey LOBBY_HOST_HEADER
                = Create("LOBBY_HOST_HEADER", "Host Lobby");
            public readonly static TranslationKey TAG_LOBBY_NAME
                = Create("TAG_LOBBY_NAME", "Lobby Name");
            public readonly static TranslationKey TAG_LOBBY_PASSWORD
                = Create("TAG_LOBBY_PASSWORD", "Lobby Password");
            public readonly static TranslationKey TAG_MOTD
                = Create("TAG_MOTD", "Message Of The Day");
            public readonly static TranslationKey TAG_LOBBY_TYPE
                = Create("TAG_LOBBY_TYPE", "Lobby Type");
            public readonly static TranslationKey TAG_MAX_PLAYERS
                = Create("TAG_MAX_PLAYERS", "Max Players");
            public readonly static TranslationKey TAG_STREAM_MODE
                = Create("TAG_STREAM_MODE", "Stream Mode (Disable Chat)");
            public readonly static TranslationKey TAG_LOBBY_REALM
                = Create("TAG_LOBBY_REALM", "Lobby Realm");
            public readonly static TranslationKey BUTTON_RETURN
                = Create("BUTTON_RETURN", "Return");
            public readonly static TranslationKey BUTTON_HOST_LOBBY
                = Create("BUTTON_HOST_LOBBY", "Host Lobby");

            public readonly static TranslationKey PLACEHOLDER_LOBBY_NAME
                = Create("PLACEHOLDER_LOBBY_NAME", "Enter text...");
            public readonly static TranslationKey PLACEHOLDER_LOBBY_PASSWORD
                = Create("PLACEHOLDER_LOBBY_PASSWORD", "Enter Password...");
            public readonly static TranslationKey PLACEHOLDER_MOTD
                = Create("PLACEHOLDER_MOTD", "Enter Message...");
        }
    }
}