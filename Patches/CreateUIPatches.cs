using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

namespace Localyssation.Patches
{
    internal static class CreateUIPatches
    {
        private static Dropdown languageDropdown;

        [HarmonyPatch(typeof(SettingsManager), nameof(SettingsManager.Start))]
        [HarmonyPostfix]
        public static void SettingsManager_Start(SettingsManager __instance)
        {
            // find a "dropdown" settings cell to use for our custom language selector option
            var dropdownSettingsCell = __instance.transform.Find("Canvas_SettingsMenu/_dolly_settingsMenu/_dolly_videoSettingsTab/_backdrop_videoSettings/Scroll View/Viewport/Content/_cell_screenResolution");
            // find a container for interface settings (internally called "network") that we will put our option in
            var settingsContainer = __instance.transform.Find("Canvas_SettingsMenu/_dolly_settingsMenu/_dolly_networkSettingsTab/Image/Scroll View/Viewport/Content");
            // find the "ui settings" header
            var uiSettingsHeader = settingsContainer.Find("_header_uiSettings");
            if (dropdownSettingsCell && settingsContainer && uiSettingsHeader)
            {
                // create our own setting cell, and put it in the "ui settings" section
                var languageSelectCell = Object.Instantiate(dropdownSettingsCell.gameObject, settingsContainer);
                languageSelectCell.name = "_cell_localyssationLanguage";
                languageSelectCell.transform.SetSiblingIndex(uiSettingsHeader.GetSiblingIndex() + 1);

                // edit the dropdown
                languageDropdown = languageSelectCell.GetComponentInChildren<Dropdown>();
                languageDropdown.gameObject.name = "Dropdown_localyssationLanguage";
                languageDropdown.ClearOptions();
                languageDropdown.onValueChanged = new Dropdown.DropdownEvent();
                var dropdownOptions = new List<Dropdown.OptionData>();
                var languageIndex = 0;
                var currentLanguageIndex = 0;
                foreach (var language in Localyssation.languagesList)
                {
                    dropdownOptions.Add(new Dropdown.OptionData(language.name));
                    if (language == Localyssation.currentLanguage) currentLanguageIndex = languageIndex;
                    languageIndex++;
                }
                languageDropdown.AddOptions(dropdownOptions);
                languageDropdown.value = currentLanguageIndex;
                languageDropdown.onValueChanged.AddListener((valueIndex) => {
                    Localyssation.ChangeLanguage(Localyssation.languagesList[valueIndex]);
                });

                // make the cell title translateable
                LangAdjustables.RegisterText(languageSelectCell.transform.Find("Text").GetComponent<Text>(), LangAdjustables.GetStringFunc("SETTINGS_NETWORK_CELL_LOCALYSSATION_LANGUAGE"));
            }
        }

        [HarmonyPatch(typeof(SettingsManager), nameof(SettingsManager.Save_SettingsData))]
        [HarmonyPostfix]
        public static void SettingsManager_Save_SettingsData(SettingsManager __instance)
        {
            if (languageDropdown)
            {
                var language = Localyssation.languagesList[languageDropdown.value];
                Localyssation.ChangeLanguage(language);
                Localyssation.configLanguage.Value = language.code;
            }
        }

        [HarmonyPatch(typeof(SettingsManager), nameof(SettingsManager.Load_SettingsData))]
        [HarmonyPostfix]
        public static void SettingsManager_Load_SettingsData(SettingsManager __instance)
        {
            if (languageDropdown)
            {
                if (Localyssation.languages.TryGetValue(Localyssation.configLanguage.Value, out var previouslySelectedLanguage))
                {
                    Localyssation.ChangeLanguage(previouslySelectedLanguage);
                    languageDropdown.value = Localyssation.languagesList.IndexOf(previouslySelectedLanguage);
                }
            }
        }
    }
}