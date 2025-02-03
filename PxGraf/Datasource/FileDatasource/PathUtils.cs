#nullable enable
using PxGraf.Datasource.DatabaseConnection;
using PxGraf.Models.Queries;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace PxGraf.Datasource.FileDatasource
{
    public static class PathUtils
    {
        public static string BuildAndSanitizePath(string rootPath, IReadOnlyList<string> groupHierarcy)
        {
            return BuildAndSanitizePath(rootPath, Path.Combine([.. groupHierarcy]));
        }

        public static string BuildAndSanitizePath(string rootPath, PxTableReference reference)
        {
            return BuildAndSanitizePath(rootPath, reference.ToPath());
        }

        /// <summary>
        /// TODO: summary
        /// </summary>
        /// <param name="groupHierarchy"></param>
        /// <param name="config"></param>
        public static void DatabaseWhitelistCheck(IReadOnlyList<string> groupHierarchy, LocalFilesystemDatabaseConfig config)
        {
            if (groupHierarchy.Count == 0 || config.DatabaseWhitelist.Length == 0)
                return;

            DatabaseWhitelistCheck(groupHierarchy[0], config); // Database name is the first part of the groupHierarchy
        }

        /// <summary>
        /// TODO: Summary
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="config"></param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public static void DatabaseWhitelistCheck(string databaseName, LocalFilesystemDatabaseConfig config)
        {
            if (config.DatabaseWhitelist.Length > 0 && !config.DatabaseWhitelist.Contains(databaseName, StringComparer.OrdinalIgnoreCase))
            {
                throw new DirectoryNotFoundException($"Database {databaseName} is not defined in the allowed databases");
            }
        }

        private static string BuildAndSanitizePath(string rootPath, string userPath)
        {
            // Get the full path of the root folder
            string rootFullPath = Path.GetFullPath(rootPath);

            // Combine the root folder with the user input path
            string combinedPath = Path.Combine(rootFullPath, userPath);

            // Get the full path of the combined path
            string fullPath = Path.GetFullPath(combinedPath);

            // Check if the full path starts with the root folder's full path
            if (!fullPath.StartsWith(rootFullPath, StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Access to the path is denied.");
            }

            return fullPath;
        }
    }
}
#nullable disable
