using PxGraf.Language;
using System.Collections.Generic;

namespace PxGraf.Data.MetaData
{
    /// <summary>
    /// A readonly interface for cube meta objects.
    /// </summary>
    public interface IReadOnlyCubeMeta
    {
        /// <summary>
        /// The multilanguage strings of this cube meta have all these languages.
        /// </summary>
        public IReadOnlyList<string> Languages { get; }

        /// <summary>
        /// Header text of the query / visualization.
        /// </summary>
        public IReadOnlyMultiLanguageString Header { get; }

        /// <summary>
        /// Some arbitrary information about the cube.
        /// </summary>
        public IReadOnlyMultiLanguageString Note { get; }

        /// <summary>
        /// An ordered collection of the variables that define the cube.
        /// </summary>
        public IReadOnlyList<IReadOnlyVariable> Variables { get; }

        /// <summary>
        /// Returns a deep copy of the table meta object that is not read only and can be mutated.
        /// </summary>
        /// <returns></returns>
        public CubeMeta Clone();

        /// <summary>
        /// Returns a transformed deep copy of the metadata. This copy can be mutated.
        /// The copy has the same structure as the provided map,
        /// but contains all relevant metadata from the original.
        /// </summary>
        public CubeMeta GetTransform(IReadOnlyList<VariableMap> map);

        /// <summary>
        /// Provides only the structure information about the cube meta.
        /// (variable codes and included variable value codes in order)
        /// </summary>
        public CubeMap BuildMap();
    }
}
