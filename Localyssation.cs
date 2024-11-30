using BepInEx;
using HarmonyLib;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Security.Permissions;
using UnityEngine;
using System.Linq;

#pragma warning disable CS0618

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace Localyssation
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class Localyssation : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "com.themysticsword.localyssation";
        public const string PLUGIN_NAME = "Localyssation";
        public const string PLUGIN_VERSION = "0.0.1";

        public static Localyssation instance;

        public static Language defaultLanguage;
        public static Language currentLanguage;
        public static Dictionary<string, Language> languages = new Dictionary<string, Language>();
        public static readonly List<Language> languagesList = new List<Language>();

        public event System.Action<Language> onLanguageChanged;
        internal void CallOnLanguageChanged(Language newLanguage) { if (onLanguageChanged != null) onLanguageChanged.Invoke(newLanguage); }

        internal static BepInEx.Logging.ManualLogSource logger;
        internal static BepInEx.Configuration.ConfigFile config;

        internal static BepInEx.Configuration.ConfigEntry<string> configLanguage;
        internal static BepInEx.Configuration.ConfigEntry<bool> configWriteDefaultLangToFile;

        private void Awake()
        {
            instance = this;
            logger = Logger;
            config = Config;

            defaultLanguage = CreateDefaultLanguage();
            RegisterLanguage(defaultLanguage);
            ChangeLanguage(defaultLanguage);
            LoadInstalledLanguageFiles();

            configLanguage = config.Bind("General", "Language", defaultLanguage.code, "Currently selected language's code");
            if (languages.TryGetValue(configLanguage.Value, out var previouslySelectedLanguage))
                ChangeLanguage(previouslySelectedLanguage);

            configWriteDefaultLangToFile = config.Bind("Developers", "Write Default Language File", false, "If true, the default game language's strings will be written to a file in LocalLow\\KisSoft\\ATLYSS\\Localyssation");

            Harmony harmony = new Harmony(PLUGIN_GUID);
            harmony.PatchAll(typeof(Patches.GameLoadPatches));
            harmony.PatchAll(typeof(Patches.ReplaceTextPatches));
            harmony.PatchAll(typeof(Patches.CreateUIPatches));
            OnSceneLoaded.Init();
            LangAdjustables.Init();
        }

        public static void LoadInstalledLanguageFiles()
        {
            var filePaths = Directory.GetFiles(Paths.PluginPath, "*.language", SearchOption.AllDirectories);
            foreach (var filePath in filePaths)
            {
                var fileText = File.ReadAllText(filePath);
                var language = JsonConvert.DeserializeObject<Language>(fileText);
                RegisterLanguage(language);
            }
        }

        public static void RegisterLanguage(Language language)
        {
            if (languages.TryGetValue(language.code, out var existingLanguage))
            {
                existingLanguage.name = language.name;
                existingLanguage.strings = language.strings;
                return;
            }
            languages[language.code] = language;
            languagesList.Add(language);
        }

        public static void ChangeLanguage(Language newLanguage)
        {
            if (currentLanguage == newLanguage) return;

            currentLanguage = newLanguage;
            instance.CallOnLanguageChanged(newLanguage);
        }

        internal static Language CreateDefaultLanguage()
        {
            var language = new Language();
            language.code = "en-US";
            language.name = "English (US)";

            language.strings = new Dictionary<string, string>()
            {
                // general
                { "GAME_LOADING", "Loading..." },

                // main menu
                { "MAIN_MENU_BUTTON_SINGLEPLAY", "Singleplayer" },
                { "MAIN_MENU_BUTTON_SINGLEPLAY_TOOLTIP", "Start a Singleplayer Game." },

                { "MAIN_MENU_BUTTON_MULTIPLAY", "Multiplayer" },
                { "MAIN_MENU_BUTTON_MULTIPLAY_TOOLTIP", "Start a Netplay Game." },
                { "MAIN_MENU_BUTTON_MULTIPLAY_DISABLED_TOOLTIP", "Multiplayer is disabled on this demo." },

                { "MAIN_MENU_BUTTON_SETTINGS", "Settings" },
                { "MAIN_MENU_BUTTON_SETTINGS_TOOLTIP", "Configure Game Settings." },

                { "MAIN_MENU_BUTTON_QUIT", "Quit" },
                { "MAIN_MENU_BUTTON_QUIT_TOOLTIP", "End The Application." },

                // character select
                { "CHARACTER_SELECT_HEADER", "Character Select" },
                { "CHARACTER_SELECT_HEADER_GAME_MODE_SINGLEPLAYER", "Singleplayer" },
                { "CHARACTER_SELECT_HEADER_GAME_MODE_HOST_MULTIPLAYER_PUBLIC", "Host Game (Public)" },
                { "CHARACTER_SELECT_HEADER_GAME_MODE_HOST_MULTIPLAYER_FRIENDS", "Host Game (Friends)" },
                { "CHARACTER_SELECT_HEADER_GAME_MODE_HOST_MULTIPLAYER_PRIVATE", "Host Game (Private)" },
                { "CHARACTER_SELECT_HEADER_GAME_MODE_JOIN_MULTIPLAYER", "Join Game" },
                { "CHARACTER_SELECT_HEADER_GAME_MODE_LOBBY_QUERY", "Lobby Query" },

                { "CHARACTER_SELECT_BUTTON_CREATE_CHARACTER", "Create Character" },
                { "CHARACTER_SELECT_BUTTON_DELETE_CHARACTER", "Delete Character" },
                { "CHARACTER_SELECT_BUTTON_SELECT_CHARACTER", "Select Character" },
                { "CHARACTER_SELECT_BUTTON_RETURN", "Return" },

                { "CHARACTER_SELECT_DATA_ENTRY_EMPTY_SLOT", "Empty Slot" },
                { "FORMAT_CHARACTER_SELECT_DATA_ENTRY_INFO", "Lv-{0} {1} {2}" },

                { "CHARACTER_SELECT_CHARACTER_DELETE_PROMPT_TEXT", "Type in the character's name to confirm." },
                { "CHARACTER_SELECT_CHARACTER_DELETE_PROMPT_PLACEHOLDER_TEXT", "Enter Nickname..." },
                { "CHARACTER_SELECT_CHARACTER_DELETE_BUTTON_CONFIRM", "Delete Character" },
                { "CHARACTER_SELECT_CHARACTER_DELETE_BUTTON_RETURN", "Return" },

                // settings
                { "SETTINGS_TAB_BUTTON_VIDEO", "Display" },
                { "SETTINGS_TAB_BUTTON_AUDIO", "Audio" },
                { "SETTINGS_TAB_BUTTON_INPUT", "Input" },
                { "SETTINGS_TAB_BUTTON_NETWORK", "Interface" },

                { "SETTINGS_VIDEO_HEADER_GAME_EFFECT_SETTINGS", "Display Sensitive Settings" },
                { "SETTINGS_VIDEO_CELL_PROPORTIONS_TOGGLE", "Limit Player Character Proportions" },
                { "SETTINGS_VIDEO_CELL_JIGGLE_BONES_TOGGLE", "Disable Suggestive Jiggle Bones" },
                { "SETTINGS_VIDEO_CELL_CLEAR_UNDERCLOTHES_TOGGLE", "Enable Clear Clothing" },

                { "SETTINGS_VIDEO_HEADER_VIDEO_SETTINGS", "Video Settings" },
                { "SETTINGS_VIDEO_CELL_FULLSCREEN_TOGGLE", "Fullscreen Mode" },
                { "SETTINGS_VIDEO_CELL_VERTICAL_SYNC", "Vertical Sync / Lock 60 FPS" },
                { "SETTINGS_VIDEO_CELL_ANISOTROPIC_FILTERING", "Anisotropic Filtering" },
                { "SETTINGS_VIDEO_CELL_SCREEN_RESOLUTION", "Screen Resolution" },
                { "SETTINGS_VIDEO_CELL_ANTI_ALIASING", "Anti Aliasing" },
                { "SETTINGS_VIDEO_CELL_ANTI_ALIASING_OPTION_1", "Disabled" },
                { "SETTINGS_VIDEO_CELL_ANTI_ALIASING_OPTION_2", "2x Multi Sampling" },
                { "SETTINGS_VIDEO_CELL_ANTI_ALIASING_OPTION_3", "4x Multi Sampling" },
                { "SETTINGS_VIDEO_CELL_ANTI_ALIASING_OPTION_4", "8x Multi Sampling" },
                { "SETTINGS_VIDEO_CELL_TEXTURE_FILTERING", "Texture Filtering" },
                { "SETTINGS_VIDEO_CELL_TEXTURE_FILTERING_OPTION_1", "Bilnear (Smooth)" },
                { "SETTINGS_VIDEO_CELL_TEXTURE_FILTERING_OPTION_2", "Nearest (Crunchy)" },
                { "SETTINGS_VIDEO_CELL_TEXTURE_QUALITY", "Texture Quality" },
                { "SETTINGS_VIDEO_CELL_TEXTURE_QUALITY_OPTION_1", "High" },
                { "SETTINGS_VIDEO_CELL_TEXTURE_QUALITY_OPTION_2", "Medium" },
                { "SETTINGS_VIDEO_CELL_TEXTURE_QUALITY_OPTION_3", "Low" },
                { "SETTINGS_VIDEO_CELL_TEXTURE_QUALITY_OPTION_4", "Very Low" },

                { "SETTINGS_VIDEO_HEADER_CAMERA_SETTINGS", "Camera Display Settings" },
                { "SETTINGS_VIDEO_CELL_FIELD_OF_VIEW", "Field Of View" },
                { "SETTINGS_VIDEO_CELL_CAMERA_SMOOTHING", "Camera Smoothing" },
                { "SETTINGS_VIDEO_CELL_CAMERA_HORIZ", "Camera X Position" },
                { "SETTINGS_VIDEO_CELL_CAMERA_VERT", "Camera Y Position" },
                { "SETTINGS_VIDEO_CELL_CAMERA_RENDER_DISTANCE", "Render Distance" },
                { "SETTINGS_VIDEO_CELL_CAMERA_RENDER_DISTANCE_OPTION_1", "Very Near" },
                { "SETTINGS_VIDEO_CELL_CAMERA_RENDER_DISTANCE_OPTION_2", "Near" },
                { "SETTINGS_VIDEO_CELL_CAMERA_RENDER_DISTANCE_OPTION_3", "Far" },
                { "SETTINGS_VIDEO_CELL_CAMERA_RENDER_DISTANCE_OPTION_4", "Very Far" },

                { "SETTINGS_VIDEO_HEADER_POST_PROCESSING", "Post Processing" },
                { "SETTINGS_VIDEO_CELL_CAMERA_BITCRUSH_SHADER", "Enable Bitcrush Shader" },
                { "SETTINGS_VIDEO_CELL_CAMERA_WATER_EFFECT", "Enable Underwater Distortion Shader" },
                { "SETTINGS_VIDEO_CELL_CAMERA_SHAKE", "Enable Screen Shake" },
                { "SETTINGS_VIDEO_CELL_WEAPON_GLOW", "Disable Weapon Glow Effect" },

                { "SETTINGS_AUDIO_HEADER_AUDIO_SETTINGS", "Audio Settings" },
                { "SETTINGS_AUDIO_CELL_MASTER_VOLUME", "Master Volume" },
                { "SETTINGS_AUDIO_CELL_MUTE_APPLICATION", "Mute Application" },
                { "SETTINGS_AUDIO_CELL_MUTE_MUSIC", "Mute Music" },

                { "SETTINGS_AUDIO_HEADER_AUDIO_CHANNEL_SETTINGS", "Audio Channels" },
                { "SETTINGS_AUDIO_CELL_GAME_VOLUME", "Game Volume" },
                { "SETTINGS_AUDIO_CELL_GUI_VOLUME", "GUI Volume" },
                { "SETTINGS_AUDIO_CELL_AMBIENCE_VOLUME", "Ambience Volume" },
                { "SETTINGS_AUDIO_CELL_MUSIC_VOLUME", "Music Volume" },
                { "SETTINGS_AUDIO_CELL_VOICE_VOLUME", "Voice Volume" },

                { "SETTINGS_INPUT_HEADER_INPUT_SETTINGS", "Input Settings" },
                { "SETTINGS_INPUT_CELL_AXIS_TYPE", "Analog Stick Axis Type" },
                { "SETTINGS_INPUT_CELL_AXIS_TYPE_OPTION_1", "WASD (8 Directional)" },
                { "SETTINGS_INPUT_CELL_AXIS_TYPE_OPTION_2", "Xbox" },
                { "SETTINGS_INPUT_CELL_AXIS_TYPE_OPTION_3", "Playstation 4" },

                { "SETTINGS_INPUT_HEADER_CAMERA_CONTROL", "Camera Control" },
                { "SETTINGS_INPUT_CELL_CAMERA_SENSITIVITY", "Axis Sensitivity" },
                { "SETTINGS_INPUT_CELL_INVERT_X_CAMERA_AXIS", "Invert X Axis" },
                { "SETTINGS_INPUT_CELL_INVERT_Y_CAMERA_AXIS", "Invert Y Axis" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_RESET_CAMERA", "Reset Camera" },

                { "SETTINGS_INPUT_HEADER_MOVEMENT", "Movement" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_UP", "Up" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_DOWN", "Down" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_LEFT", "Left" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_RIGHT", "Right" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_JUMP", "Jump" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_DASH", "Dash" },

                { "SETTINGS_INPUT_HEADER_STRAFING", "Strafing" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_LOCK_DIRECTION", "Strafe" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_STRAFE_MODE", "Strafe / Aim Mode" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_STRAFE_MODE_OPTION_1", "Hold Strafe Key" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_STRAFE_MODE_OPTION_2", "Toggle Strafe Key" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_STRAFE_WEAPON", "Strafe While Holding Weapon" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_STRAFE_CASTING", "Strafe While Casting Offensive Skills" },

                { "SETTINGS_INPUT_HEADER_ACTION", "Action" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_ATTACK", "Attack" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_CHARGE_ATTACK", "Charge Attack" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_BLOCK", "Block" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_TARGET", "Lock On" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_INTERACT", "Interact" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_PVP_FLAG", "PvP Flag Toggle" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_SKILL_SLOT_01", "Skill Slot 1" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_SKILL_SLOT_02", "Skill Slot 2" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_SKILL_SLOT_03", "Skill Slot 3" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_SKILL_SLOT_04", "Skill Slot 4" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_SKILL_SLOT_05", "Skill Slot 5" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_SKILL_SLOT_06", "Skill Slot 6" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_RECALL", "Recall" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_QUICKSWAP_WEAPON", "Quickswap Weapon" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_SHEATHE_WEAPON", "Sheathe / Unsheathe Weapon" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_SIT", "Sit" },

                { "SETTINGS_INPUT_HEADER_CONSUMABLE_SLOTS", "Consumable Quick Slots" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_QUICK_SLOT_01", "Quick Slot 1" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_QUICK_SLOT_02", "Quick Slot 2" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_QUICK_SLOT_03", "Quick Slot 3" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_QUICK_SLOT_04", "Quick Slot 4" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_QUICK_SLOT_05", "Quick Slot 5" },

                { "SETTINGS_INPUT_HEADER_INTERFACE", "Interface" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_HOST_CONSOLE", "Host Console" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_LEXICON", "Open Lexicon" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_TAB_MENU", "Open Tab Menu" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_STATS_TAB", "Stats Tab" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_SKILLS_TAB", "Skills Tab" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_ITEM_TAB", "Item Tab" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_QUEST_TAB", "Quest Tab" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_WHO_TAB", "Who Tab" },
                { "SETTINGS_INPUT_CELL_KEYBINDING_HIDE_UI", "Hide Game UI" },

                { "SETTINGS_NETWORK_HEADER_UI_SETTINGS", "UI Settings" },
                { "SETTINGS_NETWORK_CELL_LOCALYSSATION_LANGUAGE", "Language" },
                { "SETTINGS_NETWORK_CELL_DISPLAY_CREEP_NAMETAGS", "Display Enemy Nametags" },
                { "SETTINGS_NETWORK_CELL_DISPLAY_LOCAL_NAMETAG", "Display Local Character Name Tag" },
                { "SETTINGS_NETWORK_CELL_DISPLAY_HOST_TAG", "Display [HOST] Tag on Host Character" },
                { "SETTINGS_NETWORK_CELL_HIDE_DUNGEON_MINIMAP", "Hide Dungeon Minimap" },
                { "SETTINGS_NETWORK_CELL_HIDE_FPS_COUNTER", "Hide FPS Counter" },
                { "SETTINGS_NETWORK_CELL_HIDE_PING_COUNTER", "Hide Ping Counter" },

                { "SETTINGS_NETWORK_HEADER_CLIENT_SETTINGS", "Client Settings" },
                { "SETTINGS_NETWORK_CELL_ENABLE_PVP_ON_MAP_ENTER", "Flag for PvP when available" },

                { "SETTINGS_BUTTON_RESET_TO_DEFAULTS", "Reset to Defaults" },
                { "SETTINGS_BUTTON_RESET", "Reset" },
                { "SETTINGS_BUTTON_CANCEL", "Cancel" },
                { "SETTINGS_BUTTON_APPLY", "Apply" },

                // equipment
                { "FORMAT_EQUIP_ITEM_RARITY", "[{0}]" },
                { "FORMAT_EQUIP_LEVEL_REQUIREMENT", "Lv-{0}" },
                { "FORMAT_EQUIP_CLASS_REQUIREMENT", "Class: {0}" },
                { "FORMAT_EQUIP_WEAPON_CONDITION", "\n<color=lime>- <color=yellow>{0}%</color> chance to apply {1}.</color>" },
                { "FORMAT_QUEST_TRACK_ELEMENT", "{0}: ({1} / {2})" },
                { "EQUIP_TOOLTIP_TYPE_HELM", "Helm (Armor)" },
                { "EQUIP_TOOLTIP_TYPE_CHESTPIECE", "Chestpiece (Armor)" },
                { "EQUIP_TOOLTIP_TYPE_LEGGINGS", "Leggings (Armor)" },
                { "EQUIP_TOOLTIP_TYPE_CAPE", "Cape (Armor)" },
                { "EQUIP_TOOLTIP_TYPE_RING", "Ring (Armor)" },
                { "FORMAT_EQUIP_TOOLTIP_TYPE_WEAPON", "{0} (Weapon)" },
                { "EQUIP_TOOLTIP_TYPE_SHIELD", "Shield" },
                { "FORMAT_EQUIP_STATS_DAMAGE_SCALED", "<color=#c5e384>({0} - {1})</color> Damage" },
                { "FORMAT_EQUIP_STATS_DAMAGE_SCALED_POWERFUL", "<color=#efcc00>({0} - {1})</color> Damage" },
                { "FORMAT_EQUIP_STATS_DAMAGE_COMPARE_BASE", "\n<color=grey>(Base Damage: {0} - {1})</color>" },
                { "FORMAT_EQUIP_STATS_DAMAGE_UNSCALED", "({0} - {1}) Damage" },
                { "FORMAT_EQUIP_STATS_BLOCK_THRESHOLD", "Block threshold: {0} damage" }
            };
            return language;
        }

        internal static void WriteLanguageToFile(Language language)
        {
            var filePath = Path.Combine(Application.persistentDataPath, "Localyssation", $"{language.code}.language");
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, JsonConvert.SerializeObject(language, Formatting.Indented));
        }

        public static string GetString(string key)
        {
            string result;
            if (currentLanguage.strings.TryGetValue(key, out result)) return result;
            if (defaultLanguage.strings.TryGetValue(key, out result)) return result;
            return key;
        }
    }

    public class Language
    {
        public string code = "";
        public string name = "";
        public bool shrinkOverflowingText = false;
        public Dictionary<string, string> strings = new Dictionary<string, string>();

        public void RegisterKey(string key, string defaultValue)
        {
            if (strings.ContainsKey(key)) return;
            strings[key] = defaultValue;
        }
    }

    public static class KeyUtil
    {
        public static string Normalize(string key)
        {
            return new string(
                key.ToUpper().Replace(" ", "_")
                .Where(x => "ABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789".Contains(x))
                .ToArray());
        }

        public static string GetForAsset(ScriptableItem asset)
        {
            return $"ITEM_{Normalize(asset._itemName)}";
        }

        public static string GetForAsset(ScriptableWeaponType asset)
        {
            return $"WEAPON_TYPE_{Normalize(asset._weaponTypeName)}";
        }

        public static string GetForAsset(ScriptableCreep asset)
        {
            return $"CREEP_{Normalize(asset._creepName)}";
        }

        public static string GetForAsset(ScriptableQuest asset)
        {
            return $"QUEST_{Normalize(asset._questName)}";
        }

        public static string GetForAsset(QuestTriggerRequirement asset)
        {
            return $"QUEST_TRIGGER_{Normalize(asset._questTriggerTag)}";
        }

        public static string GetForAsset(ScriptableCondition asset)
        {
            return $"CONDITION_{Normalize(asset._conditionName)}";
        }

        public static string GetForAsset(ScriptableStatModifier asset)
        {
            return $"STAT_MODIFIER_{Normalize(asset._modifierTag)}";
        }

        public static string GetForAsset(ScriptablePlayerRace asset)
        {
            return $"RACE_{Normalize(asset._raceName)}";
        }

        public static string GetForAsset(ScriptableCombatElement asset)
        {
            return $"COMBAT_ELEMENT_{Normalize(asset._elementName)}";
        }

        public static string GetForAsset(ScriptablePlayerBaseClass asset)
        {
            return $"PLAYER_CLASS_{Normalize(asset._className)}";
        }

        public static string GetForAsset(ScriptableSkill asset)
        {
            return $"SKILL_{Normalize(asset._skillName)}";
        }

        public static string GetForAsset(ItemRarity asset)
        {
            return $"ITEM_RARITY_{Normalize(asset.ToString())}";
        }

        public static string GetForAsset(DamageType asset)
        {
            return $"DAMAGE_TYPE_{Normalize(asset.ToString())}";
        }
    }
}
