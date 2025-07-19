using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Localyssation
{
    public class FontBundle
    {
        public class MetaFontBundleInfo
        {
            public string bundleName;
            public FontBundleInfo[] fontBundles;
        }
        public class FontBundleInfo
        {
            public string bundleFile = "";
            public FontInfo[] fontInfos = new FontInfo[] { };

            public class FontInfo
            {
                public string name = "";
                public string tmpVariant = "";    // optional
                public float sizeMultiplier = 1;
            }

        }
        public class LoadedFont
        {
            public Font uguiFont;
            public TMP_FontAsset tmpFont;
            public FontBundleInfo.FontInfo info;

            public bool isValid()
            {
                return uguiFont != null && tmpFont != null && info != null;
            }
        }

        public string fileSystemPath;
        public MetaFontBundleInfo info;
        public Dictionary<string, LoadedFont> loadedFonts = null;
        public Dictionary<FontBundleInfo, AssetBundle> bundles = null;

        public static void LoadFontBundles()
        {

        }

        public bool LoadFromFileSystem()
        {
            if (string.IsNullOrEmpty(fileSystemPath)) return false;
            var infoFilePath = Path.Combine(fileSystemPath, "localyssationFontBundle.json");
            try
            {
                info = JsonConvert.DeserializeObject<MetaFontBundleInfo>(File.ReadAllText(infoFilePath));
            }
            catch (Exception e)
            {
                Localyssation.logger.LogError(e);
                if (bundles == null)
                    bundles = new Dictionary<FontBundleInfo, AssetBundle>();
                if (loadedFonts == null)
                    loadedFonts = new Dictionary<string, LoadedFont>();
                return false;
            }


            // infos valid
            bundles = info.fontBundles.ToDictionary(info => info,
                info =>
                {
                    try
                    {
                        string bundlePath = Path.Combine(fileSystemPath, info.bundleFile);
                        if (!File.Exists(bundlePath))
                        {
                            Localyssation.logger.LogWarning($"Cannot find assetBundle `{info.bundleFile}` in folder `{fileSystemPath}`.");
                            return null;
                        }
                        return AssetBundle.LoadFromFile(bundlePath);
                    }
                    catch (Exception e)
                    {
                        Localyssation.logger.LogError(e);
                        return null;
                    }
                }
            );

            // Remove errored bundle
            bundles = bundles.Where(kv => kv.Value != null).ToDictionary(kv => kv.Key, kv => kv.Value);

            IEnumerable<KeyValuePair<string, LoadedFont>> loadedFontBuffer = new Dictionary<string, LoadedFont>();
            bundles.Do(kvPair =>
            {
                var bundleInfo = kvPair.Key;
                var bundle = kvPair.Value;
                loadedFontBuffer = loadedFontBuffer.Concat(bundleInfo.fontInfos.ToDictionary(fontInfo => fontInfo.name,
                    fontInfo =>
                    {
                        try
                        {
                            return new LoadedFont()
                            {
                                info = fontInfo,
                                uguiFont = bundle.LoadAsset<Font>(fontInfo.name),
                                tmpFont = bundle.LoadAsset<TMP_FontAsset>(string.IsNullOrWhiteSpace(fontInfo.tmpVariant) ? fontInfo.name + " SDF" : fontInfo.tmpVariant)
                            };
                        }
                        catch (Exception e)
                        {
                            Localyssation.logger.LogError(e);
                            return null;
                        }
                    }
                ));

                Localyssation.logger.LogInfo($"Loaded font bundle `{bundleInfo.bundleFile}`");
            });
            loadedFonts = loadedFontBuffer.Where(kv => kv.Value.isValid()).ToDictionary(kv => kv.Key, kv => kv.Value);

            return true;
        }

    }

}
