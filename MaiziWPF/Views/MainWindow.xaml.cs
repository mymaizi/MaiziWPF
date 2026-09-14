using MaterialDesignThemes.Wpf;
using System;
using System.Windows;

namespace MaiziWPF.Views
{
    public partial class MainWindow : Window
    {
        public static SnackbarMessageQueue SnackbarMessageQueue { get; private set; } = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));

        public MainWindow()
        {
            InitializeComponent();
            MainSnackbar.MessageQueue = SnackbarMessageQueue;
        }
    }
}