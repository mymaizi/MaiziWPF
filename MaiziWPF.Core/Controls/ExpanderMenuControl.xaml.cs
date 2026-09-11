using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MaiziWPF.Core
{
    /// <summary>
    /// ExpanderMenuControl.xaml 的交互逻辑
    /// </summary>
    public partial class ExpanderMenuControl : UserControl
    {
        public ExpanderMenuControl()
        {
            InitializeComponent();
            this.AddHandler(Expander.ExpandedEvent, new RoutedEventHandler(OnExpanderExpanded));
        }

        private void OnExpanderExpanded(object sender, RoutedEventArgs e)
        {
            var expandedExpander = e.OriginalSource as Expander;
            if (expandedExpander == null) return;

            DependencyObject parent = VisualTreeHelper.GetParent(expandedExpander);
            while (parent != null)
            {
                if (parent is ItemsControl itemsControl)
                {
                    CollapseSiblingExpanders(itemsControl, expandedExpander);
                    break;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }
        }

        private void CollapseSiblingExpanders(ItemsControl itemsControl, Expander exclude)
        {
            for (int i = 0; i < itemsControl.Items.Count; i++)
            {
                var container = itemsControl.ItemContainerGenerator.ContainerFromIndex(i);
                if (container != null)
                {
                    var expander = FindChildExpander(container);
                    if (expander != null && expander != exclude && expander.IsExpanded)
                    {
                        expander.IsExpanded = false;
                    }
                }
            }
        }

        private Expander FindChildExpander(DependencyObject parent)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is Expander expander)
                    return expander;

                var found = FindChildExpander(child);
                if (found != null)
                    return found;
            }
            return null;
        }
    }
}