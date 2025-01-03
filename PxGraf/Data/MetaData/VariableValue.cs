#nullable enable
using Px.Utils.Language;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace PxGraf.Data.MetaData
{
    /// <summary>
    /// Contais all metadata of one variable value.
    /// </summary>
    public partial class VariableValue
    {
        /// <summary>
        /// Unique, language independent identifier for this variable value.
        /// </summary>
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        /// <summary>
        /// Human readable name in one or more languages.
        /// </summary>
        [JsonPropertyName("name")]
        public MultilanguageString? Name { get; set; }

        /// <summary>
        /// Some arbitrary information about the variable value.
        /// </summary>
        [JsonPropertyName("note")]
        public MultilanguageString? Note { get; set; }

        /// <summary>
        /// True if this variable value represents a sum of values, example "ALL" or somethis simillar.
        /// Called "elimination value" in pxweb.
        /// </summary>
        [JsonPropertyName("isSum")]
        public bool IsSumValue { get; set; } = false;

        /// <summary>
        /// Contains properties that only content type variables have.
        /// Composition is used to avoid the following:
        /// ContentVariables would contain ContentVariableValues
        /// but regular Variables would contain just VariableValues
        /// so building a working class hierarchy and using it via typecasting
        /// requires too much effort with very little benefits.
        /// </summary>
        [JsonPropertyName("contentComponent")]
        [AllowNull]
        public ContentComponent? ContentComponent { get; set; }

        /// <summary>
        /// TBA: feature not yet implemented.
        /// </summary>
        [JsonIgnore]
        [AllowNull]
        public VirtualComponent? VirtualComponent { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="note"></param>
        /// <param name="isSumValue"></param>
        public VariableValue(
            string code,
            MultilanguageString name,
            MultilanguageString? note,
            bool isSumValue)
        {
            Code = code;
            Name = name;
            Note = note;
            IsSumValue = isSumValue;
        }

        /// <summary>
        /// Constrcutor for converting Px.Utils DimensionValue to PxGraf VariableValue.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="note"></param>
        /// <param name="isSumValue"></param>
        /// <param name="contentComponent"></param>
        public VariableValue(
            string code,
            MultilanguageString name,
            MultilanguageString? note,
            bool isSumValue,
            ContentComponent? contentComponent)
        {
            Code = code;
            Name = name;
            Note = note;
            IsSumValue = isSumValue;
            ContentComponent = contentComponent;
        }

        /// <summary>
        /// Default constructor for deserialization.
        /// </summary>
        public VariableValue() { }
    }
}
#nullable disable