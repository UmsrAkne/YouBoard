using System;
using System.Text.Json.Serialization;

namespace YouBoard.Models.Request
{
    /// <summary>
    /// JSON payload for POST /issues/{id}/timeTracking/workItems
    /// </summary>
    public class WorkItemAddPayload
    {
        public WorkItemAddPayload(TimeSpan duration, string comment = "", DateTimeOffset? date = null)
        {
            if (duration.TotalMinutes < 1)
            {
                throw new ArgumentException("Work item must be at least 1 minute long.", nameof(duration));
            }

            Duration = new DurationMinutes { Minutes = (int)duration.TotalMinutes, };
            Text = comment ?? string.Empty;
            Date = (date ?? DateTimeOffset.Now).ToUnixTimeMilliseconds();
        }

        [JsonPropertyName("duration")]
        public DurationMinutes Duration { get; }

        [JsonPropertyName("text")]
        public string Text { get; }

        [JsonPropertyName("date")]
        public long Date { get; }

        public class DurationMinutes
        {
            [JsonPropertyName("minutes")]
            public int Minutes { get; set; }
        }
    }
}