using System.Text.Json.Serialization;

namespace YouBoard.Models
{
    public class CustomField
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public FieldValue Value { get; set; }

        [JsonPropertyName("$type")]
        public string Type { get; set; }
    }
}