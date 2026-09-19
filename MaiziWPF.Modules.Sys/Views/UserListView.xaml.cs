using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MaiziWPF.Services.Domain;

namespace MaiziWPF.Modules.Sys
{
    public partial class UserListView : UserControl
    {
        public UserListView()
        {
            InitializeComponent();
        }

        private void SelectAllCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox selectAllCheckBox && selectAllCheckBox.IsChecked.HasValue)
            {
                bool isChecked = selectAllCheckBox.IsChecked.Value;
                for (int i = 0; i < UserDataGrid.Items.Count; i++)
                {
                    if (UserDataGrid.ItemContainerGenerator.ContainerFromIndex(i) is DataGridRow row)
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