using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MaiziWPF.Core
{
    public static class GridViewAssist
    {
        public static readonly DependencyProperty ColumnRatiosProperty =
            DependencyProperty.RegisterAttached(
                "ColumnRatios",
                typeof(string),
                typeof(GridViewAssist),
                new FrameworkPropertyMetadata(string.Empty, OnColumnRatiosChanged));

        public static string GetColumnRatios(DependencyObject obj) =>
            (string)obj.GetValue(ColumnRatiosProperty);

        public static void SetColumnRatios(DependencyObject obj, string value) =>
            obj.SetValue(ColumnRatiosProperty, value);

        private static void OnColumnRatiosChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ListView listView) return;
            if (string.IsNullOrWhiteSpace((string)e.NewValue)) return;

            listView.Loaded += (_, _) => ApplyRatios(listView);
            listView.SizeChanged += (_, _) => ApplyRatios(listView);
        }

        private static void ApplyRatios(ListView listView)
        {
            if (listView.View is not GridView gridView) return;

            var ratiosStr = GetColumnRatios(listView);
            var ratios = ratiosStr
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => double.TryParse(s.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var r) ? r : 0)
                .ToArray();

            if (ratios.Length == 0) return;

            Debug.Assert(Math.Abs(ratios.Sum() - 1.0) < 0.01,
                $"GridViewAssist.ColumnRatios 比例之和应为 1.0，当前为 {ratios.Sum():F3}");

            var totalWidth = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth - 4;
            if (totalWidth <= 0) return;

            var columns = gridView.Columns;
            for (int i = 0; i < columns.Count && i < ratios.Length; i++)
            {
                columns[i].Width = Math.Max(ratios[i] * totalWidth, 40);
            }
        }
    }
}