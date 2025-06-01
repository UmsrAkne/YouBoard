using Prism.Mvvm;
using YouBoard.Utils;

namespace YouBoard.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainWindowViewModel : BindableBase
    {
        private readonly AppVersionInfo appVersionInfo = new ();

        public ProjectListViewModel ProjectListViewModel { get; private set; } = new ();

        public string Title => appVersionInfo.Title;
    }
}