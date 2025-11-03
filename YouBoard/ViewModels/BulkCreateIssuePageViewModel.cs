using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using YouBoard.Models;
using YouBoard.Services;
using YouBoard.Utils;

namespace YouBoard.ViewModels
{
    public class BulkCreateIssuePageViewModel : BindableBase, IDialogAware
    {
        private string inputText;
        private ObservableCollection<IssueWrapper> parsedIssues = new ();
        private IYouTrackIssueClient client;
        private string projectShortName = string.Empty;
        private bool canPost = true;
        private string postButtonText = "Post All";

        public event Action<IDialogResult> RequestClose;

        public string Title => "Bulk Create Issues";

        public DelegateCommand ParseIssueListCommand => new DelegateCommand(() =>
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

        public AsyncRelayCommand PostAllCommand => new(async () =>
        {
            if (client == null || ParsedIssues == null || ParsedIssues.Count == 0)
            {
                return;
            }

            CanPost = false;
            PostButtonText = "Posting...";
            try
            {
                foreach (var issue in ParsedIssues.ToList())
                {
                    await client.CreateIssueAsync(projectShortName, issue);
                }

                PostButtonText = $"Created {ParsedIssues.Count} issues.";
                ParsedIssues.Clear();
                CloseDialog(ButtonResult.OK);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                CanPost = true;
                PostButtonText = "Post All";
            }
        }, () => CanPost);

        public string InputText { get => inputText; set => SetProperty(ref inputText, value); }

        public ObservableCollection<IssueWrapper> ParsedIssues
        {
            get => parsedIssues;
            set => SetProperty(ref parsedIssues, value);
        }

        public bool CanPost { get => canPost; set => SetProperty(ref canPost, value); }

        public string PostButtonText { get => postButtonText; set => SetProperty(ref postButtonText, value); }

        public IssueType[] AvailableIssueTypes { get; } = Enum.GetValues(typeof(IssueType)).Cast<IssueType>().ToArray();

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
            // Cleanup if needed
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters?.ContainsKey("client") == true)
            {
                client = parameters.GetValue<IYouTrackIssueClient>("client");
            }

            if (parameters?.ContainsKey("projectShortName") == true)
            {
                projectShortName = parameters.GetValue<string>("projectShortName");
            }
        }

        // Helper methods to raise close if needed later
        private void CloseDialog(ButtonResult result = ButtonResult.None)
        {
            RequestClose?.Invoke(new DialogResult(result));
        }
    }
}