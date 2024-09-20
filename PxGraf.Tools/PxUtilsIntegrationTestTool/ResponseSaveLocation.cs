using PxGraf.Settings;

namespace Tools.PxUtilsIntegrationTestTool
{
    internal static class ResponseSaveLocation
    {
        internal static string SetSaveLocation(string dataSource)
        {
            string path = Program.Config.Paths.ResponseDirectory;
            if (string.IsNullOrEmpty(path))
                throw new InvalidOperationException("Response directory path is not set in the configuration file.");
            string finalPath = Path.Combine(path, dataSource);
            if (!ToolsUtilities.CreateDirectoryOrRemoveContents(finalPath))
                throw new InvalidOperationException("Something went wrong with setting up the result directory.");

            return finalPath;
        }
    }
}
