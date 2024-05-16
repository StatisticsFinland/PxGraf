using Microsoft.Extensions.Primitives;
using PxGraf.Data.MetaData;
using PxGraf.Enums;
using PxGraf.Language;
using PxGraf.Models.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PxGraf.Utility
{
    /// <summary>
    /// Utility functions for extracting and generating information about the metadata.
    /// </summary>
    public static class CubeMetaExtensions
    {
        /// <summary>
        /// Returns the nuber of multivalue variables in the given meta object.
        /// Multivalue variable is a variable with more than one value.
        /// </summary>
        public static int GetNumberOfMultivalueVariables(this IReadOnlyCubeMeta cubeMeta)
        {
            return cubeMeta.Variables.Count(variable => variable.IncludedValues.Count > 1);
        }

        /// <summary>
        /// Returns a list that contains only the multivalue variables from the cube meta in the same order as they are in the cube meta.
        /// </summary>
        public static IReadOnlyList<IReadOnlyVariable> GetMultivalueVariables(this IReadOnlyCubeMeta cubeMeta)
        {
            return cubeMeta.Variables
                .Where(variable => variable.IncludedValues.Count > 1)
                .ToList();
        }

        /// <summary>
        /// Returns a list that contains only the multivalue variables from the cube meta in the same order as they are in the cube meta.
        /// </summary>
        public static IReadOnlyList<IReadOnlyVariable> GetSinglevalueVariables(this IReadOnlyCubeMeta cubeMeta)
        {
            return cubeMeta.Variables
                .Where(variable => variable.IncludedValues.Count == 1)
                .ToList();
        }

        /// <summary>
        /// Returns true if variables has more than one included value.
        /// </summary>
        public static bool IsMultivalue(this IReadOnlyVariable variable)
        {
            return variable.IncludedValues != null && variable.IncludedValues.Count > 1;
        }

        /// <summary>
        /// Returns a list of all of the multiselect variables from the given meta object ordered by the number of selected values in them.
        /// </summary>
        /// <param name="cubeMeta"></param>
        /// <returns></returns>
        public static IReadOnlyList<IReadOnlyVariable> GetSortedMultivalueVariables(this IReadOnlyCubeMeta cubeMeta)
        {
            return cubeMeta.Variables
                .Where(variable => variable.IncludedValues.Count > 1)
                .OrderByDescending(x => x.IncludedValues.Count)
                .ToList();
        }

        /// <summary>
        /// Returns the second largest multiselect variable from the given cube meta.
        /// Multiselect variable is a variable with more than one selected value.
        /// </summary>
        public static IReadOnlyVariable GetLargestMultivalueVariable(this IReadOnlyCubeMeta cubeMeta)
        {
            var multiselects = GetSortedMultivalueVariables(cubeMeta);
            return multiselects != null && multiselects.Any() ? multiselects[0] : null;

        }

        /// <summary>
        /// Returns the second largest multivalue variable from the given cube meta.
        /// Multivalue variable is a variable that contains more than one value.
        /// </summary>
        public static IReadOnlyVariable GetSmallerMultivalueVariable(this IReadOnlyCubeMeta cubeMeta)
        {
            var multiselects = GetSortedMultivalueVariables(cubeMeta);
            return multiselects != null && multiselects.Count > 1 ? multiselects[1] : null;
        }

        /// <summary>
        /// Returns the time dimennsion if the query contains one, if not, returns a variable which is ordinal and has the most values.
        /// If the query contains neither time or progressive dimensions, returns null.
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyVariable GetMultivalueTimeOrLargestOrdinal(this IReadOnlyCubeMeta cubeMeta)
        {
            var multiselects = GetSortedMultivalueVariables(cubeMeta); //OBS: multiselects are sorted here so First() can be used!
            if (multiselects.FirstOrDefault(v => v.Type == VariableType.Time) is IReadOnlyVariable timeVar) return timeVar;
            if (multiselects.FirstOrDefault(v => v.Type == VariableType.Ordinal) is IReadOnlyVariable ordinalVar) return ordinalVar;

            return null;
        }

        /// <summary>
        /// Returns the content variable of the query (there should ever be only one),
        /// throws an exception if there is none or more than one.
        /// </summary>
        public static IReadOnlyVariable GetContentVariable(this IReadOnlyCubeMeta meta)
        {
            try
            {
                return meta.Variables.Single(v => v.Type == VariableType.Content);
            }
            catch (InvalidOperationException e)
            {
                throw new ArgumentException("Provided cube meta does not contain a content variable or the content variable can not be recognized.", e);
            }
        }

        /// <summary>
        /// Returns the time variable of the query (there should ever be only one),
        /// throws an exception if there is none or more than one.
        /// </summary>
        public static IReadOnlyVariable GetTimeVariable(this IReadOnlyCubeMeta meta)
        {
            try
            {
                return meta.Variables.Single(v => v.Type == VariableType.Time);
            }
            catch (InvalidOperationException e)
            {
                throw new ArgumentException("Provided cube meta does not contain a time variable or the time variable can not be recognized.", e);
            }
        }

        /// <summary>
        /// If all content variablevalues have the same unit, returns that unit in the given language.
        /// </summary>
        public static string GetUnambiguousUnitsInLang(this IReadOnlyCubeMeta meta, string lang)
        {
            var units = GetContentVariable(meta).IncludedValues.Select(cvv => cvv.ContentComponent.Unit[lang]);
            return UtilityFunctions.UnambiguousOrDefault(units);
        }

        /// <summary>
        /// If all content variablevalues have the same source, returns that source in the given language.
        /// </summary>
        public static string GetUnambiguousSourceInLang(this IReadOnlyCubeMeta meta, string lang)
        {
            var sources = GetContentVariable(meta).IncludedValues.Select(cvv => cvv.ContentComponent.Source[lang]);
            return UtilityFunctions.UnambiguousOrDefault(sources);
        }

        /// <summary>
        /// Returns the latest update time from the content variable values.
        /// </summary>
        public static string GetLastUpdated(this IReadOnlyCubeMeta meta)
        {
            var times = GetContentVariable(meta).IncludedValues.Select(cvv => cvv.ContentComponent.LastUpdated);
            return times.ToList().FindAll(s => !string.IsNullOrEmpty(s)).OrderByDescending(q => q.ToLower()).FirstOrDefault();
        }

        /// <summary>
        /// Creates the default chart header for all languages in the metaobject based on the variable values.
        /// </summary>
        public static MultiLanguageString CreateDefaultChartHeader(this IReadOnlyCubeMeta cubeMeta, CubeQuery query)
        {
            var defaultChartHeader = new MultiLanguageString();
            foreach (var language in cubeMeta.Languages)
            {
                defaultChartHeader.AddTranslation(language, CreateDefaultChartHeader(cubeMeta, language, query));
            }

            return defaultChartHeader;
        }

        /// <summary>
        /// Takes the header from the cube meta and replaces the [FIRST] and [LAST] placeholders with the first and last time values.
        /// </summary>
        public static IReadOnlyMultiLanguageString GetHeaderWithoutTimePlaceholders(this IReadOnlyCubeMeta meta)
        {
            var timeVar = meta.GetTimeVariable();
            var header = meta.Header.Clone();

            if (timeVar != null)
            {
                foreach (string lang in header.Languages)
                {
                    var singleLangTitle = header[lang];
                    singleLangTitle = singleLangTitle.Replace("[FIRST]", timeVar.IncludedValues[0].Name[lang]);
                    singleLangTitle = singleLangTitle.Replace("[LAST]", timeVar.IncludedValues[timeVar.IncludedValues.Count - 1].Name[lang]);
                    header.EditTranslation(lang, singleLangTitle);
                }
            }
            return header;
        }

        /// <summary>
        /// Generates a default header in one language for the visualization based on the current metadata.
        /// </summary>
        private static string CreateDefaultChartHeader(IReadOnlyCubeMeta cubeMeta, string language, CubeQuery query)
        {
            Localization localization = Localization.FromLanguage(language);
            StringBuilder titleBuilder = new();

            titleBuilder.AppendContentVariableText(cubeMeta, language);

            titleBuilder.AppendSingleValueVariableTexts(cubeMeta, language);

            titleBuilder.AppendTimeVariableText(cubeMeta, query, localization);

            List<string> dimensionTexts = cubeMeta.Variables
                .Where(v => v.Type != VariableType.Time && IsMultiValue(v))
                .OrderBy(v => v.Type == VariableType.Content ? 0 : 1) // We want content dimension appear at the begin [orderBy is stable so otherwise original order is maintained]
                .Select(v => v.Name[language])
                .ToList();

            if (dimensionTexts.Count == 0)
            {
                return titleBuilder.ToString();
            }
            else if (dimensionTexts.Count == 1)
            {
                return $"{titleBuilder} {localization.Translation.TitleVariable} {dimensionTexts.Single()}";
            }
            else
            {
                return $"{titleBuilder} {localization.Translation.TitleVariablePlural} {string.Join(", ", dimensionTexts)}";
            }
        }

        private static void AppendContentVariableText(this StringBuilder builder, IReadOnlyCubeMeta cubeMeta, string language)
        {
            IReadOnlyVariable contentVariable = cubeMeta.Variables.SingleOrDefault(v => v.Type == VariableType.Content);
            if (contentVariable is not null)
            {
                if (contentVariable.IncludedValues.Count == 1)
                {
                    builder.Append(contentVariable.IncludedValues[0].Name[language]);
                }
                else
                {
                    builder.Append(contentVariable.Name[language]);
                }
            }
        }

        private static void AppendSingleValueVariableTexts(this StringBuilder builder, IReadOnlyCubeMeta cubeMeta, string language)
        {
            foreach (IReadOnlyVariable singleVar in cubeMeta.Variables.Where(v => !IsMultiValue(v) && v.Type != VariableType.Content && v.Type != VariableType.Time))
            {
                if (singleVar.IncludedValues.Any())
                {
                    IReadOnlyVariableValue variableValue = singleVar.IncludedValues[0];

                    if (!variableValue.IsSumValue)
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append(", ");
                        }

                        builder.Append(variableValue.Name[language]);
                    }
                }
            }
        }

        private static void AppendTimeVariableText(this StringBuilder builder, IReadOnlyCubeMeta cubeMeta, CubeQuery query, Localization localization)
        {
            IReadOnlyVariable timeVariable = cubeMeta.Variables.SingleOrDefault(v => v.Type == VariableType.Time);
            if (timeVariable != null && !query.VariableQueries[timeVariable.Code].Selectable)
            {
                if (builder.Length > 0)
                {
                    builder.Append(' ');
                }

                if (IsMultiValue(timeVariable))
                {
                    builder.Append(string.Format(localization.Translation.HeaderTimeFormats.TimeRange, "[FIRST]", "[LAST]"));
                }
                else
                {
                    builder.Append(string.Format(localization.Translation.HeaderTimeFormats.SingleTimeValue, "[FIRST]"));
                }
            }
        }

        private static bool IsMultiValue(IReadOnlyVariable variable)
        {
            return variable.IncludedValues.Count >= 2;
        }
    }
}
