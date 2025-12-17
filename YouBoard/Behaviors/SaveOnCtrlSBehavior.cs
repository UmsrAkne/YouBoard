using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
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

        // ReSharper disable once ArrangeModifiersOrder
        public static readonly DependencyProperty IsDirtyProperty =
            DependencyProperty.RegisterAttached(
                "IsDirty",
                typeof(bool),
                typeof(SaveOnCtrlSBehavior),
                new FrameworkPropertyMetadata(false));

        public string FieldName
        {
            get => (string)GetValue(FieldNameProperty);
            set => SetValue(FieldNameProperty, value);
        }

        public static bool GetIsDirty(DependencyObject obj)
            => (bool)obj.GetValue(IsDirtyProperty);

        public static void SetIsDirty(DependencyObject obj, bool value)
            => obj.SetValue(IsDirtyProperty, value);

        protected override void OnAttached()
        {
            AssociatedObject.PreviewKeyDown += OnPreviewKeyDown;

            if (AssociatedObject is NumericUpDown nud)
            {
                Console.WriteLine("set event");
                nud.ValueChanged += OnValueChanged;
            }
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewKeyDown -= OnPreviewKeyDown;

            if (AssociatedObject is NumericUpDown nud)
            {
                nud.ValueChanged -= OnValueChanged;
            }
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

        private void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            SetIsDirty(AssociatedObject, true);
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

            SetIsDirty(AssociatedObject, false);
            e.Handled = true;
        }
    }
}