namespace PxGraf.Models.Responses
{
    /// <summary>
    /// Stores information about the validation of a table.
    /// </summary>
    public class TableMetaValidationResult
    { 
        public bool TableHasContentVariable { get; }
        public bool TableHasTimeVariable { get; }
        public bool AllVariablesContainValues { get; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TableMetaValidationResult(bool tableHasContentVariable, bool tableHasTimeVariable, bool allVariablesContainValues)
        {
            TableHasContentVariable = tableHasContentVariable;
            TableHasTimeVariable = tableHasTimeVariable;
            AllVariablesContainValues = allVariablesContainValues;
        }
    }
}
