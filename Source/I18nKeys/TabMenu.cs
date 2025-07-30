using Localyssation.Util;
using System.Collections.Generic;
using System.Linq;

namespace Localyssation
{
    internal static partial class I18nKeys
    {

        internal static class TabMenu
        {
            internal static void Init() { }

            public static readonly string PAGER_FORMAT
                = Create("PAGER_FORMAT", "Page ( {0} / {1} )");
            public static readonly string PAGER_1_PAGE
                = Create("PAGER_1_PAGE", "Page ( 1 / 1 )");

            public static readonly TranslationKey POINTS_AVAILABLE
                = Create("TAB_MENU_POINTS_AVAILABLE", "Points Available");


            public static readonly string CELL_STATS_HEADER
                = Create("TAB_MENU_CELL_STATS_HEADER", "Stats");

            public static readonly string CELL_STATS_ATTRIBUTE_POINT_COUNTER
                = Create("TAB_MENU_CELL_STATS_ATTRIBUTE_POINT_COUNTER", "Points");
            public static readonly string CELL_STATS_BUTTON_APPLY_ATTRIBUTE_POINTS
                = Create("TAB_MENU_CELL_STATS_BUTTON_APPLY_ATTRIBUTE_POINTS", "Apply");

            public static readonly string CELL_STATS_INFO_CELL_NICK_NAME
                = Create("TAB_MENU_CELL_STATS_INFO_CELL_NICK_NAME", "Nickname");
            public static readonly string CELL_STATS_INFO_CELL_RACE_NAME
                = Create("TAB_MENU_CELL_STATS_INFO_CELL_RACE_NAME", "Race");
            public static readonly string CELL_STATS_INFO_CELL_CLASS_NAME
                = Create("TAB_MENU_CELL_STATS_INFO_CELL_CLASS_NAME", "Class");
            public static readonly string CELL_STATS_INFO_CELL_LEVEL_COUNTER
                = Create("TAB_MENU_CELL_STATS_INFO_CELL_LEVEL_COUNTER", "Level");
            public static readonly string CELL_STATS_INFO_CELL_EXPERIENCE
                = Create("TAB_MENU_CELL_STATS_INFO_CELL_EXPERIENCE", "Experience");

            public static readonly string CELL_STATS_INFO_CELL_MAX_HEALTH
                = Create("TAB_MENU_CELL_STATS_INFO_CELL_MAX_HEALTH", "Health");
            public static readonly string CELL_STATS_INFO_CELL_MAX_MANA
                = Create("TAB_MENU_CELL_STATS_INFO_CELL_MAX_MANA", "Mana");
            public static readonly string CELL_STATS_INFO_CELL_MAX_STAMINA
                = Create("TAB_MENU_CELL_STATS_INFO_CELL_MAX_STAMINA", "Stamina");

            public static readonly string CELL_STATS_INFO_CELL_ATTACK
                = Create("TAB_MENU_CELL_STATS_INFO_CELL_ATTACK", "Attack Power");
            public static readonly string CELL_STATS_INFO_CELL_RANGED_POWER
                = Create("TAB_MENU_CELL_STATS_INFO_CELL_RANGED_POWER", "Dex Power");
            public static readonly string CELL_STATS_INFO_CELL_PHYS_CRITICAL
                = Create("TAB_MENU_CELL_STATS_INFO_CELL_PHYS_CRITICAL", "Phys. Crit %");

            public static readonly string CELL_STATS_INFO_CELL_MAGIC_POW
                = Create("TAB_MENU_CELL_STATS_INFO_CELL_MAGIC_POW", "Mgk. Power");
            public static readonly string CELL_STATS_INFO_CELL_MAGIC_CRIT
                = Create("TAB_MENU_CELL_STATS_INFO_CELL_MAGIC_CRIT", "Mgk. Crit %");

            public static readonly string CELL_STATS_INFO_CELL_DEFENSE
                = Create("TAB_MENU_CELL_STATS_INFO_CELL_DEFENSE", "Defense");
            public static readonly string CELL_STATS_INFO_CELL_MAGIC_DEF
                = Create("TAB_MENU_CELL_STATS_INFO_CELL_MAGIC_DEF", "Mgk. Defense");

            public static readonly string CELL_STATS_INFO_CELL_EVASION
                = Create("TAB_MENU_CELL_STATS_INFO_CELL_EVASION", "Evasion %");
            public static readonly string CELL_STATS_INFO_CELL_MOVE_SPD
                = Create("TAB_MENU_CELL_STATS_INFO_CELL_MOVE_SPD", "Mov Spd %");

            public static readonly string CELL_STATS_TOOLTIP_BASE_STAT_BEGIN
                = Create("TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_BEGIN", "<b>Base Stat:</b> <i>");
            public static readonly string CELL_STATS_TOOLTIP_BASE_STAT_END_CRIT
                = Create("TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_END_CRIT", "%</i> (Critical %)");
            public static readonly string CELL_STATS_TOOLTIP_BASE_STAT_END_EVASION
                = Create("TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_END_EVASION", "%</i> (Evasion %)");
            public static readonly string CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_ATTACK_POW
                = Create("TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_ATTACK_POW", "{0}</i> (Attack Power)");
            public static readonly string CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_MAX_MP
                = Create("TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_MAX_MP", "{0}</i> (Max Mana)");
            public static readonly string CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_MAX_HP
                = Create("TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_MAX_HP", "{0}</i> (Max Health)");
            public static readonly string CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_RANGE_POW
                = Create("TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_RANGE_POW", "{0}</i> (Dex Power)");
            public static readonly string CELL_STATS_TOOLTIP_BASE_STAT_END_MAGIC_CRIT
                = Create("TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_END_MAGIC_CRIT", "%</i> (Magic Critical %)");
            public static readonly string CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_MAGIC_DEF
                = Create("TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_MAGIC_DEF", "{0}</i> (Magic Defense)");
            public static readonly string CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_DEFENSE
                = Create("TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_DEFENSE", "{0}</i> (Defense)");
            public static readonly string CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_MAGIC_POW
                = Create("TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_MAGIC_POW", "{0}</i> (Magic Power)");
            public static readonly string CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_MAX_STAM
                = Create("TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_MAX_STAM", "{0}</i> (Max Stamina)");


            public static readonly string CELL_SKILLS_HEADER
                = Create("TAB_MENU_CELL_SKILLS_HEADER", "Skills");

            public static readonly string CELL_SKILLS_SKILL_POINT_COUNTER
                = Create("TAB_MENU_CELL_SKILLS_SKILL_POINT_COUNTER", "Skill Points");

            public static readonly string CELL_SKILLS_CLASS_TAB_TOOLTIP_NOVICE
                = Create("TAB_MENU_CELL_SKILLS_CLASS_TAB_TOOLTIP_NOVICE", "General Skills");
            public static readonly string CELL_SKILLS_CLASS_TAB_TOOLTIP
                = Create("TAB_MENU_CELL_SKILLS_CLASS_TAB_TOOLTIP", "{0} Skills");

            public static readonly string CELL_SKILLS_CLASS_HEADER_NOVICE
                = Create("TAB_MENU_CELL_SKILLS_CLASS_HEADER_NOVICE", "General Skillbook");
            public static readonly string CELL_SKILLS_CLASS_HEADER
                = Create("TAB_MENU_CELL_SKILLS_CLASS_HEADER", "{0} Skillbook");

            public static readonly string CELL_OPTIONS_HEADER
                = Create("TAB_MENU_CELL_OPTIONS_HEADER", "Options");
            public static readonly string CELL_OPTIONS_BUTTON_SETTINGS
                = Create("TAB_MENU_CELL_OPTIONS_BUTTON_SETTINS", "Settings");
            public static readonly string CELL_OPTIONS_BUTTON_SAVE_FILE
                = Create("TAB_MENU_CELL_OPTIONS_BUTTON_SAVE_FILE", "Save File");
            public static readonly string CELL_OPTIONS_BUTTON_INVITE_TO_LOBBY
                = Create("TAB_MENU_CELL_OPTIONS_BUTTON_INVITE_TO_LOBBY", "Invite to Lobby");
            public static readonly string CELL_OPTIONS_BUTTON_HOST_CONSOLE
                = Create("TAB_MENU_CELL_OPTIONS_BUTTON_HOST_CONSOLE", "Host Console");
            public static readonly string CELL_OPTIONS_BUTTON_SAVE_AND_QUIT
                = Create("TAB_MENU_CELL_OPTIONS_BUTTON_SAVE_AND_QUIT", "Save & Quit");
            public static readonly string CELL_OPTIONS_CONFIRM_QUIT_HEADER
                = Create("TAB_MENU_CELL_OPTIONS_CONFIRM_QUIT_HEADER", "Save and Quit Game?");
            public static readonly string CELL_OPTIONS_CONFIRM_QUIT_CONFIRM
                = Create("TAB_MENU_CELL_OPTIONS_CONFIRM_QUIT_CONFIRM", "Confirm");
            public static readonly string CELL_OPTIONS_CONFIRM_QUIT_CANCEL
                = Create("TAB_MENU_CELL_OPTIONS_CONFIRM_QUIT_CANCEL", "Cancel");

            private static string CreateCellItems(string key, string value = "")
            {
                return Create($"TAB_MENU_CELL_ITEMS_{key}", value);
            }
            public static readonly string CELL_ITEMS_HEADER
                = Create("TAB_MENU_CELL_ITEMS_HEADER", "Items");
            public static readonly string CELL_ITEMS_EQUIP_TAB_HEADER_EQUIPMENT
                = Create("TAB_MENU_CELL_ITEMS_EQUIP_TAB_HEADER_EQUIPMENT", "Equipment");
            public static readonly string CELL_ITEMS_EQUIP_TAB_HEADER_VANITY
                = Create("TAB_MENU_CELL_ITEMS_EQUIP_TAB_HEADER_VANITY", "Vanity");
            public static readonly string CELL_ITEMS_EQUIP_TAB_HEADER_STAT
                = Create("TAB_MENU_CELL_ITEMS_EQUIP_TAB_HEADER_STAT", "Stats");

            public static readonly string CELL_ITEMS_INVENTORY_TYPE_EQUIPMENT
                = CreateCellItems("INVENTORY_TYPE_EQUIPMENT", "Equipment");
            public static readonly string CELL_ITEMS_INVENTORY_TYPE_CONSUMABLE
                = CreateCellItems("INVENTORY_TYPE_CONSUMABLE", "Consumables");
            public static readonly string CELL_ITEMS_INVENTORY_TYPE_TRADE_ITEM
                = CreateCellItems("INVENTORY_TYPE_TRADE_TYPE", "Trade Items");
            public static readonly string CELL_ITEMS_INVENTORY_SORT_ITEMS
                = CreateCellItems("INVENTORY_SORT_ITEMS", "Sort Items");


            public static readonly TranslationKey DROP_ITEM_ABANDON_QUEST_FORMAT
                = Create("TAB_MENU_DROP_ITEM_ABANDON_QUEST_FORMAT", "Abandoned Quest: {0}");


            public static readonly IDictionary<string, string> CELL_ITEMS_PROMPT_BUTTONS
                = new[] {
                    "equip", "transmogrify", "remove", "use", "split", "drop", "cancel"
                }.ToDictionary(
                    x => x,
                    x => CreateCellItems($"PROMPT_BUTTON_{x.ToUpper()}", char.ToUpper(x[0]) + x.Substring(1))
                );

            public static readonly string CELL_QUESTS_HEADER
                = Create("TAB_MENU_CELL_QUESTS_HEADER", "Quests");
            public static readonly string CELL_QUESTS_BUTTON_ABANDON
                = Create("TAB_MENU_CELL_QUESTS_BUTTON_ABANDON", "Abandon Quest");



            public static readonly string CELL_WHO_HEADER
                = Create("TAB_MENU_CELL_WHO_HEADER", "Who");
            public static readonly string CELL_WHO_BUTTON_INVITE_TO_PARTY
                = Create("TAB_MENU_CELL_WHO_BUTTON_INVITE_TO_PARTY", "Invite to Party");
            public static readonly string CELL_WHO_BUTTON_LEAVE_PARTY
                = Create("TAB_MENU_CELL_WHO_BUTTON_LEAVE_PARTY", "Leave Party");
            public static readonly string CELL_WHO_BUTTON_MUTE_PEER
                = Create("TAB_MENU_CELL_WHO_BUTTON_MUTE_PEER", "Mute / Unmute");
            public static readonly string CELL_WHO_BUTTON_REFRESH_LIST
                = Create("TAB_MENU_CELL_WHO_BUTTON_REFRESH_LIST", "Refresh");
        }

    }
}