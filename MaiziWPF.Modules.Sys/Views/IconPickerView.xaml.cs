using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MaiziWPF.Modules.Sys
{
    public partial class IconPickerView : UserControl
    {
        public IconPickerView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is IconPickerViewModel oldVm)
            {
                oldVm.PropertyChanged -= OnViewModelPropertyChanged;
            }

            if (e.NewValue is IconPickerViewModel newVm)
            {
                newVm.SelectedIcon = SelectedIcon;
                newVm.PropertyChanged += OnViewModelPropertyChanged;
            }
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IconPickerViewModel.SelectedIcon))
            {
                if (sender is IconPickerViewModel vm)
                {
                    SelectedIcon = vm.SelectedIcon;
                }
            }
        }

        public string SelectedIcon
        {
            get => (string)GetValue(SelectedIconProperty);
            set => SetValue(SelectedIconProperty, value);
        }

        public static readonly DependencyProperty SelectedIconProperty =
            DependencyProperty.Register(
                nameof(SelectedIcon),
                typeof(string),
                typeof(IconPickerView),
                new FrameworkPropertyMetadata(string.Empty,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedIconChanged));

        private static void OnSelectedIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IconPickerView view && view.DataContext is IconPickerViewModel vm)
            {
                vm.SelectedIcon = (string)e.NewValue;
            }
        }
    }
}