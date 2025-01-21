namespace PxGraf.ChartTypeSelection.JsonObjects
{
    /// <summary>
    /// Used to hide properties needed for json parsing/unparsing when using ChartTypeLimits
    /// </summary>
    public interface IChartTypeLimits
    {
        /// <summary>
        /// The allowed range for the number of selevtions with multiple values.
        /// </summary>
        public DimensionRange NumberOfMultiselectsRange { get; }

        /// <summary>
        /// The allowed range for the time dimension size
        /// </summary>
        public DimensionRange TimeRange { get; }

        /// <summary>
        /// The allowed range for the size of irregular time dimensions
        /// </summary>
        public DimensionRange IrregularTimeRange { get; }

        /// <summary>
        /// The allowed range for the content dimension size
        /// </summary>
        public DimensionRange ContentRange { get; }

        /// <summary>
        /// The allowed range for unique units in content dimension
        /// </summary>
        public DimensionRange ContentUnitRange { get; }

        /// <summary>
        /// The allowed range for the first multiselect dimension
        /// </summary>
        public DimensionRange FirstMultiselectRange { get; }

        /// <summary>
        /// The allowed range for the second multiselect dimension
        /// </summary>
        public DimensionRange SecondMultiselectRange { get; }

        /// <summary>
        /// Range for all the dimensions not handled with the other ranges.
        /// </summary>
        public DimensionRange AdditionalMultiselectDimensionsRange { get; }

        /// <summary>
        /// The range for the product of number of selections in first and second classifier dimension
        /// </summary>
        public DimensionRange ProductOfMultiselectsRange { get; }
    }
}
