
using BepInEx.Configuration;
using Localyssation.Util;

namespace Localyssation
{
    internal static partial class I18nKeys
    {
        internal static partial class Settings
        {
            public static class Mod
            {
                internal static void Init() { }
                private static TranslationKey Create(string key, string defaultValue = "")
                {
                    return I18nKeys.Create($"SETTINGS_MOD_CELL_LOCALYSSATION_{key}", defaultValue);
                }

                private static TranslationKey Create(ConfigDefinition configEntry)
                {
                    return Create(KeyUtil.Normalize(configEntry.Key), configEntry.Key);
                }



                public static readonly TranslationKey LANGUAGE
                    = Create(ConfigDefinitions.Language);
                public static readonly TranslationKey TRANSLATOR_MODE
                    = Create(ConfigDefinitions.TraslatorMode);
                public static readonly TranslationKey CREATE_DEFAULT_LANGUAGE_FILES
                    = Create(ConfigDefinitions.CreateDefaultLanguageFiles);
                public static readonly TranslationKey SHOW_TRANSLATION_KEY
                    = Create(ConfigDefinitions.ShowTranslationKey);
                public static readonly TranslationKey EXPORT_EXTRA
                    = Create(ConfigDefinitions.ExportExtra);
                public static readonly TranslationKey RELOAD_LANGUAGE_KEYBIND
                    = Create(ConfigDefinitions.ReloadLanguageKeybind);
                public static readonly TranslationKey LOG_VANILLA_FONTS
                    = Create(ConfigDefinitions.LogVanillaFonts);

                public static readonly TranslationKey RELOAD_FONT_BUNDLES_KEYBIND
                    = Create(ConfigDefinitions.ReloadFontBundlesKeybind);
                public static readonly TranslationKey SWITCH_TRANSLATION_KEYBIND
                    = Create(ConfigDefinitions.SwitchTranslationKeybind);

                public static readonly TranslationKey ADD_MISSING_KEYS_TO_CURRENT_LANGUAGE
                    = Create("ADD_MISSING_KEYS_TO_CURRENT_LANGUAGE", "Add Missing Keys to Current Language");
                public static readonly TranslationKey LOG_UNTRANSLATED_STRINGS
                    = Create("LOG_UNTRANSLATED_STRINGS", "Log Untranslated Strings");
            }

        }
    }
}