using System.Windows;

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

        public CustomCheckBox()
        {
            InitializeComponent();
        }

        public bool IsCheck
        {
            get => (bool)GetValue(IsCheckProperty);
            set => SetValue(IsCheckProperty, value);
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            IsCheck = !IsCheck;
        }
    }
}