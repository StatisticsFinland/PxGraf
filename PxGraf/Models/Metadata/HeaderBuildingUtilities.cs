using Px.Utils.Language;
using Px.Utils.Models.Metadata.Dimensions;
using PxGraf.Language;
using PxGraf.Models.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Px.Utils.Models.Metadata.Enums;

namespace PxGraf.Models.Metadata
{
    /// <summary>
    /// Utility class for building headers.
    /// </summary>
    public static class HeaderBuildingUtilities
    {
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

                langToHeader.AppendContentDimensionText(dimensions.First(v => v.Type == DimensionType.Content), language);
                langToHeader.AppendSingleValueDimensionTexts(dimensions, language);

                IReadOnlyDimension? timeDim = dimensions.FirstOrDefault(v => v.Type == DimensionType.Time);
                if (timeDim != null && (!query?.DimensionQueries[timeDim.Code].Selectable ?? false))
                {
                    langToHeader[language].AppendTimeValuePlaceholders(translation, timeDim.Values.Count > 1);
                }

                List<string> dimensionTexts = dimensions
                    .Where(v => v.Type != DimensionType.Time && v.Values.Count > 1)
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

        private static void AppendContentDimensionText(this Dictionary<string, StringBuilder> builders, IReadOnlyDimension contentDim, string language)
        {
            if (contentDim.Values.Count == 1)
            {
                builders[language].Append(contentDim.Values[0].Name[language]);
            }
            else
            {
                builders[language].Append(contentDim.Name[language]);
            }
        }

        private static void AppendSingleValueDimensionTexts(this Dictionary<string, StringBuilder> builders, IReadOnlyList<IReadOnlyDimension> dimensions, string language)
        {
            IEnumerable<IReadOnlyDimension> singleValueDimensions = dimensions
                .Where(d => d.Values.Count == 1 && d.Type != DimensionType.Content && d.Type != DimensionType.Time)
                .Where(singleDim => singleDim.Values.Count != 0);


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

                    builders[language].Append(dimensionValue.Name[language]);
                }
            }
        }
    }
}
