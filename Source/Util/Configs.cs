using BepInEx.Configuration;
using Localyssation.LangAdjutable;
using Localyssation.LanguageModule;
using Nessie.ATLYSS.EasySettings.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Localyssation.Util
{
    public static class LocalyssationConfig
    {
        private static ConfigFile config;

        public static readonly ConfigDefinition configLanguageDefinition = new ConfigDefinition("General", "Language");
        internal static ConfigEntry<string> configLanguage { get; private set; }
        public static string Language { get => configLanguage.Value; }

        public static readonly ConfigDefinition configTraslatorModeDefinition = new ConfigDefinition("Translators", "Translator Mode");
        internal static ConfigEntry<bool> configTranslatorMode { get; private set; }
        public static bool TranslatorMode { get => configTranslatorMode.Value; }

        public static readonly ConfigDefinition configCreateDefaultLanguageFilesDefinition = new ConfigDefinition("Translators", "Create Default Language Files On Load");
        internal static ConfigEntry<bool> configCreateDefaultLanguageFiles { get; private set; }
        public static bool CreateDefaultLanguageFiles { get => configCreateDefaultLanguageFiles.Value; }

        public static readonly ConfigDefinition configShowTranslationKeyDefinition = new ConfigDefinition("Translators", "Show Translation Key");
        internal static ConfigEntry<bool> configShowTranslationKey { get; private set; }
        public static bool ShowTranslationKey { get => configShowTranslationKey.Value; }

        public static readonly ConfigDefinition configExportExtraDefinition = new ConfigDefinition("Translators", "Export Extra Info");
        internal static ConfigEntry<bool> configExportExtra { get; private set; }
        public static bool ExportExtra { get => configExportExtra.Value; }

        public static readonly ConfigDefinition configReloadLanguageKeybindDefinition = new ConfigDefinition("Translators", "Reload Language Keybind");
        internal static ConfigEntry<KeyCode> configReloadLanguageKeybind { get; private set; }
        public static KeyCode ReloadLanguageKeybind { get => configReloadLanguageKeybind.Value; }

        public static void Init(ConfigFile _config)
        {
            config = _config;
            configLanguage = config.Bind(configLanguageDefinition, LanguageManager.DefaultLanguage.info.code, new ConfigDescription("Currently selected language's code"));
            if (LanguageManager.GetLanguage(Language, out var previouslySelectedLanguage))
                LanguageManager.ChangeLanguage(previouslySelectedLanguage);

            configTranslatorMode = config.Bind(
                configTraslatorModeDefinition, 
                false, 
                new ConfigDescription("Enables the features of this section")
                );
            configCreateDefaultLanguageFiles = config.Bind(
                configCreateDefaultLanguageFilesDefinition, 
                true, 
                new ConfigDescription("If enabled, files for the default game language will be created in the mod's directory on game load")
                );
            configReloadLanguageKeybind = config.Bind(
                configReloadLanguageKeybindDefinition, 
                KeyCode.F10, 
                new ConfigDescription("When you press this button, your current language's files will be reloaded mid-game")
                );
            configShowTranslationKey = config.Bind(
                configShowTranslationKeyDefinition, 
                false, 
                new ConfigDescription("Show translation keys instead of translated string for debugging.")
                );
            configExportExtra = config.Bind(
                configExportExtraDefinition, 
                false, 
                new ConfigDescription("Export quest and item data and image to markdown for translation referencing.")
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
        private AtlyssKeyButton reloadLanguageKeybind;
        private AtlyssToggle exportExtraToggle;
        private AtlyssButton createMissingForCurrentLangButton;
        private AtlyssButton logUntranslatedStringsButton;

        private static Color ENABLED_COLOR = Color.white;
        private static Color DISABLED_COLOR = Color.grey;

        private static SettingsGUI instance = null;
        public static void Init()
        {
            if (instance == null)
                instance = new SettingsGUI();
        }

        // Require language loaded
        public SettingsGUI()
        {
            //TrySetupSettingsTab();
            Nessie.ATLYSS.EasySettings.Settings.OnInitialized.AddListener(TrySetupSettingsTab);

            Nessie.ATLYSS.EasySettings.Settings.OnApplySettings.AddListener(() =>
            {

            });
        }

        private void TrySetupSettingsTab()
        {
            //if (settingsTabSetup || !settingsTabReady || !languagesLoaded) return;
            //settingsTabSetup = true;

            var tab = Nessie.ATLYSS.EasySettings.Settings.ModTab;
            LangAdjustables.RegisterText(tab.TabButton.Label, LangAdjustables.GetStringFunc(I18nKeys.Settings.BUTTON_MODS));

            tab.AddHeader("Localyssation");

            languageKeys = LanguageManager.languages.Select(kv => kv.Key).ToList();
            var currentLanguageIndex = languageKeys.IndexOf(LanguageManager.CurrentLanguage.info.code);
            languageDropdown = tab.AddDropdown("Language", languageKeys.Select(key => LanguageManager.languages[key].info.name).ToList(), currentLanguageIndex);
            languageDropdown.OnValueChanged.AddListener(OnLanguageDropdownChanged);
            LangAdjustables.RegisterText(languageDropdown.Label, LangAdjustables.GetStringFunc(I18nKeys.Settings.Mod.LANGUAGE));


            translatorModeToggle = tab.AddToggle(LocalyssationConfig.configTranslatorMode);
            translatorModeToggle.OnValueChanged.AddListener(OnTranslatorModeChanged);
            LangAdjustables.RegisterText(translatorModeToggle.Label, LangAdjustables.GetStringFunc(I18nKeys.Settings.Mod.TRANSLATOR_MODE));

            showTranslationKeyToggle = tab.AddToggle(LocalyssationConfig.configShowTranslationKey);
            showTranslationKeyToggle.OnValueChanged.AddListener((v) =>
            {
                LanguageManager.ChangeLanguage(LanguageManager.CurrentLanguage, true);    // refresh all
            });
            LangAdjustables.RegisterText(showTranslationKeyToggle.Label, LangAdjustables.GetStringFunc(I18nKeys.Settings.Mod.SHOW_TRANSLATION_KEY));

            createDefaultLanguageFilesToggle = tab.AddToggle(LocalyssationConfig.configCreateDefaultLanguageFiles);
            LangAdjustables.RegisterText(createDefaultLanguageFilesToggle.Label, LangAdjustables.GetStringFunc(I18nKeys.Settings.Mod.CREATE_DEFAULT_LANGUAGE_FILES));

            reloadLanguageKeybind = tab.AddKeyButton(LocalyssationConfig.configReloadLanguageKeybind);
            LangAdjustables.RegisterText(reloadLanguageKeybind.Label, LangAdjustables.GetStringFunc(I18nKeys.Settings.Mod.RELOAD_LANGUAGE_KEYBIND));

            exportExtraToggle = tab.AddToggle(LocalyssationConfig.configExportExtra);
            LangAdjustables.RegisterText(exportExtraToggle.Label, LangAdjustables.GetStringFunc(I18nKeys.Settings.Mod.EXPORT_EXTRA));

            createMissingForCurrentLangButton = tab.AddButton(
                Localyssation.GetDefaultString(I18nKeys.Settings.Mod.ADD_MISSING_KEYS_TO_CURRENT_LANGUAGE), 
                OnAddMissingKeyButtonPressed
                );
            LangAdjustables.RegisterText(createMissingForCurrentLangButton.ButtonLabel,
                LangAdjustables.GetStringFunc(I18nKeys.Settings.Mod.ADD_MISSING_KEYS_TO_CURRENT_LANGUAGE));

            logUntranslatedStringsButton = tab.AddButton(
                Localyssation.GetDefaultString(I18nKeys.Settings.Mod.LOG_UNTRANSLATED_STRINGS), 
                OnLogUntranslated
                );
            LangAdjustables.RegisterText(logUntranslatedStringsButton.ButtonLabel,
                LangAdjustables.GetStringFunc(I18nKeys.Settings.Mod.LOG_UNTRANSLATED_STRINGS)
                );
            

            OnTranslatorModeChanged(LocalyssationConfig.TranslatorMode);

        }

        private static void ChangeAtlyssSettingsElementsEnabled<T>(T uiElement, bool enabled)
        {
            if (uiElement is AtlyssButton button)
            {
                //button.ButtonLabel.enabled = enabled;
                button.Button.enabled = enabled;
                button.ButtonLabel.color = enabled ? ENABLED_COLOR : DISABLED_COLOR;
                return;
            }
            if (uiElement is AtlyssKeyButton keyButton)
            {
                //keyButton.Label.enabled = enabled;
                keyButton.Button.enabled = enabled;
                //keyButton.ButtonLabel.enabled = enabled;
                keyButton.Label.color = enabled ? ENABLED_COLOR : DISABLED_COLOR;
                keyButton.ButtonLabel.color = enabled ? ENABLED_COLOR : DISABLED_COLOR;
                return;
            }
            if (uiElement is AtlyssToggle toggle)
            {
                //toggle.Label.enabled = enabled;
                toggle.Toggle.enabled = enabled;
                toggle.Label.color = enabled ? ENABLED_COLOR : DISABLED_COLOR;
                return;
            }
            Localyssation.logger.LogError($"Unknown UI element type: {uiElement.GetType()}");
        }

        private void OnTranslatorModeChanged(bool value)
        {
            ChangeAtlyssSettingsElementsEnabled(showTranslationKeyToggle, value);
            ChangeAtlyssSettingsElementsEnabled(createDefaultLanguageFilesToggle, value);
            ChangeAtlyssSettingsElementsEnabled(reloadLanguageKeybind, value);
            ChangeAtlyssSettingsElementsEnabled(exportExtraToggle, value);
            ChangeAtlyssSettingsElementsEnabled(createMissingForCurrentLangButton, value);
            ChangeAtlyssSettingsElementsEnabled(logUntranslatedStringsButton, value);
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

    }
}
