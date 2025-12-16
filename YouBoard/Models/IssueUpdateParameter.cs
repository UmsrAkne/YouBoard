using Prism.Mvvm;

namespace YouBoard.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class IssueUpdateParameter : BindableBase
    {
        private IssueWrapper issueWrapper;
        private string updatePropertyName;

        public IssueWrapper IssueWrapper { get => issueWrapper; set => SetProperty(ref issueWrapper, value); }

        public string UpdatePropertyName
        {
            get => updatePropertyName;
            set => SetProperty(ref updatePropertyName, value);
        }
    }
}