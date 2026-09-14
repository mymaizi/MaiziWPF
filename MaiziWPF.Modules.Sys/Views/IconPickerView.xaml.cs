using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MaiziWPF.Modules.Sys
{
    public partial class IconPickerView : UserControl
    {
        public IconPickerView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
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
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is IconPickerViewModel vm)
            {
                BindingOperations.ClearBinding(this, SelectedIconProperty);
                var binding = new Binding(nameof(vm.SelectedIcon))
                {
                    Source = vm,
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                BindingOperations.SetBinding(this, SelectedIconProperty, binding);
            }
        }
    }
}