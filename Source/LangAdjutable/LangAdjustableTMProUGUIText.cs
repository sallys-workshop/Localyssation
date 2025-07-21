
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using static Localyssation.LangAdjutable.LangAdjustables;
using Localyssation.LanguageModule;
using static Localyssation.LanguageModule.Language;

#pragma warning disable IDE0130
namespace Localyssation.LangAdjutable
{
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
            Localyssation.instance.OnLanguageChanged += onLanguageChanged;
        }

        public void Start()
        {
            AdjustToLanguage(LanguageManager.CurrentLanguage);
        }

        private void onLanguageChanged(Language newLanguage)
        {
            AdjustToLanguage(newLanguage);
        }
        private bool ReplaceFontIfMatch(string originalFontName, BundledFontLookupInfo replacementFontLookupInfo)
        {
            if (
                replacementFontLookupInfo != null &&
                FontManager.TMPfonts.TryGetValue(replacementFontLookupInfo.fontName, out var loadedFont)
            )
            {
                if (text.font == loadedFont) return true;
                if (Regex.IsMatch(text.font.name, originalFontName + @"\s*SDF\w*"))
                {
                    text.font = loadedFont;
                    text.fontSize = (int)(orig_fontSize);
                    text.lineSpacing = orig_lineSpacing;
                    fontReplaced = true;
                    return true;
                }
            }
            return false;
        }

        private bool ReplaceFontForPath(string path, BundledFontLookupInfo replacementFontLookupInfo)
        {
            if (Util.GetPath(text.transform) == path)
            {
                if (
                    replacementFontLookupInfo != null &&
                    FontManager.TMPfonts.TryGetValue(replacementFontLookupInfo.fontName, out var loadedFont))
                {
                    if (text.font == loadedFont) return true;
                    text.font = loadedFont;
                    text.fontSize = (int)(orig_fontSize);
                    text.lineSpacing = orig_lineSpacing;
                    fontReplaced = true;
                    return true;
                }
            }
            return false;
        }

        public void AdjustToLanguage(Language newLanguage)
        {
            bool TryReplaceFont()
            {
                return newLanguage.info.fontReplacement.Select(kvPair =>
                {
                    string originalFontName = kvPair.Key;
                    BundledFontLookupInfo replacementFontLookupInfo = kvPair.Value;
                    return ReplaceFontIfMatch(originalFontName, replacementFontLookupInfo);
                })
                    .Concat(
                        newLanguage.info.componentSpecifiedFontReplacement.Select(kvPair =>
                        {
                            string path = kvPair.Key;
                            BundledFontLookupInfo replacementFontLookupInfo = kvPair.Value;
                            return ReplaceFontForPath(path, replacementFontLookupInfo);
                        })
                    )
                    .Any(b => b);
            }

            var fontReplacedThisTime = false;
            if (!fontReplaced)
            {
                orig_font = text.font;
                orig_fontSize = text.fontSize;
                orig_lineSpacing = text.lineSpacing;
            }
            if (TryReplaceFont())
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
            Localyssation.instance.OnLanguageChanged -= onLanguageChanged;
            registeredTMProUGUITexts.Remove(text);
        }
    }

}