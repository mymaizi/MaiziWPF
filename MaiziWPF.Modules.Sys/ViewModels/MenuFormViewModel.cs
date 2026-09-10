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
        private readonly IDialogHostService _dialogHostService;

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

        private string _status = "0";
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

        public MenuFormViewModel(ISysMenuService menuService, IDialogHostService dialogHostService)
            : base(dialogHostService)
        {
            _menuService = menuService;
            _dialogHostService = dialogHostService;

            AcceptCommand = new DelegateCommand(() =>
            {
                SaveMenu();
            });
        }

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
                await _dialogHostService.AlertAsync("请输入菜单名称", AlertType.Info);
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
                Remark = Remark
            };

            if (!_menuService.CheckMenuNameUnique(menu))
            {
                await _dialogHostService.AlertAsync("菜单名称已存在", AlertType.Info);
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
                await _dialogHostService.CloseDialogAsync();
            }
            catch (Exception ex)
            {
                await _dialogHostService.AlertAsync(ex.Message, AlertType.Error);
            }
        }
    }
}