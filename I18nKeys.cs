using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localyssation
{
    // Symbolized key strings, for better referring
    // Less string constants, less typos
    internal static class I18nKeys
    {

        // In case of lazy loading (experience from Java)
        public static void Init()
        {
            Item.init();
            Lore.init();
            Equipment.init();
            Quest.init(); 
            MainMenu.init();
            CharacterSelect.init();
            CharacterCreation.init();
            TabMenu.init();
            SkillMenu.init();
            Settings.init();
        }

        internal static readonly Dictionary<string, string> TR_KEYS = new Dictionary<string, string>();

        private static string create(string key, string defaultString = "")
        {
            if (string.IsNullOrEmpty(key)) { throw new ArgumentNullException("key is empty"); }
            if (string.IsNullOrEmpty(defaultString))
            {
                defaultString = key;
            }
            if (TR_KEYS.ContainsKey(key))
            {
                throw new ArgumentException($"key `{key}` Already Exists!");
            }
            TR_KEYS[key] = defaultString;
            return key;
        }

        public static string getDefaulted(string key)
        {
            bool success = TR_KEYS.TryGetValue(key, out string value);
            if (success)
            {
                return value;
            }
            return key;
        }


        internal static class Item
        {
            internal static void init() { }
            public static readonly string FORMAT_ITEM_RARITY
                = create("FORMAT_ITEM_RARITY", "[{0}]");
            public static readonly string FORMAT_ITEM_TOOLTIP_VENDOR_VALUE_COUNTER
                = create("FORMAT_ITEM_TOOLTIP_VENDOR_VALUE_COUNTER", "{0}");
            public static readonly string FORMAT_ITEM_TOOLTIP_VENDOR_VALUE_COUNTER_MULTIPLE
                = create("FORMAT_ITEM_TOOLTIP_VENDOR_VALUE_COUNTER_MULTIPLE", "<color=grey>(x{0} each)</color> {1}");
            // Gamble
            public static readonly string TOOLTIP_GAMBLE_ITEM_NAME 
                = create("ITEM_TOOLTIP_GAMBLE_ITEM_NAME", "Mystery Item");
            public static readonly string TOOLTIP_GAMBLE_ITEM_RARITY 
                = create("ITEM_TOOLTIP_GAMBLE_ITEM_RARITY", "[Unknown]");
            public static readonly string TOOLTIP_GAMBLE_ITEM_DESC 
                = create("ITEM_TOOLTIP_GAMBLE_ITEM_DESCRIPTION", "You can't really see what this is until you buy it.");

            // Consumable
            public static readonly string TOOLTIP_CONSUMABLE_DESCRIPTION_HEALTH_APPLY
                = create("ITEM_TOOLTIP_CONSUMABLE_DESCRIPTION_HEALTH_APPLY", "Recovers {0} Health.");
            public static readonly string TOOLTIP_CONSUMABLE_DESCRIPTION_MANA_APPLY
                = create("ITEM_TOOLTIP_CONSUMABLE_DESCRIPTION_MANA_APPLY", "Recovers {0} Mana.");
            public static readonly string TOOLTIP_CONSUMABLE_DESCRIPTION_STAMINA_APPLY
                = create("ITEM_TOOLTIP_CONSUMABLE_DESCRIPTION_STAMINA_APPLY", "Recovers {0} Stamina.");
            public static readonly string TOOLTIP_CONSUMABLE_DESCRIPTION_EXP_GAIN
                = create("ITEM_TOOLTIP_CONSUMABLE_DESCRIPTION_EXP_GAIN", "Gain {0} Experience on use.");
        }
        
        internal static class Lore
        {
            internal static void init() { }
            public static readonly string CROWN = create("CROWN", "crown");
            public static readonly string CROWN_PLURAL = create("CROWN_PLURAL", "crowns");
            public static readonly string GAME_LOADING
				= create("GAME_LOADING", "Loading...");
            public static readonly string EXP_COUNTER_MAX
				= create("EXP_COUNTER_MAX", "MAX");
            public static readonly string COMBAT_ELEMENT_NORMAL_NAME
				= create("COMBAT_ELEMENT_NORMAL_NAME", "Normal");
        }

        internal static class Equipment
        {
            internal static void init() { }
            public static readonly string TOOLTIP_GAMBLE_ITEM_NAME
                = create("EQUIP_TOOLTIP_GAMBLE_ITEM_NAME", "Mystery Gear");

            public static readonly string TOOLTIP_GAMBLE_ITEM_RARITY
				= create("EQUIP_TOOLTIP_GAMBLE_ITEM_RARITY", "[Unknown]");
            public static readonly string TOOLTIP_GAMBLE_ITEM_TYPE
				= create("EQUIP_TOOLTIP_GAMBLE_ITEM_TYPE", "???");
            public static readonly string TOOLTIP_GAMBLE_ITEM_DESCRIPTION
				= create("EQUIP_TOOLTIP_GAMBLE_ITEM_DESCRIPTION", "You can't really see what this is until you buy it.");

            public static readonly string FORMAT_LEVEL_REQUIREMENT
				= create("FORMAT_EQUIP_LEVEL_REQUIREMENT", "Lv-{0}");
            public static readonly string FORMAT_CLASS_REQUIREMENT
				= create("FORMAT_EQUIP_CLASS_REQUIREMENT", "Class: {0}");
            public static readonly string FORMAT_WEAPON_CONDITION
				= create("FORMAT_EQUIP_WEAPON_CONDITION", "\n<color=lime>- <color=yellow>{0}%</color> chance to apply {1}.</color>");
            public static readonly string TOOLTIP_TYPE_HELM
				= create("EQUIP_TOOLTIP_TYPE_HELM", "Helm (Armor)");
            public static readonly string TOOLTIP_TYPE_CHESTPIECE
				= create("EQUIP_TOOLTIP_TYPE_CHESTPIECE", "Chestpiece (Armor)");
            public static readonly string TOOLTIP_TYPE_LEGGINGS
				= create("EQUIP_TOOLTIP_TYPE_LEGGINGS", "Leggings (Armor)");
            public static readonly string TOOLTIP_TYPE_CAPE
				= create("EQUIP_TOOLTIP_TYPE_CAPE", "Cape (Armor)");
            public static readonly string TOOLTIP_TYPE_RING
				= create("EQUIP_TOOLTIP_TYPE_RING", "Ring (Armor)");
            public static readonly string FORMAT_TOOLTIP_TYPE_WEAPON
				= create("FORMAT_EQUIP_TOOLTIP_TYPE_WEAPON", "{0} (Weapon)");
            public static readonly string TOOLTIP_TYPE_SHIELD
				= create("EQUIP_TOOLTIP_TYPE_SHIELD", "Shield");
            public static readonly string FORMAT_STATS_DAMAGE_SCALED
				= create("FORMAT_EQUIP_STATS_DAMAGE_SCALED", "<color=#c5e384>({0} - {1})</color> Damage");
            public static readonly string FORMAT_STATS_DAMAGE_SCALED_POWERFUL
				= create("FORMAT_EQUIP_STATS_DAMAGE_SCALED_POWERFUL", "<color=#efcc00>({0} - {1})</color> Damage");
            public static readonly string FORMAT_STATS_DAMAGE_COMPARE_BASE
				= create("FORMAT_EQUIP_STATS_DAMAGE_COMPARE_BASE", "\n<color=grey>(Base Damage: {0} - {1})</color>");
            public static readonly string FORMAT_STATS_DAMAGE_UNSCALED
				= create("FORMAT_EQUIP_STATS_DAMAGE_UNSCALED", "({0} - {1}) Damage");
            public static readonly string FORMAT_STATS_BLOCK_THRESHOLD
				= create("FORMAT_EQUIP_STATS_BLOCK_THRESHOLD", "Block threshold: {0} damage");
        }
    
        internal static class Quest
        {
            internal static void init() { }
            public static readonly string FORMAT_REQUIRED_LEVEL
				= create("FORMAT_QUEST_REQUIRED_LEVEL", "(lv-{0})");
            public static readonly string TYPE_CLASS
				= create("QUEST_TYPE_CLASS", "(Class Tome)");
            public static readonly string TYPE_MASTERY
				= create("QUEST_TYPE_MASTERY", "(Mastery Scroll)");
            public static readonly string MENU_SUMMARY_NO_QUESTS
				= create("QUEST_MENU_SUMMARY_NO_QUESTS", "No Quests in Quest Log.");
            public static readonly string MENU_HEADER_UNSELECTED
				= create("QUEST_MENU_HEADER_UNSELECTED", "Select a Quest.");
            public static readonly string FORMAT_MENU_CELL_LOG_COUNTER
				= create("FORMAT_QUEST_MENU_CELL_QUEST_LOG_COUNTER", "Quest Log: ({0} / {1})");
            public static readonly string FORMAT_MENU_CELL_FINISHED_COUNTER
				= create("FORMAT_QUEST_MENU_CELL_FINISHED_QUEST_COUNTER", "Completed Quests: {0}");
            public static readonly string FORMAT_MENU_CELL_REWARD_EXP
				= create("FORMAT_QUEST_MENU_CELL_REWARD_EXP", "{0} exp");
            public static readonly string FORMAT_MENU_CELL_REWARD_CURRENCY
				= create("FORMAT_QUEST_MENU_CELL_REWARD_CURRENCY", "{0} Crowns");
            public static readonly string MENU_CELL_SLOT_EMPTY
				= create("QUEST_MENU_CELL_SLOT_EMPTY", "Empty Slot");
            public static readonly string SELECTION_MANAGER_ACCEPT_BUTTON_ACCEPT
				= create("QUEST_SELECTION_MANAGER_QUEST_ACCEPT_BUTTON_ACCEPT", "Accept Quest");
            public static readonly string SELECTION_MANAGER_ACCEPT_BUTTON_LOCKED
				= create("QUEST_SELECTION_MANAGER_QUEST_ACCEPT_BUTTON_LOCKED", "Quest Locked");
            public static readonly string SELECTION_MANAGER_ACCEPT_BUTTON_INCOMPLETE
				= create("QUEST_SELECTION_MANAGER_QUEST_ACCEPT_BUTTON_INCOMPLETE", "Quest Incomplete");
            public static readonly string SELECTION_MANAGER_ACCEPT_BUTTON_TURN_IN
				= create("QUEST_SELECTION_MANAGER_QUEST_ACCEPT_BUTTON_TURN_IN", "Complete Quest");
            public static readonly string SELECTION_MANAGER_ACCEPT_BUTTON_UNSELECTED
				= create("QUEST_SELECTION_MANAGER_QUEST_ACCEPT_BUTTON_UNSELECTED", "Select a Quest");
            public static readonly string FORMAT_PROGRESS
				= create("FORMAT_QUEST_PROGRESS", "{0}: ({1} / {2})");
            public static readonly string FORMAT_PROGRESS_CREEPS_KILLED
				= create("FORMAT_QUEST_PROGRESS_CREEPS_KILLED", "{0} slain");
        }
    
    
        internal static class MainMenu
        {
            internal static void init() { }
            public static readonly string BUTTON_SINGLEPLAY
				= create("MAIN_MENU_BUTTON_SINGLEPLAY", "Singleplayer");
            public static readonly string BUTTON_SINGLEPLAY_TOOLTIP
				= create("MAIN_MENU_BUTTON_SINGLEPLAY_TOOLTIP", "Start a Singleplayer Game.");

            public static readonly string BUTTON_MULTIPLAY
				= create("MAIN_MENU_BUTTON_MULTIPLAY", "Multiplayer");
            public static readonly string BUTTON_MULTIPLAY_TOOLTIP
				= create("MAIN_MENU_BUTTON_MULTIPLAY_TOOLTIP", "Start a Netplay Game.");
            public static readonly string BUTTON_MULTIPLAY_DISABLED_TOOLTIP
				= create("MAIN_MENU_BUTTON_MULTIPLAY_DISABLED_TOOLTIP", "Multiplayer is disabled on this demo.");

            public static readonly string BUTTON_SETTINGS
				= create("MAIN_MENU_BUTTON_SETTINGS", "Settings");
            public static readonly string BUTTON_SETTINGS_TOOLTIP
				= create("MAIN_MENU_BUTTON_SETTINGS_TOOLTIP", "Configure Game Settings.");

            public static readonly string BUTTON_QUIT
				= create("MAIN_MENU_BUTTON_QUIT", "Quit");
            public static readonly string BUTTON_QUIT_TOOLTIP
				= create("MAIN_MENU_BUTTON_QUIT_TOOLTIP", "End The Application.");
        }

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

        internal static class CharacterCreation
        {
            internal static void init() { }
            public static readonly string HEADER
				= create("CHARACTER_CREATION_HEADER", "Character Creation");
            public static readonly string HEADER_RACE_NAME
				= create("CHARACTER_CREATION_HEADER_RACE_NAME", "Race Select");
            public static readonly string RACE_DESCRIPTOR_HEADER_INITIAL_SKILL
				= create("CHARACTER_CREATION_RACE_DESCRIPTOR_HEADER_INITIAL_SKILL", "Initial Skill");
            public static readonly string BUTTON_SET_TO_DEFAULTS
				= create("CHARACTER_CREATION_BUTTON_SET_TO_DEFAULTS", "Defaults");
            public static readonly string CHARACTER_NAME_PLACEHOLDER_TEXT
				= create("CHARACTER_CREATION_CHARACTER_NAME_PLACEHOLDER_TEXT", "Enter Name...");
            public static readonly string BUTTON_CREATE_CHARACTER
				= create("CHARACTER_CREATION_BUTTON_CREATE_CHARACTER", "Create Character");
            public static readonly string BUTTON_RETURN
				= create("CHARACTER_CREATION_BUTTON_RETURN", "Return");

            public static readonly string CUSTOMIZER_HEADER_COLOR
				= create("CHARACTER_CREATION_CUSTOMIZER_HEADER_COLOR", "Color");
            public static readonly string CUSTOMIZER_COLOR_BODY_HEADER
				= create("CHARACTER_CREATION_CUSTOMIZER_COLOR_BODY_HEADER", "Body");
            public static readonly string CUSTOMIZER_COLOR_BODY_TEXTURE
				= create("CHARACTER_CREATION_CUSTOMIZER_COLOR_BODY_TEXTURE", "Texture");
            public static readonly string CUSTOMIZER_COLOR_HAIR_HEADER
				= create("CHARACTER_CREATION_CUSTOMIZER_COLOR_HAIR_HEADER", "Hair");
            public static readonly string CUSTOMIZER_COLOR_HAIR_LOCK_COLOR
				= create("CHARACTER_CREATION_CUSTOMIZER_COLOR_HAIR_LOCK_COLOR", "Lock Color");

            public static readonly string CUSTOMIZER_HEADER_HEAD
				= create("CHARACTER_CREATION_CUSTOMIZER_HEADER_HEAD", "Head");
            public static readonly string CUSTOMIZER_HEAD_HEAD_WIDTH
				= create("CHARACTER_CREATION_CUSTOMIZER_HEAD_HEAD_WIDTH", "Head Width");
            public static readonly string CUSTOMIZER_HEAD_HEAD_MOD
				= create("CHARACTER_CREATION_CUSTOMIZER_HEAD_HEAD_MOD", "Modify");
            public static readonly string CUSTOMIZER_HEAD_VOICE_PITCH
				= create("CHARACTER_CREATION_CUSTOMIZER_HEAD_VOICE_PITCH", "Voice Pitch");
            public static readonly string CUSTOMIZER_HEAD_HAIR_STYLE
				= create("CHARACTER_CREATION_CUSTOMIZER_HEAD_HAIR_STYLE", "Hair");
            public static readonly string CUSTOMIZER_HEAD_EARS
				= create("CHARACTER_CREATION_CUSTOMIZER_HEAD_EARS", "Ears");
            public static readonly string CUSTOMIZER_HEAD_EYES
				= create("CHARACTER_CREATION_CUSTOMIZER_HEAD_EYES", "Eyes");
            public static readonly string CUSTOMIZER_HEAD_MOUTH
				= create("CHARACTER_CREATION_CUSTOMIZER_HEAD_MOUTH", "Mouth");

            public static readonly string CUSTOMIZER_HEADER_BODY
				= create("CHARACTER_CREATION_CUSTOMIZER_HEADER_BODY", "Body");
            public static readonly string CUSTOMIZER_BODY_HEIGHT
				= create("CHARACTER_CREATION_CUSTOMIZER_BODY_HEIGHT", "Height");
            public static readonly string CUSTOMIZER_BODY_WIDTH
				= create("CHARACTER_CREATION_CUSTOMIZER_BODY_WIDTH", "Width");
            public static readonly string CUSTOMIZER_BODY_CHEST
				= create("CHARACTER_CREATION_CUSTOMIZER_BODY_CHEST", "Chest");
            public static readonly string CUSTOMIZER_BODY_ARMS
				= create("CHARACTER_CREATION_CUSTOMIZER_BODY_ARMS", "Arms");
            public static readonly string CUSTOMIZER_BODY_BELLY
				= create("CHARACTER_CREATION_CUSTOMIZER_BODY_BELLY", "Belly");
            public static readonly string CUSTOMIZER_BODY_BOTTOM
				= create("CHARACTER_CREATION_CUSTOMIZER_BODY_BOTTOM", "Bottom");
            public static readonly string CUSTOMIZER_BODY_TAIL
				= create("CHARACTER_CREATION_CUSTOMIZER_BODY_TAIL", "Tail");
            public static readonly string CUSTOMIZER_BODY_TOGGLE_LEFT_HANDED
				= create("CHARACTER_CREATION_CUSTOMIZER_BODY_TOGGLE_LEFT_HANDED", "Mirror Body");

            public static readonly string CUSTOMIZER_HEADER_TRAIT
				= create("CHARACTER_CREATION_CUSTOMIZER_HEADER_TRAIT", "Trait");
            public static readonly string CUSTOMIZER_TRAIT_EQUIPMENT
				= create("CHARACTER_CREATION_CUSTOMIZER_TRAIT_EQUIPMENT", "Equipment");
            public static readonly string CUSTOMIZER_TRAIT_WEAPON_LOADOUT
				= create("CHARACTER_CREATION_CUSTOMIZER_TRAIT_WEAPON_LOADOUT", "Weapon");
            public static readonly string CUSTOMIZER_TRAIT_GEAR_DYE
				= create("CHARACTER_CREATION_CUSTOMIZER_TRAIT_GEAR_DYE", "Dye");
            public static readonly string CUSTOMIZER_TRAIT_ATTRIBUTES
				= create("CHARACTER_CREATION_CUSTOMIZER_TRAIT_ATTRIBUTES", "Attributes");
            public static readonly string CUSTOMIZER_TRAIT_RESET_ATTRIBUTE_POINTS
				= create("CHARACTER_CREATION_CUSTOMIZER_TRAIT_RESET_ATTRIBUTE_POINTS", "Reset Points");
        }

        internal static class TabMenu
        {
            internal static void init() { }
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


            public static readonly string CELL_ITEMS_HEADER
                = create("TAB_MENU_CELL_ITEMS_HEADER", "Items");
            public static readonly string CELL_ITEMS_EQUIP_TAB_HEADER_EQUIPMENT
                = create("TAB_MENU_CELL_ITEMS_EQUIP_TAB_HEADER_EQUIPMENT", "Equipment");
            public static readonly string CELL_ITEMS_EQUIP_TAB_HEADER_VANITY
                = create("TAB_MENU_CELL_ITEMS_EQUIP_TAB_HEADER_VANITY", "Vanity");
            public static readonly string CELL_ITEMS_EQUIP_TAB_HEADER_STAT
                = create("TAB_MENU_CELL_ITEMS_EQUIP_TAB_HEADER_STAT", "Stats");

            public static readonly string CELL_QUESTS_HEADER
                = create("TAB_MENU_CELL_QUESTS_HEADER", "Quests");



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
        
        internal static class SkillMenu
        {
            internal static void init() { }

            public static readonly string RANK_SOULBOUND
				= create("SKILL_RANK_SOULBOUND", "Soulbound Skill");
            public static readonly string RANK
				= create("FORMAT_SKILL_RANK", "[Rank {0} / {1}]");
            public static readonly string TOOLTIP_DAMAGE_TYPE
				= create("FORMAT_SKILL_TOOLTIP_DAMAGE_TYPE", "{0} Skill");
            public static readonly string TOOLTIP_ITEM_COST
				= create("FORMAT_SKILL_TOOLTIP_ITEM_COST", "x{0} {1}");
            public static readonly string TOOLTIP_MANA_COST
				= create("FORMAT_SKILL_TOOLTIP_MANA_COST", "{0} Mana");
            public static readonly string TOOLTIP_HEALTH_COST
				= create("FORMAT_SKILL_TOOLTIP_HEALTH_COST", "{0} Health");
            public static readonly string TOOLTIP_STAMINA_COST
				= create("FORMAT_SKILL_TOOLTIP_STAMINA_COST", "{0} Stamina");
            public static readonly string TOOLTIP_CAST_TIME_INSTANT
				= create("SKILL_TOOLTIP_CAST_TIME_INSTANT", "Instant Cast");
            public static readonly string TOOLTIP_CAST_TIME
				= create("FORMAT_SKILL_TOOLTIP_CAST_TIME", "{0} sec Cast");
            public static readonly string TOOLTIP_COOLDOWN
				= create("FORMAT_SKILL_TOOLTIP_COOLDOWN", "{0} sec Cooldown");
            public static readonly string TOOLTIP_PASSIVE
				= create("SKILL_TOOLTIP_PASSIVE", "Passive Skill");

            public static readonly string TOOLTIP_RANK_DESCRIPTOR_NEXT_RANK
				= create("SKILL_TOOLTIP_RANK_DESCRIPTOR_NEXT_RANK", "\n<color=white><i>[Next Rank]</i></color>");
            public static readonly string TOOLTIP_RANK_DESCRIPTOR_CURRENT_RANK
				= create("FORMAT_SKILL_TOOLTIP_RANK_DESCRIPTOR_CURRENT_RANK", "\n<color=white><i>[Rank {0}]</i></color>");
            public static readonly string TOOLTIP_RANK_DESCRIPTOR_REQUIRED_LEVEL
				= create("FORMAT_SKILL_TOOLTIP_RANK_DESCRIPTOR_REQUIRED_LEVEL", "<color=red>\n(Requires Lv. {0})</color>");
            public static readonly string TOOLTIP_RANK_DESCRIPTOR_COOLDOWN
				= create("FORMAT_SKILL_TOOLTIP_RANK_DESCRIPTOR_COOLDOWN", "<color=yellow>{0} sec cooldown.</color>");
            public static readonly string TOOLTIP_RANK_DESCRIPTOR_CAST_TIME
				= create("FORMAT_SKILL_TOOLTIP_RANK_DESCRIPTOR_CAST_TIME", "<color=yellow>{0} sec cast time.</color>");
            public static readonly string TOOLTIP_RANK_DESCRIPTOR_CAST_TIME_INSTANT
				= create("SKILL_TOOLTIP_RANK_DESCRIPTOR_CAST_TIME_INSTANT", "<color=yellow>instant cast time.</color>");

            public static readonly string TOOLTIP_RANK_DESCRIPTOR_CONDITION_CANCEL_ON_HIT
				= create("SKILL_TOOLTIP_RANK_DESCRIPTOR_CONDITION_CANCEL_ON_HIT", " <color=yellow>Cancels if hit.</color>");
            public static readonly string TOOLTIP_RANK_DESCRIPTOR_CONDITION_IS_PERMANENT
				= create("SKILL_TOOLTIP_RANK_DESCRIPTOR_CONDITION_IS_PERMANENT", " <color=yellow>Permanent.</color>");
            public static readonly string TOOLTIP_RANK_DESCRIPTOR_CONDITION_DURATION
				= create("FORMAT_SKILL_TOOLTIP_RANK_DESCRIPTOR_CONDITION_DURATION", " <color=yellow>Lasts for {0} seconds.</color>");
            public static readonly string TOOLTIP_RANK_DESCRIPTOR_CONDITION_IS_STACKABLE
				= create("SKILL_TOOLTIP_RANK_DESCRIPTOR_CONDITION_IS_STACKABLE", " <color=yellow>Stackable.</color>");
            public static readonly string TOOLTIP_RANK_DESCRIPTOR_CONDITION_IS_REFRESHABLE
				= create("SKILL_TOOLTIP_RANK_DESCRIPTOR_CONDITION_IS_REFRESHABLE", " <color=yellow>Refreshes when re-applied.</color>");
        }


      

        internal static class Settings
        {
            internal static void init() 
            {
                Video.init();
                Audio.init();
                Input.init();
                Network.init();
            }
            public static readonly string BUTTON_VIDEO
				= create("SETTINGS_TAB_BUTTON_VIDEO", "Display");
            public static readonly string BUTTON_AUDIO
				= create("SETTINGS_TAB_BUTTON_AUDIO", "Audio");
            public static readonly string BUTTON_INPUT
				= create("SETTINGS_TAB_BUTTON_INPUT", "Input");
            public static readonly string BUTTON_NETWORK
				= create("SETTINGS_TAB_BUTTON_NETWORK", "Interface");

            public static readonly string BUTTON_RESET_TO_DEFAULTS
				= create("SETTINGS_BUTTON_RESET_TO_DEFAULTS", "Reset to Defaults");
            public static readonly string BUTTON_RESET
				= create("SETTINGS_BUTTON_RESET", "Reset");
            public static readonly string BUTTON_CANCEL
				= create("SETTINGS_BUTTON_CANCEL", "Cancel");
            public static readonly string BUTTON_APPLY
				= create("SETTINGS_BUTTON_APPLY", "Apply");


            internal static class Video
            {
                internal static void init() { }
                public static readonly string HEADER_GAME_EFFECT_SETTINGS
				    = create("SETTINGS_VIDEO_HEADER_GAME_EFFECT_SETTINGS", "Display Sensitive Settings");
                public static readonly string CELL_PROPORTIONS_TOGGLE
				    = create("SETTINGS_VIDEO_CELL_PROPORTIONS_TOGGLE", "Limit Player Character Proportions");
                public static readonly string CELL_JIGGLE_BONES_TOGGLE
				    = create("SETTINGS_VIDEO_CELL_JIGGLE_BONES_TOGGLE", "Disable Suggestive Jiggle Bones");
                public static readonly string CELL_CLEAR_UNDERCLOTHES_TOGGLE
				    = create("SETTINGS_VIDEO_CELL_CLEAR_UNDERCLOTHES_TOGGLE", "Enable Clear Clothing");

                public static readonly string HEADER_VIDEO_SETTINGS
				    = create("SETTINGS_VIDEO_HEADER_VIDEO_SETTINGS", "Video Settings");
                public static readonly string CELL_FULLSCREEN_TOGGLE
				    = create("SETTINGS_VIDEO_CELL_FULLSCREEN_TOGGLE", "Fullscreen Mode");
                public static readonly string CELL_VERTICAL_SYNC
				    = create("SETTINGS_VIDEO_CELL_VERTICAL_SYNC", "Vertical Sync / Lock 60 FPS");
                public static readonly string CELL_ANISOTROPIC_FILTERING
				    = create("SETTINGS_VIDEO_CELL_ANISOTROPIC_FILTERING", "Anisotropic Filtering");
                public static readonly string CELL_SCREEN_RESOLUTION
				    = create("SETTINGS_VIDEO_CELL_SCREEN_RESOLUTION", "Screen Resolution");
                public static readonly string CELL_ANTI_ALIASING
				    = create("SETTINGS_VIDEO_CELL_ANTI_ALIASING", "Anti Aliasing");
                public static readonly string CELL_ANTI_ALIASING_OPTION_1
				    = create("SETTINGS_VIDEO_CELL_ANTI_ALIASING_OPTION_1", "Disabled");
                public static readonly string CELL_ANTI_ALIASING_OPTION_2
				    = create("SETTINGS_VIDEO_CELL_ANTI_ALIASING_OPTION_2", "2x Multi Sampling");
                public static readonly string CELL_ANTI_ALIASING_OPTION_3
				    = create("SETTINGS_VIDEO_CELL_ANTI_ALIASING_OPTION_3", "4x Multi Sampling");
                public static readonly string CELL_ANTI_ALIASING_OPTION_4
				    = create("SETTINGS_VIDEO_CELL_ANTI_ALIASING_OPTION_4", "8x Multi Sampling");
                public static readonly string CELL_TEXTURE_FILTERING
				    = create("SETTINGS_VIDEO_CELL_TEXTURE_FILTERING", "Texture Filtering");
                public static readonly string CELL_TEXTURE_FILTERING_OPTION_1
				    = create("SETTINGS_VIDEO_CELL_TEXTURE_FILTERING_OPTION_1", "Bilnear (Smooth)");
                public static readonly string CELL_TEXTURE_FILTERING_OPTION_2
				    = create("SETTINGS_VIDEO_CELL_TEXTURE_FILTERING_OPTION_2", "Nearest (Crunchy)");
                public static readonly string CELL_TEXTURE_QUALITY
				    = create("SETTINGS_VIDEO_CELL_TEXTURE_QUALITY", "Texture Quality");
                public static readonly string CELL_TEXTURE_QUALITY_OPTION_1
				    = create("SETTINGS_VIDEO_CELL_TEXTURE_QUALITY_OPTION_1", "High");
                public static readonly string CELL_TEXTURE_QUALITY_OPTION_2
				    = create("SETTINGS_VIDEO_CELL_TEXTURE_QUALITY_OPTION_2", "Medium");
                public static readonly string CELL_TEXTURE_QUALITY_OPTION_3
				    = create("SETTINGS_VIDEO_CELL_TEXTURE_QUALITY_OPTION_3", "Low");
                public static readonly string CELL_TEXTURE_QUALITY_OPTION_4
				    = create("SETTINGS_VIDEO_CELL_TEXTURE_QUALITY_OPTION_4", "Very Low");

                public static readonly string HEADER_CAMERA_SETTINGS
				    = create("SETTINGS_VIDEO_HEADER_CAMERA_SETTINGS", "Camera Display Settings");
                public static readonly string CELL_FIELD_OF_VIEW
				    = create("SETTINGS_VIDEO_CELL_FIELD_OF_VIEW", "Field Of View");
                public static readonly string CELL_CAMERA_SMOOTHING
				    = create("SETTINGS_VIDEO_CELL_CAMERA_SMOOTHING", "Camera Smoothing");
                public static readonly string CELL_CAMERA_HORIZ
				    = create("SETTINGS_VIDEO_CELL_CAMERA_HORIZ", "Camera X Position");
                public static readonly string CELL_CAMERA_VERT
				    = create("SETTINGS_VIDEO_CELL_CAMERA_VERT", "Camera Y Position");
                public static readonly string CELL_CAMERA_RENDER_DISTANCE
				    = create("SETTINGS_VIDEO_CELL_CAMERA_RENDER_DISTANCE", "Render Distance");
                public static readonly string CELL_CAMERA_RENDER_DISTANCE_OPTION_1
				    = create("SETTINGS_VIDEO_CELL_CAMERA_RENDER_DISTANCE_OPTION_1", "Very Near");
                public static readonly string CELL_CAMERA_RENDER_DISTANCE_OPTION_2
				    = create("SETTINGS_VIDEO_CELL_CAMERA_RENDER_DISTANCE_OPTION_2", "Near");
                public static readonly string CELL_CAMERA_RENDER_DISTANCE_OPTION_3
				    = create("SETTINGS_VIDEO_CELL_CAMERA_RENDER_DISTANCE_OPTION_3", "Far");
                public static readonly string CELL_CAMERA_RENDER_DISTANCE_OPTION_4
				    = create("SETTINGS_VIDEO_CELL_CAMERA_RENDER_DISTANCE_OPTION_4", "Very Far");

                public static readonly string HEADER_POST_PROCESSING
				    = create("SETTINGS_VIDEO_HEADER_POST_PROCESSING", "Post Processing");
                public static readonly string CELL_CAMERA_BITCRUSH_SHADER
				    = create("SETTINGS_VIDEO_CELL_CAMERA_BITCRUSH_SHADER", "Enable Bitcrush Shader");
                public static readonly string CELL_CAMERA_WATER_EFFECT
				    = create("SETTINGS_VIDEO_CELL_CAMERA_WATER_EFFECT", "Enable Underwater Distortion Shader");
                public static readonly string CELL_CAMERA_SHAKE
				    = create("SETTINGS_VIDEO_CELL_CAMERA_SHAKE", "Enable Screen Shake");
                public static readonly string CELL_WEAPON_GLOW
				    = create("SETTINGS_VIDEO_CELL_WEAPON_GLOW", "Disable Weapon Glow Effect");
            }

            internal static class Audio
            {
                internal static void init() { }
                public static readonly string HEADER_AUDIO_SETTINGS
				    = create("SETTINGS_AUDIO_HEADER_AUDIO_SETTINGS", "Audio Settings");
                public static readonly string CELL_MASTER_VOLUME
				    = create("SETTINGS_AUDIO_CELL_MASTER_VOLUME", "Master Volume");
                public static readonly string CELL_MUTE_APPLICATION
				    = create("SETTINGS_AUDIO_CELL_MUTE_APPLICATION", "Mute Application");
                public static readonly string CELL_MUTE_MUSIC
				    = create("SETTINGS_AUDIO_CELL_MUTE_MUSIC", "Mute Music");

                public static readonly string HEADER_AUDIO_CHANNEL_SETTINGS
				    = create("SETTINGS_AUDIO_HEADER_AUDIO_CHANNEL_SETTINGS", "Audio Channels");
                public static readonly string CELL_GAME_VOLUME
				    = create("SETTINGS_AUDIO_CELL_GAME_VOLUME", "Game Volume");
                public static readonly string CELL_GUI_VOLUME
				    = create("SETTINGS_AUDIO_CELL_GUI_VOLUME", "GUI Volume");
                public static readonly string CELL_AMBIENCE_VOLUME
				    = create("SETTINGS_AUDIO_CELL_AMBIENCE_VOLUME", "Ambience Volume");
                public static readonly string CELL_MUSIC_VOLUME
				    = create("SETTINGS_AUDIO_CELL_MUSIC_VOLUME", "Music Volume");
                public static readonly string CELL_VOICE_VOLUME
				    = create("SETTINGS_AUDIO_CELL_VOICE_VOLUME", "Voice Volume");
            }

            internal static class Input
            {
                internal static void init() { }
                public static readonly string HEADER_INPUT_SETTINGS
				    = create("SETTINGS_INPUT_HEADER_INPUT_SETTINGS", "Input Settings");
                public static readonly string CELL_AXIS_TYPE
				    = create("SETTINGS_INPUT_CELL_AXIS_TYPE", "Analog Stick Axis Type");
                public static readonly string CELL_AXIS_TYPE_OPTION_1
				    = create("SETTINGS_INPUT_CELL_AXIS_TYPE_OPTION_1", "WASD (8 Directional)");
                public static readonly string CELL_AXIS_TYPE_OPTION_2
				    = create("SETTINGS_INPUT_CELL_AXIS_TYPE_OPTION_2", "Xbox");
                public static readonly string CELL_AXIS_TYPE_OPTION_3
				    = create("SETTINGS_INPUT_CELL_AXIS_TYPE_OPTION_3", "Playstation 4");

                public static readonly string HEADER_CAMERA_CONTROL
				    = create("SETTINGS_INPUT_HEADER_CAMERA_CONTROL", "Camera Control");
                public static readonly string CELL_CAMERA_SENSITIVITY
				    = create("SETTINGS_INPUT_CELL_CAMERA_SENSITIVITY", "Axis Sensitivity");
                public static readonly string CELL_INVERT_X_CAMERA_AXIS
				    = create("SETTINGS_INPUT_CELL_INVERT_X_CAMERA_AXIS", "Invert X Axis");
                public static readonly string CELL_INVERT_Y_CAMERA_AXIS
				    = create("SETTINGS_INPUT_CELL_INVERT_Y_CAMERA_AXIS", "Invert Y Axis");
                public static readonly string CELL_KEYBINDING_RESET_CAMERA
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_RESET_CAMERA", "Reset Camera");

                public static readonly string HEADER_MOVEMENT
				    = create("SETTINGS_INPUT_HEADER_MOVEMENT", "Movement");
                public static readonly string CELL_KEYBINDING_UP
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_UP", "Up");
                public static readonly string CELL_KEYBINDING_DOWN
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_DOWN", "Down");
                public static readonly string CELL_KEYBINDING_LEFT
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_LEFT", "Left");
                public static readonly string CELL_KEYBINDING_RIGHT
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_RIGHT", "Right");
                public static readonly string CELL_KEYBINDING_JUMP
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_JUMP", "Jump");
                public static readonly string CELL_KEYBINDING_DASH
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_DASH", "Dash");

                public static readonly string HEADER_STRAFING
				    = create("SETTINGS_INPUT_HEADER_STRAFING", "Strafing");
                public static readonly string CELL_KEYBINDING_LOCK_DIRECTION
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_LOCK_DIRECTION", "Strafe");
                public static readonly string CELL_KEYBINDING_STRAFE_MODE
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_STRAFE_MODE", "Strafe / Aim Mode");
                public static readonly string CELL_KEYBINDING_STRAFE_MODE_OPTION_1
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_STRAFE_MODE_OPTION_1", "Hold Strafe Key");
                public static readonly string CELL_KEYBINDING_STRAFE_MODE_OPTION_2
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_STRAFE_MODE_OPTION_2", "Toggle Strafe Key");
                public static readonly string CELL_KEYBINDING_STRAFE_WEAPON
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_STRAFE_WEAPON", "Strafe While Holding Weapon");
                public static readonly string CELL_KEYBINDING_STRAFE_CASTING
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_STRAFE_CASTING", "Strafe While Casting Offensive Skills");

                public static readonly string HEADER_ACTION
				    = create("SETTINGS_INPUT_HEADER_ACTION", "Action");
                public static readonly string CELL_KEYBINDING_ATTACK
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_ATTACK", "Attack");
                public static readonly string CELL_KEYBINDING_CHARGE_ATTACK
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_CHARGE_ATTACK", "Charge Attack");
                public static readonly string CELL_KEYBINDING_BLOCK
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_BLOCK", "Block");
                public static readonly string CELL_KEYBINDING_TARGET
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_TARGET", "Lock On");
                public static readonly string CELL_KEYBINDING_INTERACT
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_INTERACT", "Interact");
                public static readonly string CELL_KEYBINDING_PVP_FLAG
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_PVP_FLAG", "PvP Flag Toggle");
                public static readonly string CELL_KEYBINDING_SKILL_SLOT_01
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_SKILL_SLOT_01", "Skill Slot 1");
                public static readonly string CELL_KEYBINDING_SKILL_SLOT_02
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_SKILL_SLOT_02", "Skill Slot 2");
                public static readonly string CELL_KEYBINDING_SKILL_SLOT_03
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_SKILL_SLOT_03", "Skill Slot 3");
                public static readonly string CELL_KEYBINDING_SKILL_SLOT_04
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_SKILL_SLOT_04", "Skill Slot 4");
                public static readonly string CELL_KEYBINDING_SKILL_SLOT_05
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_SKILL_SLOT_05", "Skill Slot 5");
                public static readonly string CELL_KEYBINDING_SKILL_SLOT_06
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_SKILL_SLOT_06", "Skill Slot 6");
                public static readonly string CELL_KEYBINDING_RECALL
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_RECALL", "Recall");
                public static readonly string CELL_KEYBINDING_QUICKSWAP_WEAPON
			    	= create("SETTINGS_INPUT_CELL_KEYBINDING_QUICKSWAP_WEAPON", "Quickswap Weapon");
                public static readonly string CELL_KEYBINDING_SHEATHE_WEAPON
		    		= create("SETTINGS_INPUT_CELL_KEYBINDING_SHEATHE_WEAPON", "Sheathe / Unsheathe Weapon");
                public static readonly string CELL_KEYBINDING_SIT
	    			= create("SETTINGS_INPUT_CELL_KEYBINDING_SIT", "Sit");

                public static readonly string HEADER_CONSUMABLE_SLOTS
				    = create("SETTINGS_INPUT_HEADER_CONSUMABLE_SLOTS", "Consumable Quick Slots");
                public static readonly string CELL_KEYBINDING_QUICK_SLOT_01
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_QUICK_SLOT_01", "Quick Slot 1");
                public static readonly string CELL_KEYBINDING_QUICK_SLOT_02
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_QUICK_SLOT_02", "Quick Slot 2");
                public static readonly string CELL_KEYBINDING_QUICK_SLOT_03
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_QUICK_SLOT_03", "Quick Slot 3");
                public static readonly string CELL_KEYBINDING_QUICK_SLOT_04
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_QUICK_SLOT_04", "Quick Slot 4");
                public static readonly string CELL_KEYBINDING_QUICK_SLOT_05
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_QUICK_SLOT_05", "Quick Slot 5");

                public static readonly string HEADER_INTERFACE
				    = create("SETTINGS_INPUT_HEADER_INTERFACE", "Interface");
                public static readonly string CELL_KEYBINDING_HOST_CONSOLE
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_HOST_CONSOLE", "Host Console");
                public static readonly string CELL_KEYBINDING_LEXICON
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_LEXICON", "Open Lexicon");
                public static readonly string CELL_KEYBINDING_TAB_MENU
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_TAB_MENU", "Open Tab Menu");
                public static readonly string CELL_KEYBINDING_STATS_TAB
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_STATS_TAB", "Stats Tab");
                public static readonly string CELL_KEYBINDING_SKILLS_TAB
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_SKILLS_TAB", "Skills Tab");
                public static readonly string CELL_KEYBINDING_ITEM_TAB
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_ITEM_TAB", "Item Tab");
                public static readonly string CELL_KEYBINDING_QUEST_TAB
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_QUEST_TAB", "Quest Tab");
                public static readonly string CELL_KEYBINDING_WHO_TAB
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_WHO_TAB", "Who Tab");
                public static readonly string CELL_KEYBINDING_HIDE_UI
				    = create("SETTINGS_INPUT_CELL_KEYBINDING_HIDE_UI", "Hide Game UI");
            }

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
