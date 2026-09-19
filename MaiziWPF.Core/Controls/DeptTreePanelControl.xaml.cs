using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using Prism.Ioc;

namespace MaiziWPF.Core
{
    public partial class DeptTreePanelControl : UserControl
    {
        private List<SysDept> _allDepts = new();

        public static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register(
                nameof(HeaderText),
                typeof(string),
                typeof(DeptTreePanelControl),
                new PropertyMetadata("部门结构"));

        public string HeaderText
        {
            get => (string)GetValue(HeaderTextProperty);
            set => SetValue(HeaderTextProperty, value);
        }

        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register(
                nameof(IsExpanded),
                typeof(bool),
                typeof(DeptTreePanelControl),
                new PropertyMetadata(true));

        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register(
                nameof(SearchText),
                typeof(string),
                typeof(DeptTreePanelControl),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSearchTextChanged));

        public string SearchText
        {
            get => (string)GetValue(SearchTextProperty);
            set => SetValue(SearchTextProperty, value);
        }

        private static void OnSearchTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (DeptTreePanelControl)d;
            control.ApplyFilter((string)e.NewValue);
        }

        public static readonly DependencyProperty SelectedDeptIdProperty =
            DependencyProperty.Register(
                nameof(SelectedDeptId),
                typeof(long),
                typeof(DeptTreePanelControl),
                new FrameworkPropertyMetadata(0L, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public long SelectedDeptId
        {
            get => (long)GetValue(SelectedDeptIdProperty);
            set => SetValue(SelectedDeptIdProperty, value);
        }

        internal static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(IEnumerable),
                typeof(DeptTreePanelControl),
                new PropertyMetadata(null));

        private IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public DeptTreePanelControl()
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
                _allDepts = deptService.SelectDeptList(new SysDept(), true);
                ItemsSource = _allDepts;
            }
        }

        private void ApplyFilter(string keyword)
        {
            keyword = keyword?.Trim();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                ItemsSource = _allDepts;
            }
            else
            {
                var filtered = FilterTree(_allDepts, keyword);
                ItemsSource = filtered;
            }
        }

        private static List<SysDept> FilterTree(List<SysDept> source, string keyword)
        {
            if (source == null) return new List<SysDept>();

            var result = new List<SysDept>();
            foreach (var dept in source)
            {
                var filteredChilds = FilterTree(dept.Childs, keyword);
                var selfMatch = dept.DeptName != null
                    && dept.DeptName.Contains(keyword, System.StringComparison.OrdinalIgnoreCase);

                if (selfMatch || filteredChilds.Count > 0)
                {
                    var clone = CloneDept(dept);
                    clone.Childs = filteredChilds;
                    result.Add(clone);
                }
            }
            return result;
        }

        private static SysDept CloneDept(SysDept source)
        {
            return new SysDept
            {
                Id = source.Id,
                ParentId = source.ParentId,
                DeptName = source.DeptName,
                DeptCategory = source.DeptCategory,
                OrderNum = source.OrderNum,
                Leader = source.Leader,
                Phone = source.Phone,
                Email = source.Email,
                Status = source.Status,
                Ancestors = source.Ancestors,
                Childs = source.Childs
            };
        }

        private void PART_TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue == null) return;

            var idProp = e.NewValue.GetType().GetProperty("Id");
            if (idProp != null && idProp.GetValue(e.NewValue) is long id)
            {
                SelectedDeptId = id;
            }
        }
    }
}