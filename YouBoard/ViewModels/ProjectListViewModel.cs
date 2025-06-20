using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        private object selectedItem;

        public event EventHandler ItemChosen;

        public IYouTrackProjectClient YouTrackProjectClient { get; set; }

        public ObservableCollection<ProjectWrapper> ProjectWrappers
        {
            get => projectWrappers;
            set => SetProperty(ref projectWrappers, value);
        }

        public string TabType { get; set; } = "ProjectList";

        public string Header { get; set; } = "Projects";

        public object SelectedItem
        {
            get => selectedItem;
            set
            {
                if (selectedItem is ProjectWrapper wrapper)
                {
                    wrapper.IsSelected = false;
                }

                if (value is ProjectWrapper w)
                {
                    w.IsSelected = true;
                }

                SetProperty(ref selectedItem, value);
            }
        }

        public AsyncRelayCommand LoadProjectsCommand => new AsyncRelayCommand(async () =>
        {
            if (YouTrackProjectClient == null)
            {
                return;
            }

            var jsonFileName = "projects.json";
            var localProjectList = YoutrackProjectClient.LoadProjectsFromJsonFile(jsonFileName);
            ProjectWrappers = new ObservableCollection<ProjectWrapper>(localProjectList);

            var finallyList = await YouTrackProjectClient.MergeProjectsWithRemoteData(localProjectList);
            ProjectWrappers = new ObservableCollection<ProjectWrapper>(finallyList);

            YoutrackProjectClient.SaveProjectsToJsonFile(ProjectWrappers.ToList(), jsonFileName);
        });

        public DelegateCommand<ProjectWrapper> SaveProjectsCommand => new ((param) =>
        {
            param.ProjectProfile.IsFavorite = !param.ProjectProfile.IsFavorite;
            YoutrackProjectClient.SaveProjectsToJsonFile(ProjectWrappers.ToList(), "projects.json");
        });

        public DelegateCommand<ProjectWrapper> ProjectChosenCommand => new ((param) =>
        {
            SelectedItem = param;
            ItemChosen?.Invoke(this, EventArgs.Empty);
        });
    }
}