namespace PxGraf.Datasource.ApiDatasource.SerializationModels
{
    public class DataBaseListingBase
    {
        public string Text { get; set; }
    }

    public class TableListResponseItem : DataBaseListingBase
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public string Updated { get; set; }
    }

    public class DataBaseListResponseItem : DataBaseListingBase
    {
        public string Dbid { get; set; }
    }
}
