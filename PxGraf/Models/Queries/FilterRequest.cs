﻿using System.Collections.Generic;

namespace PxGraf.Models.Queries
{
    /// <summary>
    /// Request object for filtering values for visualization.
    /// </summary>
    public class FilterRequest
    {
        /// <summary>
        /// Reference to the table in Px file system.
        /// </summary>
        public PxTableReference TableReference { get; set; }

        /// <summary>
        /// Dictionary that maps dimension codes and their respective filters.
        /// </summary>
        public Dictionary<string, IValueFilter> Filters { get; set; }
    }
}
