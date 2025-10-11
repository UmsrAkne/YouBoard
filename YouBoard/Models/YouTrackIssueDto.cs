using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Mvvm;
using YouBoard.Utils;

namespace YouBoard.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class YouTrackIssueDto : BindableBase
    {
        public string IdReadable { get; set; } = string.Empty;

        public string Summary { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public long Created { get; set; }

        public List<CustomField> CustomFields { get; set; }

        public TimeSpan EstimatedDuration
        {
            get
            {
                var field = CustomFields.FirstOrDefault(f => f.Name == "予測");

                if (field?.Value?.ExtensionData != null &&
                    field.Value.ExtensionData.TryGetValue("minutes", out var minutesEl) &&
                    minutesEl.TryGetInt32(out var minutes))
                {
                    return TimeSpan.FromMinutes(minutes);
                }

                return TimeSpan.Zero;
            }
        }

        public bool IsDone()
        {
            var stateField = CustomFields?.FirstOrDefault(f => f.Name == "State");
            return stateField?.Value?.Name == "完了";
        }

        public IssueType GetIssueType()
        {
            var stateField = CustomFields?.FirstOrDefault(f => f.Name == "Type");
            return IssueTypeHelper.FromString(stateField?.Value?.Name);
        }

        public IssueState GetState()
        {
            var stateField = CustomFields?.FirstOrDefault(f => f.Name == "State");
            return stateField == null ?
                IssueState.Created :
                IssueStateHelper.FromString(stateField.Value?.Name);
        }
    }
}