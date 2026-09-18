using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MaiziWPF.Core
{
    public partial class PageControl : UserControl
    {
        public static readonly DependencyProperty PrevCommandProperty =
        DependencyProperty.Register(
           "PrevCommand",
           typeof(ICommand),
           typeof(PageControl),
           new FrameworkPropertyMetadata(null));

        public ICommand PrevCommand
        {
            get => (ICommand)GetValue(PrevCommandProperty);
            set => SetValue(PrevCommandProperty, value);
        }

        public static readonly DependencyProperty NextCommandProperty =
        DependencyProperty.Register(
           "NextCommand",
           typeof(ICommand),
           typeof(PageControl),
           new FrameworkPropertyMetadata(null));

        public ICommand NextCommand
        {
            get => (ICommand)GetValue(NextCommandProperty);
            set => SetValue(NextCommandProperty, value);
        }

        public static readonly DependencyProperty PageNumberProperty =
          DependencyProperty.Register(
          "PageNumber",
          typeof(int),
          typeof(PageControl),
          new FrameworkPropertyMetadata(0, OnPageNumberPropertyChanged));

        public int PageNumber
        {
            get => (int)GetValue(PageNumberProperty);
            set => SetValue(PageNumberProperty, value);
        }

        public static readonly DependencyProperty PageSizeProperty =
           DependencyProperty.Register(
           "PageSize",
           typeof(int),
           typeof(PageControl),
           new FrameworkPropertyMetadata(0, OnPageSizePropertyChanged));

        public int PageSize
        {
            get => (int)GetValue(PageSizeProperty);
            set => SetValue(PageSizeProperty, value);
        }

        public static readonly DependencyProperty CountProperty =
           DependencyProperty.Register(
           "Count",
           typeof(long),
           typeof(PageControl),
           new FrameworkPropertyMetadata(0L, OnCountPropertyChanged));

        public long Count
        {
            get => (long)GetValue(CountProperty);
            set => SetValue(CountProperty, value);
        }

        public static readonly DependencyProperty PageSizeItemsProperty =
            DependencyProperty.Register(
                "PageSizeItems",
                typeof(IEnumerable<int>),
                typeof(PageControl),
                new FrameworkPropertyMetadata(new List<int> { 10, 20, 50, 100 }));

        public IEnumerable<int> PageSizeItems
        {
            get => (IEnumerable<int>)GetValue(PageSizeItemsProperty);
            set => SetValue(PageSizeItemsProperty, value);
        }

        public static readonly DependencyProperty PageSizeChangedCommandProperty =
            DependencyProperty.Register(
                "PageSizeChangedCommand",
                typeof(ICommand),
                typeof(PageControl),
                new FrameworkPropertyMetadata(null));

        public ICommand PageSizeChangedCommand
        {
            get => (ICommand)GetValue(PageSizeChangedCommandProperty);
            set => SetValue(PageSizeChangedCommandProperty, value);
        }

        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.Register(
                "IsBusy",
                typeof(bool),
                typeof(PageControl),
                new FrameworkPropertyMetadata(false));

        public bool IsBusy
        {
            get => (bool)GetValue(IsBusyProperty);
            set => SetValue(IsBusyProperty, value);
        }

        public PageControl()
        {
            InitializeComponent();
            PageSizeCombo.ItemsSource = PageSizeItems;
        }

        private static void OnPageNumberPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SetPageInfo(d);
        }

        private static void OnPageSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PageControl)d;
            if (control.PageSizeCombo == null) return;

            var pageSize = (int)e.NewValue;
            if (pageSize <= 0) return;

            var items = (List<int>)control.PageSizeItems;
            if (!items.Contains(pageSize))
                items.AddFirst(pageSize);

            control.PageSizeCombo.SelectedItem = pageSize;
            SetPageInfo(d);
        }

        private static void OnCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SetPageInfo(d);
        }

        private static void SetPageInfo(DependencyObject d)
        {
            PageControl control = (PageControl)d;
            if (control.PageSize <= 0) return;

            var totalPage = (int)Math.Ceiling((double)control.Count / control.PageSize);

            if (control.FindName("TotalText") is TextBlock totalText)
                totalText.Text = $"共 {control.Count} 条";

            if (control.FindName("CurrentText") is TextBlock currentText)
                currentText.Text = $"第 {control.PageNumber}/{totalPage} 页";

            if (control.FindName("PrevButton") is Button prevBtn)
                prevBtn.IsEnabled = control.PageNumber > 1;

            if (control.FindName("NextButton") is Button nextBtn)
                nextBtn.IsEnabled = control.PageNumber < totalPage;
        }

        private void PageSizeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PageSizeCombo.SelectedItem is int selectedSize && selectedSize != PageSize)
            {
                PageSize = selectedSize;
                PageSizeChangedCommand?.Execute(selectedSize);
            }
        }
    }
}