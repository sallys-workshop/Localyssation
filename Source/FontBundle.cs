using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Localyssation
{
    public class FontBundle
    {
        public class FontBundleInfo
        {
            public string bundleName = "";
            public FontInfo[] fontInfos = new FontInfo[] { };
        }

        public class FontInfo
        {
            public string name = "";
            public float sizeMultiplier = 1;
        }

        public class LoadedFont
        {
            public Font uguiFont;
            public TMPro.TMP_FontAsset tmpFont;
            public FontInfo info;
        }

        public FontBundleInfo info = new FontBundleInfo();
        public string fileSystemPath;
        public AssetBundle bundle;
        public Dictionary<string, LoadedFont> loadedFonts = new Dictionary<string, LoadedFont>();

        public bool LoadFromFileSystem()
        {
            if (string.IsNullOrEmpty(fileSystemPath)) return false;

            var infoFilePath = Path.Combine(fileSystemPath, "localyssationFontBundle.json");
            try
            {
                info = JsonConvert.DeserializeObject<FontBundleInfo>(File.ReadAllText(infoFilePath));

                var bundleFilePath = Path.Combine(fileSystemPath, info.bundleName);
                if (!File.Exists(bundleFilePath)) return false;
                bundle = AssetBundle.LoadFromFile(bundleFilePath);
                foreach (var fontInfo in info.fontInfos)
                {
                    var loadedUGUIFont = bundle.LoadAsset<Font>(fontInfo.name);
                    var loadedTMPFont = bundle.LoadAsset<TMPro.TMP_FontAsset>($"{fontInfo.name} SDF");
                    if (loadedUGUIFont && loadedTMPFont)
                    {
                        loadedFonts[fontInfo.name] = new LoadedFont
                        {
                            uguiFont = loadedUGUIFont,
                            tmpFont = loadedTMPFont,
                            info = fontInfo
                        };
                    }
                }
                return true;
            }
            catch (System.Exception e)
            {
                Localyssation.logger.LogError(e);
                return false;
            }
        }
    }

}
