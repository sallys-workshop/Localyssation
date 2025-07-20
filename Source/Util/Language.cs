using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Localyssation
{
    public class Language
    {
        public class LanguageInfo
        {
            public string code = "";
            public string name = "";
            public bool autoShrinkOverflowingText = false;
            //public BundledFontLookupInfo fontReplacementCentaur = new BundledFontLookupInfo();
            //public BundledFontLookupInfo fontReplacementTerminalGrotesque = new BundledFontLookupInfo();
            //public BundledFontLookupInfo fontReplacementLibrationSans = new BundledFontLookupInfo();
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
            public string bundleName = "";
            public string fontName = "";
        }

        public LanguageInfo info = new LanguageInfo();
        public string fileSystemPath;
        public Dictionary<string, string> strings = new Dictionary<string, string>();

        public void RegisterKey(string key, string defaultValue)
        {
            if (strings.ContainsKey(key)) return;
            strings[key] = defaultValue;
        }

        public bool LoadFromFileSystem(bool forceOverwrite = false)
        {
            if (string.IsNullOrEmpty(fileSystemPath)) return false;

            var infoFilePath = Path.Combine(fileSystemPath, "localyssationLanguage.json");
            var stringsFilePath = Path.Combine(fileSystemPath, "strings.tsv");
            try
            {
                info = JsonConvert.DeserializeObject<LanguageInfo>(File.ReadAllText(infoFilePath));

                foreach (var tsvRow in TSVUtil.parseTsvWithHeaders(File.ReadAllText(stringsFilePath)))
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

                var stringsFilePath = Path.Combine(fileSystemPath, "strings.tsv");
                var tsvRows = strings.Select(x => new List<string>() { x.Key, x.Value }).ToList();
                tsvRows.Insert(0, new List<string>() { "key", "value" });
                File.WriteAllText(stringsFilePath, TSVUtil.makeTsv(tsvRows));

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
