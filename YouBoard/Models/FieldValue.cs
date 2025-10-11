using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace YouBoard.Models
{
    public class FieldValue
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement> ExtensionData { get; set; }
    }
}