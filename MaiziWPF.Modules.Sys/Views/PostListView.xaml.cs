using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MaiziWPF.Modules.Sys
{
    public partial class PostListView : UserControl
    {
        public PostListView()
        {
            InitializeComponent();
        }

        private void SelectAllCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && PostDataGrid != null)
            {
                if (checkBox.IsChecked == true)
                    PostDataGrid.SelectAll();
                else
                    PostDataGrid.UnselectAll();
            }
        }

        private void RowCheckBox_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                var row = FindParent<DataGridRow>(checkBox);
                if (row != null)
                    row.IsSelected = !row.IsSelected;
                e.Handled = true;
            }
        }

        private static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(child);
            if (parent == null) return null;
            return parent is T t ? t : FindParent<T>(parent);
        }
    }
}