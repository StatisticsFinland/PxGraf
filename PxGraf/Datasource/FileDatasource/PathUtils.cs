#nullable enable
using PxGraf.Datasource.DatabaseConnection;
using PxGraf.Models.Queries;
using System;
using System.Collections.Generic;
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
        /// Checks whether a given group hierarchy is under a database that has been whitelisted
        /// </summary>
        /// <param name="groupHierarchy">List of strings that define the group hierarchy</param>
        /// <param name="whitelist">List of allowed databases</param>
        /// <returns>True if the first part of the group hierarchy is included in the whitelisted database names or the list doesn't exist. Otherwise false.</returns>
        public static bool IsDatabaseWhitelisted(IReadOnlyList<string> groupHierarchy, string[] whitelist)
        {
            if (groupHierarchy.Count == 0 || whitelist.Length == 0)
                return true;

            return IsDatabaseWhitelisted(groupHierarchy[0], whitelist); // Database name is the first part of the groupHierarchy
        }

        /// <summary>
        /// Checks whether a given database name is included in the local file system database whitelist
        /// </summary>
        /// <param name="databaseName">Name of the database to check</param>
        /// <param name="whitelist">List of allowed databases</param>
        /// <returns>True if the given name is included in the whitelisted databases array, or database whitelist doesn't exist. Otherwise false.</returns>
        public static bool IsDatabaseWhitelisted(string databaseName, string[] whitelist)
        {
            if (whitelist.Length == 0 || whitelist.Contains(databaseName, StringComparer.OrdinalIgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
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
