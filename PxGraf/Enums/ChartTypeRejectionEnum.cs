namespace PxGraf.Enums
{
    /// <summary>
    /// Defines different reasons why spesific chart type is not valid for a given data set
    /// </summary>
    public enum RejectionReason
    {
        /// <summary>
        /// Time dimension required but not provided.
        /// </summary>
        TimeRequired,

        /// <summary>
        /// Time dimension provided but not allowed.
        /// </summary>
        TimeNotAllowed,

        /// <summary>
        /// The size of time dimensions is too large.
        /// </summary>
        TimeOverMax,

        /// <summary>
        /// The size of the time dimension is too small.
        /// </summary>
        TimeBelowMin,

        /// <summary>
        /// Values in the time dimension are not in order OR the values are not consecutive.
        /// </summary>
        IrregularTimeNotAllowed,

        /// <summary>
        /// The size of the irregular time dimension is too large.
        /// </summary>
        IrregularTimeOverMax,

        /// <summary>
        /// The size of the irregular time dimension is too small.
        /// </summary>
        IrregularTimeBelowMin,

        /// <summary>
        /// Content dimension required, but not provided.
        /// </summary>
        ContentRequired,

        /// <summary>
        /// Content dimension provided but not allowed.
        /// </summary>
        ContentNotAllowed,

        /// <summary>
        /// The size of the content dimension is too large.
        /// </summary>
        ContentOverMax,

        /// <summary>
        /// The size of the content dimenension is too small.
        /// </summary>
        ContentBelowMin,

        /// <summary>
        /// All selected content dimension values should have same unit.
        /// </summary>
        UnambiguousContentUnitRequired,

        /// <summary>
        /// The count of the unique units in content dimenension is too small.
        /// </summary>
        ContentUnitsBelowMin,

        /// <summary>
        /// The count of the unique units in content dimenension is too large.
        /// </summary>
        ContentUnitsOverMax,

        /// <summary>
        /// The size of the classifier1 dimension is too small.
        /// </summary>
        FirstMultiselectBelowMin,

        /// <summary>
        /// The size of the classifier1 dimension is too large.
        /// </summary>
        FirstMultiselectOverMax,

        /// <summary>
        /// The size of the second classifier dimension is too small.
        /// </summary>
        SecondMultiselectBelowMin,

        /// <summary>
        /// The size of the second classifier dimension is too large.
        /// </summary>
        SecondMultiselectOverMax,

        /// <summary>
        /// Three or more classifier dimensions required, but not provided.
        /// </summary>
        AdditionalDimensionsRequired,

        /// <summary>
        /// More than two classifier dimensions provided but not allowed.
        /// </summary>
        AdditionalDimensionsNotAllowed,

        /// <summary>
        /// The size of the extra classifier dimension is too small.
        /// </summary>
        AdditionalDimensionsBelowMin,

        /// <summary>
        /// The size of the extra classifier dimension is too large.
        /// </summary>
        AdditionalDimensionsOverMax,

        /// <summary>
        /// The product of two first classifier dimensions is too large.
        /// </summary>
        MultiselectProductBelowMin,

        /// <summary>
        /// The product of two first classifier dimensions is too large.
        /// </summary>
        MultiselectProductOverMax,

        /// <summary>
        /// Does not contain enough selections with multiple values.
        /// </summary>
        NotEnoughMultiselections,

        /// <summary>
        /// Does contain too many selections with multiple values.
        /// </summary>
        TooManyMultiselections,

        /// <summary>
        /// Does not contain time or progressive selection.
        /// </summary>
        TimeOrProgressiveRequired,

        /// <summary>
        /// Progressive multiselect dimensions are not allowed.
        /// </summary>
        ProgressiveNotAllowed,

        /// <summary>
        /// Progressive multiselect dimension is required.
        /// </summary>
        ProgressiveRequired,

        /// <summary>
        /// Selection contain unallowed combination value.
        /// </summary>
        CombinationValuesNotAllowed,

        /// <summary>
        /// Data contains unallowed negative values.
        /// </summary>
        NegativeDataNotAllowed,

        /// <summary>
        /// Data is required.
        /// </summary>
        DataRequired
    }
}
