using System;
using System.Collections.Generic;
using System.Linq;

namespace PxGraf.Storage
{
    /// <summary>
    /// Provides utility methods for normalizing and validating file paths and extensions.
    /// </summary>
    public static class PathNormalizer
    {
        /// <summary>
        /// Normalizes a file extension to ensure it starts with a dot.
        /// </summary>
        /// <param name="fileExtension">The file extension to normalize.</param>
        /// <returns>Normalized file extension with leading dot, or empty string if input is null or empty.</returns>
        public static string NormalizeFileExtension(string fileExtension)
        {
            if (string.IsNullOrEmpty(fileExtension))
            {
                return string.Empty;
            }

            if (fileExtension.StartsWith('.'))
            {
                return fileExtension;
            }

            return $".{fileExtension}";
        }

        /// <summary>
        /// Combines path segments with a forward slash, handling edge cases.
        /// </summary>
        public static string CombineAndNormalizeAzurePaths(params string[] paths)
        {
            if (paths == null || paths.Length == 0)
            {
                return string.Empty;
            }

            string[] nonEmptyPaths = [.. paths
                .Where(p => !string.IsNullOrEmpty(p))
                .Select(NormalizeToAzurePath)
                .Where(p => !string.IsNullOrEmpty(p))];
            return string.Join("/", nonEmptyPaths);
        }

        /// <summary>
        /// Validates that a path does not escape the root path boundary using ".." segments.
        /// </summary>
        /// <param name="combinedPath">The combined path to validate.</param>
        /// <param name="rootPath">The root path to compare against.</param>
        /// <exception cref="UnauthorizedAccessException">Thrown if the path attempts to escape the root boundary.</exception>
        public static void ValidatePathSecurity(string combinedPath, string rootPath)
        {
            string normalizedCombinedPath = NormalizeToAzurePath(combinedPath);
            string normalizedRootPath = NormalizeToAzurePath(rootPath);

            if (string.IsNullOrEmpty(normalizedRootPath))
            {
                return;
            }

            if (normalizedCombinedPath.Equals(normalizedRootPath, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (!normalizedCombinedPath.StartsWith(normalizedRootPath + "/", StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Access to the path is denied.");
            }
        }

        /// <summary>
        /// Normalizes a path by removing "." segments and resolving ".." segments.
        /// Also converts backslashes to forward slashes and removes duplicate slashes.
        /// </summary>
        /// <param name="path">The path to normalize.</param>
        /// <returns>Normalized path with resolved relative segments.</returns>
        public static string NormalizeToAzurePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            path = path.Replace('\\', '/');
            string[] segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            List<string> normalizedSegments = [];

            foreach (string segment in segments)
            {
                if (segment == "..")
                {
                    if (normalizedSegments.Count > 0)
                    {
                        normalizedSegments.RemoveAt(normalizedSegments.Count - 1);
                    }
                }
                else if (segment != ".")
                {
                    normalizedSegments.Add(segment);
                }
            }

            return string.Join("/", normalizedSegments);
        }
    }
}
