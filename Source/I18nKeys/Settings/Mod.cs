
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
                private static string Create(string key, string defaultValue="")
                {
                    return I18nKeys.Create($"SETTINGS_MOD_CELL_LOCALYSSATION_{key}", defaultValue);
                }

                private static string Create(ConfigDefinition configEntry)
                {
                    return Create(KeyUtil.Normalize(configEntry.Key), configEntry.Key);
                }

                

                public static readonly string LANGUAGE
                    = Create(LocalyssationConfig.configLanguageDefinition);
                public static readonly string TRANSLATOR_MODE
                    = Create(LocalyssationConfig.configTraslatorModeDefinition);
                public static readonly string CREATE_DEFAULT_LANGUAGE_FILES
                    = Create(LocalyssationConfig.configCreateDefaultLanguageFilesDefinition);
                public static readonly string SHOW_TRANSLATION_KEY
                    = Create(LocalyssationConfig.configShowTranslationKeyDefinition);
                public static readonly string EXPORT_EXTRA
                    = Create(LocalyssationConfig.configExportExtraDefinition);
                public static readonly string RELOAD_LANGUAGE_KEYBIND
                    = Create(LocalyssationConfig.configReloadLanguageKeybindDefinition);

                public static readonly string ADD_MISSING_KEYS_TO_CURRENT_LANGUAGE
                    = Create(nameof(ADD_MISSING_KEYS_TO_CURRENT_LANGUAGE), "Add Missing Keys to Current Language");
                public static readonly string LOG_UNTRANSLATED_STRINGS
                    = Create(nameof(LOG_UNTRANSLATED_STRINGS), "Log Untranslated Strings");
            }

        }
    }
}