using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using MaiziWPF.Core;
using MaiziWPF.ViewModels;

namespace MaiziWPF.Views
{
    /// <summary>
    /// Interaction logic for MainView
    /// </summary>
    public partial class MainView : UserControl
    {
        private System.Windows.Threading.DispatcherTimer _closeTimer;
        private bool _isMouseInPopup;

        public MainView()
        {
            InitializeComponent();
            _closeTimer = new System.Windows.Threading.DispatcherTimer();
            _closeTimer.Interval = System.TimeSpan.FromMilliseconds(150);
            _closeTimer.Tick += CloseTimer_Tick;
        }

        private void UserAvatarBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            _closeTimer.Stop();
            _isMouseInPopup = false;
            if (DataContext is MainViewModel vm)
            {
                vm.IsMoreMenuOpen = true;
            }
        }

        private void UserAvatarBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!_isMouseInPopup)
            {
                _closeTimer.Start();
            }
        }

        private void UserMenuPopup_MouseEnter(object sender, MouseEventArgs e)
        {
            _closeTimer.Stop();
            _isMouseInPopup = true;
        }

        private void UserMenuPopup_MouseLeave(object sender, MouseEventArgs e)
        {
            _isMouseInPopup = false;
            _closeTimer.Start();
        }

        private void UserMenuPopup_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // 点击 Popup 外部关闭
            if (e.OriginalSource is Border border && border == (sender as Popup)?.Child)
            {
                return;
            }
            if (DataContext is MainViewModel vm)
            {
                vm.IsMoreMenuOpen = false;
            }
        }

        private void CloseTimer_Tick(object sender, object e)
        {
            _closeTimer.Stop();
            if (!_isMouseInPopup && DataContext is MainViewModel vm)
            {
                vm.IsMoreMenuOpen = false;
            }
        }

        private void TabRegionControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is not TabControl tabControl) return;

            tabControl.Dispatcher.BeginInvoke(new System.Action(() =>
            {
                var template = tabControl.Template;
                if (template == null) return;

                var scrollViewer = template.FindName("PART_HeaderScrollViewer", tabControl) as ScrollViewer;
                var leftButton = template.FindName("PART_ScrollLeft", tabControl) as RepeatButton;
                var rightButton = template.FindName("PART_ScrollRight", tabControl) as RepeatButton;

                if (scrollViewer == null || leftButton == null || rightButton == null) return;

                const double scrollOffset = 120;

                leftButton.Click += (_, _) =>
                {
                    scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - scrollOffset);
                };

                rightButton.Click += (_, _) =>
                {
                    scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + scrollOffset);
                };

                scrollViewer.ScrollChanged += (_, _) =>
                {
                    leftButton.Visibility = scrollViewer.HorizontalOffset > 0
                        ? System.Windows.Visibility.Visible
                        : System.Windows.Visibility.Collapsed;
                    rightButton.Visibility = scrollViewer.HorizontalOffset < scrollViewer.ScrollableWidth
                        ? System.Windows.Visibility.Visible
                        : System.Windows.Visibility.Collapsed;
                };

                leftButton.Visibility = System.Windows.Visibility.Collapsed;

                // 设置首页 Tab 的 MinWidth
                ApplyDashboardTabMinWidth(tabControl);
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private void ApplyDashboardTabMinWidth(TabControl tabControl)
        {
            for (int i = 0; i < tabControl.Items.Count; i++)
            {
                if (tabControl.ItemContainerGenerator.ContainerFromIndex(i) is TabItem tabItem
                    && tabItem.Content is FrameworkElement view
                    && (view.DataContext as ITabItemInfo)?.Component == "DashboardView")
                {
                    tabItem.MinWidth = 110;
                }
            }
        }
    }
}