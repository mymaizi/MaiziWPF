using MaiziWPF.Core;
using System.Windows;
using System.Windows.Controls;

namespace MaiziWPF.Modules.Sys
{
    public partial class RoleListView : UserControl
    {
        public RoleListView()
        {
            InitializeComponent();
        }

        private void SelectAllCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && RoleDataGrid != null)
            {
                if (checkBox.IsChecked == true)
                    RoleDataGrid.SelectAll();
                else
                    RoleDataGrid.UnselectAll();
            }
        }

        private void RowCheckBox_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                var row = VisualControlHelper.FindParent<DataGridRow>(checkBox);
                if (row != null)
                    row.IsSelected = !row.IsSelected;
                e.Handled = true;
            }
        }
    }
}