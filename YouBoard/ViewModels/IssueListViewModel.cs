using Prism.Mvvm;

namespace YouBoard.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class IssueListViewModel : BindableBase, ITabViewModel
    {
        public string Header { get; set; }
    }
}