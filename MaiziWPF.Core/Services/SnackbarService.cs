using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace MaiziWPF.Core
{
    public class SnackbarService : ISnackbarService
    {
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;

        public SnackbarService(ISnackbarMessageQueue snackbarMessageQueue)
        {
            _snackbarMessageQueue = snackbarMessageQueue ?? throw new ArgumentNullException(nameof(snackbarMessageQueue));
        }

        public void Enqueue(string message)
        {
            _snackbarMessageQueue.Enqueue(message);
        }

        public void EnqueueInfo(string message)
        {
            _snackbarMessageQueue.Enqueue(BuildContent(message, PackIconKind.InfoCircle, new SolidColorBrush(Color.FromRgb(33, 150, 243))));
        }

        public void EnqueueWarning(string message)
        {
            _snackbarMessageQueue.Enqueue(BuildContent(message, PackIconKind.Warning, new SolidColorBrush(Color.FromRgb(255, 152, 0))));
        }

        public void EnqueueError(string message)
        {
            _snackbarMessageQueue.Enqueue(BuildContent(message, PackIconKind.Error, new SolidColorBrush(Color.FromRgb(244, 67, 54))));
        }

        public void EnqueueSuccess(string message)
        {
            _snackbarMessageQueue.Enqueue(BuildContent(message, PackIconKind.CheckCircle, new SolidColorBrush(Color.FromRgb(76, 175, 80))));
        }

        private static object BuildContent(string message, PackIconKind iconKind, SolidColorBrush iconColor)
        {
            var panel = new StackPanel { Orientation = Orientation.Horizontal };
            panel.Children.Add(new PackIcon
            {
                Kind = iconKind,
                Width = 20,
                Height = 20,
                Margin = new Thickness(0, 0, 8, 0),
                Foreground = iconColor,
                VerticalAlignment = VerticalAlignment.Center
            });
            panel.Children.Add(new TextBlock
            {
                Text = message,
                VerticalAlignment = VerticalAlignment.Center
            });
            return panel;
        }
    }
}