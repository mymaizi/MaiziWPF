using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Prism.Commands;

namespace MaiziWPF.Core
{
    public partial class MultiSelectComboBox : ToggleButton
    {
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(IEnumerable),
                typeof(MultiSelectComboBox),
                new FrameworkPropertyMetadata(null, OnItemsSourceChanged));

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(
                nameof(SelectedItems),
                typeof(ObservableCollection<Checked>),
                typeof(MultiSelectComboBox),
                new FrameworkPropertyMetadata(new ObservableCollection<Checked>(), OnSelectedItemsChanged));

        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register(
                nameof(SearchText),
                typeof(string),
                typeof(MultiSelectComboBox),
                new FrameworkPropertyMetadata(string.Empty, OnSearchTextChanged));

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(
                nameof(Placeholder),
                typeof(string),
                typeof(MultiSelectComboBox),
                new PropertyMetadata("请选择"));

        public static readonly DependencyProperty LoadMoreCommandProperty =
            DependencyProperty.Register(
                nameof(LoadMoreCommand),
                typeof(ICommand),
                typeof(MultiSelectComboBox),
                new PropertyMetadata(null));

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public ObservableCollection<Checked> SelectedItems
        {
            get { return (ObservableCollection<Checked>)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }

        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        public ICommand LoadMoreCommand
        {
            get { return (ICommand)GetValue(LoadMoreCommandProperty); }
            set { SetValue(LoadMoreCommandProperty, value); }
        }

        public ObservableCollection<Checked> FilteredItems { get; } = new ObservableCollection<Checked>();

        public ICommand RemoveItemCommand { get; }

        public MultiSelectComboBox()
        {
            InitializeComponent();
            RemoveItemCommand = new DelegateCommand<Checked>(RemoveItem);
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (MultiSelectComboBox)d;

            if (e.OldValue is INotifyCollectionChanged oldCollection)
                oldCollection.CollectionChanged -= control.OnSourceCollectionChanged;

            if (e.NewValue is INotifyCollectionChanged newCollection)
                newCollection.CollectionChanged += control.OnSourceCollectionChanged;

            control.UpdateFilteredItems();
        }

        private void OnSourceCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateFilteredItems();
        }

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (MultiSelectComboBox)d;

            if (e.OldValue is INotifyCollectionChanged oldCollection)
                oldCollection.CollectionChanged -= control.SelectedItems_CollectionChanged;

            if (e.NewValue is INotifyCollectionChanged newCollection)
                newCollection.CollectionChanged += control.SelectedItems_CollectionChanged;

            control.UpdateListBoxSelection();
        }

        private static void OnSearchTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (MultiSelectComboBox)d;
            control.UpdateFilteredItems();
        }

        private void UpdateFilteredItems()
        {
            FilteredItems.Clear();
            if (ItemsSource == null) return;

            var items = ItemsSource.Cast<Checked>();
            var filtered = string.IsNullOrWhiteSpace(SearchText)
                ? items
                : items.Where(x => x.Name != null && x.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

            foreach (var item in filtered)
            {
                FilteredItems.Add(item);
            }
        }

        private bool _isUpdating;

        private void SelectedItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (_isUpdating) return;
            _isUpdating = true;
            try
            {
                UpdateListBoxSelection();
            }
            finally
            {
                _isUpdating = false;
            }
        }

        private void UpdateListBoxSelection()
        {
            PART_ListBox.SelectionChanged -= ListBox_SelectionChanged;

            PART_ListBox.SelectedItems.Clear();
            foreach (var item in SelectedItems)
            {
                var matched = FilteredItems.FirstOrDefault(x => x.Id == item.Id);
                if (matched != null)
                {
                    PART_ListBox.SelectedItems.Add(matched);
                }
            }

            PART_ListBox.SelectionChanged += ListBox_SelectionChanged;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isUpdating) return;
            _isUpdating = true;
            try
            {
                if (e.AddedItems != null)
                {
                    var addedList = e.AddedItems.Cast<Checked>().ToList();
                    foreach (Checked item in addedList)
                    {
                        if (!SelectedItems.Any(x => x.Id == item.Id))
                        {
                            SelectedItems.Add(item);
                        }
                    }
                }

                if (e.RemovedItems != null)
                {
                    var removedList = e.RemovedItems.Cast<Checked>().ToList();
                    foreach (Checked item in removedList)
                    {
                        var toRemove = SelectedItems.FirstOrDefault(x => x.Id == item.Id);
                        if (toRemove != null)
                        {
                            SelectedItems.Remove(toRemove);
                        }
                    }
                }
            }
            finally
            {
                _isUpdating = false;
            }
        }

        private void RemoveItem(Checked item)
        {
            if (item != null)
            {
                SelectedItems.Remove(item);
            }
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                bool isNearBottom = scrollViewer.VerticalOffset >= scrollViewer.ScrollableHeight - 20;
                if (isNearBottom && LoadMoreCommand != null && LoadMoreCommand.CanExecute(null))
                {
                    LoadMoreCommand.Execute(null);
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_ListBox.SelectionChanged += ListBox_SelectionChanged;
            PART_ListBox.Loaded += (s, e) =>
            {
                var scrollViewer = FindScrollViewer(PART_ListBox);
                if (scrollViewer != null)
                {
                    scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
                }
            };
        }

        private ScrollViewer FindScrollViewer(DependencyObject root)
        {
            if (root is ScrollViewer) return (ScrollViewer)root;

            int childrenCount = VisualTreeHelper.GetChildrenCount(root);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(root, i);
                var result = FindScrollViewer(child);
                if (result != null) return result;
            }

            return null;
        }
    }
}