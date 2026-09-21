using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MaiziWPF.Modules.Sys
{
    public partial class LoginInfoListView : UserControl
    {
        public LoginInfoListView()
        {
            InitializeComponent();
        }

        private void SelectAllCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox selectAllCheckBox && selectAllCheckBox.IsChecked.HasValue)
            {
                bool isChecked = selectAllCheckBox.IsChecked.Value;
                for (int i = 0; i < LoginInfoDataGrid.Items.Count; i++)
                {
                    if (LoginInfoDataGrid.ItemContainerGenerator.ContainerFromIndex(i) is DataGridRow row)
                    {
                        row.IsSelected = isChecked;
                    }
                }
            }
        }

        private void RowCheckBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                var row = FindVisualParent<DataGridRow>(checkBox);
                if (row != null)
                {
                    row.IsSelected = !row.IsSelected;
                    e.Handled = true;
                }
            }
        }

        private static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(child);
            while (parent != null && parent is not T)
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent as T;
        }
    }
}