using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MaiziWPF.Modules.Sys
{
    public partial class IconPickerView : UserControl
    {
        public IconPickerView()
        {
            this.DataContextChanged += (s,args) =>
            {
                if (DataContext is IconPickerViewModel vm)
                {
                    vm.PropertyChanged += (s, args) => 
                    {
                        if (args.PropertyName == nameof(IconPickerViewModel.SelectedIcon))
                            SelectedIcon = vm.SelectedIcon;
                    };
                }
            };
            InitializeComponent();
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