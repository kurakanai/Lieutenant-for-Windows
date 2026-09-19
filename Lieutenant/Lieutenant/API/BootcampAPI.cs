using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static Lieutenant.API.BootcampAPI;

namespace Lieutenant.API
{
    public static class BootcampAPI
    {
        public record BootCampCatalog(List<ProductFamily> Families, Dictionary<string, int> ModelLinkIndex, List<string> DownloadLinks);

        public record ProductFamily([property: JsonPropertyName("product_family")] string Name, [property: JsonPropertyName("subfamilies")] List<Subfamily> Subfamilies)
        {
            public override string ToString() => Name ?? string.Empty;
        }

        public record Subfamily([property: JsonPropertyName("subfamily_name")] string Name, [property: JsonPropertyName("models")] List<Model> Models)
        {
            public override string ToString() => Name ?? string.Empty;
        }

        public record Model([property: JsonPropertyName("modelName")] string Name, [property: JsonPropertyName("years")] List<ModelYear> Years)
        {
            public override string ToString() => Name ?? string.Empty;
        }
    }
    public class ModelYear
    {
        [JsonPropertyName("year")]
        public JsonElement RawYear { get; set; }

        [JsonIgnore]
        public string YearDisplay => RawYear.ValueKind switch
        {
            JsonValueKind.Number => RawYear.GetInt32().ToString(),
            JsonValueKind.String => RawYear.GetString() ?? string.Empty,
            _ => string.Empty
        };

        [JsonPropertyName("model_id")]
        public List<object> RawModelIds { get; set; } = new();

        [JsonIgnore]
        public List<string> ModelIds => ExtractStrings(RawModelIds);

        private static List<string> ExtractStrings(IEnumerable<object> items)
        {
            var result = new List<string>();
            foreach (var item in items)
            {
                if (item is JsonElement elem)
                {
                    if (elem.ValueKind == JsonValueKind.String)
                        result.Add(elem.GetString()!);
                    else if (elem.ValueKind == JsonValueKind.Array)
                        result.AddRange(elem.EnumerateArray().Select(e => e.GetString()!));
                }
            }
            return result;
        }

        public override string ToString() => YearDisplay;
    }

    public class BootCampCatalogConverter : JsonConverter<BootCampCatalog>
    {
        public override BootCampCatalog Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            var families = JsonSerializer.Deserialize<List<ProductFamily>>(root[0].GetRawText(), options)!;
            var linkIndex = JsonSerializer.Deserialize<Dictionary<string, int>>(root[1].GetRawText(), options)!;
            var links = JsonSerializer.Deserialize<List<string>>(root[2].GetRawText(), options)!;

            return new BootCampCatalog(families, linkIndex, links);
        }

        public override void Write(Utf8JsonWriter writer, BootCampCatalog value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
