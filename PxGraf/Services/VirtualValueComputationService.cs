using Microsoft.Extensions.Logging;
using Px.Utils.Models;
using Px.Utils.Models.Data;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Operations;
using PxGraf.Models.Metadata;
using PxGraf.Models.Queries;
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
                    List<VirtualValueDefinition> ordered = VirtualValueMetadataBuilder.OrderDefinitions(dimEntry.Value.VirtualValueDefinitions);
                    Dictionary<string, int> typeCounters = [];
                    foreach (VirtualValueDefinition def in ordered)
                    {
                        string opType = VirtualValueMetadataBuilder.GetOperationType(def);
                        typeCounters[opType] = typeCounters.GetValueOrDefault(opType, 0) + 1;
                        matrix = ApplySingleDefinition(matrix, dimEntry.Key, def, dimEntry.Value, typeCounters[opType]);
                    }
                }
            }
            return matrix;
        }

        private Matrix<DecimalDataValue> ApplySingleDefinition(
            Matrix<DecimalDataValue> matrix,
            string dimensionCode,
            VirtualValueDefinition def,
            DimensionQuery dimensionQuery,
            int sequenceNumber)
        {
            DimensionValue newValue = VirtualValueMetadataBuilder.CreateValue(
                matrix.Metadata,
                dimensionCode,
                def,
                dimensionQuery,
                sequenceNumber);
            IReadOnlyDimension dimension = matrix.Metadata.Dimensions.First(d => d.Code == dimensionCode);
            int valueIndex = dimension.Values.Count;

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
                    if (Math.Abs(div.Constant) <= double.Epsilon)
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

    }
}
