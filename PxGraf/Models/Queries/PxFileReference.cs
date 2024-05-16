using System;
using System.Collections.Generic;
using System.Text;

namespace PxGraf.Models.Queries
{
    /// <summary>
    /// Reference to a file in the Px file system.
    /// </summary>
    public class PxFileReference
    {
        /// <summary>
        /// Name of the Px file.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of strings that represent the path to the Px file in the system excluding the name.
        /// </summary>
        public List<string> Hierarchy { get; set; }

        /// <summary>
        /// Converts the file reference to a path.
        /// </summary>
        public string ToPath()
        {
            return $"{string.Join("/", Hierarchy)}/{Name}";
        }

        /// <summary>
        /// Constructor for the file reference.
        /// </summary>
        public PxFileReference()
        {
            Hierarchy = new();
        }

        /// <summary>
        /// Constructor for the file reference with provided hierarchy and name.
        /// </summary>
        /// <param name="hierarchy">List of strings that represent the path to the Px file in the system excluding the name.</param>
        /// <param name="name">Name of the Px file.</param>
        public PxFileReference(List<string> hierarchy, string name)
        {
            Hierarchy = hierarchy;
            Name = name;
        }

        /// <summary>
        /// Constructor for the file reference with provided path that includes both hierarchy and name.
        /// </summary>
        /// <param name="path">Path to the Px file in the system.</param>
        public PxFileReference(string path)
        {
            string[] parts = path.Split('/');
            Hierarchy = new(parts);
            Name = Hierarchy[^1];
            Hierarchy.RemoveAt(Hierarchy.Count - 1);
        }

        /// <summary>
        /// Builds a cache key hash for the file reference.
        /// </summary>
        /// <param name="seed">Optional seed for generating the hash.</param>
        /// <returns>Cache key hash for the file reference.</returns>
        public string BuildCacheKeyHash(string seed = "")
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes($"{seed}pxFileReference{ToPath()}");
            byte[] hashBytes = System.Security.Cryptography.MD5.HashData(inputBytes);
            return BitConverter.ToString(hashBytes);
        }
    }
}
