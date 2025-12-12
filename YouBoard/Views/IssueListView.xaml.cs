using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace YouBoard.Views
{
    public partial class IssueListView
    {
        public IssueListView()
        {
            InitializeComponent();
        }

        private void InnerListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;

                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
                {
                    RoutedEvent = MouseWheelEvent,
                    Source = sender,
                };

                var current = sender as DependencyObject;

                while (current != null)
                {
                    current = VisualTreeHelper.GetParent(current);
                    if (current is not UIElement element)
                    {
                        continue;
                    }

                    element.RaiseEvent(eventArg);
                    break;
                }
            }
        }
    }
}