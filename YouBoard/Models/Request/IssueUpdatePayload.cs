using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace YouBoard.Models.Request
{
    public class IssueUpdatePayload
    {
        [JsonPropertyName("customFields")]
        public List<IssueCustomFields> CustomFields { get; set; }

        [JsonPropertyName("summary")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Summary { get; set; } = null;

        [JsonPropertyName("description")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Description { get; set; } = null;
    }
}