using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using Prism.Commands;
using Prism.Ioc;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MaiziWPF.Modules.Sys
{
    public class MenuListViewModel : PageBindableBase<SysMenu, QueryMenuInput>
    {
        private readonly ISysMenuService _menuService;
        private readonly ISnackbarService _snackbarService;
        private readonly IDialogHostService _dialogHostService;
        private readonly ICurrentUserService _currentUserService;

        public MenuListViewModel(ISysMenuService menuService, ISnackbarService snackbarService, IDialogHostService dialogHostService, IContainerProvider containerProvider, ICurrentUserService currentUserService)
        {
            _menuService = menuService;
            _snackbarService = snackbarService;
            _dialogHostService = dialogHostService;
            _currentUserService = currentUserService;

            RegisterQueryFunc(input => _menuService.SelectMenuTreeByUserId(_currentUserService.UserId), new QueryMenuInput() { PageNumber = 1, PageSize = 10 },
                resetAction: qpi => QueryPageInfo = new QueryMenuInput());

            AddButtonCommand = new DelegateCommand<SysMenu?>(async (menu) => await AddMenu(menu));
            EditButtonCommand = new DelegateCommand<SysMenu>(async (menu) => await EditMenu(menu));
            DeleteButtonCommand = new DelegateCommand<SysMenu>(async (menu) => await DeleteMenu(menu));
        }

        public DelegateCommand<SysMenu?> AddButtonCommand { get; }
        public DelegateCommand<SysMenu> EditButtonCommand { get; }
        public DelegateCommand<SysMenu> DeleteButtonCommand { get; }

        private async Task AddMenu(SysMenu? parentMenu = null)
        {
            await _dialogHostService.ShowDialogAsync<MenuFormView>(vm =>
            {
                var form = (MenuFormViewModel)vm;
                form.DialogTitle = parentMenu == null ? "新增菜单" : "新增子菜单";
                form.IsEditMode = false;
                form.ParentId = parentMenu?.Id ?? 0;
                form.MenuId = 0;
                form.OnSaveSuccessCallback = () =>
                {
                    SearchButtonCommand.Execute(this);
                };
            });
        }

        private async Task EditMenu(SysMenu menu)
        {
            if (menu == null) return;

            await _dialogHostService.ShowDialogAsync<MenuFormView>(vm =>
            {
                var form = (MenuFormViewModel)vm;
                form.DialogTitle = "编辑菜单";
                form.IsEditMode = true;
                form.MenuId = menu.Id;
                form.ParentId = menu.ParentId;
                form.MenuName = menu.MenuName;
                form.MenuType = menu.MenuType;
                form.OrderNum = menu.OrderNum;
                form.Icon = menu.Icon;
                form.Component = menu.Component;
                form.Perms = menu.Perms;
                form.Status = menu.Status;
                form.BackupStatus = menu.Status;
                form.Remark = menu.Remark;
                form.Path = menu.Path;
                form.Query = menu.QueryParam;
                form.IsFrame = menu.IsFrame == "Y";
                form.IsCache = menu.IsCache == "Y";
                form.IsVisible = menu.Visible == "0";
                form.OnSaveSuccessCallback = () =>
                {
                    SearchButtonCommand.Execute(this);
                };
            });
        }

        private async Task DeleteMenu(SysMenu menu)
        {
            if (menu == null) return;

            try
            {
                var result = await _dialogHostService.ConfirmAsync($"确定要删除菜单 '{menu.MenuName}' 及其所有子菜单吗？", "确认删除");
                if (result)
                {
                    _menuService.DeleteMenuCascade(menu.Id);
                    _snackbarService.EnqueueSuccess("删除成功");
                    SearchButtonCommand.Execute(this);
                }
            }
            catch (System.Exception ex)
            {
                _snackbarService.EnqueueError($"删除失败：{ex.Message}");
            }
        }
    }
}