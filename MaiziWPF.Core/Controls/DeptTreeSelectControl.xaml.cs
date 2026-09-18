using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using Prism.Ioc;

namespace MaiziWPF.Core
{
    public partial class DeptTreeSelectControl : ToggleButton
    {
        public static readonly DependencyProperty SelectedDeptIdProperty =
            DependencyProperty.Register(
                nameof(SelectedDeptId),
                typeof(long),
                typeof(DeptTreeSelectControl),
                new FrameworkPropertyMetadata(0L, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedDeptIdChanged));

        public static readonly DependencyProperty DisplayTextProperty =
            DependencyProperty.Register(
                nameof(DisplayText),
                typeof(string),
                typeof(DeptTreeSelectControl),
                new FrameworkPropertyMetadata(string.Empty));

        internal static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(IEnumerable),
                typeof(DeptTreeSelectControl),
                new FrameworkPropertyMetadata(null, OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (DeptTreeSelectControl)d;

            if (e.OldValue is INotifyCollectionChanged oldCollection)
                oldCollection.CollectionChanged -= control.OnCollectionChanged;

            if (e.NewValue is INotifyCollectionChanged newCollection)
                newCollection.CollectionChanged += control.OnCollectionChanged;

            control.UpdateDisplayText();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateDisplayText();
        }

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(
                nameof(Placeholder),
                typeof(string),
                typeof(DeptTreeSelectControl),
                new PropertyMetadata("请选择"));

        public long SelectedDeptId
        {
            get { return (long)GetValue(SelectedDeptIdProperty); }
            set { SetValue(SelectedDeptIdProperty, value); }
        }

        public string DisplayText
        {
            get { return (string)GetValue(DisplayTextProperty); }
            set { SetValue(DisplayTextProperty, value); }
        }

        private IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        public DeptTreeSelectControl()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            var container = ContainerLocator.Container;
            if (container != null)
            {
                var deptService = container.Resolve<ISysDeptService>();
                var list = deptService.SelectDeptList(new SysDept(), true);
                ItemsSource = list;
            }

            UpdateDisplayText();
        }

        private static void OnSelectedDeptIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (DeptTreeSelectControl)d;
            control.UpdateDisplayText();
        }

        private void UpdateDisplayText()
        {
            if (SelectedDeptId == 0)
            {
                DisplayText = Placeholder;
                return;
            }

            var name = FindDeptNameById(ItemsSource, SelectedDeptId, "DeptName", "Childs");
            DisplayText = name ?? SelectedDeptId.ToString();
        }

        private void PART_TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selected = e.NewValue;
            if (selected == null) return;

            var idProp = selected.GetType().GetProperty("Id");
            var nameProp = selected.GetType().GetProperty("DeptName");

            if (idProp != null)
            {
                var idVal = idProp.GetValue(selected);
                if (idVal is long id)
                {
                    SelectedDeptId = id;
                    IsChecked = false;
                }
            }
        }

        private void ClearSelection_Click(object sender, RoutedEventArgs e)
        {
            SelectedDeptId = 0;
            IsChecked = false;
        }

        private string FindDeptNameById(IEnumerable items, long targetId, string nameProp, string childProp)
        {
            if (items == null) return null;

            foreach (var item in items)
            {
                var idProp = item.GetType().GetProperty("Id");
                if (idProp != null)
                {
                    var idVal = idProp.GetValue(item);
                    if (idVal is long id && id == targetId)
                    {
                        var nProp = item.GetType().GetProperty(nameProp);
                        return nProp?.GetValue(item)?.ToString();
                    }
                }

                var cProp = item.GetType().GetProperty(childProp);
                if (cProp != null)
                {
                    var children = cProp.GetValue(item) as IEnumerable;
                    if (children != null)
                    {
                        var result = FindDeptNameById(children, targetId, nameProp, childProp);
                        if (result != null) return result;
                    }
                }
            }
            return null;
        }
    }
}