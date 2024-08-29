using Px.Utils.Language;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Data;
using PxGraf.Data.MetaData;
using PxGraf.Settings;
using PxGraf.Utility;
using System;
using System.Collections.Generic;
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
            List<Dimension> dimensions = [];
            Dictionary<string, MetaProperty> matrixProperties = [];
            if (pxGrafMeta.Note != null)
            {
                Dictionary<string, string> note = [];
                foreach (var lang in pxGrafMeta.Note.Languages)
                {
                    note[lang] = $"\"{pxGrafMeta.Note[lang]}\""; // Meta property values must be enclosed
                }
                matrixProperties[PxSyntaxConstants.NOTE_KEY] = new(PxSyntaxConstants.NOTE_KEY, new MultilanguageString(note));
            }
            foreach(Variable dimension in pxGrafMeta.Variables)
            {
                Dictionary<string, MetaProperty> dimensionProperties = [];
                if (dimension.Type == DimensionType.Content)
                {
                    List<ContentDimensionValue> contentDimValues = [];
                    foreach (VariableValue dimValue in dimension.IncludedValues)
                    {
                        dimValue.AddEliminationKeyIfSumValue(dimensionProperties);
                        DateTime lastUpdate = PxSyntaxConstants.ParsePxDateTime(dimValue.ContentComponent.LastUpdated);
                        ContentDimensionValue cdv = new(dimValue.Code, dimValue.Name, dimValue.ContentComponent.Unit, lastUpdate, dimValue.ContentComponent.NumberOfDecimals);
                        contentDimValues.Add(cdv);
                    }
                    dimensions.Add(new ContentDimension(dimension.Code, dimension.Name, dimensionProperties, contentDimValues));
                    matrixProperties[PxSyntaxConstants.SOURCE_KEY] = new(PxSyntaxConstants.SOURCE_KEY, dimension.IncludedValues[0].ContentComponent.Source);
                }
                else if (dimension.Type == DimensionType.Time)
                {
                    List<DimensionValue> values = [];
                    foreach (VariableValue dimValue in dimension.IncludedValues)
                    {
                        dimValue.AddEliminationKeyIfSumValue(dimensionProperties);
                        DimensionValue tdv = new(dimValue.Code, dimValue.Name);
                        values.Add(tdv);
                    }
                    TimeDimensionInterval intervals = TimeVarIntervalParser.DetermineIntervalFromCodes(values.Select(v => v.Code).ToList());
                    dimensions.Add(new TimeDimension(dimension.Code, dimension.Name, dimensionProperties, values, intervals));
                }
                else
                {
                    List<DimensionValue> values = [];
                    foreach (VariableValue dimValue in dimension.IncludedValues)
                    {
                        dimValue.AddEliminationKeyIfSumValue(dimensionProperties);
                        DimensionValue dv = new(dimValue.Code, dimValue.Name);
                        values.Add(dv);
                    }
                    dimensions.Add(new Dimension(dimension.Code, dimension.Name, dimensionProperties, values, dimension.Type));
                }
            }
            string defaultLang = pxGrafMeta.GetDefaultLanguage();
            return new(defaultLang, pxGrafMeta.Languages, dimensions, matrixProperties);
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
                dimensionProperties[PxSyntaxConstants.ELIMINATION_KEY] = new MetaProperty(PxSyntaxConstants.ELIMINATION_KEY, dimensionValue.Code);
            }
        }
    }
}
