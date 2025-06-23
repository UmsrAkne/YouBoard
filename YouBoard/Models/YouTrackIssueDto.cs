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

        public List<CustomField> CustomFields { get; set; }

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