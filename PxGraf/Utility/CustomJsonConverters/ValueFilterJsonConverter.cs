using PxGraf.Exceptions;
using PxGraf.Models.Queries;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;
using System;

namespace PxGraf.Utility.CustomJsonConverters
{
    public class ValueFilterJsonConverter : JsonConverter<IValueFilter>
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
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
            public JsonElement Query { get; set; }
        }

        public override void Write(Utf8JsonWriter writer, IValueFilter value, JsonSerializerOptions options)
        {
            if (value is ItemFilter itemFilter)
            {
                JsonSerializer.Serialize(writer, new { type = "item", query = itemFilter.Codes });
                return;
            }

            if (value is AllFilter)
            {
                JsonSerializer.Serialize(writer, new { type = "all" });
                return;
            }

            if (value is FromFilter fromFilter)
            {
                JsonSerializer.Serialize(writer, new { type = "from", query = fromFilter.Code });
                return;
            }

            if (value is TopFilter topFilter)
            {
                JsonSerializer.Serialize(writer, new { type = "top", query = topFilter.Count });
            }
        }

        public override IValueFilter Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            ValueFilterJsonModel filterWrapper = JsonSerializer.Deserialize<ValueFilterJsonModel>(ref reader, options);

            switch (filterWrapper.Type)
            {
                case FilterType.Item:
                    {
                        List<string> codesList = filterWrapper.Query.Deserialize<List<string>>();
                        return new ItemFilter(codesList);
                    }
                case FilterType.All:
                    {
                        return new AllFilter();
                    }
                case FilterType.From:
                    {
                        string fromCode = filterWrapper.Query.Deserialize<string>();
                        return new FromFilter(fromCode);
                    }
                case FilterType.Top:
                    {
                        int topCount = filterWrapper.Query.Deserialize<int>();
                        return new TopFilter(topCount);
                    }
                default: throw new UnknownFilterTypeException("Unknown filter type: " + filterWrapper.Type);
            }
        }

    }
}
