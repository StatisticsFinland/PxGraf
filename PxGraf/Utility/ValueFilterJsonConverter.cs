using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PxGraf.Exceptions;
using PxGraf.Models.Queries;
using System;
using System.Collections.Generic;

namespace PxGraf.Utility
{
    public class ValueFilterJsonConverter : JsonConverter
    {
        public enum FilterType
        {
            Item,
            All,
            From,
            Top,
        }

        public class ValueFilterJsonModel
        {
            public FilterType Type { get; set; }
            public JToken Query { get; set; }
        }

        public override bool CanWrite => true;
        public override bool CanRead => true;


        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is ItemFilter itemFilter)
            {
                serializer.Serialize(writer, new { type = "item", query = itemFilter.Codes });
                return;
            }

            if (value is AllFilter)
            {
                serializer.Serialize(writer, new { type = "all" });
                return;
            }

            if (value is FromFilter fromFilter)
            {
                serializer.Serialize(writer, new { type = "from", query = fromFilter.Code });
                return;
            }

            if (value is TopFilter topFilter)
            {
                serializer.Serialize(writer, new { type = "top", query = topFilter.Count });
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IValueFilter);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.ReadFrom(reader);
            var filterWrapper = obj.ToObject<ValueFilterJsonModel>(serializer);

            switch (filterWrapper.Type)
            {
                case FilterType.Item:
                    {
                        var codes = filterWrapper.Query.ToObject<List<string>>(serializer);
                        return new ItemFilter(codes);
                    }
                case FilterType.All:
                    {
                        return new AllFilter();
                    }
                case FilterType.From:
                    {
                        var fromCode = filterWrapper.Query.ToObject<string>(serializer);
                        return new FromFilter(fromCode);
                    }
                case FilterType.Top:
                    {
                        var topCount = filterWrapper.Query.ToObject<int>(serializer);
                        return new TopFilter(topCount);
                    }
                default: throw new UnknownFilterTypeException("Unknown filter type: " + filterWrapper.Type);
            }
        }
    }
}
