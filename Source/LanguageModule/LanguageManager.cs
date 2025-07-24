using BepInEx;
using HarmonyLib;
using Localyssation.Util;
using MonoMod.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Localyssation.LanguageModule
{
    public static class LanguageManager
    {
        public static Language DefaultLanguage { get; private set; }
        public static Language CurrentLanguage { get; private set; }

        public static readonly Dictionary<string, Language> languages = new Dictionary<string, Language>();
        public static List<Language> LanguagesList { get => languages.Values.ToList(); }

        public static void Init()
        {
            DefaultLanguage = CreateDefaultLanguage();
            RegisterLanguage(DefaultLanguage);
            ChangeLanguage(DefaultLanguage);
            LoadLanguagesFromFileSystem();
        }

        public static void ChangeLanguage(string key)
        {
            ChangeLanguage(languages[key]);
        }
        public static void ChangeLanguage(Language newLanguage)
        {
            if (CurrentLanguage == newLanguage) return;

            CurrentLanguage = newLanguage;
            Localyssation.instance.CallOnLanguageChanged(newLanguage);
        }

        internal static Language CreateDefaultLanguage()
        {
            var language = new Language
            {
                info = new Language.LanguageInfo { code = "en-US", name = "English (US)" },
                fileSystemPath = Path.Combine(Path.GetDirectoryName(Localyssation.dllPath), "defaultLanguage")
            };

            I18nKeys.Init();
            language.GetStrings().AddRange(I18nKeys.TR_KEYS);
            return language;
        }

        public static bool GetLanguage(string languageCode, out Language language)
        {
            return languages.TryGetValue(languageCode, out language);
        }

        public static void RegisterKey(string key, string defaultValue)
        {
            DefaultLanguage.RegisterKey(key, defaultValue);
        }
        public static void RegisterKey(TranslationKey key, string defaultValue)
        {
            DefaultLanguage.RegisterKey(key.key, defaultValue);
        }

        public static void LoadLanguagesFromFileSystem()
        {
            var filePaths = Directory.GetFiles(Paths.PluginPath, "localyssationLanguage.json", SearchOption.AllDirectories);
            filePaths.Do(filePath => 
            {
                var langPath = Path.GetDirectoryName(filePath);

                var loadedLanguage = new Language
                {
                    fileSystemPath = langPath
                };
                if (loadedLanguage.LoadFromFileSystem())
                    RegisterLanguage(loadedLanguage);
            });
        }

        private static void RegisterLanguage(Language language)
        {
            if (languages.ContainsKey(language.info.code)) return;

            languages[language.info.code] = language;
        }


        public static void UpdateDefaultLanguageFile()
        {
            Directory.CreateDirectory(DefaultLanguage.fileSystemPath);
            DefaultLanguage.WriteToFileSystem("default_language", true);
        }
    }
}
