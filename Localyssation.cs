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
    [BepInDependency(Nessie.ATLYSS.EasySettings.MyPluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class Localyssation : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "com.themysticsword.localyssation";
        public const string PLUGIN_NAME = "Localyssation";
        public const string PLUGIN_VERSION = "0.0.3";

        public static Localyssation instance;

        internal static System.Reflection.Assembly assembly;
        internal static string dllPath;

        public static Language defaultLanguage;
        public static Language currentLanguage;
        public static Dictionary<string, Language> languages = new Dictionary<string, Language>();
        public static readonly List<Language> languagesList = new List<Language>();

        public event System.Action<Language> onLanguageChanged;
        internal void CallOnLanguageChanged(Language newLanguage) { if (onLanguageChanged != null) onLanguageChanged.Invoke(newLanguage); }

        internal static BepInEx.Logging.ManualLogSource logger;
        internal static BepInEx.Configuration.ConfigFile config;

        internal static BepInEx.Configuration.ConfigEntry<string> configLanguage;
        internal static BepInEx.Configuration.ConfigEntry<bool> configTranslatorMode;
        internal static BepInEx.Configuration.ConfigEntry<bool> configCreateDefaultLanguageFiles;
        internal static BepInEx.Configuration.ConfigEntry<KeyCode> configReloadLanguageKeybind;

        internal static bool settingsTabReady = false;
        internal static bool languagesLoaded = false;
        internal static bool settingsTabSetup = false;
        internal static Nessie.ATLYSS.EasySettings.UIElements.AtlyssDropdown languageDropdown;

        private void Awake()
        {
            instance = this;
            logger = Logger;
            config = Config;

            assembly = System.Reflection.Assembly.GetExecutingAssembly();
            dllPath = new System.Uri(assembly.CodeBase).LocalPath;

            defaultLanguage = CreateDefaultLanguage();
            RegisterLanguage(defaultLanguage);
            ChangeLanguage(defaultLanguage);
            LoadLanguagesFromFileSystem();

            configLanguage = config.Bind("General", "Language", defaultLanguage.info.code, "Currently selected language's code");
            if (languages.TryGetValue(configLanguage.Value, out var previouslySelectedLanguage))
                ChangeLanguage(previouslySelectedLanguage);

            configTranslatorMode = config.Bind("Translators", "Translator Mode", false, "Enables the features of this section");
            configCreateDefaultLanguageFiles = config.Bind("Translators", "Create Default Language Files On Load", true, "If enabled, files for the default game language will be created in the mod's directory on game load");
            configReloadLanguageKeybind = config.Bind("Translators", "Reload Language Keybind", KeyCode.F10, "When you press this button, your current language's files will be reloaded mid-game");

            Nessie.ATLYSS.EasySettings.Settings.OnInitialized.AddListener(() =>
            {
                settingsTabReady = true;
                TrySetupSettingsTab();
            });

            Harmony harmony = new Harmony(PLUGIN_GUID);
            harmony.PatchAll();
            harmony.PatchAll(typeof(Patches.GameLoadPatches));
            harmony.PatchAll(typeof(Patches.ReplaceTextPatches));
            OnSceneLoaded.Init();
            LangAdjustables.Init();
        }

        private static void TrySetupSettingsTab()
        {
            if (settingsTabSetup || !settingsTabReady || !languagesLoaded) return;
            settingsTabSetup = true;

            var tab = Nessie.ATLYSS.EasySettings.Settings.ModTab;

            tab.AddHeader("Localyssation");

            var languageNames = new List<string>();
            var currentLanguageIndex = 0;
            for (var i = 0; i < languagesList.Count; i++)
            {
                var language = languagesList[i];
                languageNames.Add(language.info.name);
                if (language == currentLanguage) currentLanguageIndex = i;
            }
            languageDropdown = tab.AddDropdown("Language", languageNames, currentLanguageIndex);
            languageDropdown.OnValueChanged.AddListener((valueIndex) =>
            {
                var language = languagesList[valueIndex];
                ChangeLanguage(language);
                configLanguage.Value = language.info.code;
            });
            LangAdjustables.RegisterText(languageDropdown.Label, LangAdjustables.GetStringFunc("SETTINGS_NETWORK_CELL_LOCALYSSATION_LANGUAGE", languageDropdown.LabelText));

            tab.AddToggle(configTranslatorMode);
            if (configTranslatorMode.Value)
            {
                tab.AddToggle(configCreateDefaultLanguageFiles);
                tab.AddKeyButton(configReloadLanguageKeybind);
                tab.AddButton("Add Missing Keys to Current Language", () =>
                {
                    foreach (var kvp in defaultLanguage.strings)
                    {
                        if (!currentLanguage.strings.ContainsKey(kvp.Key))
                        {
                            currentLanguage.strings[kvp.Key] = kvp.Value;
                        }
                    }
                    currentLanguage.WriteToFileSystem();
                });
                tab.AddButton("Log Untranslated Strings", () =>
                {
                    var changedCount = 0;
                    var totalCount = 0;
                    logger.LogMessage($"Logging strings that are the same in {defaultLanguage.info.name} and {currentLanguage.info.name}:");
                    foreach (var kvp in currentLanguage.strings)
                    {
                        if (defaultLanguage.strings.TryGetValue(kvp.Key, out var valueInDefaultLanguage))
                        {
                            totalCount += 1;
                            if (kvp.Value == valueInDefaultLanguage) logger.LogMessage(kvp.Key);
                            else changedCount += 1;
                        }
                    }
                    logger.LogMessage($"Done! {changedCount}/{totalCount} ({(changedCount / totalCount * 100f):0.00}%) strings are different between the languages.");
                });
            }
        }

        private void Update()
        {
            if (configTranslatorMode.Value)
            {
                if (UnityInput.Current.GetKeyDown(configReloadLanguageKeybind.Value))
                {
                    currentLanguage.LoadFromFileSystem(true);
                    CallOnLanguageChanged(currentLanguage);
                }
            }
        }

        public static void LoadLanguagesFromFileSystem()
        {
            var filePaths = Directory.GetFiles(Paths.PluginPath, "localyssationLanguage.json", SearchOption.AllDirectories);
            foreach (var filePath in filePaths)
            {
                var langPath = Path.GetDirectoryName(filePath);

                var loadedLanguage = new Language();
                loadedLanguage.fileSystemPath = langPath;
                if (loadedLanguage.LoadFromFileSystem())
                    RegisterLanguage(loadedLanguage);
            }

            languagesLoaded = true;
            TrySetupSettingsTab();
        }

        public static void RegisterLanguage(Language language)
        {
            if (languages.ContainsKey(language.info.code)) return;

            languages[language.info.code] = language;
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
            language.info.code = "en-US";
            language.info.name = "English (US)";
            language.fileSystemPath = Path.Combine(Path.GetDirectoryName(dllPath), "defaultLanguage");

            language.strings = new Dictionary<string, string>()
            {
                // general
                { "GAME_LOADING", "Loading..." },
                { "EXP_COUNTER_MAX", "MAX" },

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

                // character creation
                { "CHARACTER_CREATION_HEADER", "Character Creation" },
                { "CHARACTER_CREATION_HEADER_RACE_NAME", "Race Select" },
                { "CHARACTER_CREATION_RACE_DESCRIPTOR_HEADER_INITIAL_SKILL", "Initial Skill" },
                { "CHARACTER_CREATION_BUTTON_SET_TO_DEFAULTS", "Defaults" },
                { "CHARACTER_CREATION_CHARACTER_NAME_PLACEHOLDER_TEXT", "Enter Name..." },
                { "CHARACTER_CREATION_BUTTON_CREATE_CHARACTER", "Create Character" },
                { "CHARACTER_CREATION_BUTTON_RETURN", "Return" },

                { "CHARACTER_CREATION_CUSTOMIZER_HEADER_COLOR", "Color" },
                { "CHARACTER_CREATION_CUSTOMIZER_COLOR_BODY_HEADER", "Body" },
                { "CHARACTER_CREATION_CUSTOMIZER_COLOR_BODY_TEXTURE", "Texture" },
                { "CHARACTER_CREATION_CUSTOMIZER_COLOR_HAIR_HEADER", "Hair" },
                { "CHARACTER_CREATION_CUSTOMIZER_COLOR_HAIR_LOCK_COLOR", "Lock Color" },

                { "CHARACTER_CREATION_CUSTOMIZER_HEADER_HEAD", "Head" },
                { "CHARACTER_CREATION_CUSTOMIZER_HEAD_HEAD_WIDTH", "Head Width" },
                { "CHARACTER_CREATION_CUSTOMIZER_HEAD_HEAD_MOD", "Modify" },
                { "CHARACTER_CREATION_CUSTOMIZER_HEAD_VOICE_PITCH", "Voice Pitch" },
                { "CHARACTER_CREATION_CUSTOMIZER_HEAD_HAIR_STYLE", "Hair" },
                { "CHARACTER_CREATION_CUSTOMIZER_HEAD_EARS", "Ears" },
                { "CHARACTER_CREATION_CUSTOMIZER_HEAD_EYES", "Eyes" },
                { "CHARACTER_CREATION_CUSTOMIZER_HEAD_MOUTH", "Mouth" },

                { "CHARACTER_CREATION_CUSTOMIZER_HEADER_BODY", "Body" },
                { "CHARACTER_CREATION_CUSTOMIZER_BODY_HEIGHT", "Height" },
                { "CHARACTER_CREATION_CUSTOMIZER_BODY_WIDTH", "Width" },
                { "CHARACTER_CREATION_CUSTOMIZER_BODY_CHEST", "Chest" },
                { "CHARACTER_CREATION_CUSTOMIZER_BODY_ARMS", "Arms" },
                { "CHARACTER_CREATION_CUSTOMIZER_BODY_BELLY", "Belly" },
                { "CHARACTER_CREATION_CUSTOMIZER_BODY_BOTTOM", "Bottom" },
                { "CHARACTER_CREATION_CUSTOMIZER_BODY_TAIL", "Tail" },
                { "CHARACTER_CREATION_CUSTOMIZER_BODY_TOGGLE_LEFT_HANDED", "Mirror Body" },

                { "CHARACTER_CREATION_CUSTOMIZER_HEADER_TRAIT", "Trait" },
                { "CHARACTER_CREATION_CUSTOMIZER_TRAIT_EQUIPMENT", "Equipment" },
                { "CHARACTER_CREATION_CUSTOMIZER_TRAIT_WEAPON_LOADOUT", "Weapon" },
                { "CHARACTER_CREATION_CUSTOMIZER_TRAIT_GEAR_DYE", "Dye" },
                { "CHARACTER_CREATION_CUSTOMIZER_TRAIT_ATTRIBUTES", "Attributes" },
                { "CHARACTER_CREATION_CUSTOMIZER_TRAIT_RESET_ATTRIBUTE_POINTS", "Reset Points" },

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
                { "SETTINGS_NETWORK_CELL_DISPLAY_GLOBAL_NICKNAME_TAGS", "Display Global Nametags <color=cyan>(@XX)</color>" },
                { "SETTINGS_NETWORK_CELL_DISPLAY_LOCAL_NAMETAG", "Display Local Character Name Tag" },
                { "SETTINGS_NETWORK_CELL_DISPLAY_HOST_TAG", "Display [HOST] Tag on Host Character" },
                { "SETTINGS_NETWORK_CELL_HIDE_DUNGEON_MINIMAP", "Hide Dungeon Minimap" },
                { "SETTINGS_NETWORK_CELL_HIDE_FPS_COUNTER", "Hide FPS Counter" },
                { "SETTINGS_NETWORK_CELL_HIDE_PING_COUNTER", "Hide Ping Counter" },
                { "SETTINGS_NETWORK_CELL_HIDE_STAT_POINT_COUNTER", "Hide Stat Point Notice Panel" },
                { "SETTINGS_NETWORK_CELL_HIDE_SKILL_POINT_COUNTER", "Hide Skill Point Notice Panel" },

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
                { "FORMAT_EQUIP_STATS_BLOCK_THRESHOLD", "Block threshold: {0} damage" },

                // tab menu
                { "TAB_MENU_CELL_STATS_HEADER", "Stats" },

                { "TAB_MENU_CELL_STATS_ATTRIBUTE_POINT_COUNTER", "Points" },
                { "TAB_MENU_CELL_STATS_BUTTON_APPLY_ATTRIBUTE_POINTS", "Apply" },

                { "TAB_MENU_CELL_STATS_INFO_CELL_NICK_NAME", "Nickname" },
                { "TAB_MENU_CELL_STATS_INFO_CELL_RACE_NAME", "Race" },
                { "TAB_MENU_CELL_STATS_INFO_CELL_CLASS_NAME", "Class" },
                { "TAB_MENU_CELL_STATS_INFO_CELL_LEVEL_COUNTER", "Level" },
                { "TAB_MENU_CELL_STATS_INFO_CELL_EXPERIENCE", "Experience" },

                { "TAB_MENU_CELL_STATS_INFO_CELL_MAX_HEALTH", "Health" },
                { "TAB_MENU_CELL_STATS_INFO_CELL_MAX_MANA", "Mana" },
                { "TAB_MENU_CELL_STATS_INFO_CELL_MAX_STAMINA", "Stamina" },

                { "TAB_MENU_CELL_STATS_INFO_CELL_ATTACK", "Attack Power" },
                { "TAB_MENU_CELL_STATS_INFO_CELL_RANGED_POWER", "Dex Power" },
                { "TAB_MENU_CELL_STATS_INFO_CELL_PHYS_CRITICAL", "Phys. Crit %" },

                { "TAB_MENU_CELL_STATS_INFO_CELL_MAGIC_POW", "Mgk. Power" },
                { "TAB_MENU_CELL_STATS_INFO_CELL_MAGIC_CRIT", "Mgk. Crit %" },

                { "TAB_MENU_CELL_STATS_INFO_CELL_DEFENSE", "Defense" },
                { "TAB_MENU_CELL_STATS_INFO_CELL_MAGIC_DEF", "Mgk. Defense" },

                { "TAB_MENU_CELL_STATS_INFO_CELL_EVASION", "Evasion %" },
                { "TAB_MENU_CELL_STATS_INFO_CELL_MOVE_SPD", "Mov Spd %" },

                { "TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_BEGIN", "<b>Base Stat:</b> <i>" },
                { "TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_END_CRIT", "%</i> (Critical %)" },
                { "TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_END_EVASION", "%</i> (Evasion %)" },
                { "TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_ATTACK_POW", "{0}</i> (Attack Power)" },
                { "TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_MAX_MP", "{0}</i> (Max Mana)" },
                { "TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_MAX_HP", "{0}</i> (Max Health)" },
                { "TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_RANGE_POW", "{0}</i> (Dex Power)" },
                { "TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_END_MAGIC_CRIT", "%</i> (Magic Critical %)" },
                { "TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_MAGIC_DEF", "{0}</i> (Magic Defense)" },
                { "TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_DEFENSE", "{0}</i> (Defense)" },
                { "TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_MAGIC_POW", "{0}</i> (Magic Power)" },
                { "TAB_MENU_CELL_STATS_TOOLTIP_BASE_STAT_FORMAT_MAX_STAM", "{0}</i> (Max Stamina)" },


                { "TAB_MENU_CELL_SKILLS_HEADER", "Skills" },

                { "TAB_MENU_CELL_SKILLS_SKILL_POINT_COUNTER", "Skill Points" },

                { "TAB_MENU_CELL_SKILLS_CLASS_TAB_TOOLTIP_NOVICE", "General Skills" },
                { "TAB_MENU_CELL_SKILLS_CLASS_TAB_TOOLTIP", "{0} Skills" },

                { "TAB_MENU_CELL_SKILLS_CLASS_HEADER_NOVICE", "General Skillbook" },
                { "TAB_MENU_CELL_SKILLS_CLASS_HEADER", "{0} Skillbook" },

                { "SKILL_RANK_SOULBOUND", "Soulbound Skill" },
                { "FORMAT_SKILL_RANK", "[Rank {0} / {1}]" },
                { "FORMAT_SKILL_TOOLTIP_DAMAGE_TYPE", "{0} Skill" },
                { "FORMAT_SKILL_TOOLTIP_ITEM_COST", "x{0} {1}" },
                { "FORMAT_SKILL_TOOLTIP_MANA_COST", "{0} Mana" },
                { "FORMAT_SKILL_TOOLTIP_HEALTH_COST", "{0} Health" },
                { "FORMAT_SKILL_TOOLTIP_STAMINA_COST", "{0} Stamina" },
                { "SKILL_TOOLTIP_CAST_TIME_INSTANT", "Instant Cast" },
                { "FORMAT_SKILL_TOOLTIP_CAST_TIME", "{0} sec Cast" },
                { "FORMAT_SKILL_TOOLTIP_COOLDOWN", "{0} sec Cooldown" },
                { "SKILL_TOOLTIP_PASSIVE", "Passive Skill" },

                { "SKILL_TOOLTIP_RANK_DESCRIPTOR_NEXT_RANK", "\n<color=white><i>[Next Rank]</i></color>" },
                { "FORMAT_SKILL_TOOLTIP_RANK_DESCRIPTOR_CURRENT_RANK", "\n<color=white><i>[Rank {0}]</i></color>" },
                { "FORMAT_SKILL_TOOLTIP_RANK_DESCRIPTOR_REQUIRED_LEVEL", "<color=red>\n(Requires Lv. {0})</color>" },
                { "FORMAT_SKILL_TOOLTIP_RANK_DESCRIPTOR_COOLDOWN", "<color=yellow>{0} sec cooldown.</color>" },
                { "FORMAT_SKILL_TOOLTIP_RANK_DESCRIPTOR_CAST_TIME", "<color=yellow>{0} sec cast time.</color>" },
                { "SKILL_TOOLTIP_RANK_DESCRIPTOR_CAST_TIME_INSTANT", "<color=yellow>instant cast time.</color>" },

                { "SKILL_TOOLTIP_RANK_DESCRIPTOR_CONDITION_CANCEL_ON_HIT", " <color=yellow>Cancels if hit.</color>" },
                { "SKILL_TOOLTIP_RANK_DESCRIPTOR_CONDITION_IS_PERMANENT", " <color=yellow>Permanent.</color>" },
                { "FORMAT_SKILL_TOOLTIP_RANK_DESCRIPTOR_CONDITION_DURATION", " <color=yellow>Lasts for {0} seconds.</color>" },
                { "SKILL_TOOLTIP_RANK_DESCRIPTOR_CONDITION_IS_STACKABLE", " <color=yellow>Stackable.</color>" },
                { "SKILL_TOOLTIP_RANK_DESCRIPTOR_CONDITION_IS_REFRESHABLE", " <color=yellow>Refreshes when re-applied.</color>" },

                // quests
                { "FORMAT_QUEST_REQUIRED_LEVEL", "(lv-{0})" },
                { "QUEST_TYPE_CLASS", "(Class Tome)" },
                { "QUEST_TYPE_MASTERY", "(Mastery Scroll)" },
                { "QUEST_MENU_SUMMARY_NO_QUESTS", "No Quests in Quest Log." },
                { "QUEST_MENU_HEADER_UNSELECTED", "Select a Quest." },
                { "FORMAT_QUEST_MENU_CELL_QUEST_LOG_COUNTER", "Quest Log: ({0} / {1})" },
                { "FORMAT_QUEST_MENU_CELL_FINISHED_QUEST_COUNTER", "Completed Quests: {0}" },
                { "FORMAT_QUEST_MENU_CELL_REWARD_EXP", "{0} exp" },
                { "FORMAT_QUEST_MENU_CELL_REWARD_CURRENCY", "{0} Crowns" },
                { "QUEST_MENU_CELL_SLOT_EMPTY", "Empty Slot" },
                { "QUEST_SELECTION_MANAGER_QUEST_ACCEPT_BUTTON_ACCEPT", "Accept Quest" },
                { "QUEST_SELECTION_MANAGER_QUEST_ACCEPT_BUTTON_LOCKED", "Quest Locked" },
                { "QUEST_SELECTION_MANAGER_QUEST_ACCEPT_BUTTON_INCOMPLETE", "Quest Incomplete" },
                { "QUEST_SELECTION_MANAGER_QUEST_ACCEPT_BUTTON_TURN_IN", "Complete Quest" },
                { "QUEST_SELECTION_MANAGER_QUEST_ACCEPT_BUTTON_UNSELECTED", "Select a Quest" },
                { "FORMAT_QUEST_PROGRESS", "{0}: ({1} / {2})" },
                { "FORMAT_QUEST_PROGRESS_CREEPS_KILLED", "{0} slain" },
            };
            return language;
        }

        public const string GET_STRING_DEFAULT_VALUE_ARG_UNSPECIFIED = "SAME_AS_KEY";
        public static string GetStringRaw(string key, string defaultValue = GET_STRING_DEFAULT_VALUE_ARG_UNSPECIFIED)
        {
            string result;
            if (currentLanguage.strings.TryGetValue(key, out result)) return result;
            if (defaultLanguage.strings.TryGetValue(key, out result)) return result;
            return (defaultValue == GET_STRING_DEFAULT_VALUE_ARG_UNSPECIFIED ? key : defaultValue);
        }

        private delegate string TextEditTagFunc(string str, string arg, int fontSize);
        private static Dictionary<string, TextEditTagFunc> textEditTags = new Dictionary<string, TextEditTagFunc>()
        {
            {
                "firstupper",
                (str, arg, fontSize) =>
                {
                    if (str.Length > 0)
                    {
                        var letter = str[0].ToString();
                        str = str.Remove(0, 1);
                        str = str.Insert(0, letter.ToUpper());
                    }
                    return str;
                }
            },
            {
                "firstlower",
                (str, arg, fontSize) =>
                {
                    if (str.Length > 0)
                    {
                        var letter = str[0].ToString();
                        str = str.Remove(0, 1);
                        str = str.Insert(0, letter.ToLower());
                    }
                    return str;
                }
            },
            {
                "scale",
                (str, arg, fontSize) =>
                {
                    if (fontSize > 0) {
                        try {
                            var scale = float.Parse(arg, System.Globalization.CultureInfo.InvariantCulture);
                            str = $"<size={System.Math.Round(fontSize * scale)}>{str}</size>";
                        }
                        catch { }
                    }
                    else
                    {
                        str = $"<scalefallback={arg}>{str}</scalefallback>";
                    }
                    return str;
                }
            },
            {
                "scalefallback",
                (str, arg, fontSize) =>
                {
                    if (fontSize > 0) {
                        try {
                            var scale = float.Parse(arg, System.Globalization.CultureInfo.InvariantCulture);
                            str = $"<size={System.Math.Round(fontSize * scale)}>{str}</size>";
                        }
                        catch { }
                    }
                    return str;
                }
            }
        };
        private static List<string> defaultAppliedTextEditTags = new List<string>() {
            "firstupper", "firstlower", "scale"
        };
        public static string ApplyTextEditTags(string str, int fontSize = -1, List<string> appliedTextEditTags = null)
        {
            if (appliedTextEditTags == null) appliedTextEditTags = defaultAppliedTextEditTags;

            var result = str;

            foreach (var tag in textEditTags)
            {
                if (!appliedTextEditTags.Contains(tag.Key)) continue;

                while (true)
                {
                    // find bounds of the tagged text
                    var openingTagBeginning = $"<{tag.Key}";
                    var openingTagIndex = result.IndexOf(openingTagBeginning);
                    if (openingTagIndex == -1) break;

                    var openingTagEndIndex = result.IndexOf(">", openingTagIndex + openingTagBeginning.Length);
                    if (openingTagEndIndex == -1) break;

                    var closingTag = $"</{tag.Key}>";
                    var closingTagIndex = result.IndexOf(closingTag, openingTagEndIndex + 1);
                    if (closingTagIndex == -1) break;

                    // get the full opening tag string and get arguments (if they exist)
                    var openingTag = result.Substring(openingTagIndex + 1, openingTagEndIndex - 1);
                    var arg = "";
                    if (openingTag.Contains("="))
                    {
                        var split = openingTag.Split('=');
                        if (split.Length == 2) arg = split[1];
                    }

                    // get tagged text
                    var stringInTag = "";
                    if ((openingTagEndIndex + 1) <= (closingTagIndex - 1))
                        stringInTag = result.Substring(openingTagEndIndex + 1, closingTagIndex - openingTagEndIndex - 1);

                    // edit tagged text
                    var editedString = tag.Value(stringInTag, arg, fontSize);

                    // remove tags from the displayed string, and replace tagged text with newly edited text
                    result = result
                        .Remove(closingTagIndex, closingTag.Length)
                        .Remove(openingTagIndex, openingTagEndIndex - openingTagIndex + 1);

                    // replace tagged text
                    result = result
                        .Remove(openingTagIndex, stringInTag.Length)
                        .Insert(openingTagIndex, editedString);
                }
            }

            return result;
        }

        public static string GetString(string key, string defaultValue = GET_STRING_DEFAULT_VALUE_ARG_UNSPECIFIED, int fontSize = -1)
        {
            return ApplyTextEditTags(GetStringRaw(key, defaultValue), fontSize);
        }
    }

    public class Language
    {
        public class LanguageInfo
        {
            public string code = "";
            public string name = "";
            public bool autoShrinkOverflowingText = false;
        }

        public LanguageInfo info = new LanguageInfo();
        public string fileSystemPath;
        public Dictionary<string, string> strings = new Dictionary<string, string>();

        public void RegisterKey(string key, string defaultValue)
        {
            if (strings.ContainsKey(key)) return;
            strings[key] = defaultValue;
        }

        public bool LoadFromFileSystem(bool forceOverwrite = false)
        {
            if (string.IsNullOrEmpty(fileSystemPath)) return false;

            var infoFilePath = Path.Combine(fileSystemPath, "localyssationLanguage.json");
            var stringsFilePath = Path.Combine(fileSystemPath, "strings.tsv");
            var stringScaleFactorsFilePath = Path.Combine(fileSystemPath, "stringScaleFactors.tsv");
            try
            {
                info = JsonConvert.DeserializeObject<LanguageInfo>(File.ReadAllText(infoFilePath));

                foreach (var tsvRow in TSVUtil.parseTsvWithHeaders(File.ReadAllText(stringsFilePath)))
                {
                    if (!forceOverwrite) RegisterKey(tsvRow["key"], tsvRow["value"]);
                    else strings[tsvRow["key"]] = tsvRow["value"];
                }

                return true;
            }
            catch (System.Exception e)
            {
                Localyssation.logger.LogError(e);
                return false;
            }
        }

        public bool WriteToFileSystem()
        {
            if (string.IsNullOrEmpty(fileSystemPath)) return false;

            try
            {
                Directory.CreateDirectory(fileSystemPath);

                var infoFilePath = Path.Combine(fileSystemPath, "localyssationLanguage.json");
                File.WriteAllText(infoFilePath, JsonConvert.SerializeObject(info, Formatting.Indented));

                var stringsFilePath = Path.Combine(fileSystemPath, "strings.tsv");
                var tsvRows = strings.Select(x => new List<string>() { x.Key, x.Value }).ToList();
                tsvRows.Insert(0, new List<string>() { "key", "value" });
                File.WriteAllText(stringsFilePath, TSVUtil.makeTsv(tsvRows));

                return true;
            }
            catch (System.Exception e)
            {
                Localyssation.logger.LogError(e);
                return false;
            }
        }
    }

    public static class TSVUtil
    {
        public static string makeTsv(List<List<string>> rows, string delimeter = "\t")
        {
            var rowStrs = new List<string>();
            List<string> headerRow = null;
            for (var i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                for (var j = 0; j < row.Count; j++)
                {
                    row[j] = row[j].Replace("\n", "\\n").Replace("\t", "\\t");
                }

                var rowStr = string.Join(delimeter, row);

                if (headerRow == null)
                {
                    headerRow = row;
                }
                else if (headerRow.Count != row.Count)
                {
                    Localyssation.logger.LogError($"Row {i} has {row.Count} columns, which does not match header column count (${headerRow.Count})");
                    Localyssation.logger.LogError($"Row content: {rowStr}");
                    return string.Join(delimeter, headerRow);
                }
                rowStrs.Add(rowStr);
            }
            return string.Join("\n", rowStrs);
        }

        public static List<List<string>> parseTsv(string tsv, string delimeter = "\t")
        {
            var parsedTsv = new List<List<string>>();
            List<string> headerRow = null;
            var splitTsv = tsv.Split(new[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < splitTsv.Length; i++)
            {
                var rowStr = splitTsv[i];
                if (rowStr.EndsWith("\r")) rowStr = rowStr.Substring(0, rowStr.Length - 2); // convert CRLF to LF
                
                var row = new List<string>(Split(rowStr, delimeter));
                for (var j = 0; j < row.Count; j++)
                {
                    row[j] = row[j].Replace("\\n", "\n").Replace("\\t", "\t");
                }

                if (headerRow == null)
                {
                    headerRow = row;
                }
                else if (headerRow.Count != row.Count)
                {
                    Localyssation.logger.LogError($"Row {i} has {row.Count} columns, which does not match header column count (${headerRow.Count})");
                    Localyssation.logger.LogError($"Row content: {rowStr}");
                    return new List<List<string>>() { headerRow };
                }
                parsedTsv.Add(row);
            }
            return parsedTsv;
        }

        public static List<Dictionary<string, string>> parseTsvWithHeaders(string tsv, string delimeter = "\t")
        {
            var parsedTsv = parseTsv(tsv, delimeter);
            var withHeaders = new List<Dictionary<string, string>>();
            if (parsedTsv.Count <= 0) return withHeaders;

            var headerRow = parsedTsv[0];
            for (var i = 1; i < parsedTsv.Count; i++)
            {
                var dict = parsedTsv[i]
                    .Select((x, y) => new KeyValuePair<string, string>(headerRow[y], x))
                    .ToDictionary(x => x.Key, x => x.Value);
                withHeaders.Add(dict);
            }
            return withHeaders;
        }

        public static List<string> Split(string str, string delimeter)
        {
            var result = new List<string>();

            var delimeterIsEscape = delimeter.StartsWith("\\");

            var splitStartIndex = 0;
            var searchIndex = 0;
            while (true)
            {
                var delimIndex = str.IndexOf(delimeter, searchIndex);
                if (delimIndex == -1)
                {
                    result.Add(str.Substring(splitStartIndex, str.Length - splitStartIndex));
                    break;
                }

                searchIndex = delimIndex + delimeter.Length;
                if (!delimeterIsEscape || (delimIndex > 0 && str[delimIndex - 1] != '\\'))
                {
                    result.Add(str.Substring(splitStartIndex, delimIndex - splitStartIndex));
                    splitStartIndex = searchIndex;
                }

                if (searchIndex >= str.Length)
                {
                    result.Add(str.Substring(splitStartIndex, str.Length - splitStartIndex));
                    break;
                }
            }

            return result;
        }
    }

    public static class Util
    {
        public static string GetChildTransformPath(Transform transform, int depth = 0)
        {
            var str = transform.name;
            if (depth > 0)
            {
                var parent = transform.parent;
                if (parent != null)
                {
                    str = $"{GetChildTransformPath(parent, depth - 1)}/{str}";
                }
            }
            return str;
        }
    }

    public static class KeyUtil
    {
        public static string Normalize(string key)
        {
            return new string(
                key.ToUpper().Replace(" ", "_").Replace("/", "_")
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

        public static string GetForAsset(ScriptableStatAttribute asset)
        {
            return $"STAT_ATTRIBUTE_{Normalize(asset._attributeName)}";
        }

        public static string GetForAsset(ItemRarity asset)
        {
            return $"ITEM_RARITY_{Normalize(asset.ToString())}";
        }

        public static string GetForAsset(DamageType asset)
        {
            return $"DAMAGE_TYPE_{Normalize(asset.ToString())}";
        }

        public static string GetForAsset(ScriptableDialogData asset)
        {
            return $"{Normalize(asset.name.ToString())}";
        }
    }
}
