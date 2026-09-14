using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;

namespace MaiziWPF.Modules.Sys
{
    public class MenuFormViewModel : FormBindableBase
    {
        private readonly ISysMenuService _menuService;

        public ObservableCollection<SysMenu> MenuTreeItems { get; set; } = new();

        private bool _isEditMode;
        public bool IsEditMode
        {
            get { return _isEditMode; }
            set { SetProperty(ref _isEditMode, value); }
        }

        private long _menuId;
        public long MenuId
        {
            get { return _menuId; }
            set { SetProperty(ref _menuId, value); }
        }

        private long _parentId;
        public long ParentId
        {
            get { return _parentId; }
            set { SetProperty(ref _parentId, value); }
        }

        private string _menuType = "M";
        public string MenuType
        {
            get { return _menuType; }
            set { SetProperty(ref _menuType, value); }
        }

        private string _menuName;
        public string MenuName
        {
            get { return _menuName; }
            set { SetProperty(ref _menuName, value); }
        }

        private int _orderNum;
        public int OrderNum
        {
            get { return _orderNum; }
            set { SetProperty(ref _orderNum, value); }
        }

        private string _icon;
        public string Icon
        {
            get { return _icon; }
            set { SetProperty(ref _icon, value); }
        }

        private string _component;
        public string Component
        {
            get { return _component; }
            set { SetProperty(ref _component, value); }
        }

        private string _perms;
        public string Perms
        {
            get { return _perms; }
            set { SetProperty(ref _perms, value); }
        }

        private string _status = "N";
        public string Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }

        private string _remark;
        public string Remark
        {
            get { return _remark; }
            set { SetProperty(ref _remark, value); }
        }

        private bool _isExternalLink;
        public bool IsExternalLink
        {
            get { return _isExternalLink; }
            set { SetProperty(ref _isExternalLink, value); }
        }

        private string _path;
        public string Path
        {
            get { return _path; }
            set { SetProperty(ref _path, value); }
        }

        private string _query;
        public string Query
        {
            get { return _query; }
            set { SetProperty(ref _query, value); }
        }

        private bool _isCache = true;
        public bool IsCache
        {
            get { return _isCache; }
            set { SetProperty(ref _isCache, value); }
        }

        private bool _isVisible = true;
        public bool IsVisible
        {
            get { return _isVisible; }
            set { SetProperty(ref _isVisible, value); }
        }

        private bool _isIconPickerOpen;
        public bool IsIconPickerOpen
        {
            get { return _isIconPickerOpen; }
            set { SetProperty(ref _isIconPickerOpen, value); }
        }

        private bool _isFrame = true;
        public bool IsFrame
        {
            get { return _isFrame; }
            set { SetProperty(ref _isFrame, value); }
        }

        public MenuFormViewModel(ISysMenuService menuService, ISnackbarService snackbarService)
            : base(snackbarService)
        {
            _menuService = menuService;

            AcceptCommand = new DelegateCommand(() =>
            {
                SaveMenu();
            });

            OpenIconPickerCommand = new DelegateCommand(() =>
            {
                IsIconPickerOpen = !IsIconPickerOpen;
            });

            CloseIconPickerCommand = new DelegateCommand(() =>
            {
                IsIconPickerOpen = false;
            });
        }

        public DelegateCommand OpenIconPickerCommand { get; }
        public DelegateCommand CloseIconPickerCommand { get; }

        public void LoadMenuTree()
        {
            MenuTreeItems.Clear();
            var list = _menuService.SelectMenuList(new SysMenu(), 1);
            MenuTreeItems.AddRange(list);
        }

        private async void SaveMenu()
        {
            if (string.IsNullOrWhiteSpace(MenuName))
            {
                ShowWarning("请输入菜单名称");
                return;
            }

            var menu = new SysMenu
            {
                Id = MenuId,
                ParentId = ParentId,
                MenuName = MenuName,
                MenuType = MenuType,
                OrderNum = OrderNum,
                Icon = Icon,
                Component = Component,
                Perms = Perms,
                Status = Status,
                Remark = Remark,
                Path = Path,
                QueryParam = Query,
                IsFrame = IsFrame ? "1" : "0",
                IsCache = IsCache ? "0" : "1",
                Visible = IsVisible ? "0" : "1",
            };

            if (!_menuService.CheckMenuNameUnique(menu))
            {
                ShowWarning("菜单名称已存在");
                return;
            }

            try
            {
                if (IsEditMode)
                {
                    _menuService.UpdateMenu(menu);
                }
                else
                {
                    _menuService.InsertMenu(menu);
                }
                OnSaveSuccessCallback?.Invoke();
                ShowSuccess("保存成功");
                CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError($"保存失败：{ex.Message}");
            }
        }
    }
}