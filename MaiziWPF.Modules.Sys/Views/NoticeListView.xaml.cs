using MaiziWPF.Core;
using System.Windows;
using System.Windows.Controls;

namespace MaiziWPF.Modules.Sys
{
    public partial class NoticeListView : UserControl
    {
        public NoticeListView()
        {
            InitializeComponent();
        }

        private void SelectAllCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox selectAllCheckBox && selectAllCheckBox.IsChecked.HasValue)
            {
                bool isChecked = selectAllCheckBox.IsChecked.Value;
                for (int i = 0; i < NoticeDataGrid.Items.Count; i++)
                {
                    if (NoticeDataGrid.ItemContainerGenerator.ContainerFromIndex(i) is DataGridRow row)
                    {
                        row.IsSelected = isChecked;
                    }
                }
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