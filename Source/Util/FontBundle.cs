using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using TMPro;
using UnityEngine;
using BepInEx;
using MonoMod.Utils;
using System.Collections.Frozen;

namespace Localyssation.Util
{
    public class FontBundle
    {

        public string fileSystemPath;
        public readonly Dictionary<string, Font> fonts = new Dictionary<string, Font>();
        public readonly Dictionary<string, TMP_FontAsset> TMPfonts = new Dictionary<string, TMP_FontAsset>();

        public bool LoadFromFileSystem()
        {
            if (string.IsNullOrEmpty(fileSystemPath)) return false;

            var bundle = AssetBundle.LoadFromFile(fileSystemPath);

            Localyssation.logger.LogInfo($"Loading font bundle `{fileSystemPath}`");

            // Font
            Localyssation.logger.LogInfo("Found Fonts:");
            bundle.LoadAllAssets(typeof(Font)).Cast<Font>().Do(font =>
            {
                Localyssation.logger.LogInfo($"\t- {font.name}");
                fonts.Add(font.name, font);
            });

            // TMP_FontAsset
            Localyssation.logger.LogInfo($"Found TMP_FontAsset:");
            bundle.LoadAllAssets(typeof(TMP_FontAsset)).Cast<TMP_FontAsset>().Do(font =>
            {
                Localyssation.logger.LogInfo($"\t- {font.name}");
                TMPfonts.Add(font.name, font);
            });


            return true;
        }

    }

    public static class FontManager
    {
        private static readonly Dictionary<string, Font> availableFonts = new Dictionary<string, Font>();
        private static readonly Dictionary<string, TMP_FontAsset> availableTMP_FontAssets = new Dictionary<string, TMP_FontAsset>();

        public static FrozenDictionary<string, Font> Fonts { get { return availableFonts.ToFrozenDictionary(); } }
        public static FrozenDictionary<string, TMP_FontAsset> TMPfonts { get { return availableTMP_FontAssets.ToFrozenDictionary(); } }


        public static void LoadFontBundlesFromFileSystem()
        {
            string[] filePaths = Directory.GetFiles(Paths.PluginPath, "*.fontbundle", SearchOption.AllDirectories);
            Localyssation.logger.LogInfo($"Found {filePaths.Length} fontBundles");
            foreach (var filePath in filePaths)
            {
                var loadedFontBundle = new FontBundle
                {
                    fileSystemPath = filePath
                };
                if (loadedFontBundle.LoadFromFileSystem())
                    RegisterFontBundle(loadedFontBundle);
                else
                    Localyssation.logger.LogError($"Error occured when loading font bundle `{filePath}`");
            }
            // Set all font has unifont fallback

            // no fallback for Font!

            var UNIFONT_SDF = TMPfonts["unifont SDF"];

            TMPfonts.Values
                .SkipWhile(font => font == UNIFONT_SDF)
                .DoIf(
                    font => !font.fallbackFontAssetTable.Contains(UNIFONT_SDF),
                    font => font.fallbackFontAssetTable.Add(UNIFONT_SDF)
                );

            Resources.UnloadUnusedAssets();
            Resources.LoadAll<TMP_FontAsset>("").Cast<TMP_FontAsset>().DoIf(
                font => !font.fallbackFontAssetTable.Contains(UNIFONT_SDF),
                font => font.fallbackFontAssetTable.Add(UNIFONT_SDF)
            );

        }

        private static void RegisterFontBundle(FontBundle fontBundle)
        {
            availableFonts.AddRange(fontBundle.fonts);
            availableTMP_FontAssets.AddRange(fontBundle.TMPfonts);
        }
    }

    public static class FontHelper
    {
        public static void DetectVanillaFonts()
        {
            Resources.UnloadUnusedAssets();
            Localyssation.logger.LogInfo("Fonts used by Vanilla:");
            Resources.LoadAll<Font>("").Cast<Font>().Do(font =>
            {
                Localyssation.logger.LogInfo($"\t - {font.name}");
            });
            Localyssation.logger.LogInfo("TMP_FontAsset used by Vanilla:");
            Resources.LoadAll<TMP_FontAsset>("").Cast<TMP_FontAsset>().Do(font =>
            {
                Localyssation.logger.LogInfo($"\t - {font.name}");
            });
        }
    }
}
