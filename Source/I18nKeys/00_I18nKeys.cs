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
            Equipment.init();
            Feedback.Init();
            Item.Init();
            Lore.Init();
            MainMenu.init();
            Quest.init();
            ScriptableStatusCondition.init();
            Settings.Init();
            SkillMenu.init();
            TabMenu.Init();
        }

        internal static readonly Dictionary<string, string> TR_KEYS = new Dictionary<string, string>();

        private static string Create(string key, string defaultString = "")
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
            TR_KEYS[key] = defaultString;
            return key;
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
