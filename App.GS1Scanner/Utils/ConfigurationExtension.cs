using Microsoft.Extensions.Configuration;
using Microsoft.Maui.Storage;
using System.IO;
using System.Text;

namespace App.GS1Scanner.Utils
{
    internal static class ConfigurationExtension
    {
        public static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder();

            var basePath = FileSystem.AppDataDirectory;

            var appSettingsPath = Path.Combine(basePath, "appsettings.json");

            CopyAsset("appsettings.json", appSettingsPath);

            builder.AddJsonFile(appSettingsPath, optional: false, reloadOnChange: false);

            return builder.Build();
        }

        private static void CopyAsset(string assetName, string outputPath)
        {
            using var stream = FileSystem.OpenAppPackageFileAsync(assetName).Result;
            using var reader = new StreamReader(stream);
            var content = reader.ReadToEnd();
            File.WriteAllText(outputPath, content, Encoding.UTF8);
        }
    }
}
