using System.Text.Json;
using Tools.PxUtilsIntegrationTestTool;

namespace Tools
{
    internal class Program
    {
        internal static Configuration Config;

        internal static async Task Main()
        {
            string configFilePath = Path.Combine(Directory.GetCurrentDirectory(), "configuration.json");
            string jsonString = await File.ReadAllTextAsync(configFilePath);
            var options = new JsonSerializerOptions { IncludeFields = true };
            Configuration? config = JsonSerializer.Deserialize<Configuration>(jsonString, options);
            Config = config ?? throw new JsonException("Configuration file could not be parsed properly");

            int option;
            do
            {
                Console.WriteLine("Select an option:");
                Console.WriteLine("1. Collect responses from PxGraf instances");
                Console.WriteLine("2. Compare responses");
                Console.WriteLine("3. Exit");
                option = ToolsUtilities.GetNumericAnswer(3);
                if (option == 1)
                {
                    ResponseCollector responseCollector = new();
                    await responseCollector.Start();
                }
                else
                {
                    ResponseComparer responseComparer = new();
                    await responseComparer.Start();
                }
            } while (option != 3);
        }
    }
}
