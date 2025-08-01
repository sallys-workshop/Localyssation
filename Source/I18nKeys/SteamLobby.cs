
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
            public readonly static TranslationKey LOBBY_HOST_HEADER_STEAM_UNAVAILABLE
                = Create("LOBBY_HOST_HEADER_STEAM_UNAVAILABLE", "Steam is not Initialized.");

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

            public readonly static TranslationKey LOBBY_TYPE_DESCRIPTION_PUBLIC
                = Create("LOBBY_TYPE_DESCRIPTION_PUBLIC", "Public lobbies will be advertised for anyone to join.");
            public readonly static TranslationKey LOBBY_TYPE_DESCRIPTION_FRIENDS
                = Create("LOBBY_TYPE_DESCRIPTION_FRIENDS", "Friend lobbies are joinable by invite or from the Steam friends list.");
            public readonly static TranslationKey LOBBY_TYPE_DESCRIPTION_PRIVATE
                = Create("LOBBY_TYPE_DESCRIPTION_PRIVATE", "Private lobbies are by invitation from Host only.");

            public readonly static TranslationKey CLEARED_HIDDEN_LOBBY_CACHE
                = Create("CLEARED_HIDDEN_LOBBY_CACHE", "Cleared hidden lobby cache...");
            public readonly static TranslationKey HIDDEN_LOBBY_COUNT_FORMAT
                = Create("HIDDEN_LOBBY_COUNT_FORMAT", "Hidden Lobbies: {0}");
            public readonly static TranslationKey STEAM_NOT_INITIALIZED
                = Create("STEAM_NOT_INITIALIZED", "Steam is not initialized.");
            public readonly static TranslationKey SEARCHING_FOR_LOBBIES
                = Create("SEARCHING_FOR_LOBBIES", "Searching...");
            public readonly static TranslationKey NO_LOBBIES_FOUND
                = Create("NO_LOBBIES_FOUND", "No lobbies found.");

            public readonly static TranslationKey LOBBY_FOUNDED_COUNT_FORMAT_1
                = Create("LOBBY_FOUNDED_COUNT_FORMAT_1", "{0} lobby found.");
            public readonly static TranslationKey LOBBY_FOUNDED_COUNT_FORMAT_PLURAL
                = Create("LOBBY_FOUNDED_COUNT_FORMAT_PLURAL", "{0} lobbies found.");

            public readonly static TranslationKey LOBBY_FULL
                = Create("LOBBY_FULL", "Lobby Full");

            public readonly static TranslationKey LOBBY_PASSWORD_LOBBY
                = Create("LOBBY_PASSWORD_LOBBY", "Password Lobby");
            public readonly static TranslationKey LOBBY_DIFFERENT_VERSION
                = Create("LOBBY_DIFFERENT_VERSION", "Different Version");
            public readonly static TranslationKey LOBBY_INVALID
                = Create("LOBBY_INVALID", "Invalid Lobby");
            public readonly static TranslationKey LOBBY_JOIN_FRIEND
                = Create("LOBBY_JOIN_FRIEND", "Join Lobby (Friend)");

            public readonly static TranslationKey LOBBY_PLAYER_COUNT
                = Create("LOBBY_PLAYER_COUNT", "Players: ");
            public readonly static TranslationKey UNTITLED_LOBBY
                = Create("UNTITLED_LOBBY", "Untitled Lobby");
            public readonly static TranslationKey LOBBY_PING_FORMAT
                = Create("LOBBY_PING_FORMAT", "Ping: {0}ms");

            public readonly static TranslationKey MODDED_LOBBY
                = Create("MODDED_LOBBY", "Modded Lobby");
            public readonly static TranslationKey JOIN_LOBBY
                = Create("JOIN_LOBBY", "Join Lobby");

            public readonly static TranslationKey CLEAR_HIDDEN_LOBBY_BUTTON
                = Create("CLEAR_HIDDEN_LOBBY_BUTTON", "Clear Hidden Lobby Cache");

            public readonly static TranslationKey LOBBY_LIST_FILTER_BASE
                = Create("LOBBY_LIST_FILTER", "Lobby List Filter");
            public readonly static TranslationKey[] LOBBY_LIST_FILTER
                //= new TranslationKey[]
                //{
                //    Create(LOBBY_LIST_FILTER_BASE.Option[0], "Worldwide"),
                //    Create("")
                //};
                = CreateOptions(LOBBY_LIST_FILTER_BASE, new[] { 
                    "Nearby", "Far", "Worldwide"
                });
            public readonly static TranslationKey LOBBY_TYPE_BASE
                = Create("LOBBY_TYPE", "Lobby Type");
            public readonly static TranslationKey[] LOBBY_TYPES
                = CreateOptions(LOBBY_TYPE_BASE, new[] {
                    "Public",
                    "Friends",
                    "Private"
                });
            public readonly static TranslationKey PLACEHOLDER_LOBBY_NAME_SEARCH
                = Create("PLACEHOLDER_LOBBY_NAME_SEARCH", "Search Lobby Name...");
        }
    }
}