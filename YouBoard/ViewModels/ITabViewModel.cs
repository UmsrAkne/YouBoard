using System;

namespace YouBoard.ViewModels
{
    public interface ITabViewModel
    {
        event EventHandler ItemChosen;

        public string TabType { get; set; }
        
        public string Header { get; set; }

        public object SelectedItem { get; set; }

        public string Title { get; set; }
    }
}