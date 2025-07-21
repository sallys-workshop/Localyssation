using BepInEx;
using System.IO;

namespace Localyssation.Exporter
{
    internal static class ExportUtil
    {
        public static readonly string EXPORT_FOLDER = GenerateExportFolder();

        private static string GenerateExportFolder()
        {
            var filePaths = Directory.GetFiles(Paths.PluginPath, "Localyssation.dll", SearchOption.AllDirectories);
            var path = filePaths[0]; // Here I am!
            path = Path.GetDirectoryName(path); // where I'm from
            if (string.IsNullOrWhiteSpace(path))
            {
                path = Paths.PluginPath;
            }
            path = Path.Combine(path, "LocalyssationExtraInfoExport");
            Localyssation.logger.LogInfo(path);
            return path;
        }

        public static void InitExports()
        {
            if (LocalyssationConfig.ExportExtra)
            {
                Directory.CreateDirectory(EXPORT_FOLDER);
                Directory.Delete(EXPORT_FOLDER, true);
                Directory.CreateDirectory(EXPORT_FOLDER);
            }
        }
    }
}
