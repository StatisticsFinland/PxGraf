using PxGraf.Datasource.ApiDatasource.SerializationModels;

namespace PxGraf.Datasource.PxWebInterface.SerializationModels
{
    public static class ModelExtensions
    {
        /// <summary>
        /// Returns true if the dimension matching the provided dimension code is marked as ordinal
        /// </summary>
        /// <param name="jsonStat2"></param>
        /// <param name="dimCode"></param>
        /// <returns></returns>
        public static bool DimHasOrdinalScaleType(this JsonStat2 jsonStat2, string dimCode)
        {
            if (jsonStat2.Dimensions[dimCode]?
                .Link?.DescribedBy[0]?.Extension
                .TryGetValue(dimCode, out string extension) ?? false)
            {
                return extension.Contains("SCALE-TYPE=ordinal");
            }

            return false;
        }

        /// <summary>
        /// IMPORTANT! THIS METHOD ONLY READS DIMENSION CODES AND NAMES AND IS NOT 100% CONCLUSIVE
        /// PxWeb does not directly return combination value so this method can be used to parse that information
        /// from the response dimension object is SOME cases.
        /// </summary>
        public static bool IsSumValue(this PxMetaResponse.Dimension dimension, int valueIndex)
        {
            return dimension.Values[valueIndex].Equals("sss", System.StringComparison.CurrentCultureIgnoreCase) ||
                dimension.ValueTexts[valueIndex].Equals("yhteensä", System.StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
