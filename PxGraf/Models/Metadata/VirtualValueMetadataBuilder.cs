using Px.Utils.Language;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.MetaProperties;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PxGraf.Models.Metadata
{
    /// <summary>
    /// Adds virtual dimension values to matrix metadata without fetching or computing matrix data.
    /// This keeps metadata-only responses consistent with the metadata produced by
    /// <see cref="Services.VirtualValueComputationService"/>.
    /// </summary>
    public static class VirtualValueMetadataBuilder
    {
        /// <summary>
        /// Returns a copy of the metadata with all virtual values from the query appended.
        /// Definitions are applied in dependency order so chained virtual values are supported.
        /// </summary>
        public static IReadOnlyMatrixMetadata Build(IReadOnlyMatrixMetadata metadata, MatrixQuery query)
        {
            foreach (KeyValuePair<string, DimensionQuery> dimensionEntry in query.DimensionQueries)
            {
                if (dimensionEntry.Value.VirtualValueDefinitions?.Count > 0)
                {
                    List<VirtualValueDefinition> ordered = OrderDefinitions(dimensionEntry.Value.VirtualValueDefinitions);
                    Dictionary<string, int> typeCounters = [];
                    foreach (VirtualValueDefinition definition in ordered)
                    {
                        string operationType = GetOperationType(definition);
                        typeCounters[operationType] = typeCounters.GetValueOrDefault(operationType, 0) + 1;
                        DimensionValue newValue = CreateValue(
                            metadata,
                            dimensionEntry.Key,
                            definition,
                            dimensionEntry.Value,
                            typeCounters[operationType]);
                        metadata = AppendDimensionValue(metadata, dimensionEntry.Key, newValue);
                    }
                }
            }
            return metadata;
        }

        internal static DimensionValue CreateValue(
            IReadOnlyMatrixMetadata metadata,
            string dimensionCode,
            VirtualValueDefinition definition,
            DimensionQuery dimensionQuery,
            int sequenceNumber)
        {
            IReadOnlyDimension dimension = metadata.Dimensions.First(d => d.Code == dimensionCode);
            IReadOnlyList<string> languages = metadata.AvailableLanguages;
            MultilanguageString defaultName = new(languages.ToDictionary(language => language, language =>
            {
                string placeholder = GetOperationPlaceholder(Localization.FromLanguage(language).Translation, definition);
                return $"{placeholder} {sequenceNumber}";
            }));

            MultilanguageString resolvedName = dimensionQuery.ValueEdits.TryGetValue(
                definition.Code,
                out DimensionQuery.DimensionValueEdition edition) && edition.NameEdit is not null
                ? new MultilanguageString(languages.ToDictionary(
                    language => language,
                    language => edition.NameEdit.Languages.Contains(language) ? edition.NameEdit[language] : defaultName[language]))
                : defaultName;

            if (dimension is not ContentDimension contentDimension)
            {
                return new DimensionValue(definition.Code, resolvedName);
            }

            IReadOnlyList<string> operandCodes = definition.GetOperandCodes();
            if (operandCodes.Count == 0)
            {
                throw new InvalidOperationException($"Virtual value '{definition.Code}': definition contains no operand codes.");
            }

            ContentDimensionValue firstOperand = (ContentDimensionValue)contentDimension.Values
                .First(value => value.Code == operandCodes[0]);
            int precision = operandCodes
                .Select(code => contentDimension.Values.First(value => value.Code == code))
                .OfType<ContentDimensionValue>()
                .Min(value => value.Precision);
            MultilanguageString placeholder = new(languages.ToDictionary(
                language => language,
                language => Localization.FromLanguage(language).Translation.ComputedValuePlaceholder));

            return new ContentDimensionValue(
                definition.Code,
                resolvedName,
                placeholder,
                firstOperand.LastUpdated,
                precision,
                false,
                new Dictionary<string, MetaProperty>
                {
                    [PxSyntaxConstants.SOURCE_KEY] = new MultilanguageStringProperty(placeholder)
                });
        }

        internal static string GetOperationType(VirtualValueDefinition definition) => definition switch
        {
            SumDefinition => "sum",
            SubtractionOfTwoDefinition or SubtractionByConstantDefinition => "subtraction",
            MultiplicationOfTwoDefinition or MultiplicationByConstantDefinition => "multiplication",
            DivisionOfTwoDefinition or DivisionByConstantDefinition => "division",
            _ => "computed"
        };

        internal static List<VirtualValueDefinition> OrderDefinitions(List<VirtualValueDefinition> definitions)
        {
            HashSet<string> virtualCodes = [.. definitions.Select(definition => definition.Code)];
            Dictionary<string, HashSet<string>> dependencies = [];
            Dictionary<string, int> inDegree = [];
            Dictionary<string, VirtualValueDefinition> byCode = [];

            foreach (VirtualValueDefinition definition in definitions)
            {
                dependencies[definition.Code] = [];
                inDegree[definition.Code] = 0;
                byCode[definition.Code] = definition;
            }

            foreach (VirtualValueDefinition definition in definitions)
            {
                foreach (string operand in definition.GetOperandCodes())
                {
                    if (virtualCodes.Contains(operand) && dependencies[definition.Code].Add(operand))
                    {
                        inDegree[definition.Code]++;
                    }
                }
            }

            Queue<string> queue = new(definitions
                .Where(definition => inDegree[definition.Code] == 0)
                .Select(definition => definition.Code));
            List<VirtualValueDefinition> sorted = [];

            while (queue.Count > 0)
            {
                string node = queue.Dequeue();
                sorted.Add(byCode[node]);

                foreach (VirtualValueDefinition definition in definitions)
                {
                    if (!dependencies[definition.Code].Contains(node)) continue;
                    inDegree[definition.Code]--;
                    if (inDegree[definition.Code] == 0)
                    {
                        queue.Enqueue(definition.Code);
                    }
                }
            }

            if (sorted.Count < definitions.Count)
            {
                throw new InvalidOperationException("Circular dependency detected in virtual value definitions.");
            }

            return sorted;
        }

        private static string GetOperationPlaceholder(Translation translation, VirtualValueDefinition definition) => definition switch
        {
            SumDefinition => translation.SumPlaceholder,
            SubtractionOfTwoDefinition or SubtractionByConstantDefinition => translation.SubtractionPlaceholder,
            MultiplicationOfTwoDefinition or MultiplicationByConstantDefinition => translation.MultiplicationPlaceholder,
            DivisionOfTwoDefinition or DivisionByConstantDefinition => translation.DivisionPlaceholder,
            _ => translation.ComputedValuePlaceholder
        };

        private static MatrixMetadata AppendDimensionValue(
            IReadOnlyMatrixMetadata metadata,
            string dimensionCode,
            DimensionValue newValue)
        {
            List<Dimension> dimensions = [.. metadata.Dimensions.Select(dimension =>
            {
                if (dimension.Code != dimensionCode)
                {
                    return (Dimension)dimension;
                }

                List<DimensionValue> values = [.. dimension.Values.Cast<DimensionValue>(), newValue];
                Dictionary<string, MetaProperty> properties = new(dimension.AdditionalProperties);
                return dimension switch
                {
                    ContentDimension => new ContentDimension(
                        dimension.Code,
                        dimension.Name,
                        properties,
                        [.. values.Cast<ContentDimensionValue>()]),
                    TimeDimension timeDimension => new TimeDimension(
                        dimension.Code,
                        dimension.Name,
                        properties,
                        values,
                        timeDimension.Interval),
                    _ => new Dimension(
                        dimension.Code,
                        dimension.Name,
                        properties,
                        values,
                        dimension.Type)
                };
            })];

            return new MatrixMetadata(
                metadata.DefaultLanguage,
                metadata.AvailableLanguages,
                dimensions,
                new Dictionary<string, MetaProperty>(metadata.AdditionalProperties));
        }
    }
}