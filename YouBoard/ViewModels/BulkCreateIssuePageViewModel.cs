using System;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using YouBoard.Models;
using YouBoard.Utils;

namespace YouBoard.ViewModels
{
    public class BulkCreateIssuePageViewModel : BindableBase, IDialogAware
    {
        private string inputText;
        private ObservableCollection<IssueWrapper> parsedIssues = new ();

        public event Action<IDialogResult> RequestClose;

        public string Title => "Bulk Create Issues";

        public DelegateCommand CreateIssueListCommand => new DelegateCommand(() =>
        {
            if (string.IsNullOrWhiteSpace(InputText))
            {
                ParsedIssues = new ObservableCollection<IssueWrapper>();
                return;
            }

            var list = IssueParser.ParseIssues(InputText ?? string.Empty);
            ParsedIssues = new ObservableCollection<IssueWrapper>(list);
        });

        public DelegateCommand CancelCommand => new DelegateCommand(() =>
        {
            CloseDialog(ButtonResult.Cancel);
        });

        public string InputText { get => inputText; set => SetProperty(ref inputText, value); }

        public ObservableCollection<IssueWrapper> ParsedIssues
        {
            get => parsedIssues;
            set => SetProperty(ref parsedIssues, value);
        }

        public IssueType[] AvailableIssueTypes { get; } = Enum.GetValues(typeof(IssueType)).Cast<IssueType>().ToArray();

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
            // Cleanup if needed
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            // Initialize from parameters if needed
        }

        // Helper methods to raise close if needed later
        private void CloseDialog(ButtonResult result = ButtonResult.None)
        {
            RequestClose?.Invoke(new DialogResult(result));
        }
    }
}