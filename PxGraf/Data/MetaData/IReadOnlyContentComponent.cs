using PxGraf.Language;

namespace PxGraf.Data.MetaData
{
    public interface IReadOnlyContentComponent
    {
        public IReadOnlyMultiLanguageString Unit { get; }

        public IReadOnlyMultiLanguageString Source { get; }

        public int NumberOfDecimals { get; }

        public string LastUpdated { get; }

        public ContentComponent Clone();
    }
}
