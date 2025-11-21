using System;
using System.Collections.Generic;
using System.Text.Json;
using YouBoard.Models;

namespace YouBoard.Utils
{
    public static class IssueWrapperParser
    {
        public static List<IssueWrapper> ParseIssueWrappersFromJson(string json)
        {
            using var doc = JsonDocument.Parse(json);
            var wrappers = new List<IssueWrapper>();

            if (doc.RootElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var issueJson in doc.RootElement.EnumerateArray())
                {
                    var wrapper = ParseSingleIssueFromJson(issueJson);
                    wrappers.Add(wrapper);
                }
            }
            else if (doc.RootElement.ValueKind == JsonValueKind.Object)
            {
                var wrapper = ParseSingleIssueFromJson(doc.RootElement);
                wrappers.Add(wrapper);
            }

            return wrappers;
        }

        private static IssueWrapper ParseSingleIssueFromJson(JsonElement issueJson)
        {
            var wrapper = new IssueWrapper
            {
                Id = issueJson.GetProperty("idReadable").GetString(),
                Title = issueJson.GetProperty("summary").GetString(),
            };

            if (issueJson.TryGetProperty("created", out var createdEl))
            {
                var createdUnixMs = createdEl.GetInt64();
                wrapper.Created = DateTimeOffset.FromUnixTimeMilliseconds(createdUnixMs).UtcDateTime;
            }

            if (issueJson.TryGetProperty("description", out var descriptionEl))
            {
                wrapper.Description = descriptionEl.GetString();
            }

            // customFields
            var fields = issueJson.GetProperty("customFields").EnumerateArray();

            foreach (var field in fields)
            {
                var fieldName = field.GetProperty("name").GetString();
                var valueEl = field.GetProperty("value");

                switch (fieldName)
                {
                    case "State":
                        if (valueEl.ValueKind == JsonValueKind.Object
                            && valueEl.TryGetProperty("name", out var stateName))
                        {
                            wrapper.State = IssueStateHelper.FromString(stateName.GetString());
                            wrapper.IsComplete = stateName.GetString() == "完了";
                        }

                        break;

                    case "Type":
                        if (valueEl.ValueKind == JsonValueKind.Object
                            && valueEl.TryGetProperty("name", out var typeName))
                        {
                            wrapper.Type = IssueTypeHelper.FromString(typeName.GetString());
                        }

                        break;

                    case "予測":
                        if (valueEl.ValueKind == JsonValueKind.Object
                            && valueEl.TryGetProperty("minutes", out var minutesEl)
                            && minutesEl.TryGetInt32(out var minutes))
                        {
                            wrapper.EstimatedDuration = TimeSpan.FromMinutes(minutes);
                        }

                        break;

                    case "経過時間":
                        if (valueEl.ValueKind == JsonValueKind.Object
                            && valueEl.TryGetProperty("minutes", out var elapsedEl)
                            && elapsedEl.TryGetInt32(out var elapsed))
                        {
                            wrapper.ElapsedDuration = TimeSpan.FromMinutes(elapsed);
                        }

                        break;
                    case "EntryNo":
                        if (valueEl.ValueKind == JsonValueKind.Number)
                        {
                            if (valueEl.TryGetInt32(out var entryNo))
                            {
                                wrapper.EntryNo = entryNo;
                            }
                        }

                        break;
                }
            }

            return wrapper;
        }
    }
}