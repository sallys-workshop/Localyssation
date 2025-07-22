
namespace Localyssation
{
    internal static partial class I18nKeys
    {
        internal static partial class Settings
        {
            internal static class Network
            {
                internal static void Init() { }
                public static readonly string HEADER_UI_SETTINGS
                    = Create("SETTINGS_NETWORK_HEADER_UI_SETTINGS", "UI Settings");
                public static readonly string CELL_DISPLAY_CREEP_NAMETAGS
                    = Create("SETTINGS_NETWORK_CELL_DISPLAY_CREEP_NAMETAGS", "Display Enemy Nametags");
                public static readonly string CELL_DISPLAY_GLOBAL_NICKNAME_TAGS
                    = Create("SETTINGS_NETWORK_CELL_DISPLAY_GLOBAL_NICKNAME_TAGS", "Display Global Nametags <color=cyan>(@XX)</color>");
                public static readonly string CELL_DISPLAY_LOCAL_NAMETAG
                    = Create("SETTINGS_NETWORK_CELL_DISPLAY_LOCAL_NAMETAG", "Display Local Character Name Tag");
                public static readonly string CELL_DISPLAY_HOST_TAG
                    = Create("SETTINGS_NETWORK_CELL_DISPLAY_HOST_TAG", "Display [HOST] Tag on Host Character");
                public static readonly string CELL_HIDE_DUNGEON_MINIMAP
                    = Create("SETTINGS_NETWORK_CELL_HIDE_DUNGEON_MINIMAP", "Hide Dungeon Minimap");
                public static readonly string CELL_HIDE_FPS_COUNTER
                    = Create("SETTINGS_NETWORK_CELL_HIDE_FPS_COUNTER", "Hide FPS Counter");
                public static readonly string CELL_HIDE_PING_COUNTER
                    = Create("SETTINGS_NETWORK_CELL_HIDE_PING_COUNTER", "Hide Ping Counter");
                public static readonly string CELL_HIDE_STAT_POINT_COUNTER
                    = Create("SETTINGS_NETWORK_CELL_HIDE_STAT_POINT_COUNTER", "Hide Stat Point Notice Panel");
                public static readonly string CELL_HIDE_SKILL_POINT_COUNTER
                    = Create("SETTINGS_NETWORK_CELL_HIDE_SKILL_POINT_COUNTER", "Hide Skill Point Notice Panel");

                public static readonly string HEADER_CLIENT_SETTINGS
                    = Create("SETTINGS_NETWORK_HEADER_CLIENT_SETTINGS", "Client Settings");
                public static readonly string CELL_ENABLE_PVP_ON_MAP_ENTER
                    = Create("SETTINGS_NETWORK_CELL_ENABLE_PVP_ON_MAP_ENTER", "Flag for PvP when available");
            }

        }
    }
}