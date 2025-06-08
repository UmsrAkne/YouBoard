using System.Text.Json.Serialization;

namespace YouBoard.Models
{
    public class FieldValue
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}