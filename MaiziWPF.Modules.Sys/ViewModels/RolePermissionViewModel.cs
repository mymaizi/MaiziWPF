using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MaiziWPF.Modules.Sys
{
    public class RolePermissionViewModel : FormBindableBase
    {
        private readonly ISysRoleService _roleService;
        private readonly ISysMenuService _menuService;
        private readonly ISysDeptService _deptService;

        private long _roleId;
        public long RoleId
        {
            get { return _roleId; }
            set { SetProperty(ref _roleId, value); }
        }

        private string _roleName;
        public string RoleName
        {
            get { return _roleName; }
            set { SetProperty(ref _roleName, value); }
        }

        private string _roleKey;
        public string RoleKey
        {
            get { return _roleKey; }
            set { SetProperty(ref _roleKey, value); }
        }

        private string _dataScope = "1";
        public string DataScope
        {
            get { return _dataScope; }
            set
            {
                if (SetProperty(ref _dataScope, value))
                {
                    IsCustomDataScope = value == "2";
                }
            }
        }

        private bool _isCustomDataScope;
        public bool IsCustomDataScope
        {
            get { return _isCustomDataScope; }
            set { SetProperty(ref _isCustomDataScope, value); }
        }

        private bool _menuCheckStrictly = true;
        public bool MenuCheckStrictly
        {
            get { return _menuCheckStrictly; }
            set
            {
                if (SetProperty(ref _menuCheckStrictly, value))
                {
                    foreach (var node in MenuTreeItems)
                        node.SetStrictMode(value);
                }
            }
        }

        private bool _deptCheckStrictly = true;
        public bool DeptCheckStrictly
        {
            get { return _deptCheckStrictly; }
            set
            {
                if (SetProperty(ref _deptCheckStrictly, value))
                {
                    foreach (var node in DeptTreeItems)
                        node.SetStrictMode(value);
                }
            }
        }

        public ObservableCollection<MenuItemNode> MenuTreeItems { get; set; } = new();
        public ObservableCollection<DeptNode> DeptTreeItems { get; set; } = new();

        public ObservableCollection<DataScopeItem> DataScopeOptions { get; } = new()
        {
            new() { Label = "全部数据权限", Value = "1" },
            new() { Label = "自定义数据权限", Value = "2" },
            new() { Label = "本部门数据权限", Value = "3" },
            new() { Label = "本部门及以下数据权限", Value = "4" },
            new() { Label = "仅本人数据权限", Value = "5" }
        };

        public RolePermissionViewModel(ISysRoleService roleService, ISysMenuService menuService, ISysDeptService deptService, ISnackbarService snackbarService)
            : base(snackbarService)
        {
            _roleService = roleService;
            _menuService = menuService;
            _deptService = deptService;

            AcceptCommand = new DelegateCommand(() =>
            {
                SavePermission();
            });
        }

        public void LoadData()
        {
            LoadMenuTree();
            LoadDeptTree();
            LoadCheckedData();
        }

        private void LoadMenuTree()
        {
            var menus = _menuService.SelectMenuList(new SysMenu(), 1);
            var rootMenus = menus.Where(m => m.ParentId == 0).OrderBy(m => m.OrderNum).ToList();
            MenuTreeItems.Clear();
            foreach (var menu in rootMenus)
            {
                var node = BuildMenuNode(menu, menus);
                node.SetStrictMode(MenuCheckStrictly);
                MenuTreeItems.Add(node);
            }
        }

        private MenuItemNode BuildMenuNode(SysMenu menu, List<SysMenu> allMenus)
        {
            var node = new MenuItemNode
            {
                Id = menu.Id,
                Name = menu.MenuName,
                MenuType = menu.MenuType,
                Status = menu.Status,
                IsChecked = false
            };

            var children = allMenus.Where(m => m.ParentId == menu.Id).OrderBy(m => m.OrderNum).ToList();
            foreach (var child in children)
            {
                var childNode = BuildMenuNode(child, allMenus);
                childNode.Parent = node;
                node.Children.Add(childNode);
            }
            return node;
        }

        private void LoadDeptTree()
        {
            var depts = _deptService.SelectDeptList(new SysDept(), false);
            var rootDepts = depts.Where(d => d.ParentId == 0).OrderBy(d => d.OrderNum).ToList();
            DeptTreeItems.Clear();
            foreach (var dept in rootDepts)
            {
                var node = BuildDeptNode(dept, depts);
                node.SetStrictMode(DeptCheckStrictly);
                DeptTreeItems.Add(node);
            }
        }

        private DeptNode BuildDeptNode(SysDept dept, List<SysDept> allDepts)
        {
            var node = new DeptNode
            {
                Id = dept.Id,
                DeptName = dept.DeptName,
                Status = dept.Status,
                IsChecked = false
            };

            var children = allDepts.Where(d => d.ParentId == dept.Id).OrderBy(d => d.OrderNum).ToList();
            foreach (var child in children)
            {
                var childNode = BuildDeptNode(child, allDepts);
                childNode.Parent = node;
                node.Children.Add(childNode);
            }
            return node;
        }

        private void LoadCheckedData()
        {
            if (RoleId == 0) return;

            var checkedMenuIds = _roleService.SelectRoleMenuIds(RoleId);
            var checkedDeptIds = _roleService.SelectRoleDeptIds(RoleId);

            SetMenuCheckedByIds(MenuTreeItems, checkedMenuIds);
            SetDeptCheckedByIds(DeptTreeItems, checkedDeptIds);
        }

        private void SetMenuCheckedByIds(ObservableCollection<MenuItemNode> nodes, List<long> checkedIds)
        {
            foreach (var node in nodes)
            {
                node.IsChecked = checkedIds.Contains(node.Id);
                SetMenuCheckedByIds(node.Children, checkedIds);
            }
        }

        private void SetDeptCheckedByIds(ObservableCollection<DeptNode> nodes, List<long> checkedIds)
        {
            foreach (var node in nodes)
            {
                node.IsChecked = checkedIds.Contains(node.Id);
                SetDeptCheckedByIds(node.Children, checkedIds);
            }
        }

        private List<long> GetCheckedMenuIds()
        {
            var ids = new List<long>();
            CollectCheckedMenuIds(MenuTreeItems, ids);
            return ids;
        }

        private void CollectCheckedMenuIds(ObservableCollection<MenuItemNode> nodes, List<long> ids)
        {
            foreach (var node in nodes)
            {
                if (node.IsChecked)
                    ids.Add(node.Id);
                CollectCheckedMenuIds(node.Children, ids);
            }
        }

        private List<long> GetCheckedDeptIds()
        {
            var ids = new List<long>();
            CollectCheckedDeptIds(DeptTreeItems, ids);
            return ids;
        }

        private void CollectCheckedDeptIds(ObservableCollection<DeptNode> nodes, List<long> ids)
        {
            foreach (var node in nodes)
            {
                if (node.IsChecked)
                    ids.Add(node.Id);
                CollectCheckedDeptIds(node.Children, ids);
            }
        }

        private async void SavePermission()
        {
            try
            {
                var role = new SysRole
                {
                    RoleId = RoleId,
                    DataScope = DataScope,
                    MenuCheckStrictly = MenuCheckStrictly,
                    DeptCheckStrictly = DeptCheckStrictly
                };

                var menuIds = GetCheckedMenuIds().ToArray();
                var deptIds = GetCheckedDeptIds().ToArray();

                _roleService.UpdateRolePermission(role, menuIds, deptIds);

                OnSaveSuccessCallback?.Invoke();
                ShowSuccess("权限分配成功");
                CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError($"权限分配失败：{ex.Message}");
            }
        }
    }

    public class TreeNodeBase : BindableBase
    {
        protected bool _isStrict = true;
        protected bool _isUpdating;

        public virtual void SetStrictMode(bool isStrict)
        {
            _isStrict = isStrict;
            SetChildrenStrictMode(isStrict);
        }

        protected virtual void SetChildrenStrictMode(bool isStrict) { }

        protected void CascadeCheckToChildren()
        {
            if (_isUpdating) return;
        }

        protected void NotifyParentCheckState()
        {
            if (_isUpdating) return;
        }
    }

    public class MenuItemNode : TreeNodeBase
    {
        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (_isUpdating) return;
                if (SetProperty(ref _isChecked, value))
                {
                    RaisePropertyChanged(nameof(IsEnabled));
                    if (!_isStrict)
                    {
                        _isUpdating = true;
                        CascadeToChildren(value);
                        if (value)
                            CascadeCheckToParent();
                        else
                            UncheckParentIfNoSiblingChecked();
                        _isUpdating = false;
                    }
                }
            }
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string MenuType { get; set; }
        public string Status { get; set; }
        public bool IsEnabled => Status == "0";
        public MenuItemNode Parent { get; set; }
        public ObservableCollection<MenuItemNode> Children { get; set; } = new();

        public override void SetStrictMode(bool isStrict)
        {
            _isStrict = isStrict;
            SetChildrenStrictMode(isStrict);
        }

        protected override void SetChildrenStrictMode(bool isStrict)
        {
            foreach (var child in Children)
            {
                child._isStrict = isStrict;
                child.SetChildrenStrictMode(isStrict);
            }
        }

        private void CascadeToChildren(bool check)
        {
            foreach (var child in Children)
            {
                child.IsChecked = check;
            }
        }

        private void CascadeCheckToParent()
        {
            if (Parent != null)
            {
                Parent.IsChecked = true;
            }
        }

        private void UncheckParentIfNoSiblingChecked()
        {
            if (Parent != null && Parent.Children.All(c => !c._isChecked))
            {
                Parent.IsChecked = false;
            }
        }
    }

    public class DeptNode : TreeNodeBase
    {
        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (_isUpdating) return;
                if (SetProperty(ref _isChecked, value))
                {
                    if (!_isStrict)
                    {
                        _isUpdating = true;
                        CascadeToChildren(value);
                        if (value)
                            CascadeCheckToParent();
                        else
                            UncheckParentIfNoSiblingChecked();
                        _isUpdating = false;
                    }
                }
            }
        }

        public long Id { get; set; }
        public string DeptName { get; set; }
        public string Status { get; set; }
        public DeptNode Parent { get; set; }
        public ObservableCollection<DeptNode> Children { get; set; } = new();

        public override void SetStrictMode(bool isStrict)
        {
            _isStrict = isStrict;
            SetChildrenStrictMode(isStrict);
        }

        protected override void SetChildrenStrictMode(bool isStrict)
        {
            foreach (var child in Children)
            {
                child._isStrict = isStrict;
                child.SetChildrenStrictMode(isStrict);
            }
        }

        private void CascadeToChildren(bool check)
        {
            foreach (var child in Children)
            {
                child.IsChecked = check;
            }
        }

        private void CascadeCheckToParent()
        {
            if (Parent != null)
            {
                Parent.IsChecked = true;
            }
        }

        private void UncheckParentIfNoSiblingChecked()
        {
            if (Parent != null && Parent.Children.All(c => !c._isChecked))
            {
                Parent.IsChecked = false;
            }
        }
    }
}