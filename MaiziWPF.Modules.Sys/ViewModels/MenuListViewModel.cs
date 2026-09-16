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

        public MenuListViewModel(ISysMenuService menuService, ISnackbarService snackbarService, IDialogHostService dialogHostService, IContainerProvider containerProvider)
        {
            _menuService = menuService;
            _snackbarService = snackbarService;
            _dialogHostService = dialogHostService;

            SearchButtonCommand = new DelegateCommand<MenuListViewModel>((vm) =>
            {
                LoadDataList();
            });

            ResetButtonCommand = new DelegateCommand<MenuListViewModel>((vm) =>
            {
                QueryPageInfo = new QueryMenuInput();
                LoadDataList();
            });

            AddButtonCommand = new DelegateCommand<MenuListViewModel>(async (vm) =>
            {
                await AddMenu();
            });

            EditButtonCommand = new DelegateCommand<SysMenu>(async (menu) =>
            {
                await EditMenu(menu);
            });

            DeleteButtonCommand = new DelegateCommand<SysMenu>(async (menu) =>
            {
                await DeleteMenu(menu);
            });

            LoadDataList();
        }

        public DelegateCommand<MenuListViewModel> SearchButtonCommand { get; }
        public DelegateCommand<MenuListViewModel> ResetButtonCommand { get; }
        public DelegateCommand<MenuListViewModel> AddButtonCommand { get; }
        public DelegateCommand<SysMenu> EditButtonCommand { get; }
        public DelegateCommand<SysMenu> DeleteButtonCommand { get; }

        public override void LoadDataList()
        {
             DataList = new ObservableCollection<SysMenu>(_menuService.SelectMenuList(new SysMenu(), 1));
        }

        private async Task AddMenu()
        {
            await _dialogHostService.ShowDialogAsync<MenuFormView>(vm =>
            {
                var form = (MenuFormViewModel)vm;
                form.DialogTitle = "新增菜单";
                form.IsEditMode = false;
                form.ParentId = 0;
                form.MenuId = 0;
                form.LoadMenuTree();
                form.OnSaveSuccessCallback = () =>
                {
                    LoadDataList();
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
                form.LoadMenuTree();
                form.OnSaveSuccessCallback = () =>
                {
                    LoadDataList();
                };
            });
        }

        private async Task DeleteMenu(SysMenu menu)
        {
            if (menu == null) return;

            var result = await _dialogHostService.ConfirmAsync($"确定要删除菜单 '{menu.MenuName}' 吗？", "确认删除");
            if (result)
            {
                try
                {
                    _menuService.DeleteMenuById(menu.Id);
                    _snackbarService.EnqueueSuccess("删除成功");
                    LoadDataList();
                }
                catch (System.Exception ex)
                {
                    _snackbarService.EnqueueError($"删除失败：{ex.Message}");
                }
            }
        }
    }
}