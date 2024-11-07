#nullable enable
using Px.Utils.Language;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata.ExtensionMethods;
using PxGraf.Language;
using PxGraf.Models.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static PxGraf.Models.Queries.DimensionQuery;

namespace PxGraf.Models.Metadata
{
    /// <summary>
    /// Utility class for building headers.
    /// </summary>
    public static class HeaderBuildingUtilities
    {
        /// <summary>
        /// Gets header for the given metadata and query.
        /// </summary>
        /// <param name="meta">Metadata to be used in the header.</param>
        /// <param name="query">Query properties to check for edited header.</param>
        /// <param name="keepPlaceHolders">Whether to keep placeholders in the header.</param>
        /// <returns><see cref="MultilanguageString"/> object that contains the header for different languages.</returns>
        public static MultilanguageString GetHeader(IReadOnlyMatrixMetadata meta, MatrixQuery query, bool keepPlaceHolders = false)
        {
            MultilanguageString defaultHeader = CreateDefaultHeader(meta.Dimensions, query, meta.AvailableLanguages);
            if (query.ChartHeaderEdit != null)
            {
                Dictionary<string, string> translations = [];
                foreach (string lang in defaultHeader.Languages)
                {
                    if (query.ChartHeaderEdit.Languages.Contains(lang))
                    {
                        translations[lang] = query.ChartHeaderEdit[lang];
                    }
                    else
                    {
                        translations[lang] = defaultHeader[lang];
                    }
                }
                MultilanguageString mls = new(translations);
                return keepPlaceHolders ? mls : ReplaceTimePlaceholdersInHeader(new(translations), meta.GetTimeDimension());
            }
            else
            {
                return keepPlaceHolders ? defaultHeader : ReplaceTimePlaceholdersInHeader(defaultHeader, meta.GetTimeDimension());
            }
        }

        /// <summary>
        /// Returns default header for the given dimensions and query.
        /// </summary>
        /// <param name="dimensions">Dimensions to be used in the header.</param>
        /// <param name="query">Query properties that define the header.</param>
        /// <param name="langs">Languages to be used in the header.</param>
        /// <returns><see cref="MultilanguageString"/> object that contains the header for different languages.</returns>
        public static MultilanguageString CreateDefaultHeader(IReadOnlyList<IReadOnlyDimension> dimensions, MatrixQuery? query, IReadOnlyList<string> langs)
        {
            Dictionary<string, StringBuilder> langToHeader = [];
            foreach (string language in langs)
            {
                Translation translation = Localization.FromLanguage(language).Translation;
                langToHeader[language] = new();

                langToHeader.AppendContentDimensionText(dimensions.First(v => v.Type == DimensionType.Content), language, query);
                langToHeader.AppendSingleValueDimensionTexts(dimensions, language, query);

                IReadOnlyDimension? timeDim = dimensions.FirstOrDefault(v => v.Type == DimensionType.Time);
                if (timeDim != null && (!query?.DimensionQueries?[timeDim.Code].Selectable ?? false))
                {
                    langToHeader[language].AppendTimeValuePlaceholders(translation, timeDim.Values.Count > 1);
                }

                List<string> dimensionTexts = dimensions
                    .Where(v => v.Type != DimensionType.Time && v.Values.Count > 1)
                    // We want content dimension appear at the beginning [orderBy is stable so otherwise original order is maintained]
                    .OrderBy(v => v.Type == DimensionType.Content ? 0 : 1)
                    .Select(v => GetDimensionNameEditForLanguage(query, v, language))
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

        private static void AppendContentDimensionText(this Dictionary<string, StringBuilder> builders, IReadOnlyDimension contentDim, string language, MatrixQuery? query)
        {
            if (contentDim.Values.Count == 1)
            {
                builders[language].Append(GetDimensionValueNameEditForLanguage(query, contentDim.Code, contentDim.Values[0], language));
            }
            else
            {
                builders[language].Append(GetDimensionNameEditForLanguage(query, contentDim, language));
            }
        }

        private static void AppendSingleValueDimensionTexts(this Dictionary<string, StringBuilder> builders, IReadOnlyList<IReadOnlyDimension> dimensions, string language, MatrixQuery? query)
        {
            IEnumerable<IReadOnlyDimension> singleValueDimensions = dimensions
                .Where(d => d.Values.Count == 1 && d.Type != DimensionType.Content && d.Type != DimensionType.Time);

            foreach (var singleDim in singleValueDimensions)
            {
                string? eliminationCode = singleDim.GetEliminationValueCode();
                IReadOnlyDimensionValue dimensionValue = singleDim.Values[0];
                if (dimensionValue.Code != eliminationCode)
                {
                    if (builders[language].Length > 0)
                    {
                        builders[language].Append(", ");
                    }

                    builders[language].Append(GetDimensionValueNameEditForLanguage(query, singleDim.Code, dimensionValue, language));
                }
            }
        }

        private static string GetDimensionNameEditForLanguage(MatrixQuery? query, IReadOnlyDimension dimension, string language)
        {
            if (query?.DimensionQueries?.TryGetValue(dimension.Code, out DimensionQuery? dq) == true &&
                dq.NameEdit?.Languages.Contains(language) == true)
            {
                return dq.NameEdit[language];
            }
            return dimension.Name[language];
        }

        private static string GetDimensionValueNameEditForLanguage(MatrixQuery? query, string dimensionCode, IReadOnlyDimensionValue value, string language)
        {
            if (query?.DimensionQueries?.TryGetValue(dimensionCode, out DimensionQuery? dq) == true &&
                dq.ValueEdits?.TryGetValue(value.Code, out VariableValueEdition? dvq) == true &&
                dvq.NameEdit?.Languages.Contains(language) == true)
            {
                return dvq.NameEdit[language];
            }
            return value.Name[language];
        }
    }
}
#nullable disable
