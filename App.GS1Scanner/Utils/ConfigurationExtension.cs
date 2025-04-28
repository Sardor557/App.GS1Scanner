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
            var basePath = FileSystem.AppDataDirectory;
            var localConfigPath = Path.Combine(basePath, "appsettings.json");

            CopyFromPackage("appsettings.json", localConfigPath);
            return new ConfigurationBuilder()
                   .AddJsonFile(localConfigPath, optional: false, reloadOnChange: true)
                   .Build();
        }

        private static void CopyFromPackage(string assetName, string outputPath)
        {
            using var stream = FileSystem.OpenAppPackageFileAsync(assetName).Result;
            using var reader = new StreamReader(stream);
            var content = reader.ReadToEnd();
            File.WriteAllText(outputPath, content, Encoding.UTF8);
        }
    }
}
