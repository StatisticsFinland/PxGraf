using System;
using System.Collections.Generic;

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
        /// Combines two path segments with a forward slash, handling edge cases.
        /// </summary>
        /// <param name="rootPath">The root path segment.</param>
        /// <param name="userPath">The user-provided path segment.</param>
        /// <returns>Combined path string.</returns>
        public static string CombinePaths(string rootPath, string userPath)
        {
            if (string.IsNullOrEmpty(rootPath))
            {
                return userPath;
            }

            if (string.IsNullOrEmpty(userPath))
            {
                return rootPath;
            }

            return $"{rootPath.TrimEnd('/')}/{userPath}";
        }

        /// <summary>
        /// Validates that a path does not escape the root path boundary using ".." segments.
        /// </summary>
        /// <param name="combinedPath">The combined path to validate.</param>
        /// <param name="rootDepth">The depth of the root path (number of segments).</param>
        /// <exception cref="UnauthorizedAccessException">Thrown if the path attempts to escape the root boundary.</exception>
        public static void ValidatePathSecurity(string combinedPath, int rootDepth)
        {
            string[] segments = combinedPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            int depth = 0;

            foreach (string segment in segments)
            {
                if (segment == "..")
                {
                    depth--;
                    if (depth < rootDepth)
                    {
                        throw new UnauthorizedAccessException("Access to the path is denied.");
                    }
                }
                else if (segment != ".")
                {
                    depth++;
                }
            }
        }

        /// <summary>
        /// Normalizes a path by removing "." segments and resolving ".." segments.
        /// </summary>
        /// <param name="path">The path to normalize.</param>
        /// <returns>Normalized path with resolved relative segments.</returns>
        public static string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

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

        /// <summary>
        /// Calculates the depth of a path (number of segments).
        /// </summary>
        /// <param name="path">The path to analyze.</param>
        /// <returns>Number of path segments, or 0 if path is empty.</returns>
        public static int GetPathDepth(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return 0;
            }

            return path.Split('/', StringSplitOptions.RemoveEmptyEntries).Length;
        }
    }
}
