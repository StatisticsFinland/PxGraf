using PxGraf.Models.Responses;
using PxGraf.PxWebInterface.SerializationModels;
using PxGraf.Settings;
using System.Collections.Generic;

namespace PxGraf.Utility
{
    public static class DataBaseListingUtilities
    {
        public static string GetPreferredLanguage(Dictionary<string, string> queryParameters)
        {
            if (!queryParameters.TryGetValue("lang", out string preferredLanguage) && !queryParameters.TryGetValue("language", out preferredLanguage))
            {
                preferredLanguage = Configuration.Current.LanguageOptions.Default;
            }
            return preferredLanguage;
        }

        public static List<string> GetPrioritizedLanguages(string preferredLanguage)
        {
            List<string> languages = new(Configuration.Current.LanguageOptions.Available);
            languages.Remove(preferredLanguage);
            languages.Insert(0, preferredLanguage);
            return languages;
        }

        public static void AddOrAppendDataBaseItems(Dictionary<string, DataBaseListingItem> responseDictionary, IEnumerable<DataBaseListResponseItem> dataBases, string language)
        {
            foreach (DataBaseListResponseItem db in dataBases)
            {
                if (responseDictionary.TryGetValue(db.Dbid, out DataBaseListingItem existingItem))
                {
                    existingItem.Languages.Add(language);
                    existingItem.Text.AddTranslation(language, db.Text);
                }
                else
                {
                    responseDictionary.Add(db.Dbid, new(db, language));
                }
            }
        }

        public static void AddOrAppendTableItems(Dictionary<string, DataBaseListingItem> responseDictionary, IEnumerable<TableListResponseItem> tables, string language)
        {
            foreach (TableListResponseItem t in tables)
            {
                if (responseDictionary.TryGetValue(t.Id, out DataBaseListingItem existingItem))
                {
                    existingItem.Languages.Add(language);
                    existingItem.Text.AddTranslation(language, t.Text);
                }
                else
                {
                    responseDictionary.Add(t.Id, new(t, language));
                }
            }
        }
    }
}
