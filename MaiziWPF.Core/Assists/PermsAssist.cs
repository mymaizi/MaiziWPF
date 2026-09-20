using MaiziWPF.Services.Application.Contracts;
using Prism.Ioc;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MaiziWPF.Core
{
    public static class PermsAssist
    {
        public static readonly DependencyProperty PermsProperty =
            DependencyProperty.RegisterAttached(
                "Perms",
                typeof(string),
                typeof(PermsAssist),
                new FrameworkPropertyMetadata(string.Empty, OnPermsPropertyChanged));

        public static string GetPerms(DependencyObject obj)
        {
            return (string)obj.GetValue(PermsProperty);
        }

        public static void SetPerms(DependencyObject obj, string value)
        {
            obj.SetValue(PermsProperty, value);
        }

        private static void OnPermsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not UIElement element) return;
            if (string.IsNullOrEmpty((string)e.NewValue)) return;

            var container = ContainerLocator.Container;
            if (container == null) return;

            var currentUserService = container.Resolve<ICurrentUserService>();
            if (currentUserService == null) return;

            if (!currentUserService.HasPermission((string)e.NewValue))
            {
                element.Visibility = Visibility.Collapsed;
            }
        }
    }
}