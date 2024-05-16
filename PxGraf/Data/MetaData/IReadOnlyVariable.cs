using Newtonsoft.Json;
using PxGraf.Enums;
using PxGraf.Language;
using System.Collections.Generic;

namespace PxGraf.Data.MetaData
{
    /// <summary>
    /// A read only interface for variables.
    /// </summary>
    public interface IReadOnlyVariable
    {
        /// <summary>
        /// Unique, language independent identifier for this variable.
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; }

        /// <summary>
        /// Name of the variable in multiple languages.
        /// </summary>
        [JsonProperty("name")]
        public IReadOnlyMultiLanguageString Name { get; }

        /// <summary>
        /// Some arbitrary information about the variable.
        /// </summary>
        [JsonProperty("note")]
        public IReadOnlyMultiLanguageString Note { get; }


        /// <summary>
        /// Type of the variable, examples: content, time, geological, ordinal...
        /// </summary>
        [JsonIgnore]
        public VariableType Type { get; }

        /// <summary>
        /// Converts types to human readeble form.
        /// </summary>
        [JsonProperty("type")]
        public string JsonType { get; }

        /// <summary>
        /// Values currently included in this variable.
        /// Can be a subset of the original set of variable values.
        /// </summary>
        [JsonProperty("values")]
        public IReadOnlyList<IReadOnlyVariableValue> IncludedValues { get; }

        /// <summary>
        /// Generates a deep copy that can be mutated.
        /// </summary>
        public Variable Clone();

        /// <summary>
        /// Returns a transformed deep copy of the variable. This copy can be mutated.
        /// The copy has the same structure as the provided map,
        /// but contains all relevant metadata from the original variable.
        /// </summary>
        public Variable GetTransform(VariableMap map);

        /// <summary>
        /// Provides only the structure information about the variable.
        /// (code and included variable value codes in order)
        /// </summary>
        public VariableMap BuildVariableMap();
    }
}
