using Localyssation.Util;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

namespace Localyssation
{
    // Symbolized key strings, for better referring
    // Less string constants, less typos
    internal static partial class I18nKeys
    {

        // In case of lazy loading (experience from Java)
        // So there is a lazy static loading feature just like Java
        public static void Init()
        {

            void InitNestedType(Type t)
            {
                t.GetMethod("Init",
                    System.Reflection.BindingFlags.Static
                    | System.Reflection.BindingFlags.NonPublic
                    | System.Reflection.BindingFlags.Public
                    )
                    .Invoke(t, null);   // Invoke static method, if null, that means something breaks
                
                t.GetNestedTypes().ToList().ForEach(InitNestedType);
            }

            typeof(I18nKeys).GetNestedTypes().ToList().ForEach(InitNestedType);

            //CharacterCreation.Init();
            //CharacterSelect.Init();
            //Enums.Init();
            //Equipment.Init();
            //Feedback.Init();
            //Item.Init();
            //Lore.Init();
            //MainMenu.Init();
            //Quest.Init();
            //ScriptableStatusCondition.init();
            //Settings.Init();
            //SkillMenu.Init();
            //SteamLobby.Init();
            //TabMenu.Init();
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
            TR_KEYS[key] = defaultString;
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
