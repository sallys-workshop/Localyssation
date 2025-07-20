using BepInEx;
using HarmonyLib;
using Localyssation.LangAdjutable;
using Localyssation.Patches.ReplaceFont;
using Localyssation.Patches.ReplaceText;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Security.Permissions;
using UnityEngine;

#pragma warning disable CS0618

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace Localyssation
{
    [BepInDependency(Nessie.ATLYSS.EasySettings.MyPluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Localyssation : BaseUnityPlugin
    {

        public static Localyssation instance;
        public static Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);


        internal static System.Reflection.Assembly assembly;
        internal static string dllPath;

        public static Language defaultLanguage;
        public static Language currentLanguage;
        public static Dictionary<string, Language> languages = new Dictionary<string, Language>();
        public static readonly List<Language> languagesList = new List<Language>();

        public static Dictionary<string, FontBundle> fontBundles = new Dictionary<string, FontBundle>();
        public static readonly List<FontBundle> fontBundlesList = new List<FontBundle>();
        

        public event System.Action<Language> onLanguageChanged;
        internal void CallOnLanguageChanged(Language newLanguage) { onLanguageChanged?.Invoke(newLanguage); }

        internal static BepInEx.Logging.ManualLogSource logger;
        internal static BepInEx.Configuration.ConfigFile config;

        internal static BepInEx.Configuration.ConfigEntry<string> configLanguage;
        internal static BepInEx.Configuration.ConfigEntry<bool> configTranslatorMode;
        internal static BepInEx.Configuration.ConfigEntry<bool> configCreateDefaultLanguageFiles;
        internal static BepInEx.Configuration.ConfigEntry<bool> configShowTranslationKey;
        internal static BepInEx.Configuration.ConfigEntry<bool> configExportExtra;
        internal static BepInEx.Configuration.ConfigEntry<KeyCode> configReloadLanguageKeybind;

        internal static bool settingsTabReady = false;
        internal static bool languagesLoaded = false;
        internal static bool settingsTabSetup = false;
        internal static Nessie.ATLYSS.EasySettings.UIElements.AtlyssDropdown languageDropdown;


//#pragma warning disable IDE0051 // Suppress unused private method warning, this method is used by BepInEx
        private void Awake()
        {
            instance = this;
            logger = Logger;
            config = Config;

            assembly = System.Reflection.Assembly.GetExecutingAssembly();
            dllPath = new System.Uri(assembly.CodeBase).LocalPath;

            //GameAssetCache.Load();

            defaultLanguage = CreateDefaultLanguage();
            RegisterLanguage(defaultLanguage);
            ChangeLanguage(defaultLanguage);
            LoadLanguagesFromFileSystem();
            //ExportUtil.InitExports();
            FontManager.LoadFontBundlesFromFileSystem();
            FontHelper.DetectVanillaFonts();

            configLanguage = config.Bind("General", "Language", defaultLanguage.info.code, "Currently selected language's code");
            if (languages.TryGetValue(configLanguage.Value, out var previouslySelectedLanguage))
                ChangeLanguage(previouslySelectedLanguage);

            configTranslatorMode = config.Bind("Translators", "Translator Mode", false, "Enables the features of this section");
            configCreateDefaultLanguageFiles = config.Bind("Translators", "Create Default Language Files On Load", true, "If enabled, files for the default game language will be created in the mod's directory on game load");
            configReloadLanguageKeybind = config.Bind("Translators", "Reload Language Keybind", KeyCode.F10, "When you press this button, your current language's files will be reloaded mid-game");
            configShowTranslationKey = config.Bind("Translators", "Show Translation Key", false, "Show translation keys instead of translated string for debugging.");
            configExportExtra = config.Bind("Translators", "Export Extra Info", false, "Export quest and item data and image to markdown for translation referencing.");

            Nessie.ATLYSS.EasySettings.Settings.OnInitialized.AddListener(() =>
            {
                settingsTabReady = true;
                TrySetupSettingsTab();
            });

            Nessie.ATLYSS.EasySettings.Settings.OnApplySettings.AddListener(() =>
            {

            });


            harmony.PatchAll();
            harmony.PatchAll(typeof(Patches.GameLoadPatches));
            FRUtil.PatchAll(harmony);
            RTUtil.PatchAll(harmony);
            OnSceneLoaded.Init();
            LangAdjustables.Init();
        }
//#pragma warning restore IDE0051
        private static void TrySetupSettingsTab()
        {
            if (settingsTabSetup || !settingsTabReady || !languagesLoaded) return;
            settingsTabSetup = true;

            var tab = Nessie.ATLYSS.EasySettings.Settings.ModTab;

            tab.AddHeader("Localyssation");

            var languageNames = new List<string>();
            var currentLanguageIndex = 0;
            for (var i = 0; i < languagesList.Count; i++)
            {
                var language = languagesList[i];
                languageNames.Add(language.info.name);
                if (language == currentLanguage) currentLanguageIndex = i;
            }
            languageDropdown = tab.AddDropdown("Language", languageNames, currentLanguageIndex);
            languageDropdown.OnValueChanged.AddListener((valueIndex) =>
            {
                var language = languagesList[valueIndex];
                ChangeLanguage(language);
                configLanguage.Value = language.info.code;
            });
            LangAdjustables.RegisterText(languageDropdown.Label, LangAdjustables.GetStringFunc("SETTINGS_NETWORK_CELL_LOCALYSSATION_LANGUAGE", languageDropdown.LabelText));

            tab.AddToggle(configTranslatorMode);
            if (configTranslatorMode.Value)
            {
                var showTranslationKeyToggle = tab.AddToggle(configShowTranslationKey);
                showTranslationKeyToggle.OnValueChanged.AddListener((v) => 
                {
                    ChangeLanguage(currentLanguage);    // refresh all
                });

                tab.AddToggle(configCreateDefaultLanguageFiles);
                tab.AddToggle(configExportExtra);
                tab.AddKeyButton(configReloadLanguageKeybind);
                tab.AddButton("Add Missing Keys to Current Language", () =>
                {
                    foreach (var kvp in defaultLanguage.strings)
                    {
                        if (!currentLanguage.strings.ContainsKey(kvp.Key))
                        {
                            currentLanguage.strings[kvp.Key] = kvp.Value;
                        }
                    }
                    currentLanguage.WriteToFileSystem();
                });
                tab.AddButton("Log Untranslated Strings", () =>
                {
                    var changedCount = 0;
                    var totalCount = 0;
                    logger.LogMessage($"Logging strings that are the same in {defaultLanguage.info.name} and {currentLanguage.info.name}:");
                    foreach (var kvp in currentLanguage.strings)
                    {
                        if (defaultLanguage.strings.TryGetValue(kvp.Key, out var valueInDefaultLanguage))
                        {
                            totalCount += 1;
                            if (kvp.Value == valueInDefaultLanguage) logger.LogMessage(kvp.Key);
                            else changedCount += 1;
                        }
                    }
                    logger.LogMessage($"Done! {changedCount}/{totalCount} ({((float)changedCount / (float)totalCount * 100f):0.00}%) strings are different between the languages.");
                });
            }
        }

#pragma warning disable IDE0051 // Suppress unused private method warning, this method is used by BepInEx
        private void Update()
        {
            if (configTranslatorMode.Value)
            {
                if (UnityInput.Current.GetKeyDown(configReloadLanguageKeybind.Value))
                {
                    currentLanguage.LoadFromFileSystem(true);
                    CallOnLanguageChanged(currentLanguage);
                }
            }
        }
#pragma warning restore IDE0051

        public static void LoadLanguagesFromFileSystem()
        {
            var filePaths = Directory.GetFiles(Paths.PluginPath, "localyssationLanguage.json", SearchOption.AllDirectories);
            foreach (var filePath in filePaths)
            {
                var langPath = Path.GetDirectoryName(filePath);

                var loadedLanguage = new Language
                {
                    fileSystemPath = langPath
                };
                if (loadedLanguage.LoadFromFileSystem())
                    RegisterLanguage(loadedLanguage);
            }

            languagesLoaded = true;
            TrySetupSettingsTab();
        }

        

        public static void RegisterLanguage(Language language)
        {
            if (languages.ContainsKey(language.info.code)) return;

            languages[language.info.code] = language;
            languagesList.Add(language);
        }

        public static void ChangeLanguage(Language newLanguage)
        {
            if (currentLanguage == newLanguage) return;

            currentLanguage = newLanguage;
            instance.CallOnLanguageChanged(newLanguage);
        }

        internal static Language CreateDefaultLanguage()
        {
            var language = new Language();
            language.info.code = "en-US";
            language.info.name = "English (US)";
            language.fileSystemPath = Path.Combine(Path.GetDirectoryName(dllPath), "defaultLanguage");

            I18nKeys.Init();
            language.strings = I18nKeys.TR_KEYS;
            return language;
        }

        public const string GET_STRING_DEFAULT_VALUE_ARG_UNSPECIFIED = "SAME_AS_KEY";
        public static string GetStringRaw(string key, string defaultValue = GET_STRING_DEFAULT_VALUE_ARG_UNSPECIFIED)
        {
            if (currentLanguage.strings.TryGetValue(key, out string result)) return result;
            if (defaultLanguage.strings.TryGetValue(key, out result)) return result;
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
            if (configShowTranslationKey.Value)
            {
                return key;
            }
            return ApplyTextEditTags(GetStringRaw(key, defaultValue), fontSize);
        }

        public static string GetDefaultString(string key)
        {
            if (!defaultLanguage.strings.TryGetValue(key, out string result))
                result = "";
            return result;
        }
    }


}
