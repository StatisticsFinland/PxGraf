using Px.Utils.Language;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata.MetaProperties;
using PxGraf.Data;
using PxGraf.Data.MetaData;
using PxGraf.Settings;
using PxGraf.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PxGraf.Models.Metadata
{
    /// <summary>
    /// Extension methods for <see cref="CubeMeta"/> objects.
    /// </summary>
    public static class PxGrafMetaExtensions
    {
        public static MatrixMetadata ToMatrixMetadata(this CubeMeta pxGrafMeta)
        {
            List<Dimension> dimensions = new(pxGrafMeta.Variables.Count);
            Dictionary<string, MetaProperty> matrixProperties = [];
            if (pxGrafMeta.Note != null)
            {
                Dictionary<string, string> note = [];
                foreach (var lang in pxGrafMeta.Note.Languages)
                {
                    note[lang] = $"\"{pxGrafMeta.Note[lang]}\""; // Meta property values must be enclosed
                }
                matrixProperties[PxSyntaxConstants.NOTE_KEY] = new MultilanguageStringProperty(new(note));
            }
            foreach (Variable variable in pxGrafMeta.Variables)
            {
                Dimension dimension = variable.ConvertToDimension();
                dimensions.Add(dimension);
                if (dimension is ContentDimension)
                {
                    matrixProperties[PxSyntaxConstants.SOURCE_KEY] = new MultilanguageStringProperty(variable.Values[0].ContentComponent.Source);
                }
            }
            string defaultLang = pxGrafMeta.GetDefaultLanguage();
            MatrixMetadata meta = new(defaultLang, pxGrafMeta.Languages, dimensions, matrixProperties);
            meta = meta.AssignOrdinalDimensionTypes();
            meta.AssignSourceToContentDimensionValues();
            return meta;
        }

            /// <summary>
            /// Returns the default language of the cube.
            /// </summary>
            /// <param name="pxGrafMeta">The cube metadata.</param>
            /// <returns>The default language of the cube.</returns>
            public static string GetDefaultLanguage(this CubeMeta pxGrafMeta)
        {
            return pxGrafMeta.Languages.Contains(Configuration.Current.LanguageOptions.Default) ? 
                Configuration.Current.LanguageOptions.Default :
                pxGrafMeta.Languages[0];
        }

        /// <summary>
        /// Adds the elimination key to the dimension properties if the dimension value is a sum value.
        /// </summary>
        /// <param name="dimensionValue">The dimension value to check.</param>
        /// <param name="dimensionProperties">The dimension properties to add the elimination key to.</param>
        public static void AddEliminationKeyIfSumValue(this VariableValue dimensionValue, Dictionary<string, MetaProperty> dimensionProperties)
        {
            if (dimensionValue.IsSumValue)
            {
                dimensionProperties[PxSyntaxConstants.ELIMINATION_KEY] = new StringProperty(dimensionValue.Code);
            }
        }

        public static Dimension ConvertToDimension(this Variable input)
        {
            Dictionary<string, MetaProperty> dimensionProperties = [];
            if (input.DimensionType == DimensionType.Content)
            {
                List<ContentDimensionValue> contentDimValues = [];
                foreach (VariableValue dimValue in input.Values)
                {
                    dimValue.AddEliminationKeyIfSumValue(dimensionProperties);
                    DateTime lastUpdate = PxSyntaxConstants.ParseDateTime(dimValue.ContentComponent.LastUpdated);
                    ContentDimensionValue cdv = new(dimValue.Code, dimValue.Name, dimValue.ContentComponent.Unit, lastUpdate, dimValue.ContentComponent.NumberOfDecimals);
                    contentDimValues.Add(cdv);
                }
                return new ContentDimension(input.Code, input.Name, dimensionProperties, contentDimValues);
            }
            else if (input.DimensionType == DimensionType.Time)
            {
                List<DimensionValue> values = [];
                foreach (VariableValue dimValue in input.Values)
                {
                    dimValue.AddEliminationKeyIfSumValue(dimensionProperties);
                    DimensionValue tdv = new(dimValue.Code, dimValue.Name);
                    values.Add(tdv);
                }
                TimeDimensionInterval intervals = TimeVarIntervalParser.DetermineIntervalFromCodes(values.Select(v => v.Code).ToList());
                return new TimeDimension(input.Code, input.Name, dimensionProperties, values, intervals);
            }
            else
            {
                List<DimensionValue> values = [];
                foreach (VariableValue dimValue in input.Values)
                {
                    dimValue.AddEliminationKeyIfSumValue(dimensionProperties);
                    DimensionValue dv = new(dimValue.Code, dimValue.Name);
                    values.Add(dv);
                }
                return new Dimension(input.Code, input.Name, dimensionProperties, values, input.DimensionType);
            }
        }
    }
}
