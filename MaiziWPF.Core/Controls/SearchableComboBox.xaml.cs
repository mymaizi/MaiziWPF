using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace MaiziWPF.Core
{
    public partial class SearchableComboBox : ToggleButton, INotifyPropertyChanged
    {
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(IEnumerable),
                typeof(SearchableComboBox),
                new FrameworkPropertyMetadata(null, OnItemsSourceChanged));

        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register(
                nameof(SelectedValue),
                typeof(object),
                typeof(SearchableComboBox),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedValueChanged));

        public static readonly DependencyProperty SelectedValuePathProperty =
            DependencyProperty.Register(
                nameof(SelectedValuePath),
                typeof(string),
                typeof(SearchableComboBox),
                new PropertyMetadata("Id"));

        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register(
                nameof(DisplayMemberPath),
                typeof(string),
                typeof(SearchableComboBox),
                new PropertyMetadata("Name"));

        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register(
                nameof(SearchText),
                typeof(string),
                typeof(SearchableComboBox),
                new FrameworkPropertyMetadata(string.Empty, OnSearchTextChanged));

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(
                nameof(Placeholder),
                typeof(string),
                typeof(SearchableComboBox),
                new PropertyMetadata("请选择"));

        public static readonly DependencyProperty LoadMoreCommandProperty =
            DependencyProperty.Register(
                nameof(LoadMoreCommand),
                typeof(ICommand),
                typeof(SearchableComboBox),
                new PropertyMetadata(null));

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public object SelectedValue
        {
            get { return GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        public string SelectedValuePath
        {
            get { return (string)GetValue(SelectedValuePathProperty); }
            set { SetValue(SelectedValuePathProperty, value); }
        }

        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
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

        private string _selectedText;
        public string SelectedText
        {
            get { return _selectedText; }
            set
            {
                _selectedText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedText)));
            }
        }

        private bool _hasSelection;
        public bool HasSelection
        {
            get { return _hasSelection; }
            set
            {
                _hasSelection = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasSelection)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public SearchableComboBox()
        {
            InitializeComponent();
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (SearchableComboBox)d;

            if (e.OldValue is INotifyCollectionChanged oldCollection)
                oldCollection.CollectionChanged -= control.OnSourceCollectionChanged;

            if (e.NewValue is INotifyCollectionChanged newCollection)
                newCollection.CollectionChanged += control.OnSourceCollectionChanged;

            control.UpdateFilteredItems();
            control.RefreshSelectedText();
        }

        private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateFilteredItems();
            RefreshSelectedText();
        }

        private static void OnSearchTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (SearchableComboBox)d;
            control.UpdateFilteredItems();
        }

        private static void OnSelectedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (SearchableComboBox)d;
            control.RefreshSelectedText();
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

        private void RefreshSelectedText()
        {
            var items = ItemsSource?.Cast<Checked>();
            if (items != null && SelectedValue != null)
            {
                var selected = items.FirstOrDefault(x =>
                {
                    var prop = x.GetType().GetProperty(SelectedValuePath);
                    if (prop == null) return false;
                    var val = prop.GetValue(x);
                    return val != null && val.Equals(SelectedValue);
                });
                SelectedText = selected?.Name;
                HasSelection = selected != null;
            }
            else
            {
                SelectedText = null;
                HasSelection = false;
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PART_ListBox.SelectedItem is Checked selectedItem)
            {
                var prop = selectedItem.GetType().GetProperty(SelectedValuePath);
                if (prop != null)
                {
                    SelectedValue = prop.GetValue(selectedItem);
                }

                IsChecked = false;
                SearchText = string.Empty;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_ListBox.SelectionChanged += ListBox_SelectionChanged;
            PART_ListBox.Loaded += (s, e) =>
            {
                if (PART_ListBox.Template.FindName("ScrollViewer", PART_ListBox) is ScrollViewer scrollViewer)
                {
                    scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
                }
            };
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
    }
}