using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Prism.Commands;
using Prism.Mvvm;
using YouBoard.Models;
using YouBoard.Services;

namespace YouBoard.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ProjectListViewModel : BindableBase, ITabViewModel
    {
        private ObservableCollection<ProjectWrapper> projectWrappers = new ();

        public event EventHandler ItemChosen;

        public IYouTrackProjectClient YouTrackProjectClient { get; set; }

        public ObservableCollection<ProjectWrapper> ProjectWrappers
        {
            get => projectWrappers;
            set => SetProperty(ref projectWrappers, value);
        }

        public string TabType { get; set; } = "ProjectList";

        public string Header { get; set; } = "Projects";

        public object SelectedItem { get; set; }

        public AsyncRelayCommand LoadProjectsCommand => new AsyncRelayCommand(async () =>
        {
            if (YouTrackProjectClient == null)
            {
                return;
            }

            var p = await YouTrackProjectClient.GetProjectsAsync();
            ProjectWrappers = new ObservableCollection<ProjectWrapper>(p);
        });

        public DelegateCommand ProjectChosenCommand => new DelegateCommand(() =>
        {
            ItemChosen?.Invoke(this, EventArgs.Empty);
        });
    }
}