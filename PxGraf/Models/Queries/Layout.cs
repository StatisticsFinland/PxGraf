using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System;

namespace PxGraf.Models.Queries
{
    public class Layout
    {
        [JsonPropertyName("rowVariableCodes")] // legacy name, do not change or all the old queries break.
        public IReadOnlyList<string> RowDimensionCodes { get; set; }

        [JsonPropertyName("columnVariableCodes")] // legacy name, do not change or all the old queries break.
        public IReadOnlyList<string> ColumnDimensionCodes { get; set; }

        public Layout(IReadOnlyList<string> rowDimensionCodes, IReadOnlyList<string> columnDimensionCodes)
        {
            RowDimensionCodes = rowDimensionCodes;
            ColumnDimensionCodes = columnDimensionCodes;
        }

        [JsonConstructor]
        public Layout() { }

        public override bool Equals(object obj)
        {
            if (obj is not Layout other)
            {
                return false;
            }

            return this.RowDimensionCodes.SequenceEqual(other.RowDimensionCodes) &&
                   this.ColumnDimensionCodes.SequenceEqual(other.ColumnDimensionCodes);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
