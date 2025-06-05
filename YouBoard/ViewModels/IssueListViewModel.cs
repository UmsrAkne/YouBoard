using System;
using Prism.Mvvm;

namespace YouBoard.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class IssueListViewModel : BindableBase, ITabViewModel
    {
        public event EventHandler ItemChosen;

        public string TabType { get; set; } = "IssueList";

        public string Header { get; set; }

        public object SelectedItem { get; set; }
    }
}