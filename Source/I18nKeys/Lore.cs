
namespace Localyssation
{
    internal static partial class I18nKeys
    {

        internal static class Lore
        {
            internal static void Init() { }
            public static readonly string CROWN
                = Create("CROWN", "crown");
            public static readonly string CROWN_PLURAL
                = Create("CROWN_PLURAL", "crowns");
            public static readonly string GAME_LOADING
                = Create("GAME_LOADING", "Loading...");
            public static readonly string EXP_COUNTER_MAX
                = Create("EXP_COUNTER_MAX", "MAX");
            public static readonly string COMBAT_ELEMENT_NORMAL_NAME
                = Create("COMBAT_ELEMENT_NORMAL_NAME", "Normal");

            public static readonly string FORMAT_MAP_ZONE
                = Create("MAP_ZONE_FORMAT", "- {0} Zone -");

            public static readonly string INTERACT_TELEPORT
                = Create(nameof(INTERACT_TELEPORT), "TELEPORT");
            public static readonly string INTERACT_REEL
                = Create(nameof(INTERACT_REEL), "REEL");
            public static readonly string INTERACT_REVIVE
                = Create(nameof(INTERACT_REVIVE), "REVIVE");
            public static readonly string INTERACT_INTERACT
                = Create(nameof(INTERACT_INTERACT), "INTERACT");
            public static readonly string INTERACT_HOLD
                = Create(nameof(INTERACT_HOLD), "HOLD");
            public static readonly string INTERACT_OPEN
                = Create(nameof(INTERACT_OPEN), "OPEN");
            public static readonly string INTERACT_PICK_UP
                = Create(nameof(INTERACT_PICK_UP), "PICK UP");

            public static readonly string WORLDPORTAL_SELECT_WAYPOINT
                = Create(nameof(WORLDPORTAL_SELECT_WAYPOINT), "- Select Waypoint -");
            public static readonly string WORLDPORTAL_TITLE
                = Create(nameof(WORLDPORTAL_TITLE), "World Portal");
            public static readonly string WORLDPORTAL_TELEPORT
                = Create(nameof(WORLDPORTAL_TELEPORT), "Teleport");

            public static readonly string GAMBLING_SHOP_BUTTON_REROLL
                = Create(nameof(GAMBLING_SHOP_BUTTON_REROLL), "Re-roll Stock");
            public static readonly string DUNGEON_PORTAL_ENTER_PARTY
                = Create(nameof(DUNGEON_PORTAL_ENTER_PARTY), "Join Party");
            public static readonly string DUNGEON_PORTAL_ENTER_LEVELED
                = Create(nameof(DUNGEON_PORTAL_ENTER_LEVELED), "Enter Dungeon (LV {0}-{1})");
        }

    }
}