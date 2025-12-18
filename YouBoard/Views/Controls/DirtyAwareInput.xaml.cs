using System.Windows;

namespace YouBoard.Views.Controls
{
    public partial class DirtyAwareInput
    {
        // ReSharper disable once ArrangeModifiersOrder
        public static readonly DependencyProperty IsDirtyProperty =
            DependencyProperty.Register(
                nameof(IsDirty),
                typeof(bool),
                typeof(DirtyAwareInput),
                new PropertyMetadata(false));

        public DirtyAwareInput()
        {
            InitializeComponent();
        }

        public bool IsDirty
        {
            get => (bool)GetValue(IsDirtyProperty);
            set => SetValue(IsDirtyProperty, value);
        }
    }
}