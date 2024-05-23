namespace PxGraf.Models.Responses
{
    /// <summary>
    /// Stores information about the validation of a table.
    /// </summary>
    /// <remarks>
    /// Default constructor.
    /// </remarks>
    public class TableMetaValidationResult(bool tableHasContentVariable, bool tableHasTimeVariable, bool allVariablesContainValues)
    {
        public bool TableHasContentVariable { get; } = tableHasContentVariable;
        public bool TableHasTimeVariable { get; } = tableHasTimeVariable;
        public bool AllVariablesContainValues { get; } = allVariablesContainValues;
    }
}
