using System;
using System.Collections.ObjectModel;
using System.Linq;
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

        public string SelectedTemplateName { get; set; } = string.Empty;

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
            App.AppSettings.TemplateFileName = SelectedTemplateName;
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            var list = YamlTemplateProvider.ListTemplates();
            YamlTemplates.AddRange(list);

            var currentName = App.AppSettings.TemplateFileName;
            if (string.IsNullOrEmpty(currentName))
            {
                SelectedTemplateName = YamlTemplates
                    .FirstOrDefault(tn => string.Compare(tn, currentName, StringComparison.OrdinalIgnoreCase) == 0);
            }
        }
    }
}