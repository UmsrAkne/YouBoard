using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;
using YouBoard.Models;
using YouBoard.ViewModels;

namespace YouBoard.Behaviors
{
    public class SaveOnCtrlSBehavior : Behavior<FrameworkElement>
    {
        // ReSharper disable once ArrangeModifiersOrder
        public static readonly DependencyProperty FieldNameProperty =
            DependencyProperty.Register(
                nameof(FieldName),
                typeof(string),
                typeof(SaveOnCtrlSBehavior));

        public string FieldName
        {
            get => (string)GetValue(FieldNameProperty);
            set => SetValue(FieldNameProperty, value);
        }

        protected override void OnAttached()
        {
            AssociatedObject.PreviewKeyDown += OnPreviewKeyDown;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewKeyDown -= OnPreviewKeyDown;
        }

        private static T FindVisualAncestor<T>(DependencyObject current)
            where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T found)
                {
                    return found;
                }

                current = VisualTreeHelper.GetParent(current);
            }

            return null;
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.S || Keyboard.Modifiers != ModifierKeys.Control)
            {
                return;
            }

            // var listBox = AssociatedObject.FindAncestor<ListBox>();
            var listBox = FindVisualAncestor<ListBox>(AssociatedObject);
            if (listBox?.DataContext is not IssueListViewModel vm)
            {
                return;
            }

            vm.IssueFieldUpdateRequestAsyncCommand.Execute(
                new IssueUpdateParameter()
                {
                    IssueWrapper = (IssueWrapper)AssociatedObject.DataContext,
                    UpdatePropertyName = FieldName,
                });

            e.Handled = true;
        }
    }
}