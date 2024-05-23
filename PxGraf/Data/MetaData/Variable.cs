using Newtonsoft.Json;
using PxGraf.Enums;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.PxWebInterface.SerializationModels;
using PxGraf.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PxGraf.Data.MetaData
{
    /// <summary>
    /// All the metadata of one variable and the included values.
    /// </summary>
    public class Variable : IReadOnlyVariable
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
        public MultiLanguageString Name { get; private set; }
        IReadOnlyMultiLanguageString IReadOnlyVariable.Name => Name;

        /// <summary>
        /// Some arbitrary information about the variable.
        /// </summary>
        [JsonProperty("note")]
        public MultiLanguageString Note { get; private set; }
        IReadOnlyMultiLanguageString IReadOnlyVariable.Note => Note;

        /// <summary>
        /// Type of the variable, examples: content, time, geological, ordinal...
        /// </summary>
        [JsonIgnore]
        public VariableType Type { get; set; } // Public set for incremental building

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

        IReadOnlyList<IReadOnlyVariableValue> IReadOnlyVariable.IncludedValues => IncludedValues;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="note"></param>
        /// <param name="type"></param>
        /// <param name="includedValues"></param>
        public Variable(
            string code,
            MultiLanguageString name,
            MultiLanguageString note,
            VariableType type,
            List<VariableValue> includedValues
            )
        {
            Code = code;
            Name = name;
            Note = note;
            Type = type;
            IncludedValues = includedValues;
        }

        /// <summary>
        /// For deserialization. OBS: type is string
        /// </summary>
        [JsonConstructor]
        public Variable(
            string code,
            MultiLanguageString name,
            MultiLanguageString note,
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

        /// <summary>
        /// Utility constructor for building variables from pxweb response variables.
        /// </summary>
        /// <param name="lang"></param>
        /// <param name="metaVar"></param>
        public Variable(string lang, PxMetaResponse.Variable metaVar)
        {
            Name = new MultiLanguageString();
            Note = null; //OBS: PxWeb does not yet support variable notes.
            IncludedValues = [];

            Name.AddTranslation(lang, metaVar.Text);
            Code = metaVar.Code;

            for (int i = 0; i < metaVar.Values?.Length; i++)
            {
                IncludedValues.Add(new VariableValue(
                    code: metaVar.Values[i],
                    name: new MultiLanguageString(lang, metaVar.ValueTexts[i]),
                    note: null, //OBS: PxWeb does not yet support variable value notes.
                    isSumValue: metaVar.IsSumValue(i)
                    ));
            }
        }

        /// <summary>
        /// Returns a transformed deep copy of the variable. This copy can be mutated.
        /// The copy has the same structure as the provided map,
        /// but contains all relevant metadata from the original variable.
        /// </summary>
        public Variable GetTransform(VariableMap map)
        {
            var newValList = new List<VariableValue>();
            foreach (string mapValCode in map.ValueCodes)
            {
                newValList.Add(IncludedValues.Find(iv => iv.Code == mapValCode).Clone());
            }

            return new Variable(Code, Name.Clone(), Note?.Clone(), Type, newValList);
        }

        /// <summary>
        /// Makes a deep copy of the object
        /// </summary>
        /// <returns></returns>
        public Variable Clone()
        {
            return new Variable(
                Code,
                Name.Clone(),
                Note?.Clone(),
                Type,
                IncludedValues.Select(v => v.Clone()).ToList()
                );
        }

        /// <summary>
        /// Provides only the structure information about the variable.
        /// (code and included variable value codes in order)
        /// </summary>
        public VariableMap BuildVariableMap()
        {
            return new VariableMap(
                Code,
                IncludedValues.Select(vv => vv.Code).ToList()
            );
        }

        /// <summary>
        /// Apply requested changes to this variables
        /// </summary>
        /// <param name="query">Contains the wanted changes</param>
        public void ApplyEditionsFromQuery(VariableQuery query)
        {
            IncludedValues = query.ValueFilter.Filter(IncludedValues).Cast<VariableValue>().ToList();

            Name.Edit(query.NameEdit);
            foreach (var valueEdit in query.ValueEdits)
            {
                if (IncludedValues.SingleOrDefault(vv => vv.Code == valueEdit.Key) is VariableValue vv) // failsafe if code not found
                {
                    vv.ApplyEdition(valueEdit.Value);
                }
            }
        }

        /// <summary>
        /// Adds a new language to the meta information of this variable.
        /// </summary>
        public void AddTranslations(string lang, PxMetaResponse.Variable metaVar)
        {
            Name.AddTranslation(lang, metaVar.Text);
            foreach (VariableValue val in IncludedValues)
            {
                int index = Array.FindIndex(metaVar.Values, s => s == val.Code);
                if (index >= 0)
                {
                    val.Name.AddTranslation(lang, metaVar.ValueTexts[index]);
                }
            }
        }
    }
}
