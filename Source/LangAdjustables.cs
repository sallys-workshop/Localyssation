using System.Collections.Generic;
using UnityEngine;

namespace Localyssation
{
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

        public interface ILangAdjustable
        {
            void AdjustToLanguage(Language newLanguage);
        }

        public class LangAdjustableUIText : MonoBehaviour, ILangAdjustable
        {
            public UnityEngine.UI.Text text;
            public System.Func<int, string> newTextFunc;

            public bool fontReplaced = false;
            public int orig_fontSize;
            public float orig_lineSpacing;
            public Font orig_font;

            public bool textAutoShrinkable = true;
            public bool textAutoShrunk = false;
            public bool orig_resizeTextForBestFit = false;
            public int orig_resizeTextMaxSize;
            public int orig_resizeTextMinSize;


            public void Awake()
            {
                text = GetComponent<UnityEngine.UI.Text>();
                text.verticalOverflow = VerticalWrapMode.Overflow;
                Localyssation.instance.onLanguageChanged += onLanguageChanged;
            }

            public void Start()
            {
                AdjustToLanguage(Localyssation.currentLanguage);
            }

            private void onLanguageChanged(Language newLanguage)
            {
                AdjustToLanguage(newLanguage);
            }

            public void AdjustToLanguage(Language newLanguage)
            {
                bool TryReplaceFont(string originalFontName, Language.BundledFontLookupInfo replacementFontLookupInfo)
                {
                    
                    if (
                        replacementFontLookupInfo != null &&
                        Localyssation.fontBundles.TryGetValue(replacementFontLookupInfo.bundleName, out var fontBundle) &&
                        fontBundle.loadedFonts.TryGetValue(replacementFontLookupInfo.fontName, out var loadedFont))
                    {
                        if (text.font == loadedFont.uguiFont) return true;
                        if (text.font.name == originalFontName)
                        {
                            text.font = loadedFont.uguiFont;
                            text.fontSize = (int)(orig_fontSize * loadedFont.info.sizeMultiplier);
                            text.lineSpacing = orig_lineSpacing * loadedFont.info.sizeMultiplier;
                            fontReplaced = true;
                            return true;
                        }
                    }
                    return false;
                }

                var fontReplacedThisTime = false;
                if (!fontReplaced)
                {
                    orig_font = text.font;
                    orig_fontSize = text.fontSize;
                    orig_lineSpacing = text.lineSpacing;
                }
                if (
                    TryReplaceFont(
                        Localyssation.VanillaFonts.CENTAUR, 
                        Localyssation.currentLanguage.info.fontReplacementCentaur
                        ) 
                    || TryReplaceFont(
                        Localyssation.VanillaFonts.TERMINAL_GROTESQUE, 
                        Localyssation.currentLanguage.info.fontReplacementTerminalGrotesque
                        )
                    || TryReplaceFont(
                        Localyssation.VanillaFonts.LIBRATION_SANS,
                        Localyssation.currentLanguage.info.fontReplacementLibrationSans
                        )
                    )
                {
                    fontReplacedThisTime = true;
                }
                if (!fontReplacedThisTime && fontReplaced)
                {
                    fontReplaced = false;
                    text.font = orig_font;
                    text.fontSize = orig_fontSize;
                    text.lineSpacing = orig_lineSpacing;
                }

                if (newLanguage.info.autoShrinkOverflowingText != textAutoShrunk)
                {
                    if (newLanguage.info.autoShrinkOverflowingText)
                    {
                        if (textAutoShrinkable)
                        {
                            orig_resizeTextForBestFit = text.resizeTextForBestFit;
                            orig_resizeTextMaxSize = text.resizeTextMaxSize;
                            orig_resizeTextMinSize = text.resizeTextMinSize;

                            text.resizeTextMaxSize = text.fontSize;
                            text.resizeTextMinSize = System.Math.Min(2, text.resizeTextMinSize);
                            text.resizeTextForBestFit = true;

                            textAutoShrunk = true;
                        }
                    }
                    else
                    {
                        text.resizeTextForBestFit = orig_resizeTextForBestFit;
                        text.resizeTextMaxSize = orig_resizeTextMaxSize;
                        text.resizeTextMinSize = orig_resizeTextMinSize;

                        textAutoShrunk = false;
                    }
                }

                if (newTextFunc != null)
                {
                    text.text = newTextFunc(text.fontSize);
                }
            }

            public void OnDestroy()
            {
                Localyssation.instance.onLanguageChanged -= onLanguageChanged;
                registeredTexts.Remove(text);
            }
        }

        public class LangAdjustableTMProUGUIText : MonoBehaviour, ILangAdjustable
        {
            public TMPro.TextMeshProUGUI text;
            public System.Func<int, string> newTextFunc;

            public bool fontReplaced = false;
            public float orig_fontSize;
            public float orig_lineSpacing;
            public TMPro.TMP_FontAsset orig_font;

            public bool textAutoShrinkable = true;
            public bool textAutoShrunk = false;
            public bool orig_resizeTextForBestFit = false;
            public float orig_resizeTextMaxSize;
            public float orig_resizeTextMinSize;

            public static List<string> FONT_NAMES = new List<string>();

            public void Awake()
            {
                text = GetComponent<TMPro.TextMeshProUGUI>();
                Localyssation.instance.onLanguageChanged += onLanguageChanged;
            }

            public void Start()
            {
                AdjustToLanguage(Localyssation.currentLanguage);
            }

            private void onLanguageChanged(Language newLanguage)
            {
                AdjustToLanguage(newLanguage);
            }

            public void AdjustToLanguage(Language newLanguage)
            {
                bool TryReplaceFont(string originalFontName, Language.BundledFontLookupInfo replacementFontLookupInfo)
                {
                    if (
                        replacementFontLookupInfo != null &&
                        Localyssation.fontBundles.TryGetValue(replacementFontLookupInfo.bundleName, out var fontBundle) &&
                        fontBundle.loadedFonts.TryGetValue(replacementFontLookupInfo.fontName, out var loadedFont))
                    {
                        
                        if (text.font == loadedFont.tmpFont) return true;
                        if (text.font.name == originalFontName)
                        {
                            text.font = loadedFont.tmpFont;
                            text.fontSize = (int)(orig_fontSize * loadedFont.info.sizeMultiplier);
                            text.lineSpacing = orig_lineSpacing * loadedFont.info.sizeMultiplier;
                            fontReplaced = true;
                            return true;
                        }
                    }
                    return false;
                }

                var fontReplacedThisTime = false;
                if (!fontReplaced)
                {
                    orig_font = text.font;
                    orig_fontSize = text.fontSize;
                    orig_lineSpacing = text.lineSpacing;
                }
                if (
                    TryReplaceFont(
                        Localyssation.VanillaFonts.CENTAUR, 
                        Localyssation.currentLanguage.info.fontReplacementCentaur
                        ) 
                    || TryReplaceFont(
                        Localyssation.VanillaFonts.TERMINAL_GROTESQUE, 
                        Localyssation.currentLanguage.info.fontReplacementTerminalGrotesque
                        )
                    || TryReplaceFont(
                        Localyssation.VanillaFonts.LIBRATION_SANS,
                        Localyssation.currentLanguage.info.fontReplacementLibrationSans
                        )
                    )
                {
                    fontReplacedThisTime = true;
                }
                if (!fontReplacedThisTime && fontReplaced)
                {
                    fontReplaced = false;
                    text.font = orig_font;
                    text.fontSize = orig_fontSize;
                    text.lineSpacing = orig_lineSpacing;
                }

                if (newLanguage.info.autoShrinkOverflowingText != textAutoShrunk)
                {
                    if (newLanguage.info.autoShrinkOverflowingText)
                    {
                        if (textAutoShrinkable)
                        {
                            orig_resizeTextForBestFit = text.enableAutoSizing;
                            orig_resizeTextMaxSize = text.fontSizeMax;
                            orig_resizeTextMinSize = text.fontSizeMin;

                            text.fontSizeMax = text.fontSize;
                            text.fontSizeMin = System.Math.Min(2, text.fontSizeMin);
                            text.enableAutoSizing = true;

                            textAutoShrunk = true;
                        }
                    }
                    else
                    {
                        text.enableAutoSizing = orig_resizeTextForBestFit;
                        text.fontSizeMax = orig_resizeTextMaxSize;
                        text.fontSizeMin = orig_resizeTextMinSize;

                        textAutoShrunk = false;
                    }
                }

                if (newTextFunc != null)
                {
                    text.text = newTextFunc((int)text.fontSize);
                }
            }

            public void OnDestroy()
            {
                Localyssation.instance.onLanguageChanged -= onLanguageChanged;
                registeredTMProUGUITexts.Remove(text);
            }
        }

        public class LangAdjustableUIDropdown : MonoBehaviour, ILangAdjustable
        {
            public UnityEngine.UI.Dropdown dropdown;
            public List<System.Func<int, string>> newTextFuncs;

            public void Awake()
            {
                dropdown = GetComponent<UnityEngine.UI.Dropdown>();
                Localyssation.instance.onLanguageChanged += onLanguageChanged;

                // auto-shrink text according to options
                if (dropdown.itemText)
                {
                    dropdown.itemText.gameObject.AddComponent<LangAdjustableUIText>();
                }
                if (dropdown.captionText)
                {
                    dropdown.captionText.gameObject.AddComponent<LangAdjustableUIText>();
                }
            }

            public void Start()
            {
                AdjustToLanguage(Localyssation.currentLanguage);
            }

            private void onLanguageChanged(Language newLanguage)
            {
                AdjustToLanguage(newLanguage);
            }

            public void AdjustToLanguage(Language newLanguage)
            {
                if (newTextFuncs != null && newTextFuncs.Count == dropdown.options.Count)
                {
                    for (var i = 0; i < dropdown.options.Count; i++)
                    {
                        var option = dropdown.options[i];
                        option.text = newTextFuncs[i](-1);
                    }
                    dropdown.RefreshShownValue();
                }
            }

            public void OnDestroy()
            {
                Localyssation.instance.onLanguageChanged -= onLanguageChanged;
                registeredDropdowns.Remove(dropdown);
            }
        }
    }
}
