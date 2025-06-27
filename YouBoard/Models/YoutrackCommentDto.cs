using System.Text.Json.Serialization;

namespace YouBoard.Models
{
    public class YoutrackCommentDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("created")]
        public long Created { get; set; }
    }
}