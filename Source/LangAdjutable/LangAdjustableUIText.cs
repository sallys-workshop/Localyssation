
using System.Linq;
using UnityEngine;
using static Localyssation.LangAdjutable.LangAdjustables;
using Localyssation.LanguageModule;
using static Localyssation.LanguageModule.Language;
using Localyssation.Util;

namespace Localyssation.LangAdjutable
{

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

        private bool ReplaceFontIfMatch(string originalFontName, Language.BundledFontLookupInfo replacementFontLookupInfo)
        {
            if (
                replacementFontLookupInfo != null &&
                FontManager.Fonts.TryGetValue(replacementFontLookupInfo.fontName, out var loadedFont))
            {
                if (text.font == loadedFont) return true;
                if (text.font.name == originalFontName)
                {
                    text.font = loadedFont;
                    text.fontSize = (int)(orig_fontSize * replacementFontLookupInfo.fontScale);
                    text.lineSpacing = (orig_lineSpacing * replacementFontLookupInfo.fontScale);
                    fontReplaced = true;
                    return true;
                }
            }
            return false;
        }

        private bool ReplaceFontForPath(string path, Language.BundledFontLookupInfo replacementFontLookupInfo)
        {
            if (PathUtil.GetPath(text.transform) == path)
            {
                if (
                    replacementFontLookupInfo != null &&
                    FontManager.Fonts.TryGetValue(replacementFontLookupInfo.fontName, out var loadedFont))
                {
                    if (text.font == loadedFont) return true;
                    text.font = loadedFont;
                    text.fontSize = (int)(orig_fontSize * replacementFontLookupInfo.fontScale);
                    text.lineSpacing = (orig_lineSpacing * replacementFontLookupInfo.fontScale);
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
                text.fontSize = (int)(orig_fontSize * newLanguage.info.chatFont.fontScale);
                text.lineSpacing = (orig_lineSpacing * newLanguage.info.chatFont.fontScale);
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
                    text.resizeTextMaxSize = (int)(orig_resizeTextMaxSize * newLanguage.info.chatFont.fontScale);
                    text.resizeTextMinSize = (int)(orig_resizeTextMinSize * newLanguage.info.chatFont.fontScale);

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
            Localyssation.instance.OnLanguageChanged -= onLanguageChanged;
            registeredTexts.Remove(PathUtil.GetPath(text.transform));
        }
    }

}