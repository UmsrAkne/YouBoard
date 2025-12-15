using System.Windows;
using System.Windows.Input;

namespace YouBoard.Views.Controls
{
    public partial class CustomCheckBox
    {
        // ReSharper disable once ArrangeModifiersOrder
        public static readonly DependencyProperty IsCheckProperty =
            DependencyProperty.Register(
                nameof(IsCheck),
                typeof(bool),
                typeof(CustomCheckBox),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        // ReSharper disable once ArrangeModifiersOrder
        public static readonly DependencyProperty CheckedCommandProperty =
            DependencyProperty.Register(
                nameof(CheckedCommand),
                typeof(ICommand),
                typeof(CustomCheckBox),
                new PropertyMetadata(null));

        // ReSharper disable once ArrangeModifiersOrder
        public static readonly DependencyProperty CheckedCommandParameterProperty =
            DependencyProperty.Register(
                nameof(CheckedCommandParameter),
                typeof(object),
                typeof(CustomCheckBox),
                new PropertyMetadata(null));

        public CustomCheckBox()
        {
            InitializeComponent();
        }

        public ICommand CheckedCommand
        {
            get => (ICommand)GetValue(CheckedCommandProperty);
            set => SetValue(CheckedCommandProperty, value);
        }

        public object CheckedCommandParameter
        {
            get => GetValue(CheckedCommandParameterProperty);
            set => SetValue(CheckedCommandParameterProperty, value);
        }

        public bool IsCheck
        {
            get => (bool)GetValue(IsCheckProperty);
            set => SetValue(IsCheckProperty, value);
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            IsCheck = !IsCheck;
            if (IsCheck)
            {
                CheckedCommand?.Execute(CheckedCommandParameter);
            }
        }
    }
}