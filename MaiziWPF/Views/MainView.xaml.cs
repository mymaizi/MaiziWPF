using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
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
    }
}