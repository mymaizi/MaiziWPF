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
        private readonly IContainerProvider _containerProvider;

        public MenuListViewModel(ISysMenuService menuService, ISnackbarService snackbarService, IDialogHostService dialogHostService, IContainerProvider containerProvider)
        {
            _menuService = menuService;
            _snackbarService = snackbarService;
            _dialogHostService = dialogHostService;
            _containerProvider = containerProvider;

            SearchButtonCommand = new DelegateCommand<MenuListViewModel>((vm) =>
            {
                LoadMenuList();
            });

            ResetButtonCommand = new DelegateCommand<MenuListViewModel>((vm) =>
            {
                QueryPageInfo = new QueryMenuInput();
                LoadMenuList();
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
        }

        public DelegateCommand<MenuListViewModel> SearchButtonCommand { get; }
        public DelegateCommand<MenuListViewModel> ResetButtonCommand { get; }
        public DelegateCommand<MenuListViewModel> AddButtonCommand { get; }
        public DelegateCommand<SysMenu> EditButtonCommand { get; }
        public DelegateCommand<SysMenu> DeleteButtonCommand { get; }

        public override void LoadDataList()
        {
            LoadMenuList();
        }

        private void LoadMenuList()
        {
            DataList = new ObservableCollection<SysMenu>(_menuService.SelectMenuList(new SysMenu(), 1));
        }

        private async Task AddMenu()
        {
            await _dialogHostService.ShowDialogAsync<MenuFormView>(view =>
            {
                var vm = view.DataContext as MenuFormViewModel;
                vm.DialogTitle = "新增菜单";
                vm.IsEditMode = false;
                vm.ParentId = 0;
                vm.MenuId = 0;
                vm.LoadMenuTree();
                vm.OnSaveSuccessCallback = () =>
                {
                    LoadMenuList();
                };
            });
        }

        private async Task EditMenu(SysMenu menu)
        {
            if (menu == null) return;

            await _dialogHostService.ShowDialogAsync<MenuFormView>(view =>
            {
                var vm = view.DataContext as MenuFormViewModel;
                vm.DialogTitle = "编辑菜单";
                vm.IsEditMode = true;
                vm.MenuId = menu.Id;
                vm.ParentId = menu.ParentId;
                vm.MenuName = menu.MenuName;
                vm.MenuType = menu.MenuType;
                vm.OrderNum = menu.OrderNum;
                vm.Icon = menu.Icon;
                vm.Component = menu.Component;
                vm.Perms = menu.Perms;
                vm.Status = menu.Status;
                vm.Remark = menu.Remark;
                vm.Path = menu.Path;
                vm.Query = menu.QueryParam;
                vm.IsFrame = menu.IsFrame == "1";
                vm.IsCache = menu.IsCache == "0";
                vm.IsVisible = menu.Visible == "0";
                vm.LoadMenuTree();
                vm.OnSaveSuccessCallback = () =>
                {
                    LoadMenuList();
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
                    LoadMenuList();
                }
                catch (System.Exception ex)
                {
                    _snackbarService.EnqueueError($"删除失败：{ex.Message}");
                }
            }
        }
    }
}