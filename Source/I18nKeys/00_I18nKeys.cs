using Localyssation.LanguageModule;
using Localyssation.Util;
using System;
using System.Collections.Generic;

namespace Localyssation
{
    // Symbolized key strings, for better referring
    // Less string constants, less typos
    internal static partial class I18nKeys
    {

        // In case of lazy loading (experience from Java)
        public static void Init()
        {
            CharacterCreation.init();
            CharacterSelect.Init();
            Enums.init();
            Equipment.Init();
            Feedback.Init();
            Item.Init();
            Lore.Init();
            MainMenu.init();
            Quest.init();
            ScriptableStatusCondition.init();
            Settings.Init();
            SkillMenu.init();
            SteamLobby.Init();
            TabMenu.Init();
        }

        internal static readonly Dictionary<string, string> TR_KEYS = new Dictionary<string, string>();

        private static TranslationKey Create(string key, string defaultString = "")
        {
            if (string.IsNullOrEmpty(key)) { throw new ArgumentNullException("key is empty"); }
            if (string.IsNullOrEmpty(defaultString))
            {
                defaultString = key;
            }
            if (TR_KEYS.ContainsKey(key))
            {
                throw new ArgumentException($"key `{key}` Already Exists!");
            }
            LanguageManager.RegisterKey(new TranslationKey(key), defaultString);
            //TR_KEYS.Add(key, defaultString);
            return new TranslationKey(key);
        }

        public static string GetDefaulted(string key)
        {
            bool success = TR_KEYS.TryGetValue(key, out string value);
            if (success)
            {
                return value;
            }
            return key;
        }







    }
}
