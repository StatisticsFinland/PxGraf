using Newtonsoft.Json;
using PxGraf.Language;
using System.Diagnostics.CodeAnalysis;

namespace PxGraf.Data.MetaData
{
    /// <summary>
    /// A read only interface for variable values.
    /// </summary>
    public interface IReadOnlyVariableValue
    {
        /// <summary>
        /// Unique, language independent identifier for this variable value.
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; }

        /// <summary>
        /// Human readable name in one or more languages.
        /// </summary>
        [JsonProperty("name")]
        public IReadOnlyMultiLanguageString Name { get; }

        /// <summary>
        /// Some arbitrary information about the variable value.
        /// </summary>
        [JsonProperty("note")]
        public IReadOnlyMultiLanguageString Note { get; }

        /// <summary>
        /// True if this variable value represents a sum of values, example "ALL" or somethis simillar.
        /// Called "elimination value" in pxweb.
        /// </summary>
        [JsonProperty("isSum")]
        public bool IsSumValue { get; }

        /// <summary>
        /// Contains properties that only content type variables have.
        /// Composition is used to avoid the following:
        /// ContentVariables would contain ContentVariableValues
        /// but regular Variables would contain just VariableValues
        /// so building a working class hierarchy and using it via typecasting 
        /// requires too much effort with very little benefits.
        /// </summary>
        [AllowNull]
        [JsonProperty("contentComponent")]
        public IReadOnlyContentComponent ContentComponent { get; }

        /// <summary>
        /// TBA: feature not yet implemented.
        /// </summary>
        [AllowNull]
        [JsonIgnore]
        public VirtualComponent VirtualComponent { get; }

        /// <summary>
        /// Generates a deep copy that can be mutated.
        /// </summary>
        public VariableValue Clone();
    }
}
