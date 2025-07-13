
namespace Localyssation
{
    internal static partial class I18nKeys
    {

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

    }
}