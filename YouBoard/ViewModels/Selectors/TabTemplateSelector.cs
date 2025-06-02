using System.Windows;
using System.Windows.Controls;

namespace YouBoard.ViewModels.Selectors
{
    public class TabTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ProjectListTemplate { get; set; }

        public DataTemplate IssueListTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return item switch
            {
                ProjectListViewModel => ProjectListTemplate,
                IssueListViewModel => IssueListTemplate,
                _ => base.SelectTemplate(item, container),
            };
        }
    }
}