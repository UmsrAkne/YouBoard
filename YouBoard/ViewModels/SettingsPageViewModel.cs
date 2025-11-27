using System;
using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using YouBoard.Utils;

namespace YouBoard.ViewModels
{
    public class SettingsPageViewModel : BindableBase, IDialogAware
    {
        public event Action<IDialogResult> RequestClose;

        public ObservableCollection<string> YamlTemplates { get; set; } = new ();

        public string Title => "Settings";

        public DelegateCommand SaveCommand => new (() =>
        {
            // Not implemented per requirements
        });

        public DelegateCommand CloseCommand => new (() =>
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        });

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            // no-op
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            Console.WriteLine("SettingsPageViewModel.OnDialogOpened");
            var list = YamlTemplateProvider.ListTemplates();
            YamlTemplates.AddRange(list);
        }
    }
}