using Amazon;
using Amazon.Translate;
using Amazon.Translate.Model;
using System.IO;

namespace AmazonTranslateApp
{
    public class Program
    {
        private static readonly string ACCESS_KEY = "TYPE YOUR ACCESS KEY HERE";
        private static readonly string SECRET_KEY = "TYPE YOUR SECRET HERE";
        private static readonly int MAX_LENGHT = 1000;
        private static readonly string ROOT_FOLDER_PATH = @"C:\projetos\amazon-translate\input";
        private static readonly string OUTPUT_FILE_PREFFIX_NAME = "output";

        public static async Task<string> FileContent(string fileName)
        {
            return await File.ReadAllTextAsync(fileName);
        }

        public static async Task CreateFileWithContent(string fileName, string content)
        {
            await File.WriteAllTextAsync(fileName, content);
        }

        public static async Task TranslateAllFiles(AmazonTranslateClient client)
        {
            // Get all txt files in the current directory
            var txtFiles = Directory.GetFiles(ROOT_FOLDER_PATH, "*.txt");
            var files = 0;

            foreach (var file in txtFiles)
            {
                var fileInfo = new System.IO.FileInfo(file);
                var outputFileName = System.IO.Path.Combine(ROOT_FOLDER_PATH, $"{OUTPUT_FILE_PREFFIX_NAME}-{fileInfo.Name}");
                // only if file was not translated yet
                if (!File.Exists(outputFileName))
                {
                    var content = await FileContent(file);
                    var translateText = await TranslateTextAsync(client, content, "pt", "en");
                    await CreateFileWithContent(outputFileName, translateText);
                    files++;
                }
            }

            Console.WriteLine($"Files: {files}");
            Console.WriteLine("Done!");
        }

        public static async Task Main()
        {
            Console.Clear();
            Console.WriteLine("BASE PATH:");
            Console.WriteLine(ROOT_FOLDER_PATH);
            Console.WriteLine("");
            var client = new AmazonTranslateClient(ACCESS_KEY, SECRET_KEY, RegionEndpoint.USEast1);
            await TranslateAllFiles(client);
        }

        public static async Task<string> TranslateTextAsync(AmazonTranslateClient client, string text, string sourceLang, string targetLang)
        {
            var request = new TranslateTextRequest
            {
                Text = text,
                SourceLanguageCode = sourceLang,
                TargetLanguageCode = targetLang
            };

            var response = await client.TranslateTextAsync(request);
            return response.TranslatedText;
        }
    }
}