
namespace Localyssation
{
    internal static partial class I18nKeys
    {

        internal static class CharacterSelect
        {
            internal static void init() { }
            public static readonly string HEADER
                = create("CHARACTER_SELECT_HEADER", "Character Select");
            public static readonly string HEADER_GAME_MODE_SINGLEPLAYER
                = create("CHARACTER_SELECT_HEADER_GAME_MODE_SINGLEPLAYER", "Singleplayer");
            public static readonly string HEADER_GAME_MODE_HOST_MULTIPLAYER_PUBLIC
                = create("CHARACTER_SELECT_HEADER_GAME_MODE_HOST_MULTIPLAYER_PUBLIC", "Host Game (Public)");
            public static readonly string HEADER_GAME_MODE_HOST_MULTIPLAYER_FRIENDS
                = create("CHARACTER_SELECT_HEADER_GAME_MODE_HOST_MULTIPLAYER_FRIENDS", "Host Game (Friends)");
            public static readonly string HEADER_GAME_MODE_HOST_MULTIPLAYER_PRIVATE
                = create("CHARACTER_SELECT_HEADER_GAME_MODE_HOST_MULTIPLAYER_PRIVATE", "Host Game (Private)");
            public static readonly string HEADER_GAME_MODE_JOIN_MULTIPLAYER
                = create("CHARACTER_SELECT_HEADER_GAME_MODE_JOIN_MULTIPLAYER", "Join Game");
            public static readonly string HEADER_GAME_MODE_LOBBY_QUERY
                = create("CHARACTER_SELECT_HEADER_GAME_MODE_LOBBY_QUERY", "Lobby Query");

            public static readonly string BUTTON_CREATE_CHARACTER
                = create("CHARACTER_SELECT_BUTTON_CREATE_CHARACTER", "Create Character");
            public static readonly string BUTTON_DELETE_CHARACTER
                = create("CHARACTER_SELECT_BUTTON_DELETE_CHARACTER", "Delete Character");
            public static readonly string BUTTON_SELECT_CHARACTER
                = create("CHARACTER_SELECT_BUTTON_SELECT_CHARACTER", "Select Character");
            public static readonly string BUTTON_RETURN
                = create("CHARACTER_SELECT_BUTTON_RETURN", "Return");

            public static readonly string DATA_ENTRY_EMPTY_SLOT
                = create("CHARACTER_SELECT_DATA_ENTRY_EMPTY_SLOT", "Empty Slot");
            public static readonly string FORMAT_DATA_ENTRY_INFO
                = create("FORMAT_CHARACTER_SELECT_DATA_ENTRY_INFO", "Lv-{0} {1} {2}");

            public static readonly string CHARACTER_DELETE_PROMPT_TEXT
                = create("CHARACTER_SELECT_CHARACTER_DELETE_PROMPT_TEXT", "Type in the character's name to confirm.");
            public static readonly string CHARACTER_DELETE_PROMPT_PLACEHOLDER_TEXT
                = create("CHARACTER_SELECT_CHARACTER_DELETE_PROMPT_PLACEHOLDER_TEXT", "Enter Nickname...");
            public static readonly string CHARACTER_DELETE_BUTTON_CONFIRM
                = create("CHARACTER_SELECT_CHARACTER_DELETE_BUTTON_CONFIRM", "Delete Character");
            public static readonly string CHARACTER_DELETE_BUTTON_RETURN
                = create("CHARACTER_SELECT_CHARACTER_DELETE_BUTTON_RETURN", "Return");
        }

    }
}