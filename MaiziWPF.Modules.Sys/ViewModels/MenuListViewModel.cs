using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace MaiziWPF.Modules.Sys
{
    public class MenuListViewModel : PageBindableBase<SysMenu, QueryMenuInput>
    {
        public ObservableCollection<SysMenu> MenuItems { get; set; } = new();

        private string _menuName;
        public string MenuName { get => _menuName; set => SetProperty(ref _menuName, value); }

        private string _status;
        public string Status { get => _status; set => SetProperty(ref _status, value); }

        private readonly ISysMenuService _menuService;
        private readonly IDialogHostService _dialogHostService;

        public ICommand AddMenuCommand { get; }

        public MenuListViewModel(ISysMenuService menuService, IDialogHostService dialogHostService)
        {
            _menuService = menuService;
            _dialogHostService = dialogHostService;

            SearchButtonCommand = new DelegateCommand(() =>
            {
                SearchMenu();
            });

            AddMenuCommand = new DelegateCommand(() =>
            {
                OpenMenuForm(null);
            });

            NewOrEditButtonCommand = new DelegateCommand<SysMenu>((menu) =>
            {
                OpenMenuForm(menu);
            });

            DeleteButtonCommand = new DelegateCommand<SysMenu>(async (menu) =>
            {
                await DeleteMenu(menu);
            });

            SearchButtonCommand.Execute(this);
        }

        private void SearchMenu()
        {
            MenuItems.Clear();
            var list = _menuService.SelectMenuList(new SysMenu()
            {
                MenuName = MenuName,
                Status = Status,
            }, 1);
            MenuItems.AddRange(list);
        }

        private async void OpenMenuForm(SysMenu menu)
        {
            var isEdit = menu != null;
            var viewModel = new MenuFormViewModel(_menuService, _dialogHostService);
            viewModel.LoadMenuTree();
            viewModel.IsEditMode = isEdit;

            if (isEdit)
            {
                viewModel.MenuId = menu.Id;
                viewModel.ParentId = menu.ParentId;
                viewModel.MenuName = menu.MenuName;
                viewModel.MenuType = menu.MenuType;
                viewModel.OrderNum = menu.OrderNum;
                viewModel.Icon = menu.Icon;
                viewModel.Component = menu.Component;
                viewModel.Perms = menu.Perms;
                viewModel.Status = menu.Status;
                viewModel.Remark = menu.Remark;
            }

            viewModel.OnSaveSuccessCallback = () =>
            {
                SearchMenu();
            };

            await _dialogHostService.ShowDialogAsync(viewModel, autoClose: false);
        }

        private async System.Threading.Tasks.Task DeleteMenu(SysMenu menu)
        {
            if (menu == null) return;

            var confirmResult = await _dialogHostService.ConfirmAsync($"确定要删除菜单【{menu.MenuName}】吗？", "删除确认");
            if (!confirmResult) return;

            if (_menuService.HasChildByMenuId(menu.Id))
            {
                await _dialogHostService.AlertAsync("存在子菜单,不允许删除", AlertType.Info);
                return;
            }

            if (_menuService.CheckMenuExistRole(menu.Id))
            {
                await _dialogHostService.AlertAsync("菜单已分配,不允许删除", AlertType.Info);
                return;
            }

            try
            {
                _menuService.DeleteMenuById(menu.Id);
                SearchMenu();
            }
            catch (Exception ex)
            {
                await _dialogHostService.AlertAsync(ex.Message, AlertType.Error);
            }
        }
    }
}