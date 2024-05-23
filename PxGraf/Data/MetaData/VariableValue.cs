using Newtonsoft.Json;
using PxGraf.Language;
using PxGraf.Models.Queries;
using System.Diagnostics.CodeAnalysis;

namespace PxGraf.Data.MetaData
{
    /// <summary>
    /// Contais all metadata of one variable value.
    /// </summary>
    public partial class VariableValue : IReadOnlyVariableValue
    {
        /// <summary>
        /// Unique, language independent identifier for this variable value.
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// Human readable name in one or more languages.
        /// </summary>
        [JsonProperty("name")]
        public MultiLanguageString Name { get; set; }
        IReadOnlyMultiLanguageString IReadOnlyVariableValue.Name => Name;

        /// <summary>
        /// Some arbitrary information about the variable value.
        /// </summary>
        [JsonProperty("note")]
        public MultiLanguageString Note { get; set; }
        IReadOnlyMultiLanguageString IReadOnlyVariableValue.Note => Note;

        /// <summary>
        /// True if this variable value represents a sum of values, example "ALL" or somethis simillar.
        /// Called "elimination value" in pxweb.
        /// </summary>
        [JsonProperty("isSum")]
        public bool IsSumValue { get; set; }

        /// <summary>
        /// Contains properties that only content type variables have.
        /// Composition is used to avoid the following:
        /// ContentVariables would contain ContentVariableValues
        /// but regular Variables would contain just VariableValues
        /// so building a working class hierarchy and using it via typecasting
        /// requires too much effort with very little benefits.
        /// </summary>
        [JsonProperty("contentComponent")]
        [AllowNull]
        public ContentComponent ContentComponent { get; set; }
        IReadOnlyContentComponent IReadOnlyVariableValue.ContentComponent => ContentComponent;

        /// <summary>
        /// TBA: feature not yet implemented.
        /// </summary>
        [JsonIgnore]
        [AllowNull]
        public VirtualComponent VirtualComponent { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="note"></param>
        /// <param name="isSumValue"></param>
        public VariableValue(
            string code,
            MultiLanguageString name,
            MultiLanguageString note,
            bool isSumValue)
        {
            Code = code;
            Name = name;
            Note = note;
            IsSumValue = isSumValue;
        }

        /// <summary>
        /// Default constructor for deserialization.
        /// </summary>
        public VariableValue()
        {

        }

        /// <summary>
        /// Generates a deep copy that can be mutated.
        /// </summary>
        public VariableValue Clone()
        {
            var newValue = new VariableValue(Code, Name.Clone(), Note?.Clone(), IsSumValue);
            if (ContentComponent != null)
            {
                newValue.ContentComponent = ContentComponent.Clone();
            }
            if (VirtualComponent != null)
            {
                newValue.VirtualComponent = VirtualComponent.Clone();
            }

            return newValue;
        }

        /// <summary>
        /// Apply meta data editions to this value.
        /// </summary>
        /// <param name="valueEdit"></param>
        public void ApplyEdition(VariableQuery.VariableValueEdition valueEdit)
        {
            Name.Edit(valueEdit.NameEdit);

            if (valueEdit.ContentComponent is ContentComponentEdition cce && ContentComponent is ContentComponent cc)
            {
                var newUnit = cc.Unit.Clone();
                newUnit.Edit(cce.UnitEdit);

                var newSource = cc.Source.Clone();
                newSource.Edit(cce.SourceEdit);

                var newContentComponent = new ContentComponent(
                    newUnit,
                    newSource,
                    cc.NumberOfDecimals,
                    cc.LastUpdated
                    );
                ContentComponent = newContentComponent;
            }
        }
    }
}
