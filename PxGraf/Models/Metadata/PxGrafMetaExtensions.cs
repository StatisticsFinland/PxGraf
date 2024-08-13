using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Data.MetaData;
using PxGraf.Settings;
using PxGraf.Utility;
using System;
using System.Collections.Generic;

namespace PxGraf.Models.Metadata
{
    /// <summary>
    /// TODO: document
    /// </summary>
    public static class PxGrafMetaExtensions
    {
        public static MatrixMetadata ToMatrixMetadata(this CubeMeta pxGrafMeta)
        {
            List<Dimension> dimensions = [];
            Dictionary<string, MetaProperty> matrixProperties = [];
            foreach(Variable dimension in pxGrafMeta.Variables)
            {
                Dictionary<string, MetaProperty> dimensionProperties = [];
                if(dimension.Type == DimensionType.Content)
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
            }
            string defaultLang = pxGrafMeta.GetDefaultLanguage();
            return new(defaultLang, pxGrafMeta.Languages, dimensions, matrixProperties);
        }

        /// <summary>
        /// TODO: document
        /// </summary>
        public static string GetDefaultLanguage(this CubeMeta pxGrafMeta)
        {
            return Configuration.Current.LanguageOptions.Default ?? pxGrafMeta.Languages[0];
        }

        /// <summary>
        /// TODO: document
        /// </summary>
        public static void AddEliminationKeyIfSumValue(this VariableValue dimensionValue, Dictionary<string, MetaProperty> dimensionProperties)
        {
            if (dimensionValue.IsSumValue)
            {
                dimensionProperties[PxSyntaxConstants.ELIMINATION_KEY] = new MetaProperty(PxSyntaxConstants.ELIMINATION_KEY, dimensionValue.Code);
            }
        }
    }
}
