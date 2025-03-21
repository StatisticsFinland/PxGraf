﻿using PxGraf.ChartTypeSelection;
using PxGraf.Enums;
using PxGraf.Utility.CustomJsonConverters;
using System.Text.Json.Serialization;

namespace PxGraf.Language
{
    [JsonConverter(typeof(RequireObjectPropertiesReadOnlyConverter<RejectionReasonTranslation>))]
    public class RejectionReasonTranslation
    {
        public string TimeRequired { get; set; }
        public string TimeNotAllowed { get; set; }
        public string TimeOverMax { get; set; }
        public string TimeBelowMin { get; set; }
        public string ContentRequired { get; set; }
        public string ContentNotAllowed { get; set; }
        public string ContentOverMax { get; set; }
        public string ContentBelowMin { get; set; }
        public string UnambiguousContentUnitRequired { get; set; }
        public string DimensionBelowMin { get; set; }
        public string DimensionOverMax { get; set; }
        public string MultiselectProductBelowMin { get; set; }
        public string MultiselectProductOverMax { get; set; }
        public string NotEnoughMultiselections { get; set; }
        public string TooManyMultiselections { get; set; }
        public string TimeOrProgressiveRequired { get; set; }
        public string IrregularTimeNotAllowed { get; set; }
        public string IrregularTimeOverMax { get; set; }
        public string IrregularTimeBelowMin { get; set; }
        public string ProgressiveNotAllowed { get; set; }
        public string ProgressiveRequired { get; set; }
        public string CombinationValuesNotAllowed { get; set; }
        public string NegativeDataNotAllowed { get; set; }
        public string DataRequired { get; set; }

        public string GetTranslation(ChartRejectionInfo rejection)
        {
            return rejection.Reason switch
            {
                RejectionReason.TimeRequired => TimeRequired,
                RejectionReason.TimeNotAllowed => string.Format(TimeNotAllowed, rejection.DimensionName),
                RejectionReason.TimeOverMax => string.Format(TimeOverMax, rejection.Value, rejection.Threshold, rejection.DimensionName),
                RejectionReason.TimeBelowMin => string.Format(TimeBelowMin, rejection.Value, rejection.Threshold, rejection.DimensionName),
                RejectionReason.ContentRequired => ContentRequired,
                RejectionReason.ContentNotAllowed => string.Format(ContentNotAllowed, rejection.DimensionName),
                RejectionReason.ContentOverMax => string.Format(ContentOverMax, rejection.Value, rejection.Threshold, rejection.DimensionName),
                RejectionReason.ContentBelowMin => string.Format(ContentBelowMin, rejection.Value, rejection.Threshold, rejection.DimensionName),
                RejectionReason.UnambiguousContentUnitRequired => string.Format(UnambiguousContentUnitRequired, rejection.Value, rejection.DimensionName),
                RejectionReason.ContentUnitsOverMax => "{Content units over max}", //Should not appear in the wild
                RejectionReason.ContentUnitsBelowMin => "{Content units below min}", //Should not appear in the wild
                RejectionReason.FirstMultiselectBelowMin => string.Format(DimensionBelowMin, rejection.Value, rejection.Threshold, rejection.DimensionName),
                RejectionReason.FirstMultiselectOverMax => string.Format(DimensionOverMax, rejection.Value, rejection.Threshold, rejection.DimensionName),
                RejectionReason.SecondMultiselectBelowMin => string.Format(DimensionBelowMin, rejection.Value, rejection.Threshold, rejection.DimensionName),
                RejectionReason.SecondMultiselectOverMax => string.Format(DimensionOverMax, rejection.Value, rejection.Threshold, rejection.DimensionName),
                RejectionReason.AdditionalDimensionsRequired => "{Additional dimensions required}", //Should not appear in the wild
                RejectionReason.AdditionalDimensionsNotAllowed => "{Additional dimensions not allowed}", //Should not appear in the wild
                RejectionReason.AdditionalDimensionsBelowMin => string.Format(DimensionBelowMin, rejection.Value, rejection.Threshold, rejection.DimensionName),
                RejectionReason.AdditionalDimensionsOverMax => string.Format(DimensionOverMax, rejection.Value, rejection.Threshold, rejection.DimensionName),
                RejectionReason.MultiselectProductBelowMin => string.Format(MultiselectProductBelowMin, rejection.Value, rejection.Threshold),
                RejectionReason.MultiselectProductOverMax => string.Format(MultiselectProductOverMax, rejection.Value, rejection.Threshold),
                RejectionReason.NotEnoughMultiselections => string.Format(NotEnoughMultiselections, rejection.Value, rejection.Threshold),
                RejectionReason.TooManyMultiselections => string.Format(TooManyMultiselections, rejection.Value, rejection.Threshold),
                RejectionReason.TimeOrProgressiveRequired => TimeOrProgressiveRequired,
                RejectionReason.IrregularTimeNotAllowed => string.Format(IrregularTimeNotAllowed, rejection.DimensionName),
                RejectionReason.IrregularTimeBelowMin => string.Format(IrregularTimeBelowMin, rejection.Value, rejection.Threshold, rejection.DimensionName),
                RejectionReason.IrregularTimeOverMax => string.Format(IrregularTimeOverMax, rejection.Value, rejection.Threshold, rejection.DimensionName),
                RejectionReason.ProgressiveNotAllowed => string.Format(ProgressiveNotAllowed, rejection.DimensionName),
                RejectionReason.ProgressiveRequired => ProgressiveRequired,
                RejectionReason.CombinationValuesNotAllowed => string.Format(CombinationValuesNotAllowed, rejection.DimensionName, rejection.InvalidValueName),
                RejectionReason.NegativeDataNotAllowed => NegativeDataNotAllowed,
                RejectionReason.DataRequired => DataRequired,
                _ => throw new System.NotImplementedException(),
            };
        }
    }
}
