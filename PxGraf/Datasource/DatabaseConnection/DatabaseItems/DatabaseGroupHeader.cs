using Px.Utils.Language;

namespace PxGraf.Datasource.DatabaseConnection.DatabaseItems
{
    public class DatabaseGroupHeader(string code, MultilanguageString name)
    {
        public string Code { get; } = code;
        public MultilanguageString Name { get; } = name;
    }
}
