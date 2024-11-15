using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PxGraf.Models.Queries
{
    /// <summary>
    /// Reference to a file in the Px file system.
    /// </summary>
    public class PxTableReference
    {
        /// <summary>
        /// Name of the Px file.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of strings that represent the path to the Px file in the system excluding the name.
        /// </summary>
        public IReadOnlyList<string> Hierarchy { get; set; }

        /// <summary>
        /// Converts the file reference to a path.
        /// </summary>
        public string ToPath(string separator = "/")
        {
            return $"{string.Join(separator, Hierarchy)}{separator}{Name}";
        }

        /// <summary>
        /// Constructor for the file reference.
        /// </summary>
        public PxTableReference()
        {
            Hierarchy = [];
        }

        /// <summary>
        /// Constructor for the file reference with provided hierarchy and name.
        /// </summary>
        /// <param name="hierarchy">List of strings that represent the path to the Px file in the system excluding the name.</param>
        /// <param name="name">Name of the Px file.</param>
        public PxTableReference(IReadOnlyList<string> hierarchy, string name)
        {
            Hierarchy = hierarchy;
            Name = name;
        }

        /// <summary>
        /// Constructor for the file reference with provided path that includes both hierarchy and name.
        /// </summary>
        /// <param name="path">Path to the Px file in the system.</param>
        /// <param name="separator">Optional separator character for the path.</param>
        public PxTableReference(string path, char? separator = null)
        {
            char pathSeparator = separator ?? Path.DirectorySeparatorChar;
            List<string> parts = [.. path.Split(pathSeparator, StringSplitOptions.RemoveEmptyEntries)];
            Hierarchy = parts[0..^1];
            Name = parts[^1];
        }
    }
}
