using System.Text.Json.Serialization;

namespace YouBoard.Models.Request
{
    public class IssueCustomFields
    {
        [JsonPropertyName("name")]
        public string Name { get; init; } = string.Empty;

        [JsonPropertyName("$type")]
        public string Type { get; init; } = string.Empty;

        [JsonPropertyName("value")]
        public object Value { get; init; }
    }
}