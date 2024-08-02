using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.Enums;
using PxGraf.Data.MetaData;
using PxGraf.Settings;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace PxGraf.Models.Metadata
{
    public static class PxGrafMetaExtensions
    {
        // TODO: move to some utility class, there are multiple instances of this conversion in the codebase.
        private const string PXWEB_DATETIME_FORMAT = "yyyy'-'MM'-'dd'T'HH'.'mm'.'ss'Z'";

        public static MatrixMetadata ToMatrixMetadata(this CubeMeta pxGrafMeta)
        {
            List<Dimension> dimensions = [];
            Dictionary<string, MetaProperty> matrixProperties = [];
            foreach(Variable variable in pxGrafMeta.Variables)
            {
                Dictionary<string, MetaProperty> dimensionProperties = [];
                if(variable.Type == DimensionType.Content)
                {
                    List<ContentDimensionValue> contentDimValues = [];
                    foreach (VariableValue varValue in variable.IncludedValues)
                    {
                        if (varValue.IsSumValue) dimensionProperties["ELIMINATION"] = new("ELIMINATION", varValue.Code); // TODO: this should also be a extension method or something.
                        DateTime lastUpdate = DateTime.ParseExact(varValue.ContentComponent.LastUpdated, PXWEB_DATETIME_FORMAT, CultureInfo.InvariantCulture);
                        ContentDimensionValue cdv = new(varValue.Code, varValue.Name, varValue.ContentComponent.Unit, lastUpdate, varValue.ContentComponent.NumberOfDecimals);
                        contentDimValues.Add(cdv);
                    }
                    dimensions.Add(new ContentDimension(variable.Code, variable.Name, dimensionProperties, contentDimValues));
                    matrixProperties["SOURCE"] = new("SOURCE", variable.IncludedValues[0].ContentComponent.Source);
                }
            }
            string defaultLang = Configuration.Current.LanguageOptions.Default ?? pxGrafMeta.Languages[0]; // TODO: Meke this an extension method or something aswell.
            return new(defaultLang, pxGrafMeta.Languages, dimensions, matrixProperties);
        }
    }
}
