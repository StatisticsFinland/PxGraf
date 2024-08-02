using System.Collections.Generic;

namespace PxGraf.Models.Responses.DatabaseItems
{
    public class DatabaseGroupContents(List<DatabaseGroupHeader> headers, List<DatabaseTable> files)
    {
        public List<DatabaseGroupHeader> Headers { get; } = headers;
        public List<DatabaseTable> Files { get; } = files;
    }
}
