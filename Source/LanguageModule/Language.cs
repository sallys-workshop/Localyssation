using BepInEx;
using HarmonyLib;
using Localyssation.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Localyssation.LanguageModule
{
    public class Language
    {
        public class LanguageInfo
        {
            public string code = "";
            public string name = "";
            public bool autoShrinkOverflowingText = false;
            public BundledFontLookupInfo chatFont = new BundledFontLookupInfo();
            public Dictionary<string, BundledFontLookupInfo> fontReplacement
                = Enum.GetValues(typeof(VanillaFonts)).Cast<VanillaFonts>()
                .ToDictionary(
                    vanillaFont => vanillaFont.GetDescription(),
                    vanillaFont => new BundledFontLookupInfo()
                );
            public Dictionary<string, BundledFontLookupInfo> componentSpecifiedFontReplacement = new Dictionary<string, BundledFontLookupInfo>() {
                { "__some___example/_gameobject/_path", new BundledFontLookupInfo() }
            };

        }

        public class BundledFontLookupInfo
        {
            //public string bundleName = "";
            public string fontName = "";
            public float fontScale = 1.0f;
            //public bool asFallbackToVanilla = false;    // unused
        }

        private static readonly ISerializer YAML_SERIALIZER = new SerializerBuilder().WithDefaultScalarStyle(ScalarStyle.DoubleQuoted).Build();
        private static readonly IDeserializer YAML_DESERIALIZER = new DeserializerBuilder().Build();

        public LanguageInfo info = new LanguageInfo();
        public string fileSystemPath;
        readonly ConcurrentDictionary<string, string> strings = new ConcurrentDictionary<string, string>();

        public IDictionary<string, string> GetStrings() => strings;

        public void RegisterKey(string key, string defaultValue)
        {
            if (strings.ContainsKey(key))
            {
                if (defaultValue != strings[key])
                    Localyssation.logger.LogWarning($"Duplicate localisation key `{key}` in language `{info.name}`({info.code})");
                return;
            }
            strings[key] = defaultValue;
        }

        public bool TryGetString(string key, out string value)
        {
            return strings.TryGetValue(key, out value);
        }

        public bool ContainsKey(string key)
        {
            return strings.ContainsKey(key);
        }

        /// <summary>
        /// Parallel load
        /// </summary>
        /// <param name="forceOverwrite"></param>
        /// <returns></returns>
        public bool LoadFromFileSystem(bool forceOverwrite = false)
        {
            if (string.IsNullOrEmpty(fileSystemPath)) return false;

            var infoFilePath = Path.Combine(fileSystemPath, "localyssationLanguage.json");
            try
            {
                info = JsonConvert.DeserializeObject<LanguageInfo>(File.ReadAllText(infoFilePath));
                Localyssation.logger.LogMessage($"Loading language name={info.name}, id={info.code}");
            }
            catch (Exception e)
            {
                Localyssation.logger.LogError(e);
                return false;
            }
            if (info.code == LanguageManager.DefaultLanguage.info.code) return false;   // skip default language
            try
            {
                bool foundYML = false;
                Action<KeyValuePair<string, string>> registerAction;
                if (!forceOverwrite)
                {
                    registerAction = kv => RegisterKey(kv.Key, kv.Value);
                }
                else
                {
                    registerAction = kv => strings[kv.Key] = kv.Value;
                }
                Directory.GetFiles(Paths.PluginPath, $"*.{info.code}.yml", SearchOption.AllDirectories)
                    .OrderBy(x => Path.GetFileNameWithoutExtension(x))
                    .Do(
                stringsFilePath =>
                {
                    Localyssation.logger.LogMessage($"Found translation file {stringsFilePath}");
                    var file = File.OpenText(stringsFilePath);
                    Parallel.ForEach(YAML_DESERIALIZER.Deserialize<Dictionary<string, string>>(file), registerAction);
                    file.Close();
                    foundYML = true;
                });
                if (foundYML)
                    return true;
            }
            catch (Exception e)
            {
                Localyssation.logger.LogError(e);
            }

            var stringsFilePathTSV = Path.Combine(fileSystemPath, "strings.tsv");
            try
            {
                Localyssation.logger.LogMessage($"Parsing legacy file {stringsFilePathTSV}");
                foreach (var tsvRow in TSVUtil.parseTsvWithHeaders(File.ReadAllText(stringsFilePathTSV)))
                {
                    if (!forceOverwrite) RegisterKey(tsvRow["key"], tsvRow["value"]);
                    else strings[tsvRow["key"]] = tsvRow["value"];
                }

                return true;
            }
            catch (Exception e)
            {
                Localyssation.logger.LogError(e);
                return false;
            }
        }

        public bool WriteToFileSystem(string fileName, bool noLangCode = false)
        {
            if (string.IsNullOrEmpty(fileSystemPath)) return false;

            try
            {
                Directory.CreateDirectory(fileSystemPath);
                var infoFilePath = Path.Combine(fileSystemPath, "localyssationLanguage.json");
                File.WriteAllText(infoFilePath, JsonConvert.SerializeObject(info, Formatting.Indented));
                string translationFilePath = Path.Combine(fileSystemPath, noLangCode ? $"{fileName}.yml" : $"{fileName}.{info.code}.yml");
                var file = new StreamWriter(translationFilePath);
                YAML_SERIALIZER.Serialize(file, strings, typeof(Dictionary<string, string>));
                file.Close();
                return true;
            }
            catch (Exception e)
            {
                Localyssation.logger.LogError(e);
                return false;
            }
        }
    }

}
