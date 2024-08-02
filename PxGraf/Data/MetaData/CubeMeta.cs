using Newtonsoft.Json;
using Px.Utils.Language;
using System.Collections.Generic;

namespace PxGraf.Data.MetaData
{
    /// <summary>
    /// Information that defines the cube and gives meaning to the numeric data.
    /// </summary>
    public class CubeMeta
    {
        /// <summary>
        /// The multilanguage strings of this cube meta have all these languages.
        /// </summary>
        public IReadOnlyList<string> Languages { get; private set; }

        /// <summary>
        /// Header text of the query / visualization.
        /// </summary>
        public MultilanguageString Header { get; private set; }

        /// <summary>
        /// Some arbitrary information about the cube.
        /// </summary>
        public MultilanguageString Note { get; private set; }

        /// <summary>
        /// An ordered collection of the variables that define the cube.
        /// </summary>
        public List<Variable> Variables { get; private set; }

        /// <summary>
        /// Parameterless constructos udes when the cube meta is built in parts
        /// by appending information.
        /// </summary>
        public CubeMeta()
        {
            Languages = [];
            Variables = [];
            Header = null;
            Note = null;
        }

        /// <summary>
        /// Used when the cube meta can be built with all the required infomation available.
        /// </summary>
        [JsonConstructor]
        public CubeMeta(
            IReadOnlyList<string> languages,
            MultilanguageString header,
            MultilanguageString note,
            List<Variable> variables
        )
        {
            Languages = languages;
            Variables = variables;
            Header = header;
            Note = note;
        }
    }
}
