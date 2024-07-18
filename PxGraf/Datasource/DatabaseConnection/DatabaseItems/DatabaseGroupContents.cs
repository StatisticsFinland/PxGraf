using PxGraf.Models.Queries;
using System.Collections.Generic;

namespace PxGraf.Datasource.DatabaseConnection.DatabaseItems
{
    public class DatabaseGroupContents(List<DatabaseGroupHeader> headers, List<PxTableReference> files)
    {
        public List<DatabaseGroupHeader> Headers { get; } = headers;
        public List<PxTableReference> Files { get; } = files;
    }
}
