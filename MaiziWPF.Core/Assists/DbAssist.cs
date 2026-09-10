using MaiziWPF.Common;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MaiziWPF.Core
{
    public class DbAssist
    {

        public static readonly DependencyProperty BindProperty =
            DependencyProperty.RegisterAttached("Bind", typeof(String), typeof(DbAssist), new FrameworkPropertyMetadata(OnBindPropertyChanged));

        public static string GetBind(DependencyObject obj)
        {
            return (string)obj.GetValue(BindProperty);
        }

        public static void SetBind(DependencyObject obj, string value)
        {
            obj.SetValue(BindProperty, value);
        }

        public static List<T> GetBindField<T>(DependencyObject obj)
        {
            return (List<T>)obj.GetValue(BindFieldProperty);
        }

        public static void SetBindField<T>(DependencyObject obj, List<T> value)
        {
            obj.SetValue(BindFieldProperty, value);
        }

        public static readonly DependencyProperty BindFieldProperty =
            DependencyProperty.RegisterAttached("BindField", typeof(object), typeof(DbAssist));

        public static readonly DependencyProperty InitialSelectedIdsProperty =
            DependencyProperty.RegisterAttached("InitialSelectedIds", typeof(List<long>), typeof(DbAssist),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));

        public static List<long> GetInitialSelectedIds(DependencyObject obj)
        {
            return (List<long>)obj.GetValue(InitialSelectedIdsProperty);
        }

        public static void SetInitialSelectedIds(DependencyObject obj, List<long> value)
        {
            obj.SetValue(InitialSelectedIdsProperty, value);
        }

        private static void OnBindPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            List<Checked> datas = new();
            var container = ContainerLocator.Container;
            if (container != null)
            {
                if ((string)e.NewValue == "post" || (string)e.NewValue == "role")
                {

                    if ((string)e.NewValue == "post")
                    {
                        var service = container.Resolve<ISysPostService>();
                        datas = service.SelectPostList(new QueryPostInput() { PageNumber = 1, PageSize = 100 }).Select(a => new Checked { IsSelected = false, Name = a.PostName, Id = a.PostId }).ToList();
                    }
                    else if ((string)e.NewValue == "role")
                    {
                        var service = container.Resolve<ISysRoleService>();
                        datas = service.SelectRoleList(new QueryRoleInput() { PageNumber = 1, PageSize = 100 }).Select(a => new Checked { IsSelected = false, Name = a.RoleName, Id = a.RoleId }).ToList();
                    }
                    foreach (var item in datas)
                    {
                        item.PropertyChanged += (s, e) =>
                        {
                            if (e.PropertyName == nameof(Checked.IsSelected))
                            {
                                var selected = datas.Where(p => p.IsSelected);
                                if (d is ComboBox cb)
                                {
                                    cb.Text = selected.Select(p => p.Name).JoinAsString(",");
                                    SetBindField(cb, selected.ToList());
                                }
                            }
                        };
                    }
                }
                else if ((string)e.NewValue == "dept")
                {
                    var service = container.Resolve<ISysDeptService>();
                    var depts = service.SelectDeptList(new SysDept(), false).Select(a => new Checked { IsSelected = false, Name = a.DeptName, Id = a.Id, ParentId = a.ParentId, Childs = new System.Collections.ObjectModel.ObservableCollection<Checked>() }).ToList();
                    foreach (var item in depts)
                    {
                        item.PropertyChanged += (s, e) =>
                        {
                            if (e.PropertyName == nameof(Checked.IsSelected))
                            {
                                var selected = depts.Where(p => p.IsSelected);
                                if (d is ComboBox cb)
                                {
                                    if (cb.Template.FindName("PART_TEXT", cb) is TextBox textBox)
                                    {
                                        textBox.Text = selected.Select(p => p.Name).JoinAsString(","); ;
                                    }
                                    SetBindField(cb, selected.ToList());
                                }
                            }
                        };
                    }
                    datas = depts.BuildTreeList(a => (int)a.Id, a => (int)a.ParentId, (p, c) => p.Childs.Add(c));
                }
            }
            if (d is ComboBox cb)
            {
                cb.ItemsSource = datas;
                cb.Loaded += Cb_Loaded;
            }
        }

        private static void Cb_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ComboBox cb)
            {
                cb.Loaded -= Cb_Loaded;
                var initialIds = GetInitialSelectedIds(cb);
                if (initialIds != null && initialIds.Any())
                {
                    SetInitialSelections(cb, initialIds);
                }
            }
        }

        private static void SetInitialSelections(ComboBox cb, List<long> selectedIds)
        {
            if (cb.ItemsSource is IList<Checked> items)
            {
                var selectedItems = new List<Checked>();
                foreach (var item in items)
                {
                    if (selectedIds.Contains(item.Id))
                    {
                        item.IsSelected = true;
                        selectedItems.Add(item);
                    }
                    if (item.Childs != null)
                    {
                        SetTreeInitialSelections(item.Childs, selectedIds, selectedItems);
                    }
                }
                if (selectedItems.Any())
                {
                    cb.Text = selectedItems.Select(p => p.Name).JoinAsString(",");
                    SetBindField(cb, selectedItems);
                }
            }
        }

        private static void SetTreeInitialSelections(ObservableCollection<Checked> childs, List<long> selectedIds, List<Checked> selectedItems)
        {
            foreach (var child in childs)
            {
                if (selectedIds.Contains(child.Id))
                {
                    child.IsSelected = true;
                    selectedItems.Add(child);
                }
                if (child.Childs != null)
                {
                    SetTreeInitialSelections(child.Childs, selectedIds, selectedItems);
                }
            }
        }
    }
}