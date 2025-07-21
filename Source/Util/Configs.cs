using BepInEx.Configuration;
using Localyssation.LangAdjutable;
using Localyssation.LanguageModule;
using Nessie.ATLYSS.EasySettings.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Localyssation
{
    public static class LocalyssationConfig
    {
        internal static ConfigFile config;


        internal static ConfigEntry<string> configLanguage;
        public static string Language { get => configLanguage.Value; }

        internal static ConfigEntry<bool> configTranslatorMode;
        public static bool TranslatorMode { get => configTranslatorMode.Value; }

        internal static ConfigEntry<bool> configCreateDefaultLanguageFiles;
        public static bool CreateDefaultLanguageFiles { get => configCreateDefaultLanguageFiles.Value; }

        internal static ConfigEntry<bool> configShowTranslationKey;
        public static bool ShowTranslationKey { get => configShowTranslationKey.Value; }

        internal static ConfigEntry<bool> configExportExtra;
        public static bool ExportExtra { get => configExportExtra.Value; }

        internal static ConfigEntry<KeyCode> configReloadLanguageKeybind;
        public static KeyCode ReloadLanguageKeybind { get => configReloadLanguageKeybind.Value; }

        public static void Init(ConfigFile _config)
        {
            config = _config;
            configLanguage = config.Bind("General", "Language", LanguageManager.DefaultLanguage.info.code, "Currently selected language's code");
            if (LanguageManager.GetLanguage(Language, out var previouslySelectedLanguage))
                LanguageManager.ChangeLanguage(previouslySelectedLanguage);

            configTranslatorMode = config.Bind("Translators", "Translator Mode", false, "Enables the features of this section");
            configCreateDefaultLanguageFiles = config.Bind("Translators", "Create Default Language Files On Load", true, "If enabled, files for the default game language will be created in the mod's directory on game load");
            configReloadLanguageKeybind = config.Bind("Translators", "Reload Language Keybind", KeyCode.F10, "When you press this button, your current language's files will be reloaded mid-game");
            configShowTranslationKey = config.Bind("Translators", "Show Translation Key", false, "Show translation keys instead of translated string for debugging.");
            configExportExtra = config.Bind("Translators", "Export Extra Info", false, "Export quest and item data and image to markdown for translation referencing.");

        }
    }

    internal class SettingsGUI
    {
        internal bool settingsTabReady = false;
        internal bool languagesLoaded = false;
        internal bool settingsTabSetup = false;

        private AtlyssDropdown languageDropdown;
        private List<string> languageKeys;

        private AtlyssToggle translatorModeToggle;

        private AtlyssToggle showTranslationKeyToggle;
        private AtlyssToggle createDefaultLanguageFilesToggle;
        private AtlyssKeyButton reloadLanguageKeybind;
        private AtlyssToggle exportExtraToggle;
        private AtlyssButton createMissingForCurrentLangButton;
        private AtlyssButton logUntranslatedStringsButton;


        // Require language loaded
        public void Init()
        {
            TrySetupSettingsTab();
            Nessie.ATLYSS.EasySettings.Settings.OnInitialized.AddListener(() =>
            {
                settingsTabReady = true;
                TrySetupSettingsTab();
            });

            Nessie.ATLYSS.EasySettings.Settings.OnApplySettings.AddListener(() =>
            {

            });
        }

        private void TrySetupSettingsTab()
        {
            if (settingsTabSetup || !settingsTabReady || !languagesLoaded) return;
            settingsTabSetup = true;

            var tab = Nessie.ATLYSS.EasySettings.Settings.ModTab;

            tab.AddHeader("Localyssation");

            languageKeys = LanguageManager.languages.Select(kv => kv.Key).ToList();
            var currentLanguageIndex = languageKeys.IndexOf(LanguageManager.CurrentLanguage.info.code);
            languageDropdown = tab.AddDropdown("Language", languageKeys.Select(key => LanguageManager.languages[key].info.name).ToList(), currentLanguageIndex);
            languageDropdown.OnValueChanged.AddListener(OnLanguageDropdownChanged);
            LangAdjustables.RegisterText(languageDropdown.Label, LangAdjustables.GetStringFunc("MOD_SETTINGS_CELL_LOCALYSSATION_LANGUAGE", languageDropdown.LabelText));

            
            translatorModeToggle = tab.AddToggle(LocalyssationConfig.configTranslatorMode);
            translatorModeToggle.OnValueChanged.AddListener(OnTranslatorModeChanged);

            showTranslationKeyToggle = tab.AddToggle(LocalyssationConfig.configShowTranslationKey);
            showTranslationKeyToggle.OnValueChanged.AddListener((v) =>
            {
                LanguageManager.ChangeLanguage(LanguageManager.CurrentLanguage);    // refresh all
            });

            createDefaultLanguageFilesToggle = tab.AddToggle(LocalyssationConfig.configCreateDefaultLanguageFiles); 

            reloadLanguageKeybind = tab.AddKeyButton(LocalyssationConfig.configReloadLanguageKeybind);

            exportExtraToggle = tab.AddToggle(LocalyssationConfig.configExportExtra);

            createMissingForCurrentLangButton = tab.AddButton("Add Missing Keys to Current Language", OnAddMissingKeyButtonPressed);

            logUntranslatedStringsButton = tab.AddButton("Log Untranslated Strings", OnLogUntranslated);

            OnTranslatorModeChanged(LocalyssationConfig.TranslatorMode);

        }

        private static void ChangeAtlyssSettingsElementsEnabled<T>(T uiElement, bool enabled)
        {
            if (uiElement is AtlyssButton button)
            {
                button.ButtonLabel.enabled = enabled;
                button.Button.enabled = enabled;
                return;
            }
            if (uiElement is AtlyssKeyButton keyButton)
            {
                keyButton.Label.enabled = enabled;
                keyButton.Button.enabled = enabled;
                keyButton.ButtonLabel.enabled = enabled;
                return;
            }
            if (uiElement is AtlyssToggle toggle)
            {
                toggle.Label.enabled = enabled;
                toggle.Toggle.enabled = enabled;
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
            Localyssation.logger.LogMessage($"Done! {changedCount}/{totalCount} ({((float)changedCount / (float)totalCount * 100f):0.00}%) strings are different between the languages.");
        }

    }
}
