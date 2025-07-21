using BepInEx;
using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Core;

namespace Localyssation
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
        public Dictionary<string, string> strings = new Dictionary<string, string>();

        public void RegisterKey(string key, string defaultValue)
        {
            if (strings.ContainsKey(key))
            {
                Localyssation.logger.LogWarning($"Duplicate localisation key `{key}` in language `{info.name}`({info.code})");
                return;
            }
            strings[key] = defaultValue;
        }

        public bool LoadFromFileSystem(bool forceOverwrite = false)
        {
            if (string.IsNullOrEmpty(fileSystemPath)) return false;

            var infoFilePath = Path.Combine(fileSystemPath, "localyssationLanguage.json");
            try
            {
                info = JsonConvert.DeserializeObject<LanguageInfo>(File.ReadAllText(infoFilePath));
            }
            catch (Exception e)
            {
                Localyssation.logger.LogError(e);
                return false;
            }
            try
            {
                Directory.GetFiles(Paths.PluginPath, $"*.{info.code}.yml").Do(stringsFilePath =>
                {
                    var file = File.OpenText(stringsFilePath);
                    YAML_DESERIALIZER.Deserialize<Dictionary<string, string>>(file)
                        .Do(kv =>
                        {
                            if (!forceOverwrite)
                            {
                                RegisterKey(kv.Key, kv.Value);
                            }
                            else
                            {
                                strings[kv.Key] = kv.Value;
                            }
                        });
                    file.Close();
                });
            }
            catch (Exception e)
            {
                Localyssation.logger.LogError(e);
                return false;
            }

            var stringsFilePathTSV = Path.Combine(fileSystemPath, "strings.tsv");
            try
            {
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

        public bool WriteToFileSystem()
        {
            if (string.IsNullOrEmpty(fileSystemPath)) return false;

            try
            {
                Directory.CreateDirectory(fileSystemPath);

                var infoFilePath = Path.Combine(fileSystemPath, "localyssationLanguage.json");
                File.WriteAllText(infoFilePath, JsonConvert.SerializeObject(info, Formatting.Indented));

                //var stringsFilePath = Path.Combine(fileSystemPath, "strings.tsv");
                //var tsvRows = strings.Select(x => new List<string>() { x.Key, x.Value }).ToList();
                //tsvRows.Insert(0, new List<string>() { "key", "value" });
                //File.WriteAllText(stringsFilePath, TSVUtil.makeTsv(tsvRows));
                string translationFilePath = Path.Combine(fileSystemPath, "strings.yml");
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
