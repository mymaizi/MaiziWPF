using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MaiziWPF.Modules.Sys
{
    public class RoleListViewModel : PageBindableBase<SysRole, QueryRoleInput>
    {
        private readonly ISysRoleService _roleService;
        private readonly IDialogHostService _dialogHostService;

        public ICommand AddRoleCommand { get; }

        public RoleListViewModel(ISysRoleService roleService, IDialogHostService dialogHostService)
        {
            _roleService = roleService;
            _dialogHostService = dialogHostService;

            RegisterQueryFunc(input =>
            {
                return _roleService.SelectRoleList(input);
            }, new QueryRoleInput() { PageNumber = 1, PageSize = 10 });

            AddRoleCommand = new DelegateCommand(() =>
            {
                OpenRoleForm(null);
            });

            NewOrEditButtonCommand = new DelegateCommand<SysRole>((role) =>
            {
                OpenRoleForm(role);
            });

            DeleteButtonCommand = new DelegateCommand<SysRole>(async (role) =>
            {
                await DeleteRole(role);
            });

            SearchButtonCommand.Execute(this);
        }

        private async void OpenRoleForm(SysRole role)
        {
            var isEdit = role != null;
            var viewModel = new RoleFormViewModel(_roleService, _dialogHostService);
            viewModel.IsEditMode = isEdit;

            if (isEdit)
            {
                viewModel.RoleId = role.RoleId;
                viewModel.RoleName = role.RoleName;
                viewModel.RoleKey = role.RoleKey;
                viewModel.RoleSort = role.RoleSort;
                viewModel.DataScope = role.DataScope;
                viewModel.Status = role.Status;
                viewModel.Remark = role.Remark;
            }

            viewModel.OnSaveSuccessCallback = () =>
            {
                SearchButtonCommand.Execute(this);
            };

            await _dialogHostService.ShowDialogAsync(viewModel, autoClose: false);
        }

        private async System.Threading.Tasks.Task DeleteRole(SysRole role)
        {
            if (role == null) return;

            var confirmResult = await _dialogHostService.ConfirmAsync($"确定要删除角色【{role.RoleName}】吗？", "删除确认");
            if (!confirmResult) return;

            if (_roleService.CheckRoleExistUser(role.RoleId))
            {
                await _dialogHostService.AlertAsync("角色已分配用户,不允许删除", AlertType.Info);
                return;
            }

            try
            {
                _roleService.DeleteRoleById(role.RoleId);
                SearchButtonCommand.Execute(this);
            }
            catch (Exception ex)
            {
                await _dialogHostService.AlertAsync(ex.Message, AlertType.Error);
            }
        }
    }
}