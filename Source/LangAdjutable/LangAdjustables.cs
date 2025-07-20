using System;
using System.Collections.Generic;
#pragma warning disable IDE0130
namespace Localyssation.LangAdjutable
#pragma warning restore IDE0130
{
    public interface ILangAdjustable
    {
        void AdjustToLanguage(Language newLanguage);
    }
    // "adjustables" are in-game objects that must be automatically adjusted with
    // language-specific variants (strings, textures, etc.) whenever language is changed in-game
    internal static class LangAdjustables
    {
        public static List<ILangAdjustable> nonMonoBehaviourAdjustables = new List<ILangAdjustable>();

        public static void Init()
        {
            Localyssation.instance.onLanguageChanged += (newLanguage) =>
            {
                // copy the list to avoid the loop breaking if an entry self-destructs mid-loop
                var safeAdjustables = new List<ILangAdjustable>(nonMonoBehaviourAdjustables);
                foreach (var replaceable in safeAdjustables)
                {
                    replaceable.AdjustToLanguage(newLanguage);
                }
            };
        }

        // handy function to slot into the newTextFunc param when you need a simple key->string replacement
        public static System.Func<int, string> GetStringFunc(string key, string defaultValue = Localyssation.GET_STRING_DEFAULT_VALUE_ARG_UNSPECIFIED)
        {
            return (fontSize) => Localyssation.GetString(key, defaultValue, fontSize);
        }

        internal static Dictionary<UnityEngine.UI.Text, LangAdjustableUIText> registeredTexts = new Dictionary<UnityEngine.UI.Text, LangAdjustableUIText>();
        public static void RegisterText(UnityEngine.UI.Text text, System.Func<int, string> newTextFunc = null)
        {
            LangAdjustableUIText adjustable;
            if (!registeredTexts.TryGetValue(text, out adjustable))
                adjustable = registeredTexts[text] = text.gameObject.AddComponent<LangAdjustableUIText>();

            if (newTextFunc != null) adjustable.newTextFunc = newTextFunc;
        }

        internal static Dictionary<TMPro.TextMeshProUGUI, LangAdjustableTMProUGUIText> registeredTMProUGUITexts = new Dictionary<TMPro.TextMeshProUGUI, LangAdjustableTMProUGUIText>();
        public static void RegisterText(TMPro.TextMeshProUGUI text, System.Func<int, string> newTextFunc = null)
        {
            LangAdjustableTMProUGUIText adjustable;
            if (!registeredTMProUGUITexts.TryGetValue(text, out adjustable))
                adjustable = registeredTMProUGUITexts[text] = text.gameObject.AddComponent<LangAdjustableTMProUGUIText>();

            if (newTextFunc != null) adjustable.newTextFunc = newTextFunc;
        }

        internal static Dictionary<UnityEngine.UI.Dropdown, LangAdjustableUIDropdown> registeredDropdowns = new Dictionary<UnityEngine.UI.Dropdown, LangAdjustableUIDropdown>();
        public static void RegisterDropdown(UnityEngine.UI.Dropdown dropdown, List<System.Func<int, string>> newTextFuncs = null)
        {
            LangAdjustableUIDropdown adjustable;
            if (!registeredDropdowns.TryGetValue(dropdown, out adjustable))
                adjustable = registeredDropdowns[dropdown] = dropdown.gameObject.AddComponent<LangAdjustableUIDropdown>();

            if (newTextFuncs != null) adjustable.newTextFuncs = newTextFuncs;
        }

        internal static readonly Dictionary<TMPro.TextMeshPro, LangAdjustableTextMeshPro> registeredTextMeshPro = new Dictionary<TMPro.TextMeshPro, LangAdjustableTextMeshPro>();
        public static void RegisterTextMeshPro(TMPro.TextMeshPro textMeshPro, Func<int, string> newTextFunc = null)
        {
            if (!registeredTextMeshPro.TryGetValue(textMeshPro, out var adjustable))
            {
                adjustable = registeredTextMeshPro[textMeshPro] = textMeshPro.gameObject.AddComponent<LangAdjustableTextMeshPro>();
            }
            if (newTextFunc != null) adjustable.newTextFunc = newTextFunc;
        }
    }
}
