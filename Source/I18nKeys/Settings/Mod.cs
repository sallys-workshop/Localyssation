
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
                private static TranslationKey Create(string key, string defaultValue="")
                {
                    return I18nKeys.Create($"SETTINGS_MOD_CELL_LOCALYSSATION_{key}", defaultValue);
                }

                private static TranslationKey Create(ConfigDefinition configEntry)
                {
                    return Create(KeyUtil.Normalize(configEntry.Key), configEntry.Key);
                }

                

                public static readonly TranslationKey LANGUAGE
                    = Create(LocalyssationConfig.configLanguageDefinition);
                public static readonly TranslationKey TRANSLATOR_MODE
                    = Create(LocalyssationConfig.configTraslatorModeDefinition);
                public static readonly TranslationKey CREATE_DEFAULT_LANGUAGE_FILES
                    = Create(LocalyssationConfig.configCreateDefaultLanguageFilesDefinition);
                public static readonly TranslationKey SHOW_TRANSLATION_KEY
                    = Create(LocalyssationConfig.configShowTranslationKeyDefinition);
                public static readonly TranslationKey EXPORT_EXTRA
                    = Create(LocalyssationConfig.configExportExtraDefinition);
                public static readonly TranslationKey RELOAD_LANGUAGE_KEYBIND
                    = Create(LocalyssationConfig.configReloadLanguageKeybindDefinition);

                public static readonly TranslationKey ADD_MISSING_KEYS_TO_CURRENT_LANGUAGE
                    = Create(nameof(ADD_MISSING_KEYS_TO_CURRENT_LANGUAGE), "Add Missing Keys to Current Language");
                public static readonly TranslationKey LOG_UNTRANSLATED_STRINGS
                    = Create(nameof(LOG_UNTRANSLATED_STRINGS), "Log Untranslated Strings");
            }

        }
    }
}