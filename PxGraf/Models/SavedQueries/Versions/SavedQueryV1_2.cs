using System.Text.Json.Serialization;
using static PxGraf.Models.SavedQueries.Versions.SavedQueryV11;

namespace PxGraf.Models.SavedQueries.Versions
{
    public class SavedQueryV12 : VersionedSavedQuery
    {
        [JsonInclude]
        public readonly string Version = "1.2";
        public VisualizationSettingsV11 Settings { get; set; }
        public bool Draft { get; set; }

        #region Constructors

        [JsonConstructor]
        public SavedQueryV12() { }

        public SavedQueryV12(SavedQuery inputQuery)
        {
            Query = inputQuery.Query;
            CreationTime = inputQuery.CreationTime;
            Archived = inputQuery.Archived;
            Settings = new(inputQuery.Settings);
            Draft = inputQuery.Draft;
        }

        #endregion

        public override SavedQuery ToSavedQuery()
        {
            SavedQuery savedQuery = new()
            {
                Archived = Archived,
                Settings = BuildSettingsFromV11(Settings),
                CreationTime = CreationTime,
                Query = Query,
                Version = Version,
                Draft = Draft
            };
            return savedQuery;
        }
    }
}
