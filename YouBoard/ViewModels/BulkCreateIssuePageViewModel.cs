using System;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace YouBoard.ViewModels
{
    public class BulkCreateIssuePageViewModel : BindableBase, IDialogAware
    {
        public event Action<IDialogResult> RequestClose;

        public string Title => "Bulk Create Issues";

        public DelegateCommand CancelCommand => new DelegateCommand(() =>
        {
            CloseDialog(ButtonResult.Cancel);
        });

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