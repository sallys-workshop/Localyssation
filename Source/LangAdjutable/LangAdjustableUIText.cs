
using System.Linq;
using UnityEngine;
using static Localyssation.LangAdjutable.LangAdjustables;
using static Localyssation.Language;

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

        private bool ReplaceFontIfMatch(string originalFontName, Language.BundledFontLookupInfo replacementFontLookupInfo)
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

        private bool ReplaceFontForPath(string path, Language.BundledFontLookupInfo replacementFontLookupInfo)
        {
            if (Util.GetPath(text.transform) == path)
            {
                if (
                    replacementFontLookupInfo != null &&
                    Localyssation.fontBundles.TryGetValue(replacementFontLookupInfo.bundleName, out var fontBundle) &&
                    fontBundle.loadedFonts.TryGetValue(replacementFontLookupInfo.fontName, out var loadedFont))
                {
                    if (text.font == loadedFont.uguiFont) return true;
                    text.font = loadedFont.uguiFont;
                    text.fontSize = (int)(orig_fontSize * loadedFont.info.sizeMultiplier);
                    text.lineSpacing = orig_lineSpacing * loadedFont.info.sizeMultiplier;
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
                Localyssation.logger.LogDebug("UIText font:" + text.font.name);
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

}