
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
// Needs UnityEngine.ImageConversionModule

namespace Localyssation.Exporter
{
    internal abstract class Exporter<T> : MonoBehaviour
        where T : ScriptableObject
    {
        public abstract string Name();

        public string GetExportMarkdownFile()
        {
            return Path.Combine(ExportUtil.EXPORT_FOLDER, Name() + ".md");
        }

        public string GetExportAssetFolder()
        {
            return Path.Combine(ExportUtil.EXPORT_FOLDER, "asset", Name());
        }

        public string GetExportAssetPath(string assetName)
        {
            return Path.Combine(GetExportAssetFolder(), assetName);
        }

        public string CreateAndInsertImageAsset(string name, Sprite sp)
        {
            if (!File.Exists(GetExportAssetPath(name)))
            {
                ///[Error  : Unity Log] ArgumentException: Texture '_helmIco_30' is not readable. For this reason, scripts cannot access the memory allocated to it. You can make this texture readable in the Texture Import Settings.    
                //var buffer = sp.texture.EncodeToPNG();
                RenderTexture tmp = RenderTexture.GetTemporary(
                    sp.texture.width,
                    sp.texture.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear
                );
                Graphics.Blit(sp.texture, tmp);
                // 备份当前设置的RenderTexture
                RenderTexture previous = RenderTexture.active;

                // 将创建的临时纹理tmp设置为当前RenderTexture
                RenderTexture.active = tmp;

                // 创建一张新的可读Texture2D，并拷贝像素值
                Texture2D myTexture2D = new Texture2D(sp.texture.width, sp.texture.height);

                // 将RenderTexture的像素值拷贝到新的纹理中
                myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
                myTexture2D.Apply();

                // 重置激活的RenderTexture
                RenderTexture.active = previous;
                // 释放临时RenderTexture
                RenderTexture.ReleaseTemporary(tmp);
                // "myTexture2D"是可读纹理，并且和”texture”拥有相同的像素值
                File.WriteAllBytes(GetExportAssetPath(name + ".png"), myTexture2D.EncodeToPNG());
            }

            string uri = new Uri(Path.Combine("asset", Name(), name + ".png")).ToString();
            return $"![{name}]({uri})";
        }

        public Exporter()
        {
            Directory.CreateDirectory(GetExportAssetFolder());
        }

        public void Export(IEnumerable<T> data)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(CreateHeader());
            foreach (T item in data)
            {
                sb.AppendLine(Serialize(item));
            }
            sb.AppendLine(CreateEnding());

            File.AppendAllText(GetExportMarkdownFile(), sb.ToString());
        }

        public abstract string Serialize(T data);
        protected abstract string CreateHeader();
        protected abstract string CreateEnding();
    }
}
