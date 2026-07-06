using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PxGraf.Models.Queries
{
    /// <summary>
    /// Defines a single virtual (computed) dimension value and how it is calculated
    /// from existing real or previously-defined virtual values.
    /// </summary>
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
    [JsonDerivedType(typeof(SumDefinition), "sum")]
    [JsonDerivedType(typeof(SubtractionOfTwoDefinition), "subtractionOfTwo")]
    [JsonDerivedType(typeof(SubtractionByConstantDefinition), "subtractionByConstant")]
    [JsonDerivedType(typeof(MultiplicationOfTwoDefinition), "multiplicationOfTwo")]
    [JsonDerivedType(typeof(MultiplicationByConstantDefinition), "multiplicationByConstant")]
    [JsonDerivedType(typeof(DivisionOfTwoDefinition), "divisionOfTwo")]
    [JsonDerivedType(typeof(DivisionByConstantDefinition), "divisionByConstant")]
    public abstract class VirtualValueDefinition
    {
        /// <summary>
        /// Auto-generated unique code for this virtual value (e.g., "virtual_1").
        /// Must be unique within the list and must not collide with real dimension value codes.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Returns the list of operand codes used by this definition.
        /// Used by topological sort, cycle detection, and value computation.
        /// </summary>
        public abstract IReadOnlyList<string> GetOperandCodes();
    }

    /// <summary>
    /// Computes a virtual value as the sum of two or more operand codes, optionally adding a constant offset.
    /// </summary>
    public class SumDefinition : VirtualValueDefinition
    {
        /// <summary>
        /// Codes of the dimension values to sum. Must contain at least 2 entries.
        /// </summary>
        public List<string> OperandCodes { get; set; }

        /// <summary>
        /// Optional constant to add to the sum result.
        /// </summary>
        public double? Constant { get; set; }

        /// <inheritdoc/>
        public override IReadOnlyList<string> GetOperandCodes() => OperandCodes ?? [];
    }

    /// <summary>
    /// Computes a virtual value as the difference between two operand codes (Minuend - Subtrahend).
    /// </summary>
    public class SubtractionOfTwoDefinition : VirtualValueDefinition
    {
        /// <summary>
        /// Code of the dimension value to subtract from.
        /// </summary>
        public string Minuend { get; set; }

        /// <summary>
        /// Code of the dimension value to subtract.
        /// </summary>
        public string Subtrahend { get; set; }

        /// <inheritdoc/>
        public override IReadOnlyList<string> GetOperandCodes() => [Minuend, Subtrahend];
    }

    /// <summary>
    /// Computes a virtual value as an operand code minus a constant (Operand - Constant).
    /// </summary>
    public class SubtractionByConstantDefinition : VirtualValueDefinition
    {
        /// <summary>
        /// Code of the dimension value to subtract the constant from.
        /// </summary>
        public string Operand { get; set; }

        /// <summary>
        /// The constant to subtract from the operand.
        /// </summary>
        public double Constant { get; set; }

        /// <inheritdoc/>
        public override IReadOnlyList<string> GetOperandCodes() => [Operand];
    }

    /// <summary>
    /// Computes a virtual value as the product of two operand codes (LeftOperand × RightOperand).
    /// </summary>
    public class MultiplicationOfTwoDefinition : VirtualValueDefinition
    {
        /// <summary>
        /// Code of the left operand dimension value.
        /// </summary>
        public string LeftOperand { get; set; }

        /// <summary>
        /// Code of the right operand dimension value.
        /// </summary>
        public string RightOperand { get; set; }

        /// <inheritdoc/>
        public override IReadOnlyList<string> GetOperandCodes() => [LeftOperand, RightOperand];
    }

    /// <summary>
    /// Computes a virtual value as an operand code multiplied by a constant (Operand × Constant).
    /// </summary>
    public class MultiplicationByConstantDefinition : VirtualValueDefinition
    {
        /// <summary>
        /// Code of the dimension value to multiply.
        /// </summary>
        public string Operand { get; set; }

        /// <summary>
        /// The constant to multiply the operand by.
        /// </summary>
        public double Constant { get; set; }

        /// <inheritdoc/>
        public override IReadOnlyList<string> GetOperandCodes() => [Operand];
    }

    /// <summary>
    /// Computes a virtual value as the quotient of two operand codes (Dividend ÷ Divisor).
    /// Missing values are produced when the divisor is zero or missing.
    /// </summary>
    public class DivisionOfTwoDefinition : VirtualValueDefinition
    {
        /// <summary>
        /// Code of the dividend dimension value.
        /// </summary>
        public string Dividend { get; set; }

        /// <summary>
        /// Code of the divisor dimension value.
        /// </summary>
        public string Divisor { get; set; }

        /// <inheritdoc/>
        public override IReadOnlyList<string> GetOperandCodes() => [Dividend, Divisor];
    }

    /// <summary>
    /// Computes a virtual value as an operand code divided by a constant (Operand ÷ Constant).
    /// Zero is not validated away; cells are set to missing at computation time.
    /// </summary>
    public class DivisionByConstantDefinition : VirtualValueDefinition
    {
        /// <summary>
        /// Code of the dividend dimension value.
        /// </summary>
        public string Operand { get; set; }

        /// <summary>
        /// The constant to divide the operand by. Zero is not validated away; cells are set to missing at computation time.
        /// </summary>
        public double Constant { get; set; }

        /// <inheritdoc/>
        public override IReadOnlyList<string> GetOperandCodes() => [Operand];
    }
}
