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
    public class RoleListViewModel : PageBindableBase<SysRole, QueryRoleInput>
    {
        private readonly ISysRoleService _roleService;
        private readonly ISnackbarService _snackbarService;
        private readonly IDialogHostService _dialogHostService;
        private readonly IContainerProvider _containerProvider;

        public RoleListViewModel(ISysRoleService roleService, ISnackbarService snackbarService, IDialogHostService dialogHostService, IContainerProvider containerProvider)
        {
            _roleService = roleService;
            _snackbarService = snackbarService;
            _dialogHostService = dialogHostService;
            _containerProvider = containerProvider;

            RegisterQueryFunc(input => _roleService.SelectRoleList(input), new QueryRoleInput() { PageNumber = 1, PageSize = 10 },
                resetAction: qpi => QueryPageInfo = new QueryRoleInput());

            AddButtonCommand = new DelegateCommand<RoleListViewModel>(async (vm) => await AddRole());
            EditButtonCommand = new DelegateCommand<SysRole>(async (role) => await EditRole(role));
            DeleteButtonCommand = new DelegateCommand<SysRole>(async (role) => await DeleteRole(role));
        }

        public DelegateCommand<RoleListViewModel> AddButtonCommand { get; }
        public DelegateCommand<SysRole> EditButtonCommand { get; }
        public DelegateCommand<SysRole> DeleteButtonCommand { get; }

        private async Task AddRole()
        {
            await _dialogHostService.ShowDialogAsync<RoleFormView>(vm =>
            {
                var form = (RoleFormViewModel)vm;
                form.IsEditMode = false;
                form.RoleId = 0;
                form.OnSaveSuccessCallback = () =>
                {
                    SearchButtonCommand.Execute(this);
                };
            });
        }

        private async Task EditRole(SysRole role)
        {
            if (role == null) return;

            await _dialogHostService.ShowDialogAsync<RoleFormView>(vm =>
            {
                var form = (RoleFormViewModel)vm;
                form.IsEditMode = true;
                form.RoleId = role.RoleId;
                form.RoleName = role.RoleName;
                form.RoleKey = role.RoleKey;
                form.RoleSort = role.RoleSort;
                form.DataScope = role.DataScope;
                form.Status = role.Status;
                form.Remark = role.Remark;
                form.OnSaveSuccessCallback = () =>
                {
                    SearchButtonCommand.Execute(this);
                };
            });
        }

        private async Task DeleteRole(SysRole role)
        {
            if (role == null) return;

            var result = await _dialogHostService.ConfirmAsync($"确定要删除角色 '{role.RoleName}' 吗？", "确认删除");
            if (result)
            {
                try
                {
                    _roleService.DeleteRoleById(role.RoleId);
                    _snackbarService.EnqueueSuccess("删除成功");
                    SearchButtonCommand.Execute(this);
                }
                catch (System.Exception ex)
                {
                    _snackbarService.EnqueueError($"删除失败：{ex.Message}");
                }
            }
        }
    }
}