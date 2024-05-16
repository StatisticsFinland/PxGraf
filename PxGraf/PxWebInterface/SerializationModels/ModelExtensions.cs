namespace PxGraf.PxWebInterface.SerializationModels
{
    public static class ModelExtensions
    {
        /// <summary>
        /// Returns true if the variable matching the provided variabale code is marked as ordinal
        /// </summary>
        /// <param name="jsonStat2"></param>
        /// <param name="varCode"></param>
        /// <returns></returns>
        public static bool VarHasOrdinalScaleType(this JsonStat2 jsonStat2, string varCode)
        {
            if (jsonStat2.Dimensions[varCode]?
                .Link?.DescribedBy[0]?.Extension
                .TryGetValue(varCode, out string extension) ?? false)
            {
                return extension.Contains("SCALE-TYPE=ordinal");
            }

            return false;
        }

        /// <summary>
        /// IMPORTANT! THIS METHOD ONLY READS VARIABLE CODES AND NAMES AND IS NOT 100% CONCLUSIVE
        /// PxWeb does not directly return combination value so this method can be used to parse that information
        /// from the response variable object is SOME cases.
        /// </summary>
        public static bool IsSumValue(this PxMetaResponse.Variable variable, int valueIndex)
        {
            return variable.Values[valueIndex].ToLower() == "sss" ||
                variable.ValueTexts[valueIndex].ToLower() == "yhteensä";
        }
    }
}
