using Px.Utils.Language;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Utility;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PxGraf.Data.MetaData
{
    /// <summary>
    /// All the metadata of one variable and the included values.
    /// </summary>
    public class Variable
    {
        /// <summary>
        /// Unique, language independent identifier for this variable.
        /// </summary>
        public string Code { get; private set; }

        /// <summary>
        /// Name of the variable in multiple languages.
        /// </summary>
        public MultilanguageString Name { get; private set; }

        /// <summary>
        /// Some arbitrary information about the variable.
        /// </summary>
        public MultilanguageString Note { get; private set; }

        /// <summary>
        /// Type of the variable, examples: content, time, geological, ordinal...
        /// </summary>
        [JsonIgnore]
        public DimensionType DimensionType { get; set; } // Public set for incremental building

        /// <summary>
        /// Convers types to human readable form.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type
        {
            get => DimenionTypesEnumConverter.ToString(DimensionType);
            set => DimensionType = DimenionTypesEnumConverter.ToEnum(value);
        }

        /// <summary>
        /// Values currently included in this variable.
        /// Can be a subset of the original set of variable values.
        /// </summary>
        public List<VariableValue> Values { get; private set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="note"></param>
        /// <param name="type"></param>
        /// <param name="values"></param>
        public Variable(
            string code,
            MultilanguageString name,
            MultilanguageString note,
            DimensionType type,
            List<VariableValue> values
            )
        {
            Code = code;
            Name = name;
            Note = note;
            DimensionType = type;
            Values = values;
        }

        /// <summary>
        /// For deserialization. OBS: type is string
        /// </summary>
        [JsonConstructor]
        public Variable(
            string code,
            MultilanguageString name,
            MultilanguageString note,
            string type,
            List<VariableValue> values
            )
        {
            Code = code;
            Name = name;
            Note = note;
            Type = type;
            Values = values;
        }
    }
}
