using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MaiziWPF.Core
{
    public partial class UserSelectControl : ToggleButton
    {
        public static readonly DependencyProperty SelectedUserIdProperty =
            DependencyProperty.Register(
                nameof(SelectedUserId),
                typeof(long),
                typeof(UserSelectControl),
                new FrameworkPropertyMetadata(0L, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedUserIdChanged));

        public static readonly DependencyProperty DisplayTextProperty =
            DependencyProperty.Register(
                nameof(DisplayText),
                typeof(string),
                typeof(UserSelectControl),
                new FrameworkPropertyMetadata(string.Empty));

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(IEnumerable),
                typeof(UserSelectControl),
                new FrameworkPropertyMetadata(null, OnItemsSourceChanged));

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(
                nameof(Placeholder),
                typeof(string),
                typeof(UserSelectControl),
                new PropertyMetadata("请选择"));

        public static readonly DependencyProperty FilteredUsersProperty =
            DependencyProperty.Register(
                nameof(FilteredUsers),
                typeof(ObservableCollection<object>),
                typeof(UserSelectControl),
                new PropertyMetadata(null));

        public static readonly DependencyProperty ResultCountTextProperty =
            DependencyProperty.Register(
                nameof(ResultCountText),
                typeof(string),
                typeof(UserSelectControl),
                new PropertyMetadata(string.Empty));

        public long SelectedUserId
        {
            get { return (long)GetValue(SelectedUserIdProperty); }
            set { SetValue(SelectedUserIdProperty, value); }
        }

        public string DisplayText
        {
            get { return (string)GetValue(DisplayTextProperty); }
            set { SetValue(DisplayTextProperty, value); }
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        public ObservableCollection<object> FilteredUsers
        {
            get { return (ObservableCollection<object>)GetValue(FilteredUsersProperty); }
            set { SetValue(FilteredUsersProperty, value); }
        }

        public string ResultCountText
        {
            get { return (string)GetValue(ResultCountTextProperty); }
            set { SetValue(ResultCountTextProperty, value); }
        }

        public UserSelectControl()
        {
            InitializeComponent();
            FilteredUsers = new ObservableCollection<object>();
            UpdateDisplayText();
        }

        private static void OnSelectedUserIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (UserSelectControl)d;
            control.UpdateDisplayText();
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (UserSelectControl)d;

            if (e.OldValue is INotifyCollectionChanged oldCollection)
                oldCollection.CollectionChanged -= control.OnSourceCollectionChanged;

            if (e.NewValue is INotifyCollectionChanged newCollection)
                newCollection.CollectionChanged += control.OnSourceCollectionChanged;

            control.RebuildFilteredList();
            control.UpdateDisplayText();
        }

        private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RebuildFilteredList();
            UpdateDisplayText();
        }

        private void UpdateDisplayText()
        {
            if (SelectedUserId == 0)
            {
                DisplayText = Placeholder;
                return;
            }

            var name = FindUserNameById(ItemsSource, SelectedUserId);
            DisplayText = name ?? SelectedUserId.ToString();
        }

        private void RebuildFilteredList()
        {
            var searchText = PART_SearchBox?.Text ?? string.Empty;
            ApplyFilter(searchText);
        }

        private void ApplyFilter(string searchText)
        {
            FilteredUsers.Clear();

            if (ItemsSource == null)
            {
                ResultCountText = string.Empty;
                return;
            }

            int total = 0;

            foreach (var item in ItemsSource)
            {
                total++;
                if (string.IsNullOrWhiteSpace(searchText))
                {
                    FilteredUsers.Add(item);
                }
                else
                {
                    var name = GetPropertyValue(item, "NickName");
                    var userName = GetPropertyValue(item, "UserName");
                    var match = (name != null && name.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                        || (userName != null && userName.Contains(searchText, StringComparison.OrdinalIgnoreCase));

                    if (match)
                        FilteredUsers.Add(item);
                }
            }

            if (string.IsNullOrWhiteSpace(searchText))
                ResultCountText = $"共 {FilteredUsers.Count} 人";
            else
                ResultCountText = $"匹配 {FilteredUsers.Count} / {total} 人";
        }

        private string FindUserNameById(IEnumerable source, long userId)
        {
            if (source == null) return null;

            foreach (var item in source)
            {
                var id = GetPropertyValue(item, "UserId");
                if (id != null && long.TryParse(id, out long uid) && uid == userId)
                {
                    return GetPropertyValue(item, "NickName");
                }
            }
            return null;
        }

        private static string GetPropertyValue(object obj, string propName)
        {
            var prop = obj.GetType().GetProperty(propName);
            return prop?.GetValue(obj)?.ToString();
        }

        private void PART_SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter(PART_SearchBox.Text);
        }

        private void PART_UserList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PART_UserList.SelectedItem == null) return;

            var selected = PART_UserList.SelectedItem;
            var idVal = GetPropertyValue(selected, "UserId");
            if (idVal != null && long.TryParse(idVal, out long uid))
            {
                SelectedUserId = uid;
                IsChecked = false;
            }
        }

        private void ClearSelection_Click(object sender, RoutedEventArgs e)
        {
            SelectedUserId = 0;
            IsChecked = false;
        }
    }
}