using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localyssation
{
    // Symbolized key strings, for better referring
    // Less string constants, less typos
    internal static partial class I18nKeys
    {

        // In case of lazy loading (experience from Java)
        public static void Init()
        {
            Item.init();
            Lore.init();
            Equipment.init();
            Quest.init(); 
            MainMenu.init();
            CharacterSelect.init();
            CharacterCreation.init();
            TabMenu.init();
            SkillMenu.init();
            Settings.init();
        }

        internal static readonly Dictionary<string, string> TR_KEYS = new Dictionary<string, string>();

        private static string create(string key, string defaultString = "")
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

        public static string getDefaulted(string key)
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
