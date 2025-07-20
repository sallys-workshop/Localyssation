using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using TMPro;
using UnityEngine;

namespace Localyssation
{
    public class FontBundle
    {
        public class LoadedFont
        {
            public Font uguiFont;
            public TMP_FontAsset tmpFont;
            public string name;

            public bool IsValid()
            {
                return uguiFont != null && tmpFont != null && name != null;
            }
        }

        public string fileSystemPath;
        public Dictionary<string, LoadedFont> loadedFonts = new Dictionary<string, LoadedFont>();
        public AssetBundle bundle;


        public bool LoadFromFileSystem()
        {
            if (string.IsNullOrEmpty(fileSystemPath)) return false;
            
            bundle = AssetBundle.LoadFromFile(fileSystemPath);
            Localyssation.logger.LogInfo("Found Fonts:");
            bundle.LoadAllAssets(typeof(Font)).Cast<Font>().Do(font =>
            {
                Localyssation.logger.LogInfo($"\t{font.name}");
                TMP_FontAsset sdf = bundle.LoadAsset<TMP_FontAsset>(font.name + " SDF");
                loadedFonts.Add(font.name,
                    new LoadedFont()
                    {
                        name = font.name,
                        uguiFont = font,
                        tmpFont = sdf
                    }
                );
                    
            });
            Localyssation.logger.LogInfo($"Found TMP_FontAsset:");
            bundle.LoadAllAssets(typeof(TMP_FontAsset)).Cast<TMP_FontAsset>().Do(font =>
            {
                Localyssation.logger.LogInfo($"\t{font.name}");
            });
            
            
            return true;
        }

    }

}
