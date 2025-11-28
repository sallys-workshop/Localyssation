using BepInEx;
using HarmonyLib;
using Localyssation.LangAdjutable;
using Localyssation.LanguageModule;
using Localyssation.Patches.ReplaceFont;
using Localyssation.Patches.ReplaceText;
using Localyssation.Util;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
using UnityEngine;
using System.Linq;

#pragma warning disable CS0618

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace Localyssation
{
    [BepInDependency(Nessie.ATLYSS.EasySettings.MyPluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    //using Language = Language.Language;
    public class Localyssation : BaseUnityPlugin
    {

        public static Localyssation instance;
        public static bool ShowTranslation { get; private set; } = true;


        internal static System.Reflection.Assembly assembly;
        internal static string dllPath;


        public event System.Action<Language> OnLanguageChanged;
        internal void CallOnLanguageChanged(Language newLanguage) { OnLanguageChanged?.Invoke(newLanguage); }

        internal static BepInEx.Logging.ManualLogSource logger;

        internal static bool settingsTabReady = false;
        internal static bool languagesLoaded = false;
        internal static bool settingsTabSetup = false;


#pragma warning disable IDE0051 // Suppress unused private method warning, this method is used by BepInEx
        private void Awake()
        {
            instance = this;
            logger = Logger;

            assembly = System.Reflection.Assembly.GetExecutingAssembly();
            dllPath = new System.Uri(assembly.CodeBase).LocalPath;

            //GameAssetCache.Load();

            LanguageManager.Init();
            FontManager.LoadFontBundlesFromFileSystem();

            LocalyssationConfig.Init(Config);

            if (LocalyssationConfig.TranslatorMode && LocalyssationConfig.LogVanillaFonts)
            {
                FontHelper.DetectVanillaFonts();
            }

            SettingsGUI.Init();

            var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            harmony.PatchAll();
            harmony.PatchAll(typeof(Patches.GameLoadPatches));
            FRUtil.PatchAll(harmony);
            RTUtil.PatchAll(harmony);
            SettingsGUI.Init();
            OnSceneLoaded.Init();
            LangAdjustables.Init();
        }

#pragma warning disable IDE0051 // Suppress unused private method warning, this method is used by BepInEx
        private void Update()
        {
            if (LocalyssationConfig.TranslatorMode)
            {
                if (Input.GetKeyDown(LocalyssationConfig.ReloadLanguageKeybind))
                {
                    LanguageManager.CurrentLanguage.LoadFromFileSystem(true);
                    CallOnLanguageChanged(LanguageManager.CurrentLanguage);
                }

                if (Input.GetKeyDown(LocalyssationConfig.ReloadFontBundlesKeybind))
                {
                    FontManager.LoadFontBundlesFromFileSystem();
                    CallOnLanguageChanged(LanguageManager.CurrentLanguage);
                }

                if (Input.GetKeyDown(LocalyssationConfig.SwitchTranslationKeybind))
                {
                    ShowTranslation = !ShowTranslation;
                    CallOnLanguageChanged(LanguageManager.CurrentLanguage);
                }
            }
        }
#pragma warning restore IDE0051

        public const string GET_STRING_DEFAULT_VALUE_ARG_UNSPECIFIED = "SAME_AS_KEY";
        public static string GetStringRaw(string key, string defaultValue = GET_STRING_DEFAULT_VALUE_ARG_UNSPECIFIED)
        {
            string result;
            if (ShowTranslation)
            {
                if (LanguageManager.CurrentLanguage.TryGetString(key, out result)) return result;
            }
            if (LanguageManager.DefaultLanguage.TryGetString(key, out result)) return result;
            return (defaultValue == GET_STRING_DEFAULT_VALUE_ARG_UNSPECIFIED ? key : defaultValue);
        }

        private delegate string TextEditTagFunc(string str, string arg, int fontSize);
        private static readonly Dictionary<string, TextEditTagFunc> textEditTags = new Dictionary<string, TextEditTagFunc>()
        {
            {
                "firstupper",
                (str, arg, fontSize) =>
                {
                    if (str.Length > 0)
                    {
                        var letter = str[0].ToString();
                        str = str.Remove(0, 1);
                        str = str.Insert(0, letter.ToUpper());
                    }
                    return str;
                }
            },
            {
                "firstlower",
                (str, arg, fontSize) =>
                {
                    if (str.Length > 0)
                    {
                        var letter = str[0].ToString();
                        str = str.Remove(0, 1);
                        str = str.Insert(0, letter.ToLower());
                    }
                    return str;
                }
            },
            {
                "scale",
                (str, arg, fontSize) =>
                {
                    if (fontSize > 0) {
                        try {
                            var scale = float.Parse(arg, System.Globalization.CultureInfo.InvariantCulture);
                            str = $"<size={System.Math.Round(fontSize * scale)}>{str}</size>";
                        }
                        catch { }
                    }
                    else
                    {
                        str = $"<scalefallback={arg}>{str}</scalefallback>";
                    }
                    return str;
                }
            },
            {
                "scalefallback",
                (str, arg, fontSize) =>
                {
                    if (fontSize > 0) {
                        try {
                            var scale = float.Parse(arg, System.Globalization.CultureInfo.InvariantCulture);
                            str = $"<size={System.Math.Round(fontSize * scale)}>{str}</size>";
                        }
                        catch { }
                    }
                    return str;
                }
            }
        };
        private static readonly List<string> defaultAppliedTextEditTags = new List<string>() {
            "firstupper", "firstlower", "scale"
        };
        public static string ApplyTextEditTags(string str, int fontSize = -1, List<string> appliedTextEditTags = null)
        {
            if (appliedTextEditTags == null) appliedTextEditTags = defaultAppliedTextEditTags;

            var result = str;

            foreach (var tag in textEditTags)
            {
                if (!appliedTextEditTags.Contains(tag.Key)) continue;

                while (true)
                {
                    //temp fix: result null exception
                    if (result == null)
                    {
                        return "";
                    }
                    
                    // find bounds of the tagged text
                    var openingTagBeginning = $"<{tag.Key}";
                    var openingTagIndex = result.IndexOf(openingTagBeginning);
                    if (openingTagIndex == -1) break;

                    var openingTagEndIndex = result.IndexOf(">", openingTagIndex + openingTagBeginning.Length);
                    if (openingTagEndIndex == -1) break;

                    var closingTag = $"</{tag.Key}>";
                    var closingTagIndex = result.IndexOf(closingTag, openingTagEndIndex + 1);
                    if (closingTagIndex == -1) break;

                    // get the full opening tag string and get arguments (if they exist)
                    var openingTag = result.Substring(openingTagIndex + 1, openingTagEndIndex - 1);
                    var arg = "";
                    if (openingTag.Contains("="))
                    {
                        var split = openingTag.Split('=');
                        if (split.Length == 2) arg = split[1];
                    }

                    // get tagged text
                    var stringInTag = "";
                    if ((openingTagEndIndex + 1) <= (closingTagIndex - 1))
                        stringInTag = result.Substring(openingTagEndIndex + 1, closingTagIndex - openingTagEndIndex - 1);

                    // edit tagged text
                    var editedString = tag.Value(stringInTag, arg, fontSize);

                    // remove tags from the displayed string, and replace tagged text with newly edited text
                    result = result
                        .Remove(closingTagIndex, closingTag.Length)
                        .Remove(openingTagIndex, openingTagEndIndex - openingTagIndex + 1);

                    // replace tagged text
                    result = result
                        .Remove(openingTagIndex, stringInTag.Length)
                        .Insert(openingTagIndex, editedString);
                }
            }

            return result;
        }

        public static string GetString(string key, string defaultValue = GET_STRING_DEFAULT_VALUE_ARG_UNSPECIFIED, int fontSize = -1)
        {
            if (LocalyssationConfig.ShowTranslationKey)
            {
                return key;
            }
            return ApplyTextEditTags(GetStringRaw(key, defaultValue), fontSize);
        }

        public static string GetString(TranslationKey translationKey, string defaultValue = GET_STRING_DEFAULT_VALUE_ARG_UNSPECIFIED, int fontSize = -1)
        {
            if (LocalyssationConfig.ShowTranslationKey)
            {
                return translationKey.ToString();
            }
            return ApplyTextEditTags(GetStringRaw(translationKey.ToString(), defaultValue), fontSize);
        }

        public static string Format(TranslationKey formatKey, params object[] args)
        {
            return string.Format(GetString(formatKey), args.Select(x =>
            {
                if (x is TranslationKey key) return key.Localize();
                return x;
            }).ToArray());
        }

        public static string GetDefaultString(string key)
        {
            if (!LanguageManager.DefaultLanguage.TryGetString(key, out string result))
                result = "";
            return result;
        }
    }


}
