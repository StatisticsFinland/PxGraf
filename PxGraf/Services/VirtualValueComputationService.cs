using Microsoft.Extensions.Logging;
using Px.Utils.Language;
using Px.Utils.Models;
using Px.Utils.Models.Data;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.MetaProperties;
using Px.Utils.Operations;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
namespace PxGraf.Services
{
    /// <summary>
    /// Applies virtual (computed) dimension value definitions to a data matrix,
    /// producing an augmented matrix with the computed values appended.
    /// </summary>
    public interface IVirtualValueComputationService
    {
        /// <summary>
        /// Applies all virtual value definitions from all dimensions in the query to the matrix,
        /// returning an augmented matrix with the computed values appended.
        /// Throws <see cref="InvalidOperationException"/> if circular dependencies are detected.
        /// </summary>
        Matrix<DecimalDataValue> ApplyVirtualValues(
            Matrix<DecimalDataValue> matrix,
            MatrixQuery query);
    }

    /// <summary>
    /// Default implementation of <see cref="IVirtualValueComputationService"/>.
    /// </summary>
    public class VirtualValueComputationService(ILogger<VirtualValueComputationService> logger) : IVirtualValueComputationService
    {
        private readonly ILogger<VirtualValueComputationService> _logger = logger;
        /// <inheritdoc/>
        public Matrix<DecimalDataValue> ApplyVirtualValues(Matrix<DecimalDataValue> matrix, MatrixQuery query)
        {
            foreach (KeyValuePair<string, DimensionQuery> dimEntry in query.DimensionQueries)
            {
                if (dimEntry.Value.VirtualValueDefinitions?.Count > 0)
                {
                    List<VirtualValueDefinition> ordered = TopologicalSort(dimEntry.Value.VirtualValueDefinitions);
                    Dictionary<string, int> typeCounters = [];
                    foreach (VirtualValueDefinition def in ordered)
                    {
                        string opType = GetOperationType(def);
                        typeCounters[opType] = typeCounters.GetValueOrDefault(opType, 0) + 1;
                        matrix = ApplySingleDefinition(matrix, dimEntry.Key, def, dimEntry.Value, typeCounters[opType]);
                    }
                }
            }
            return matrix;
        }

        private static string GetOperationType(VirtualValueDefinition def) => def switch
        {
            SumDefinition => "sum",
            SubtractionOfTwoDefinition or SubtractionByConstantDefinition => "subtraction",
            MultiplicationOfTwoDefinition or MultiplicationByConstantDefinition => "multiplication",
            DivisionOfTwoDefinition or DivisionByConstantDefinition => "division",
            _ => "computed"
        };

        private static string GetOperationPlaceholder(Translation translation, VirtualValueDefinition def) => def switch
        {
            SumDefinition => translation.SumPlaceholder,
            SubtractionOfTwoDefinition or SubtractionByConstantDefinition => translation.SubtractionPlaceholder,
            MultiplicationOfTwoDefinition or MultiplicationByConstantDefinition => translation.MultiplicationPlaceholder,
            DivisionOfTwoDefinition or DivisionByConstantDefinition => translation.DivisionPlaceholder,
            _ => translation.ComputedValuePlaceholder
        };

        private Matrix<DecimalDataValue> ApplySingleDefinition(
            Matrix<DecimalDataValue> matrix,
            string dimensionCode,
            VirtualValueDefinition def,
            DimensionQuery dimensionQuery,
            int sequenceNumber)
        {
            IReadOnlyDimension dimension = matrix.Metadata.Dimensions
                .First(d => d.Code == dimensionCode);
            int valueIndex = dimension.Values.Count;

            IReadOnlyList<string> languages = matrix.Metadata.AvailableLanguages;
            MultilanguageString resolvedName;

            MultilanguageString defaultName = new(languages.ToDictionary(l => l, l =>
            {
                string placeholder = GetOperationPlaceholder(Localization.FromLanguage(l).Translation, def);
                return $"{placeholder} {sequenceNumber}";
            }));

            if (dimensionQuery.ValueEdits.TryGetValue(def.Code, out DimensionQuery.DimensionValueEdition edition) && edition.NameEdit is not null)
            {
                resolvedName = new MultilanguageString(languages.ToDictionary(l => l, l =>
                    edition.NameEdit.Languages.Contains(l) ? edition.NameEdit[l] : defaultName[l]));
            }
            else
            {
                resolvedName = defaultName;
            }

            DimensionValue newValue = CreateVirtualDimensionValue(dimension, def, resolvedName, languages);

            switch (def)
            {
                case SumDefinition sum:
                    matrix = matrix.SumToNewValue(
                        newValue,
                        new DimensionMap(dimensionCode, sum.OperandCodes),
                        valueIndex);
                    if (sum.Constant.HasValue)
                    {
                        matrix = matrix.AddConstantToSubset(
                            BuildConstantTargetMap(matrix, dimensionCode, sum.Code),
                            new DecimalDataValue((decimal)sum.Constant.Value, DataValueType.Exists));
                    }
                    break;

                case SubtractionOfTwoDefinition sub:
                    matrix = matrix.SumToNewValue(
                        newValue,
                        new DimensionMap(dimensionCode, [sub.Minuend]),
                        valueIndex);
                    matrix = matrix.ApplyRelative(
                        (a, b) => a - b,
                        new DimensionMap(dimensionCode, [sub.Code]),
                        sub.Subtrahend);
                    break;

                case SubtractionByConstantDefinition sub:
                    matrix = matrix.SumToNewValue(
                        newValue,
                        new DimensionMap(dimensionCode, [sub.Operand]),
                        valueIndex);
                    matrix = matrix.AddConstantToSubset(
                        BuildConstantTargetMap(matrix, dimensionCode, sub.Code),
                        new DecimalDataValue(-(decimal)sub.Constant, DataValueType.Exists));
                    break;

                case MultiplicationOfTwoDefinition mul:
                    matrix = matrix.MultiplyToNewValue(
                        newValue,
                        new DimensionMap(dimensionCode, [mul.LeftOperand, mul.RightOperand]),
                        valueIndex);
                    break;

                case MultiplicationByConstantDefinition mul:
                    matrix = matrix.SumToNewValue(
                        newValue,
                        new DimensionMap(dimensionCode, [mul.Operand]),
                        valueIndex);
                    matrix = matrix.MultiplySubsetByConstant(
                        BuildConstantTargetMap(matrix, dimensionCode, mul.Code),
                        new DecimalDataValue((decimal)mul.Constant, DataValueType.Exists));
                    break;

                case DivisionOfTwoDefinition div:
                    matrix = matrix.SumToNewValue(
                        newValue,
                        new DimensionMap(dimensionCode, [div.Dividend]),
                        valueIndex);
                    matrix = matrix.ApplyRelative(
                        (a, b) => b.Type == DataValueType.Exists && b.UnsafeValue != 0m
                            ? a / b
                            : new DecimalDataValue(0m, DataValueType.CanNotRepresent),
                        new DimensionMap(dimensionCode, [div.Code]),
                        div.Divisor);
                    break;

                case DivisionByConstantDefinition div:
                    matrix = matrix.SumToNewValue(
                        newValue,
                        new DimensionMap(dimensionCode, [div.Operand]),
                        valueIndex);
                    if (div.Constant == 0.0)
                    {
                        _logger.LogWarning("Virtual value has a zero constant divisor. All computed cells will be set to 'can not represent'.");
                        matrix = matrix.ApplyToSubMap(
                            BuildConstantTargetMap(matrix, dimensionCode, div.Code),
                            _ => new DecimalDataValue(0m, DataValueType.CanNotRepresent));
                    }
                    else
                    {
                        matrix = matrix.DivideSubsetByConstant(
                            BuildConstantTargetMap(matrix, dimensionCode, div.Code),
                            new DecimalDataValue((decimal)div.Constant, DataValueType.Exists));
                    }
                    break;

                default:
                    throw new InvalidOperationException($"Unsupported virtual value definition type '{def.GetType().Name}' for code '{def.Code}'.");
            }

            return matrix;
        }

        private static MatrixMap BuildConstantTargetMap(Matrix<DecimalDataValue> matrix, string dimensionCode, string targetValueCode)
        {
            List<IDimensionMap> dimensionMaps = [.. matrix.Metadata.Dimensions.Select(dim =>
                dim.Code == dimensionCode
                    ? new DimensionMap(dim.Code, [targetValueCode])
                    : (IDimensionMap)new DimensionMap(dim.Code, dim.Values.Select(v => v.Code).ToList()))];
            return new MatrixMap(dimensionMaps);
        }

        private static DimensionValue CreateVirtualDimensionValue(
            IReadOnlyDimension dimension,
            VirtualValueDefinition def,
            MultilanguageString resolvedName,
            IReadOnlyList<string> languages)
        {
            if (dimension is not ContentDimension contentDimension)
            {
                return new DimensionValue(def.Code, resolvedName);
            }

            ContentDimensionValue firstOperand = (ContentDimensionValue)contentDimension.Values
                .First(v => v.Code == def.GetOperandCodes()[0]);

            int precision = def.GetOperandCodes()
                .Select(code => contentDimension.Values.First(v => v.Code == code))
                .OfType<ContentDimensionValue>()
                .Min(cdv => cdv.Precision);

            MultilanguageString placeholder = new(languages.ToDictionary(l => l,
                l => Localization.FromLanguage(l).Translation.ComputedValuePlaceholder));

            return new ContentDimensionValue(
                def.Code,
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

        private static List<VirtualValueDefinition> TopologicalSort(List<VirtualValueDefinition> definitions)
        {
            HashSet<string> virtualCodes = [.. definitions .Select(d => d.Code)];

            Dictionary<string, HashSet<string>> dependencies = [];
            Dictionary<string, int> inDegree = [];
            Dictionary<string, VirtualValueDefinition> byCode = [];

            foreach (VirtualValueDefinition def in definitions)
            {
                dependencies[def.Code] = [];
                inDegree[def.Code] = 0;
                byCode[def.Code] = def;
            }

            foreach (VirtualValueDefinition def in definitions)
            {
                foreach (string operand in def.GetOperandCodes())
                {
                    if (virtualCodes.Contains(operand) && dependencies.ContainsKey(def.Code))
                    {
                        if (dependencies[def.Code].Add(operand))
                        {
                            inDegree[def.Code]++;
                        }
                    }
                }
            }

            Queue<string> queue = new(definitions
                .Where(d => inDegree.TryGetValue(d.Code, out int deg) && deg == 0)
                .Select(d => d.Code));
            List<VirtualValueDefinition> sorted = [];

            while (queue.Count > 0)
            {
                string node = queue.Dequeue();
                sorted.Add(byCode[node]);

                foreach (VirtualValueDefinition def in definitions)
                {
                    if (!dependencies.ContainsKey(def.Code)) continue;
                    if (!dependencies[def.Code].Contains(node)) continue;

                    inDegree[def.Code]--;
                    if (inDegree[def.Code] == 0)
                    {
                        queue.Enqueue(def.Code);
                    }
                }
            }

            if (sorted.Count < definitions.Count)
            {
                throw new InvalidOperationException("Circular dependency detected in virtual value definitions.");
            }

            return sorted;
        }
    }
}
