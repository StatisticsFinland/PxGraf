namespace PxGraf.Models.Responses
{
    /// <summary>
    /// Stores information about the validation of a table.
    /// </summary>
    /// <remarks>
    /// Default constructor.
    /// </remarks>
    public class TableMetaValidationResult(bool tableHasContentDimension, bool tableHasTimeDimension, bool allDimensionsContainValues)
    {
        public bool TableHasContentDimension { get; } = tableHasContentDimension;
        public bool TableHasTimeDimension { get; } = tableHasTimeDimension;
        public bool AllDimensionsContainValues { get; } = allDimensionsContainValues;
    }
}
