using Newtonsoft.Json;

namespace PxGraf.ChartTypeSelection.JsonObjects
{
    /// <summary>
    /// Limits that determine if a specific chart type can be drawn from the data.
    /// This class is meant to be possible to construct from a json and its state should not change afterwards.
    /// IMPORTANT: USE UNLY VIA THE IChartTypeLimits INTERFACE WHEN NOT PARSING/UNPARSING JSON!
    /// </summary>
    public class ChartTypeLimits : IChartTypeLimits
    {
        /// <summary>
        /// The allowed range for the number of selevtions with multiple values.
        /// </summary>
        public DimensionRange NumberOfMultiselectsRange { get; private set; } = new DimensionRange("Ignore");

        /// <summary>
        /// Sets the limits on how many selections can contain multiple values
        /// </summary>
        public string NumberOfMultiselects
        {
            get => NumberOfMultiselectsRange.ToString();
            set => NumberOfMultiselectsRange = new DimensionRange(value);
        }

        /// <summary>
        /// The allowed range for the time dimension size
        /// </summary>
        public DimensionRange TimeRange { get; private set; } = new DimensionRange("Ignore");

        /// <summary>
        /// Set the limits for time dimension size, example format: "1-999"
        /// </summary>
        [JsonProperty(nameof(TimeSelection))]
        public string TimeSelection
        {
            get => TimeRange.ToString();
            set => TimeRange = new DimensionRange(value);
        }

        /// <summary>
        /// The range for the product of number of selections in first and second classifier dimension
        /// </summary>
        public DimensionRange IrregularTimeRange { get; private set; } = new DimensionRange("Ignore");

        /// <summary>
        /// , example format: "1-999"
        /// </summary>
        [JsonProperty(nameof(IrregularTimeSelection))]
        public string IrregularTimeSelection
        {
            get => IrregularTimeRange.ToString();
            set => IrregularTimeRange = new DimensionRange(value);
        }

        /// <summary>
        /// The allowed range for the content dimension size
        /// </summary>
        public DimensionRange ContentRange { get; private set; } = new DimensionRange("Ignore");

        /// <summary>
        /// Set the limits for content dimension size, example format: "1-999"
        /// </summary>
        [JsonProperty(nameof(ContentSelection))]
        public string ContentSelection
        {
            get => ContentRange.ToString();
            set => ContentRange = new DimensionRange(value);
        }

        /// <summary>
        /// The allowed range for the unique units in content dimension
        /// </summary>
        public DimensionRange ContentUnitRange { get; private set; } = new DimensionRange("Ignore");

        /// <summary>
        /// Set the limits for unique units in content dimension, example format: "1-999"
        /// </summary>
        [JsonProperty(nameof(ContentUnitSelection))]
        public string ContentUnitSelection
        {
            get => ContentUnitRange.ToString();
            set => ContentUnitRange = new DimensionRange(value);
        }

        /// <summary>
        /// The allowed range for the size of the first classifier dimension.
        /// </summary>
        public DimensionRange FirstMultiselectRange { get; private set; } = new DimensionRange("Ignore");

        /// <summary>
        /// Set the limits for the first classifier dimension size, example format: "1-999"
        /// </summary>
        [JsonProperty(nameof(FirstMultiselect))]
        public string FirstMultiselect
        {
            get => FirstMultiselectRange.ToString();
            set => FirstMultiselectRange = new DimensionRange(value);
        }

        /// <summary>
        /// The allowed range for the size of the second classifier dimension.
        /// </summary>
        public DimensionRange SecondMultiselectRange { get; private set; } = new DimensionRange("Ignore");

        /// <summary>
        /// Set the limits for the second classifier dimension size, example format: "1-999"
        /// </summary>
        [JsonProperty(nameof(SecondMultiselect))]
        public string SecondMultiselect
        {
            get => SecondMultiselectRange.ToString();
            set => SecondMultiselectRange = new DimensionRange(value);
        }

        /// <summary>
        /// The allowed range for the size of the extra classifier dimensions.
        /// </summary>
        public DimensionRange AdditionalMultiselectDimensionsRange { get; private set; } = new DimensionRange("Ignore");

        /// <summary>
        /// Set the limits for the size of all of the multiselect dimensions after the first two, example format: "1-999"
        /// </summary>
        [JsonProperty(nameof(AdditionalMultiselectDimensions))]
        public string AdditionalMultiselectDimensions
        {
            get => AdditionalMultiselectDimensionsRange.ToString();
            set => AdditionalMultiselectDimensionsRange = new DimensionRange(value);
        }

        /// <summary>
        /// The range for the product of number of selections in first and second classifier dimension
        /// </summary>
        public DimensionRange ProductOfMultiselectsRange { get; private set; } = new DimensionRange("Ignore");

        /// <summary>
        /// The limits for the product of number of selections in first and second classifier dimension, example format: "1-999"
        /// </summary>
        [JsonProperty(nameof(ProductOfMultiselects))]
        public string ProductOfMultiselects
        {
            get => ProductOfMultiselectsRange.ToString();
            set => ProductOfMultiselectsRange = new DimensionRange(value);
        }
    }
}
