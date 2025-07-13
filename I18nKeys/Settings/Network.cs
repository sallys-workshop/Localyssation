
namespace Localyssation
{
    internal static partial class I18nKeys
    {
        internal static partial class Settings
        {
            internal static class Network
            {
                internal static void init() { }
                public static readonly string HEADER_UI_SETTINGS
                    = create("SETTINGS_NETWORK_HEADER_UI_SETTINGS", "UI Settings");
                public static readonly string CELL_LOCALYSSATION_LANGUAGE
                    = create("SETTINGS_NETWORK_CELL_LOCALYSSATION_LANGUAGE", "Language");
                public static readonly string CELL_DISPLAY_CREEP_NAMETAGS
                    = create("SETTINGS_NETWORK_CELL_DISPLAY_CREEP_NAMETAGS", "Display Enemy Nametags");
                public static readonly string CELL_DISPLAY_GLOBAL_NICKNAME_TAGS
                    = create("SETTINGS_NETWORK_CELL_DISPLAY_GLOBAL_NICKNAME_TAGS", "Display Global Nametags <color=cyan>(@XX)</color>");
                public static readonly string CELL_DISPLAY_LOCAL_NAMETAG
                    = create("SETTINGS_NETWORK_CELL_DISPLAY_LOCAL_NAMETAG", "Display Local Character Name Tag");
                public static readonly string CELL_DISPLAY_HOST_TAG
                    = create("SETTINGS_NETWORK_CELL_DISPLAY_HOST_TAG", "Display [HOST] Tag on Host Character");
                public static readonly string CELL_HIDE_DUNGEON_MINIMAP
                    = create("SETTINGS_NETWORK_CELL_HIDE_DUNGEON_MINIMAP", "Hide Dungeon Minimap");
                public static readonly string CELL_HIDE_FPS_COUNTER
                    = create("SETTINGS_NETWORK_CELL_HIDE_FPS_COUNTER", "Hide FPS Counter");
                public static readonly string CELL_HIDE_PING_COUNTER
                    = create("SETTINGS_NETWORK_CELL_HIDE_PING_COUNTER", "Hide Ping Counter");
                public static readonly string CELL_HIDE_STAT_POINT_COUNTER
                    = create("SETTINGS_NETWORK_CELL_HIDE_STAT_POINT_COUNTER", "Hide Stat Point Notice Panel");
                public static readonly string CELL_HIDE_SKILL_POINT_COUNTER
                    = create("SETTINGS_NETWORK_CELL_HIDE_SKILL_POINT_COUNTER", "Hide Skill Point Notice Panel");

                public static readonly string HEADER_CLIENT_SETTINGS
                    = create("SETTINGS_NETWORK_HEADER_CLIENT_SETTINGS", "Client Settings");
                public static readonly string CELL_ENABLE_PVP_ON_MAP_ENTER
                    = create("SETTINGS_NETWORK_CELL_ENABLE_PVP_ON_MAP_ENTER", "Flag for PvP when available");
            }

        }
    }
}