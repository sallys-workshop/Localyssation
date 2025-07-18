using System.Collections.Immutable;

namespace Localyssation
{
    internal static partial class I18nKeys
    {

        internal static class TabMenu
        {
            internal static void init() { }

            public static readonly string PAGER_FORMAT
                = create("PAGER_FORMAT", "Page ( {0} / {1} )");
            public static readonly string PAGER_1_PAGE
                = create("PAGER_1_PAGE", "Page ( 1 / 1 )");


            public static readonly string CELL_STATS_HEADER
                = create("TAB_MENU_CELL_STATS_HEADER", "Stats");

            public static readonly string CELL_STATS_ATTRIBUTE_POINT_COUNTER
                = create("TAB_MENU_CELL_STATS_ATTRIBUTE_POINT_COUNTER", "Points");
            public static readonly string CELL_STATS_BUTTON_APPLY_ATTRIBUTE_POINTS
                = create("TAB_MENU_CELL_STATS_BUTTON_APPLY_ATTRIBUTE_POINTS", "Apply");

            public static readonly string CELL_STATS_INFO_CELL_NICK_NAME
                = create("TAB_MENU_CELL_STATS_INFO_CELL_NICK_NAME", "Nickname");
            public static readonly string CELL_STATS_INFO_CELL_RACE_NAME
                = create("TAB_MENU_CELL_STATS_INFO_CELL_RACE_NAME", "Race");
            public static readonly string CELL_STATS_INFO_CELL_CLASS_NAME
                = create("TAB_MENU_CELL_STATS_INFO_CELL_CLASS_NAME", "Class");
            public static readonly string CELL_STATS_INFO_CELL_LEVEL_COUNTER
                = create("TAB_MENU_CELL_STATS_INFO_CELL_LEVEL_COUNTER", "Level");
            public static readonly string CELL_STATS_INFO_CELL_EXPERIENCE
                = create("TAB_MENU_CELL_STATS_INFO_CELL_EXPERIENCE", "Experience");

            public static readonly string CELL_STATS_INFO_CELL_MAX_HEALTH
                = create("TAB_MENU_CELL_STATS_INFO_CELL_MAX_HEALTH", "Health");
            public static readonly string CELL_STATS_INFO_CELL_MAX_MANA
                = create("TAB_MENU_CELL_STATS_INFO_CELL_MAX_MANA", "Mana");
            public static readonly string CELL_STATS_INFO_CELL_MAX_STAMINA
                = create("TAB_MENU_CELL_STATS_INFO_CELL_MAX_STAMINA", "Stamina");

            public static readonly string CELL_STATS_INFO_CELL_ATTACK
                = create("TAB_MENU_CELL_STATS_INFO_CELL_ATTACK", "Attack Power");
            public static readonly string CELL_STATS_INFO_CELL_RANGED_POWER
                = create("TAB_MENU_CELL_STATS_INFO_CELL_RANGED_POWER", "Dex Power");
            public static readonly string CELL_STATS_INFO_CELL_PHYS_CRITICAL
                = create("TAB_MENU_CELL_STATS_INFO_CELL_PHYS_CRITICAL", "Phys. Crit %");

            public static readonly string CELL_STATS_INFO_CELL_MAGIC_POW
                = create("TAB_MENU_CELL_STATS_INFO_CELL_MAGIC_POW", "Mgk. Power");
            public static readonly string CELL_STATS_INFO_CELL_MAGIC_CRIT
                = create("TAB_MENU_CELL_STATS_INFO_CELL_MAGIC_CRIT", "Mgk. Crit %");

            public static readonly string CELL_STATS_INFO_CELL_DEFENSE
                = create("TAB_MENU_CELL_STATS_INFO_CELL_DEFENSE", "Defense");
            public static readonly string CELL_STATS_INFO_CELL_MAGIC_DEF
                = create("TAB_MENU_CELL_STATS_INFO_CELL_MAGIC_DEF", "Mgk. Defense");

            public static readonly string CELL_STATS_INFO_CELL_EVASION
                = create("TAB_MENU_CELL_STATS_INFO_CELL_EVASION", "Evasion %");
            public static readonly string CELL_STATS_INFO_CELL_MOVE_SPD
                = create("TAB_MENU_CELL_STATS_INFO_CELL_MOVE_SPD", "Mov Spd %");

            public static readonly string CELL_STATS_TOOLTIP_BASE_STAT_BEGIN
                = create("TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_BEGIN", "<b>Base Stat:</b> <i>");
            public static readonly string CELL_STATS_TOOLTIP_BASE_STAT_END_CRIT
                = create("TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_END_CRIT", "%</i> (Critical %)");
            public static readonly string CELL_STATS_TOOLTIP_BASE_STAT_END_EVASION
                = create("TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_END_EVASION", "%</i> (Evasion %)");
            public static readonly string CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_ATTACK_POW
                = create("TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_ATTACK_POW", "{0}</i> (Attack Power)");
            public static readonly string CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_MAX_MP
                = create("TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_MAX_MP", "{0}</i> (Max Mana)");
            public static readonly string CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_MAX_HP
                = create("TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_MAX_HP", "{0}</i> (Max Health)");
            public static readonly string CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_RANGE_POW
                = create("TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_RANGE_POW", "{0}</i> (Dex Power)");
            public static readonly string CELL_STATS_TOOLTIP_BASE_STAT_END_MAGIC_CRIT
                = create("TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_END_MAGIC_CRIT", "%</i> (Magic Critical %)");
            public static readonly string CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_MAGIC_DEF
                = create("TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_MAGIC_DEF", "{0}</i> (Magic Defense)");
            public static readonly string CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_DEFENSE
                = create("TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_DEFENSE", "{0}</i> (Defense)");
            public static readonly string CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_MAGIC_POW
                = create("TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_MAGIC_POW", "{0}</i> (Magic Power)");
            public static readonly string CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_MAX_STAM
                = create("TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_MAX_STAM", "{0}</i> (Max Stamina)");


            public static readonly string CELL_SKILLS_HEADER
                = create("TAB_MENU_CELL_SKILLS_HEADER", "Skills");

            public static readonly string CELL_SKILLS_SKILL_POINT_COUNTER
                = create("TAB_MENU_CELL_SKILLS_SKILL_POINT_COUNTER", "Skill Points");

            public static readonly string CELL_SKILLS_CLASS_TAB_TOOLTIP_NOVICE
                = create("TAB_MENU_CELL_SKILLS_CLASS_TAB_TOOLTIP_NOVICE", "General Skills");
            public static readonly string CELL_SKILLS_CLASS_TAB_TOOLTIP
                = create("TAB_MENU_CELL_SKILLS_CLASS_TAB_TOOLTIP", "{0} Skills");

            public static readonly string CELL_SKILLS_CLASS_HEADER_NOVICE
                = create("TAB_MENU_CELL_SKILLS_CLASS_HEADER_NOVICE", "General Skillbook");
            public static readonly string CELL_SKILLS_CLASS_HEADER
                = create("TAB_MENU_CELL_SKILLS_CLASS_HEADER", "{0} Skillbook");

            public static readonly string CELL_OPTIONS_HEADER
                = create("TAB_MENU_CELL_OPTIONS_HEADER", "Options");
            public static readonly string CELL_OPTIONS_BUTTON_SETTINGS
                = create("TAB_MENU_CELL_OPTIONS_BUTTON_SETTINS", "Settings");
            public static readonly string CELL_OPTIONS_BUTTON_SAVE_FILE
                = create("TAB_MENU_CELL_OPTIONS_BUTTON_SAVE_FILE", "Save File");
            public static readonly string CELL_OPTIONS_BUTTON_INVITE_TO_LOBBY
                = create("TAB_MENU_CELL_OPTIONS_BUTTON_INVITE_TO_LOBBY", "Invite to Lobby");
            public static readonly string CELL_OPTIONS_BUTTON_HOST_CONSOLE
                = create("TAB_MENU_CELL_OPTIONS_BUTTON_HOST_CONSOLE", "Host Console");
            public static readonly string CELL_OPTIONS_BUTTON_SAVE_AND_QUIT
                = create("TAB_MENU_CELL_OPTIONS_BUTTON_SAVE_AND_QUIT", "Save & Quit");
            public static readonly string CELL_OPTIONS_CONFIRM_QUIT_HEADER
                = create("TAB_MENU_CELL_OPTIONS_CONFIRM_QUIT_HEADER", "Save and Quit Game?");
            public static readonly string CELL_OPTIONS_CONFIRM_QUIT_CONFIRM
                = create("TAB_MENU_CELL_OPTIONS_CONFIRM_QUIT_CONFIRM", "Confirm");
            public static readonly string CELL_OPTIONS_CONFIRM_QUIT_CANCEL
                = create("TAB_MENU_CELL_OPTIONS_CONFIRM_QUIT_CANCEL", "Cancel");

            private static string createCellItems(string key, string value="")
            {
                return create($"TAB_MENU_CELL_ITEMS_{key}", value);
            }
            public static readonly string CELL_ITEMS_HEADER
                = create("TAB_MENU_CELL_ITEMS_HEADER", "Items");
            public static readonly string CELL_ITEMS_EQUIP_TAB_HEADER_EQUIPMENT
                = create("TAB_MENU_CELL_ITEMS_EQUIP_TAB_HEADER_EQUIPMENT", "Equipment");
            public static readonly string CELL_ITEMS_EQUIP_TAB_HEADER_VANITY
                = create("TAB_MENU_CELL_ITEMS_EQUIP_TAB_HEADER_VANITY", "Vanity");
            public static readonly string CELL_ITEMS_EQUIP_TAB_HEADER_STAT
                = create("TAB_MENU_CELL_ITEMS_EQUIP_TAB_HEADER_STAT", "Stats");

            public static readonly string CELL_ITEMS_INVENTORY_TYPE_EQUIPMENT
                = createCellItems("INVENTORY_TYPE_EQUIPMENT", "Equipment");
            public static readonly string CELL_ITEMS_INVENTORY_TYPE_CONSUMABLE
                = createCellItems("INVENTORY_TYPE_CONSUMABLE", "Consumables");
            public static readonly string CELL_ITEMS_INVENTORY_TYPE_TRADE_ITEM
                = createCellItems("INVENTORY_TYPE_TRADE_TYPE", "Trade Items");
            public static readonly string CELL_ITEMS_INVENTORY_SORT_ITEMS
                = createCellItems("INVENTORY_SORT_ITEMS", "Sort Items");

            public static readonly ImmutableDictionary<string, string> CELL_ITEMS_PROMPT_BUTTONS
                = ImmutableArray.Create(
                    "equip", "transmogrify", "remove", "use", "split", "drop", "cancel"
                ).ToImmutableDictionary(
                    x => x,
                    x => createCellItems($"PROMPT_BUTTON_{x.ToUpper()}", char.ToUpper(x[0]) + x.Substring(1))
                );

            public static readonly string CELL_QUESTS_HEADER
                = create("TAB_MENU_CELL_QUESTS_HEADER", "Quests");
            public static readonly string CELL_QUESTS_BUTTON_ABANDON
                = create("TAB_MENU_CELL_QUESTS_BUTTON_ABANDON", "Abandon Quest");



            public static readonly string CELL_WHO_HEADER
                = create("TAB_MENU_CELL_WHO_HEADER", "Who");
            public static readonly string CELL_WHO_BUTTON_INVITE_TO_PARTY
                = create("TAB_MENU_CELL_WHO_BUTTON_INVITE_TO_PARTY", "Invite to Party");
            public static readonly string CELL_WHO_BUTTON_LEAVE_PARTY
                = create("TAB_MENU_CELL_WHO_BUTTON_LEAVE_PARTY", "Leave Party");
            public static readonly string CELL_WHO_BUTTON_MUTE_PEER
                = create("TAB_MENU_CELL_WHO_BUTTON_MUTE_PEER", "Mute / Unmute");
            public static readonly string CELL_WHO_BUTTON_REFRESH_LIST
                = create("TAB_MENU_CELL_WHO_BUTTON_REFRESH_LIST", "Refresh");
        }

    }
}