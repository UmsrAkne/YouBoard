using System.Collections.ObjectModel;
using Prism.Mvvm;
using YouBoard.Services;
using YouBoard.Utils;

namespace YouBoard.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainWindowViewModel : BindableBase
    {
        private readonly AppVersionInfo appVersionInfo = new ();

        public MainWindowViewModel()
        {
        }

        public MainWindowViewModel(IYouTrackProjectClient youtrackProjectClient)
        {
            ProjectListViewModel.YouTrackProjectClient = youtrackProjectClient;
        }

        public ObservableCollection<ITabViewModel> DynamicTabs { get; set; } = new ();

        public ProjectListViewModel ProjectListViewModel { get; private set; } = new ();

        public string Title => appVersionInfo.Title;
    }
}