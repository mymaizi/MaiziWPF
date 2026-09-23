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
        private System.Windows.Threading.DispatcherTimer _messageCloseTimer;
        private bool _isMouseInPopup;
        private bool _isMouseInMessagePopup;

        public MainView()
        {
            InitializeComponent();
            _closeTimer = new System.Windows.Threading.DispatcherTimer();
            _closeTimer.Interval = System.TimeSpan.FromMilliseconds(150);
            _closeTimer.Tick += CloseTimer_Tick;
            _messageCloseTimer = new System.Windows.Threading.DispatcherTimer();
            _messageCloseTimer.Interval = System.TimeSpan.FromMilliseconds(150);
            _messageCloseTimer.Tick += MessageCloseTimer_Tick;
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
            var popup = sender as Popup;
            if (popup?.Child is FrameworkElement child)
            {
                DependencyObject current = e.OriginalSource as DependencyObject;
                while (current != null)
                {
                    if (current == child)
                    {
                        return;
                    }
                    current = VisualTreeHelper.GetParent(current);
                }
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

        private void MessageIconBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            _messageCloseTimer.Stop();
            _isMouseInMessagePopup = false;
            if (DataContext is MainViewModel vm)
            {
                vm.IsMessagePopupOpen = true;
            }
        }

        private void MessageIconBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!_isMouseInMessagePopup)
            {
                _messageCloseTimer.Start();
            }
        }

        private void MessagePopup_MouseEnter(object sender, MouseEventArgs e)
        {
            _messageCloseTimer.Stop();
            _isMouseInMessagePopup = true;
        }

        private void MessagePopup_MouseLeave(object sender, MouseEventArgs e)
        {
            _isMouseInMessagePopup = false;
            _messageCloseTimer.Start();
        }

        private void MessagePopup_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var popup = sender as Popup;
            if (popup?.Child is FrameworkElement child)
            {
                DependencyObject current = e.OriginalSource as DependencyObject;
                while (current != null)
                {
                    if (current == child)
                    {
                        return;
                    }
                    current = VisualTreeHelper.GetParent(current);
                }
            }
            if (DataContext is MainViewModel vm)
            {
                vm.IsMessagePopupOpen = false;
            }
        }

        private void MessageCloseTimer_Tick(object sender, object e)
        {
            _messageCloseTimer.Stop();
            if (!_isMouseInMessagePopup && DataContext is MainViewModel vm)
            {
                vm.IsMessagePopupOpen = false;
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