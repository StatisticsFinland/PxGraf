using Newtonsoft.Json;
using Px.Utils.Language;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Utility;
using System.Collections.Generic;

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
        [JsonProperty("code")]
        public string Code { get; private set; }

        /// <summary>
        /// Name of the variable in multiple languages.
        /// </summary>
        [JsonProperty("name")]
        public MultilanguageString Name { get; private set; }

        /// <summary>
        /// Some arbitrary information about the variable.
        /// </summary>
        [JsonProperty("note")]
        public MultilanguageString Note { get; private set; }

        /// <summary>
        /// Type of the variable, examples: content, time, geological, ordinal...
        /// </summary>
        [JsonIgnore]
        public DimensionType Type { get; set; } // Public set for incremental building

        /// <summary>
        /// Convers types to human readable form.
        /// </summary>
        [JsonProperty("type")]
        public string JsonType
        {
            get => DimenionTypesEnumConverter.ToString(Type);
            set => Type = DimenionTypesEnumConverter.ToEnum(value);
        }

        /// <summary>
        /// Values currently included in this variable.
        /// Can be a subset of the original set of variable values.
        /// </summary>
        [JsonProperty("values")]
        public List<VariableValue> IncludedValues { get; private set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="note"></param>
        /// <param name="type"></param>
        /// <param name="vals"></param>
        public Variable(
            string code,
            MultilanguageString name,
            MultilanguageString note,
            DimensionType type,
            List<VariableValue> vals
            )
        {
            Code = code;
            Name = name;
            Note = note;
            Type = type;
            IncludedValues = vals;
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
            List<VariableValue> includedValues
            )
        {
            Code = code;
            Name = name;
            Note = note;
            JsonType = type;
            IncludedValues = includedValues;
        }
    }
}
