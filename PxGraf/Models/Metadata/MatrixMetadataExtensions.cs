#nullable enable
using Px.Utils.Language;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.ExtensionMethods;
using Px.Utils.Models.Metadata;
using System.Linq;
using PxGraf.Data.MetaData;
using PxGraf.Models.Queries;
using System.Collections.Generic;
using PxGraf.Utility;
using Px.Utils.Models.Metadata.Enums;
using System;
using PxGraf.Settings;
using System.Text;
using PxGraf.Language;

namespace PxGraf.Models.Metadata
{
    public static class MatrixMetadataExtensions
    {
        public static CubeMeta ToQueriedCubeMeta(this IReadOnlyMatrixMetadata input, MatrixQuery query)
        {
            MultilanguageString? note = input.GetMatrixProperty("NOTE");
            MultilanguageString? source = input.GetMatrixProperty("SOURCE"); // TODO: Source can also be a content dimension value property.
            List<Variable> dimensions = [];
            foreach (IReadOnlyDimension dimension in input.Dimensions)
            {
                MultilanguageString? dimensionNote = dimension.GetDimensionProperty("NOTE", input.DefaultLanguage);
                string? eliminationCode = dimension.GetEliminationValueCode();
                DimensionQuery dimQuery = query.DimensionQueries[dimension.Code];
                List<VariableValue> values = [];
                foreach (IReadOnlyDimensionValue value in dimQuery.ValueFilter.Filter(dimension.Values))
                {
                    DimensionQuery.VariableValueEdition valueEdit = dimQuery.ValueEdits[value.Code];
                    MultilanguageString? valueNote = value.GetValueProperty("VALUENOTE", input.DefaultLanguage);
                    MultilanguageString editedValName = value.Name.CopyAndEdit(valueEdit.NameEdit);
                    VariableValue newValue = new(value.Code, editedValName, valueNote, eliminationCode == value.Code);
                    if (dimension.Type == DimensionType.Content)
                    {
                        if (value is ContentDimensionValue cDimVal)
                        {
                            MultilanguageString editedUnit = cDimVal.Unit.CopyAndEdit(valueEdit.ContentComponent.UnitEdit);
                            MultilanguageString editedSource = source.CopyAndEdit(valueEdit.ContentComponent.SourceEdit); //TODO null handling
                            string updated = cDimVal.LastUpdated.ToString(); // TODO Convert to pxweb format, make extension method for this.
                            newValue.ContentComponent = new(editedUnit, editedSource, cDimVal.Precision, updated);
                        }
                        else
                        {
                            throw new Exception(); // TODO better exception
                        }
                    }
                    values.Add(newValue);
                }
                MultilanguageString editedDimName = dimension.Name.CopyAndEdit(dimQuery.NameEdit);
                dimensions.Add(new(dimension.Code, editedDimName, dimensionNote, dimension.Type, values));
            }

            MultilanguageString header = CreateDefaultHeader(dimensions, query, input.AvailableLanguages)
                .CopyAndEdit(query.ChartHeaderEdit)
                .CopyAndEditAll(h => h[..Configuration.Current.QueryOptions.MaxHeaderLength]);
            return new(input.AvailableLanguages, header, note, dimensions);
        }
        
        // TODO: This has dublication with the above method, combine or make this one obsolete by using MatrixMeta in creation controller.
        public static CubeMeta ToCubeMeta(this IReadOnlyMatrixMetadata input)
        {
            MultilanguageString? note = input.GetMatrixProperty("NOTE");
            MultilanguageString? source = input.GetMatrixProperty("SOURCE"); // TODO: Source can also be a content dimension value property.
            List<Variable> dimensions = [];
            foreach (IReadOnlyDimension dimension in input.Dimensions)
            {
                MultilanguageString? dimensionNote = dimension.GetDimensionProperty("NOTE", input.DefaultLanguage);
                string? eliminationCode = dimension.GetEliminationValueCode();
                List<VariableValue> values = [];
                foreach (IReadOnlyDimensionValue value in dimension.Values)
                {
                    MultilanguageString? valueNote = value.GetValueProperty("VALUENOTE", input.DefaultLanguage);
                    VariableValue newValue = new(value.Code, value.Name, valueNote, eliminationCode == value.Code);
                    if (dimension.Type == DimensionType.Content)
                    {
                        if (value is ContentDimensionValue cDimVal)
                        {
                            string updated = cDimVal.LastUpdated.ToString(); // TODO Convert to pxweb format, make extension method for this.
                            newValue.ContentComponent = new(cDimVal.Unit, source, cDimVal.Precision, updated);
                        }
                        else
                        {
                            throw new Exception(); // TODO better exception
                        }
                    }
                    values.Add(newValue);
                }
                dimensions.Add(new(dimension.Code, dimension.Name, dimensionNote, dimension.Type, values));
            }

            MultilanguageString header = CreateDefaultHeader(dimensions, null, input.AvailableLanguages)
                .CopyAndEditAll(h => h[..Configuration.Current.QueryOptions.MaxHeaderLength]);
            return new(input.AvailableLanguages, header, note, dimensions);
        }

        // TODO clean and document
        public static IReadOnlyMatrixMetadata FilterDimensionValues(this IReadOnlyMatrixMetadata input, MatrixQuery query)
        {
            List<IDimensionMap> dimensionMaps = [];
            foreach (IReadOnlyDimension dimension in input.Dimensions)
            {
                IValueFilter filter = query.DimensionQueries[dimension.Code].ValueFilter;
                List<string> valueCodes = filter.Filter(dimension.Values).Select(v => v.Code).ToList();
                dimensionMaps.Add(new DimensionMap(dimension.Code, valueCodes));
            }
            return input.GetTransform(new MatrixMap(dimensionMaps));
        }

        // TODO move, clean and document
        public static string? GetEliminationValueCode(this IReadOnlyDimension dimension)
        {
            if (dimension.AdditionalProperties.TryGetValue("ELIMINATION", out MetaProperty? property))
            {
                if (property.CanGetStringValue) return property.ValueAsString('"'); //TODO delimeter from config
                else
                {
                    MultilanguageString eliminationName = property.ValueAsMultilanguageString('"', dimension.Name.Languages.First());
                    return dimension.Values.First(v => v.Name.Equals(eliminationName)).Code;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the nuber of multivalue dimensions in the given meta object.
        /// Multivalue dimension is a dimension with more than one value.
        /// </summary>
        public static int GetNumberOfMultivalueDimensions(this IReadOnlyMatrixMetadata cubeMeta)
        {
            return cubeMeta.Dimensions.Count(dimension => dimension.Values.Count > 1);
        }

        /// <summary>
        /// Returns a list that contains only the multivalue dimensions from the cube meta in the same order as they are in the cube meta.
        /// </summary>
        public static IReadOnlyList<IReadOnlyDimension> GetMultivalueDimensions(this IReadOnlyMatrixMetadata cubeMeta)
        {
            return cubeMeta.Dimensions
                .Where(dimension => dimension.Values.Count > 1)
                .ToList();
        }

        /// <summary>
        /// Returns a list that contains only the multivalue dimensions from the cube meta in the same order as they are in the cube meta.
        /// </summary>
        public static IReadOnlyList<IReadOnlyDimension> GetSinglevalueDimensions(this IReadOnlyMatrixMetadata cubeMeta)
        {
            return cubeMeta.Dimensions
                .Where(dimension => dimension.Values.Count == 1)
                .ToList();
        }

        /// <summary>
        /// Returns true if dimensions has more than one included value.
        /// </summary>
        public static bool IsMultivalue(this IReadOnlyDimension dimension)
        {
            return dimension.Values != null && dimension.Values.Count > 1;
        }

        /// <summary>
        /// Returns a list of all of the multiselect dimensions from the given meta object ordered by the number of selected values in them.
        /// </summary>
        /// <param name="meta"></param>
        /// <returns></returns>
        public static IReadOnlyList<IReadOnlyDimension> GetSortedMultivalueDimensions(this IReadOnlyMatrixMetadata meta)
        {
            return [.. meta.Dimensions
                .Where(diemnsion => diemnsion.Values.Count > 1)
                .OrderByDescending(x => x.Values.Count)];
        }

        /// <summary>
        /// Returns the second largest multiselect dimension from the given meta.
        /// Multiselect dimension is a dimension with more than one selected value.
        /// </summary>
        public static IReadOnlyDimension? GetLargestMultivalueDimension(this IReadOnlyMatrixMetadata meta)
        {
            var multiselects = GetSortedMultivalueDimensions(meta);
            return multiselects != null && multiselects.Any() ? multiselects[0] : null;

        }

        /// <summary>
        /// Returns the second largest multivalue dimension from the given cube meta.
        /// Multivalue dimension is a dimension that contains more than one value.
        /// </summary>
        public static IReadOnlyDimension? GetSmallerMultivalueDimension(this IReadOnlyMatrixMetadata meta)
        {
            var multiselects = GetSortedMultivalueDimensions(meta);
            return multiselects != null && multiselects.Count > 1 ? multiselects[1] : null;
        }

        /// <summary>
        /// Returns the time dimennsion if the query contains one, if not, returns a dimension which is ordinal and has the most values.
        /// If the query contains neither time or progressive dimensions, returns null.
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyDimension? GetMultivalueTimeOrLargestOrdinal(this IReadOnlyMatrixMetadata meta)
        {
            var multiselects = GetSortedMultivalueDimensions(meta); //OBS: multiselects are sorted here so First() can be used!
            if (multiselects.FirstOrDefault(d => d.Type == DimensionType.Time) is IReadOnlyDimension timeDim) return timeDim;
            if (multiselects.FirstOrDefault(d => d.Type == DimensionType.Ordinal) is IReadOnlyDimension ordinalDim) return ordinalDim;

            return null;
        }

        /// <summary>
        /// If all content dimensionvalues have the same unit, returns that unit in the given language.
        /// </summary>
        public static string GetUnambiguousUnitsInLang(this IReadOnlyMatrixMetadata meta, string lang)
        {
            IEnumerable<string> units = meta.GetContentDimension().Values.Map(v => v.Unit[lang]);
            return UtilityFunctions.UnambiguousOrDefault(units);
        }

        /// <summary>
        /// Returns the latest update time from the content dimension values.
        /// </summary>
        public static DateTime GetLastUpdated(this IReadOnlyMatrixMetadata meta)
        {
            IEnumerable<DateTime> times = meta.GetContentDimension().Values.Map(cdv => cdv.LastUpdated);
            return times.OrderDescending().First();
        }

        // TODO documentation
        private static MultilanguageString CreateDefaultHeader(IReadOnlyList<Variable> dimensions, MatrixQuery? query, IReadOnlyList<string> langs)
        {
            Dictionary<string, StringBuilder> langToHeader = [];
            foreach (string language in langs)
            {
                Translation translation = Localization.FromLanguage(language).Translation;
                langToHeader[language] = new();

                langToHeader.AppendContentDimensionText(dimensions.First(v => v.Type == DimensionType.Content), language);
                langToHeader.AppendSingleValueDimensionTexts(dimensions, language);

                Variable? timeDim = dimensions.FirstOrDefault(v => v.Type == DimensionType.Time);
                if (timeDim != null && (!query?.DimensionQueries[timeDim.Code].Selectable ?? false))
                {
                    langToHeader[language].AppendTimeValuePlaceholders(translation, timeDim.IncludedValues.Count > 1);
                }

                List<string> dimensionTexts = dimensions
                    .Where(v => v.Type != DimensionType.Time && v.IncludedValues.Count > 1)
                    // We want content dimension appear at the begin [orderBy is stable so otherwise original order is maintained]
                    .OrderBy(v => v.Type == DimensionType.Content ? 0 : 1)
                    .Select(v => v.Name[language])
                    .ToList();

                if (dimensionTexts.Count == 1)
                {
                    langToHeader[language].Append($" {translation.TitleVariable} {dimensionTexts.Single()}");
                }
                else if (dimensionTexts.Count != 0)
                {
                    langToHeader[language].Append($" {translation.TitleVariablePlural} {string.Join(", ", dimensionTexts)}");
                }
            }

            return new(langToHeader.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString()));
        }

        /// <summary>
        /// Takes the header from the cube meta and replaces the [FIRST] and [LAST] placeholders with the first and last time values.
        /// </summary>
        // TODO Move and replace
        public static MultilanguageString ReplaceTimePlaceholdersInHeader(MultilanguageString header, Dimension timeDimension)
        {
            if (timeDimension != null)
            {
                foreach (string lang in header.Languages)
                {
                    string singleLangTitle = header[lang];
                    singleLangTitle = singleLangTitle.Replace("[FIRST]", timeDimension.Values[0].Name[lang]);
                    singleLangTitle = singleLangTitle.Replace("[LAST]", timeDimension.Values[^1].Name[lang]);
                    header = header.CopyAndReplace(lang, singleLangTitle);
                }
            }
            return header;
        }

        private static void AppendTimeValuePlaceholders(this StringBuilder builder, Translation translation, bool multivalue)
        {
            if (builder.Length > 0) builder.Append(' ');

            if (multivalue)
            {
                builder.Append(string.Format(translation.HeaderTimeFormats.TimeRange, "[FIRST]", "[LAST]"));
            }
            else
            {
                builder.Append(string.Format(translation.HeaderTimeFormats.SingleTimeValue, "[FIRST]"));
            }
        }

        private static void AppendContentDimensionText(this Dictionary<string, StringBuilder> builders, Variable contentDim, string language)
        {
            if (contentDim.IncludedValues.Count == 1)
            {
                builders[language].Append(contentDim.IncludedValues[0].Name[language]);
            }
            else
            {
                builders[language].Append(contentDim.Name[language]);
            }
        }

        private static void AppendSingleValueDimensionTexts(this Dictionary<string, StringBuilder> builders, IReadOnlyList<Variable> variables, string language)
        {
            IEnumerable<Variable> singleValueVariables = variables
                .Where(d => d.IncludedValues.Count == 1 && d.Type != DimensionType.Content && d.Type != DimensionType.Time)
                .Where(singleDim => singleDim.IncludedValues.Count != 0);

            foreach (var singleDim in singleValueVariables)
            {
                VariableValue dimensionValue = singleDim.IncludedValues[0];
                if (!dimensionValue.IsSumValue)
                {
                    if (builders[language].Length > 0)
                    {
                        builders[language].Append(", ");
                    }

                    builders[language].Append(dimensionValue.Name[language]);
                }
            }
        }

        private static MultilanguageString? GetDimensionProperty(this IReadOnlyDimension dimension, string propertyKey, string defaultLang)
        {
            if (dimension.AdditionalProperties.TryGetValue(propertyKey, out MetaProperty? prop) && prop.CanGetMultilanguageValue)
            {
                return prop.ValueAsMultilanguageString('"', defaultLang);
            }

            return null;
        }

        private static MultilanguageString? GetMatrixProperty(this IReadOnlyMatrixMetadata meta, string propertyKey)
        {
            if (meta.AdditionalProperties.TryGetValue(propertyKey, out MetaProperty? prop) && prop.CanGetMultilanguageValue)
            {
                return prop.ValueAsMultilanguageString('"', meta.DefaultLanguage);
            }

            return null;
        }

        private static MultilanguageString? GetValueProperty(this IReadOnlyDimensionValue value, string propertyKey, string defaultLang)
        {
            if (value.AdditionalProperties.TryGetValue(propertyKey, out MetaProperty? prop) && prop.CanGetMultilanguageValue)
            {
                return prop.ValueAsMultilanguageString('"', defaultLang);
            }

            return null;
        }
    }
}
#nullable disable
