using Localyssation.LanguageModule;
using Localyssation.Util;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using static Localyssation.LangAdjutable.LangAdjustables;

namespace Localyssation.LangAdjutable
{
    public class LangAdjustableTextMeshPro
        : MonoBehaviour, ILangAdjustable
    {

        public TextMeshPro text { get; private set; }
        public System.Func<int, string> newTextFunc;

        public bool fontReplaced = false;
        public float orig_fontSize;
        public float orig_lineSpacing;
        public TMP_FontAsset orig_font;

        public bool textAutoShrinkable = true;
        public bool textAutoShrunk = false;
        public bool orig_resizeTextForBestFit = false;
        public float orig_resizeTextMaxSize;
        public float orig_resizeTextMinSize;

        private void onLanguageChanged(Language newLanguage)
        {
            AdjustToLanguage(newLanguage);
        }
        public void Awake()
        {
            text = GetComponent<TextMeshPro>();
            //text.verticalOverflow = VerticalWrapMode.Overflow;
            Localyssation.instance.OnLanguageChanged += onLanguageChanged;
        }

        public void Start()
        {
            AdjustToLanguage(LanguageManager.CurrentLanguage);
        }

        private bool ReplaceFontIfMatch(string originalFontName, Language.BundledFontLookupInfo replacementFontLookupInfo)
        {
            if (GetLoadedFont(replacementFontLookupInfo, out var loadedFont))
            {
                if (text.font == loadedFont) return true;
                if (Regex.IsMatch(text.font.name, originalFontName + @"\s*SDF\w*"))
                {
                    ReplaceFont(loadedFont);
                    return true;
                }
            }
            return false;
        }

        private bool ReplaceFontForPath(string path, Language.BundledFontLookupInfo replacementFontLookupInfo)
        {
            if (PathUtil.GetPath(text.transform) == path)
            {
                if (GetLoadedFont(replacementFontLookupInfo, out var loadedFont))
                {
                    if (text.font != loadedFont)
                    {
                        ReplaceFont(loadedFont);
                    }
                    return true;
                }
            }
            return false;
        }

        private bool ReplaceFontAlways(Language.BundledFontLookupInfo replacementFontLookupInfo)
        {
            if (GetLoadedFont(replacementFontLookupInfo, out var loadedFont))
            {
                if (text.font != loadedFont)
                {
                    ReplaceFont(loadedFont);
                }
                return true;
            }
            return false;
        }

        private static bool GetLoadedFont(Language.BundledFontLookupInfo replacementFontLookupInfo, out TMP_FontAsset loadedFont)
        {
            if (replacementFontLookupInfo != null)
            {
                return FontManager.TMPfonts.TryGetValue(replacementFontLookupInfo.fontName, out loadedFont);
            }
            loadedFont = null;
            return false;
        }

        private void ReplaceFont(TMP_FontAsset loadedFont)
        {
            text.font = loadedFont;
            text.fontSize = (int)(orig_fontSize);
            text.lineSpacing = orig_lineSpacing;
            fontReplaced = true;
        }

        public void AdjustToLanguage(Language newLanguage)
        {
            bool TryReplaceFont()
            {
                return ReplaceFontAlways(newLanguage.info.chatFont);
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
                text.fontSize = (orig_fontSize * newLanguage.info.chatFont.fontScale);
                text.lineSpacing = (orig_lineSpacing * newLanguage.info.chatFont.fontScale);
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

        public void OnDestory()
        {
            Localyssation.instance.OnLanguageChanged -= onLanguageChanged;
            registeredTextMeshPro.Remove(text);
        }
    }
}