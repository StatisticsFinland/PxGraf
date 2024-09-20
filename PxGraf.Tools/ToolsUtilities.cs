using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.PxUtilsIntegrationTestTool;

namespace Tools
{
    internal static class ToolsUtilities
    {
        internal static bool GetBooleanAnswer()
        {
            string? response = Console.ReadLine();
            return response?.ToLower() == "y";
        }

        internal static int GetNumericAnswer(int options)
        {
            string? response = Console.ReadLine();
            if (int.TryParse(response, out int responseNumber) && responseNumber > 0 && responseNumber <= options)
                return responseNumber;
            else
            {
                Console.WriteLine("Invalid input. Try again.");
                return GetNumericAnswer(options);
            }
        }

        internal static bool CreateDirectoryOrRemoveContents(string path, bool allowOldContents = false)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (allowOldContents || !Directory.EnumerateFileSystemEntries(path).Any())
                return true;

            Console.WriteLine($"The directory at {path} is not empty. Do you want to delete the previous contents? (y/n)");
            bool delete = GetBooleanAnswer();
            if (delete)
            {
                Directory.Delete(path, true);
                Directory.CreateDirectory(path);
            }
            else
            {
                return false;
            }

            return true;
        }

        internal static string GetDataSourceUrl(string dataSource)
        {
            return dataSource switch
            {
                TokenConstants.DATASOURCE_PXUTILS => Program.Config.Urls.PxUtils,
                TokenConstants.DATASOURCE_PXWEBAPI => Program.Config.Urls.PxWebApi,
                TokenConstants.DATASOURCE_OLD => Program.Config.Urls.PxWebApiOld,
                _ => throw new ArgumentException("Invalid data source"),
            };
        }
    }
}
