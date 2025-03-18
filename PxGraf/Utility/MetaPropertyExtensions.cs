using Px.Utils.Models.Metadata.MetaProperties;
using System;

namespace PxGraf.Utility
{
    public static class MetaPropertyExtensions
    {
        public static MultilanguageStringProperty AsMultiLanguageProperty(this MetaProperty property, string lang)
        {
            if (property is MultilanguageStringProperty mlsp) return mlsp;
            else if (property is StringProperty sp) return new MultilanguageStringProperty(new(lang, sp.Value));
            else throw new ArgumentException($"Property of type {property.GetType()} can not be converted to multilanguage string property.");
        }
    }
}
