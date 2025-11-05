using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
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
        private readonly DispatcherTimer saveDebounceTimer;

        private ObservableCollection<ProjectWrapper> projectWrappers = new ();
        private object selectedItem;
        private string title = "Projects";

        // Debounce save machinery
        private bool savePending;

        public ProjectListViewModel()
        {
            saveDebounceTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000),
            };

            saveDebounceTimer.Tick += (_, _) =>
            {
                saveDebounceTimer.Stop();
                if (savePending)
                {
                    savePending = false;
                    SaveProjectsToJson();
                }
            };

            // Hook initial collection (empty by default)
            HookCollection(ProjectWrappers);
        }

        public event EventHandler ItemChosen;

        public IYouTrackProjectClient YouTrackProjectClient { get; set; }

        public ObservableCollection<ProjectWrapper> ProjectWrappers
        {
            get => projectWrappers;
            set
            {
                if (projectWrappers == value)
                {
                    return;
                }

                // Unhook an old collection
                if (projectWrappers != null)
                {
                    UnhookCollection(projectWrappers);
                }

                SetProperty(ref projectWrappers, value);

                // Hook new collection
                if (projectWrappers != null)
                {
                    HookCollection(projectWrappers);
                }
            }
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

        public string Title { get => title; set => SetProperty(ref title, value); }

        public AsyncRelayCommand LoadProjectsCommand => new AsyncRelayCommand(async () =>
        {
            if (YouTrackProjectClient == null)
            {
                return;
            }

            var jsonFileName = "projects.json";
            var localProjectList = YouTrackProjectClient.LoadProjectsFromJsonFile(jsonFileName);
            ProjectWrappers = new ObservableCollection<ProjectWrapper>(localProjectList);

            var finallyList = await YouTrackProjectClient.MergeProjectsWithRemoteData(localProjectList);
            ProjectWrappers = new ObservableCollection<ProjectWrapper>(finallyList);

            YouTrackProjectClient.SaveProjectsToJsonFile(ProjectWrappers.ToList(), jsonFileName);
        });

        public DelegateCommand<ProjectWrapper> SaveProjectsCommand => new ((param) =>
        {
            param.ProjectProfile.IsFavorite = !param.ProjectProfile.IsFavorite;
            YouTrackProjectClient.SaveProjectsToJsonFile(ProjectWrappers.ToList(), "projects.json");
        });

        public DelegateCommand<ProjectWrapper> ProjectChosenCommand => new ((param) =>
        {
            SelectedItem = param;
            ItemChosen?.Invoke(this, EventArgs.Empty);
        });

        private void HookCollection(ObservableCollection<ProjectWrapper> collection)
        {
            if (collection == null)
            {
                return;
            }

            collection.CollectionChanged += OnProjectWrappersChanged;
            foreach (var pw in collection)
            {
                HookWrapper(pw);
            }
        }

        private void UnhookCollection(ObservableCollection<ProjectWrapper> collection)
        {
            if (collection == null)
            {
                return;
            }

            collection.CollectionChanged -= OnProjectWrappersChanged;
            foreach (var pw in collection)
            {
                UnhookWrapper(pw);
            }
        }

        private void OnProjectWrappersChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ProjectWrapper pw in e.NewItems)
                {
                    HookWrapper(pw);
                }
            }

            if (e.OldItems != null)
            {
                foreach (ProjectWrapper pw in e.OldItems)
                {
                    UnhookWrapper(pw);
                }
            }
        }

        private void HookWrapper(ProjectWrapper pw)
        {
            if (pw == null)
            {
                return;
            }

            pw.PropertyChanged += OnProjectWrapperPropertyChanged;

            // Hook current profile
            if (pw.ProjectProfile != null)
            {
                WeakEventManager<ProjectProfile, PropertyChangedEventArgs>.AddHandler(
                    pw.ProjectProfile,
                    nameof(PropertyChanged),
                    OnProjectProfileChanged);
            }
        }

        private void UnhookWrapper(ProjectWrapper pw)
        {
            if (pw == null)
            {
                return;
            }

            pw.PropertyChanged -= OnProjectWrapperPropertyChanged;
            if (pw.ProjectProfile != null)
            {
                WeakEventManager<ProjectProfile, PropertyChangedEventArgs>.RemoveHandler(
                    pw.ProjectProfile,
                    nameof(PropertyChanged),
                    OnProjectProfileChanged);
            }
        }

        private void OnProjectWrapperPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is not ProjectWrapper pw)
            {
                return;
            }

            if (e.PropertyName == nameof(ProjectWrapper.ProjectProfile))
            {
                // Rewire profile handlers when replaced
                UnhookWrapper(pw);
                HookWrapper(pw);
            }
        }

        private void OnProjectProfileChanged(object sender, PropertyChangedEventArgs e)
        {
            // Any property change in ProjectProfile triggers debounced save
            // ScheduleSave
            savePending = true;
            saveDebounceTimer.Stop();
            saveDebounceTimer.Start();
        }

        private void SaveProjectsToJson()
        {
            try
            {
                if (YouTrackProjectClient == null)
                {
                    return;
                }

                YouTrackProjectClient.SaveProjectsToJsonFile(ProjectWrappers.ToList(), "projects.json");
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to save projects to json");
            }
        }
    }
}