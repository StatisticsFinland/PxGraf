using PxGraf.Enums;
using PxGraf.Language;
using System.Collections.Generic;

namespace PxGraf.Models.Responses
{
    /// <summary>
    /// Response structure for the query-info API endpoint
    /// </summary>
    public class QueryInfoResponse
    {
        /// <summary>
        /// Current size of the query
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// The limit when the end user should be warned that the size of the query is getting larger than is parctical
        /// </summary>
        public int SizeWarningLimit { get; set; }

        /// <summary>
        /// The limit when the end user should be told that the size of the query can cause technical issues and further actios will be prevented unless the size is reduced
        /// </summary>
        public int MaximumSupportedSize { get; set; }

        /// <summary>
        /// Maximum length of the header text, in any language
        /// </summary>
        public int MaximumHeaderLength { get; set; }

        /// <summary>
        /// These visualization types are valid for the query
        /// </summary>
        public List<VisualizationType> ValidVisualizations { get; }

        /// <summary>
        /// Collection of reasons why the invalid visualization types are not possible for this query.
        /// </summary>
        public Dictionary<VisualizationType, MultiLanguageString> VisualizationRejectionReasons { get; }

        public QueryInfoResponse()
        {
            ValidVisualizations = [];
            VisualizationRejectionReasons = [];
        }
    }
}
