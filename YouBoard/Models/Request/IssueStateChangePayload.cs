using System.Text.Json.Serialization;

namespace YouBoard.Models.Request
{
    public class IssueStateChangePayload
    {
        public IssueStateChangePayload(string stateName)
        {
            CustomFields = new[]
            {
                new CustomField
                {
                    Name = "State",
                    Type = "SingleEnumIssueCustomField",
                    Value = new FieldValue() { Name = stateName, },
                },
            };
        }

        [JsonPropertyName("customFields")]
        public CustomField[] CustomFields { get; }
    }
}