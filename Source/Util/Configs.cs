using BepInEx.Configuration;
using HarmonyLib;
using Localyssation.LangAdjutable;
using Localyssation.LanguageModule;
using Nessie.ATLYSS.EasySettings.UIElements;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Localyssation.I18nKeys.Settings.Mod;

namespace Localyssation.Util
{
    public static class ConfigDefinitions
    {

        public static readonly ConfigDefinition Language
            = new ConfigDefinition("General", "Language");
        public static readonly ConfigDefinition TraslatorMode
            = new ConfigDefinition("Translators", "Translator Mode");
        public static readonly ConfigDefinition CreateDefaultLanguageFiles
            = new ConfigDefinition("Translators", "Create Default Language Files On Load");
        public static readonly ConfigDefinition ShowTranslationKey
            = new ConfigDefinition("Translators", "Show Translation Key");
        public static readonly ConfigDefinition ExportExtra
            = new ConfigDefinition("Translators", "Export Extra Info");
        public static readonly ConfigDefinition ReloadLanguageKeybind
            = new ConfigDefinition("Translators", "Reload Language Keybind");

        public static readonly ConfigDefinition ReloadFontBundlesKeybind
            = new ConfigDefinition("Translators", "Reload Font Bundles Keybind");
        public static readonly ConfigDefinition SwitchTranslationKeybind
            = new ConfigDefinition("Translators", "Switch Translation Keybind");
        public static readonly ConfigDefinition LogVanillaFonts
            = new ConfigDefinition("Translators", "Log Vanilla Fonts");
    }

    public static class LocalyssationConfig
    {
        private static ConfigFile config;

        internal static ConfigEntry<string> configLanguage { get; private set; }
        public static string Language { get => configLanguage.Value; }

        internal static ConfigEntry<bool> configTranslatorMode { get; private set; }
        public static bool TranslatorMode { get => configTranslatorMode.Value; }
        internal static ConfigEntry<bool> configCreateDefaultLanguageFiles { get; private set; }
        public static bool CreateDefaultLanguageFiles { get => configCreateDefaultLanguageFiles.Value; }
        internal static ConfigEntry<bool> configShowTranslationKey { get; private set; }
        public static bool ShowTranslationKey { get => configShowTranslationKey.Value; }
        internal static ConfigEntry<bool> configExportExtra { get; private set; }
        public static bool ExportExtra { get => configExportExtra.Value; }
        internal static ConfigEntry<KeyCode> configReloadLanguageKeybind { get; private set; }
        public static KeyCode ReloadLanguageKeybind { get => configReloadLanguageKeybind.Value; }

        internal static ConfigEntry<KeyCode> configReloadFontBundlesKeybind { get; private set; }
        public static KeyCode ReloadFontBundlesKeybind { get => configReloadFontBundlesKeybind.Value; }

        internal static ConfigEntry<KeyCode> configSwitchTranslationKeybind { get; private set; }
        public static KeyCode SwitchTranslationKeybind { get => configSwitchTranslationKeybind.Value; }

        internal static ConfigEntry<bool> configLogVanillaFonts { get; private set; }
        public static bool LogVanillaFonts { get => configLogVanillaFonts.Value; }

        public static void Init(ConfigFile _config)
        {
            config = _config;
            configLanguage = config.Bind(
                ConfigDefinitions.Language,
                LanguageManager.DefaultLanguage.info.code,
                new ConfigDescription("Currently selected language's code")
                );
            if (LanguageManager.GetLanguage(Language, out var previouslySelectedLanguage))
                LanguageManager.ChangeLanguage(previouslySelectedLanguage);

            configTranslatorMode = config.Bind(
                ConfigDefinitions.TraslatorMode,
                false,
                new ConfigDescription("Enables the features of this section")
                );
            configCreateDefaultLanguageFiles = config.Bind(
                ConfigDefinitions.CreateDefaultLanguageFiles,
                true,
                new ConfigDescription("If enabled, files for the default game language will be created in the mod's directory on game load")
                );
            configReloadLanguageKeybind = config.Bind(
                ConfigDefinitions.ReloadLanguageKeybind,
                KeyCode.F10,
                new ConfigDescription("When you press this button, your current language's files will be reloaded mid-game")
                );
            configShowTranslationKey = config.Bind(
                ConfigDefinitions.ShowTranslationKey,
                false,
                new ConfigDescription("Show translation keys instead of translated string for debugging.")
                );
            configExportExtra = config.Bind(
                ConfigDefinitions.ExportExtra,
                false,
                new ConfigDescription("Export quest and item data and image to markdown for translation referencing.")
                );

            configReloadFontBundlesKeybind = config.Bind(
                ConfigDefinitions.ReloadFontBundlesKeybind,
                KeyCode.F9,
                new ConfigDescription("When you press this button, the font bundles will be reloaded mid-game")
                );

            configSwitchTranslationKeybind = config.Bind(
                ConfigDefinitions.SwitchTranslationKeybind,
                KeyCode.F11,
                new ConfigDescription("When you press this button, the translation mode will be switched mid-game")
                );

            configLogVanillaFonts = config.Bind(
                ConfigDefinitions.LogVanillaFonts,
                false,
                new ConfigDescription("Log vanilla fonts to console")
                );
        }
    }

    internal class SettingsGUI
    {
        //internal bool settingsTabReady = false;
        //internal bool languagesLoaded = false;
        //internal bool settingsTabSetup = false;

        private AtlyssDropdown languageDropdown;
        private List<string> languageKeys;

        private AtlyssToggle translatorModeToggle;

        private AtlyssToggle showTranslationKeyToggle;
        private AtlyssToggle createDefaultLanguageFilesToggle;
        private AtlyssToggle exportExtraToggle;
        private AtlyssToggle logVanillaFontsToggle;


        private AtlyssKeyButton reloadLanguageKeybind;
        private AtlyssKeyButton reloadFontBundlesKeybind;
        private AtlyssKeyButton switchTranslationKeybind;


        private AtlyssButton createMissingForCurrentLangButton;
        private AtlyssButton logUntranslatedStringsButton;

        private readonly List<BaseAtlyssElement> translatorModeElements = new List<BaseAtlyssElement>();

        private static SettingsGUI instance = null;
        public static void Init()
        {
            if (instance == null)
                instance = new SettingsGUI();
        }

        // Require language loaded
        private SettingsGUI()
        {
            //TrySetupSettingsTab();
            Nessie.ATLYSS.EasySettings.Settings.OnInitialized.AddListener(SetupSettingsTab);

            Nessie.ATLYSS.EasySettings.Settings.OnApplySettings.AddListener(() =>
            {

            });
        }

        private void SetupTranslatorModeElements()
        {
            var tab = Nessie.ATLYSS.EasySettings.Settings.ModTab;
            void RegisterTranslatorModeElement<T>(T element, TranslationKey key)
                where T : BaseAtlyssElement
            {
                translatorModeElements.Add(element);
                if (element is BaseAtlyssLabelElement labelElement)
                {
                    LangAdjustables.RegisterText(labelElement.Label, key);
                }
                if (element is AtlyssButton button)
                {
                    LangAdjustables.RegisterText(button.ButtonLabel, key);
                }
            }

            void SetupToggles()
            {
                showTranslationKeyToggle = tab.AddToggle(LocalyssationConfig.configShowTranslationKey);
                showTranslationKeyToggle.OnValueChanged.AddListener((v) =>
                {
                    LanguageManager.ChangeLanguage(LanguageManager.CurrentLanguage, true);    // refresh all
                });
                //LangAdjustables.RegisterText(showTranslationKeyToggle.Label, SHOW_TRANSLATION_KEY);
                RegisterTranslatorModeElement(showTranslationKeyToggle, SHOW_TRANSLATION_KEY);

                createDefaultLanguageFilesToggle = tab.AddToggle(LocalyssationConfig.configCreateDefaultLanguageFiles);
                //LangAdjustables.RegisterText(createDefaultLanguageFilesToggle.Label, CREATE_DEFAULT_LANGUAGE_FILES);
                RegisterTranslatorModeElement(createDefaultLanguageFilesToggle, CREATE_DEFAULT_LANGUAGE_FILES);

                exportExtraToggle = tab.AddToggle(LocalyssationConfig.configExportExtra);
                //LangAdjustables.RegisterText(exportExtraToggle.Label, EXPORT_EXTRA);
                RegisterTranslatorModeElement(exportExtraToggle, EXPORT_EXTRA);

                logVanillaFontsToggle = tab.AddToggle(LocalyssationConfig.configLogVanillaFonts);
                RegisterTranslatorModeElement(logVanillaFontsToggle, LOG_VANILLA_FONTS);
            }

            void SetupKeybind()
            {

                reloadLanguageKeybind = tab.AddKeyButton(LocalyssationConfig.configReloadLanguageKeybind);
                //LangAdjustables.RegisterText(reloadLanguageKeybind.Label, RELOAD_LANGUAGE_KEYBIND);
                RegisterTranslatorModeElement(reloadLanguageKeybind, RELOAD_LANGUAGE_KEYBIND);


                reloadFontBundlesKeybind = tab.AddKeyButton(LocalyssationConfig.configReloadFontBundlesKeybind);
                //LangAdjustables.RegisterText(reloadFontBundlesKeybind.Label, RELOAD_FONT_BUNDLES_KEYBIND);
                RegisterTranslatorModeElement(reloadFontBundlesKeybind, RELOAD_FONT_BUNDLES_KEYBIND);

                switchTranslationKeybind = tab.AddKeyButton(LocalyssationConfig.configSwitchTranslationKeybind);
                //LangAdjustables.RegisterText(switchTranslationKeybind.Label, SWITCH_TRANSLATION_KEYBIND);
                RegisterTranslatorModeElement(switchTranslationKeybind, SWITCH_TRANSLATION_KEYBIND);
            }

            void SetupButton()
            {
                createMissingForCurrentLangButton = tab.AddButton(
                    Localyssation.GetString(ADD_MISSING_KEYS_TO_CURRENT_LANGUAGE),
                    OnAddMissingKeyButtonPressed
                );
				translatorModeElements.Add(createMissingForCurrentLangButton);

                logUntranslatedStringsButton = tab.AddButton(
                    Localyssation.GetString(LOG_UNTRANSLATED_STRINGS),
                    OnLogUntranslated
                    );
				translatorModeElements.Add(logUntranslatedStringsButton);

				// AddButton provide clone of other buttons, causing affected by batch translation bug
				// So I set their names here to avoid confusion in the hierarchy
				createMissingForCurrentLangButton.Button.name = "CreateMissingForCurrentLangButton";
				logUntranslatedStringsButton.Button.name = "LogUntranslatedStringsButton";

				//LangAdjustables.RegisterText(logUntranslatedStringsButton.ButtonLabel,
				//    LangAdjustables.GetStringFunc(I18nKeys.Settings.Mod.LOG_UNTRANSLATED_STRINGS)
				//    );
			}

            SetupToggles();
            SetupKeybind();
            SetupButton();

        }

        private void SetupSettingsTab()
        {
            //if (settingsTabSetup || !settingsTabReady || !languagesLoaded) return;
            //settingsTabSetup = true;

            var tab = Nessie.ATLYSS.EasySettings.Settings.ModTab;
            LangAdjustables.RegisterText(tab.TabButton.Label, I18nKeys.Settings.BUTTON_MODS);

            tab.AddHeader("Localyssation");


            languageKeys = LanguageManager.languages.Select(kv => kv.Key).ToList();
            var currentLanguageIndex = languageKeys.IndexOf(LanguageManager.CurrentLanguage.info.code);
            languageDropdown = tab.AddDropdown("Language", 
                languageKeys.Select(key => LanguageManager.languages[key].info.name).ToList(), 
                currentLanguageIndex
                );
            languageDropdown.OnValueChanged.AddListener(OnLanguageDropdownChanged);
            LangAdjustables.RegisterText(languageDropdown.Label, LANGUAGE);


            translatorModeToggle = tab.AddToggle(LocalyssationConfig.configTranslatorMode);
            translatorModeToggle.OnValueChanged.AddListener(OnTranslatorModeChanged);
            LangAdjustables.RegisterText(translatorModeToggle.Label, TRANSLATOR_MODE);


            SetupTranslatorModeElements();

            OnTranslatorModeChanged(LocalyssationConfig.TranslatorMode);
            Localyssation.instance.OnLanguageChanged += this.OnLanguageChanged;
            OnLanguageChange();

        }

        private static void ChangeAtlyssSettingsElementsEnabled<T>(T uiElement, bool enabled)
            where T : BaseAtlyssElement
        {
            uiElement.Root.gameObject.SetActive(enabled);
        }

        private void OnTranslatorModeChanged(bool value)
        {
            translatorModeElements.ForEach(v => ChangeAtlyssSettingsElementsEnabled(v, value));
        }


        private void OnLanguageDropdownChanged(int valueIndex)
        {
            var languageKey = languageKeys[valueIndex];
            LanguageManager.ChangeLanguage(languageKey);
            LocalyssationConfig.configLanguage.Value = languageKey;
        }

        private void OnAddMissingKeyButtonPressed()
        {
            foreach (var kvp in LanguageManager.DefaultLanguage.GetStrings())
            {
                if (!LanguageManager.CurrentLanguage.ContainsKey(kvp.Key))
                {
                    LanguageManager.CurrentLanguage.RegisterKey(kvp.Key, kvp.Value);
                }
            }
            LanguageManager.CurrentLanguage.WriteToFileSystem("missing");
        }

        private void OnLogUntranslated()
        {
            var changedCount = 0;
            var totalCount = 0;
            Localyssation.logger.LogMessage($"Logging strings that are the same in {LanguageManager.DefaultLanguage.info.name} and {LanguageManager.CurrentLanguage.info.name}:");
            foreach (var kvp in LanguageManager.CurrentLanguage.GetStrings())
            {
                if (LanguageManager.DefaultLanguage.GetStrings().TryGetValue(kvp.Key, out var valueInDefaultLanguage))
                {
                    totalCount += 1;
                    if (kvp.Value == valueInDefaultLanguage)
                        Localyssation.logger.LogMessage(kvp.Key);
                    else changedCount += 1;
                }
            }
            Localyssation.logger.LogMessage($"Done! {changedCount}/{totalCount} ({changedCount / (float)totalCount * 100f:0.00}%) strings are different between the languages.");
        }

        private void OnLanguageChange()
        {

            /// Bugs in EasySettings 1.1.8 caused updating using LangAdjustables will fail
            /// For all buttons added with EasySettings, their LangAdjustable components are not deep copied
            /// Thus button texts will keep the same with the first button added

            /// That's why I replace buttons here
            createMissingForCurrentLangButton.ButtonLabel.text = Localyssation.GetString(ADD_MISSING_KEYS_TO_CURRENT_LANGUAGE);
            logUntranslatedStringsButton.ButtonLabel.text = Localyssation.GetString(LOG_UNTRANSLATED_STRINGS);
		}
        public void OnLanguageChanged(Language newLanguage)
        {
            OnLanguageChange();
        }

        ~SettingsGUI()
        {
            Localyssation.instance.OnLanguageChanged -= this.OnLanguageChanged;
        }

    }
}
