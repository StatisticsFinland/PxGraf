using Px.Utils.Language;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PxGraf.Models.Queries
{
    /// <summary>
    /// Query object for editing a variable.
    /// </summary>
    public class DimensionQuery
    {
        /// <summary>
        /// Name of the variable in available languages.
        /// </summary>
        public MultilanguageString NameEdit { get; set; }

        /// <summary>
        /// VariableValue.Code to VariableValueEdition
        /// </summary>
        public Dictionary<string, VariableValueEdition> ValueEdits { get; set; } = [];

        public IValueFilter ValueFilter { get; set; }
        public List<VirtualValueDefinition> VirtualValueDefinitions { get; set; } = [];

        /// <summary>
        /// When true the final value that is displayed on the visualization can be chosen from the included values and the rest are not shown.
        /// By default only one value can be chosen, except line charts and tables can contain a multiselect selectable variable.
        /// </summary>
        public bool Selectable { get; set; }

        public class VariableValueEdition
        {
            public MultilanguageString NameEdit { get; set; }

            [AllowNull]
            public ContentComponentEdition ContentComponent { get; set; }
        }
    }
}
