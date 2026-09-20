﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using MaiziWPF.Common;
using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using Prism.Commands;
using Prism.Ioc;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace MaiziWPF.Modules.Sys
{
    public class RoleListViewModel : PageBindableBase<SysRole, QueryRoleInput>
    {
        private readonly ISysRoleService _roleService;
        private readonly ISnackbarService _snackbarService;
        private readonly IDialogHostService _dialogHostService;
        private readonly IContainerProvider _containerProvider;

        private string _roleName;
        public string RoleName
        {
            get => _roleName;
            set
            {
                if (SetProperty(ref _roleName, value) && QueryPageInfo is QueryRoleInput qpi)
                    qpi.RoleName = value;
            }
        }

        private string _roleKey;
        public string RoleKey
        {
            get => _roleKey;
            set
            {
                if (SetProperty(ref _roleKey, value) && QueryPageInfo is QueryRoleInput qpi)
                    qpi.RoleKey = value;
            }
        }

        private string _status;
        public string Status
        {
            get => _status;
            set
            {
                if (SetProperty(ref _status, value) && QueryPageInfo is QueryRoleInput qpi)
                    qpi.Status = value;
            }
        }

        public RoleListViewModel(ISysRoleService roleService, ISnackbarService snackbarService, IDialogHostService dialogHostService, IContainerProvider containerProvider)
        {
            _roleService = roleService;
            _snackbarService = snackbarService;
            _dialogHostService = dialogHostService;
            _containerProvider = containerProvider;

            RegisterQueryFunc(input => _roleService.SelectRoleList(input), new QueryRoleInput() { PageNumber = 1, PageSize = 10 },
                resetAction: qpi =>
                {
                    RoleName = null;
                    RoleKey = null;
                    Status = null;
                    QueryPageInfo = new QueryRoleInput();
                });

            AddButtonCommand = new DelegateCommand<RoleListViewModel>(async (vm) => await AddRole());
            EditButtonCommand = new DelegateCommand<SysRole>(async (role) => await EditRole(role));
            DeleteButtonCommand = new DelegateCommand<SysRole>(async (role) => await DeleteRole(role));
            DataScopeButtonCommand = new DelegateCommand<SysRole>(async (role) => await OpenDataScope(role));
            AuthUserButtonCommand = new DelegateCommand<SysRole>(async (role) => await OpenAuthUser(role));
            BatchDeleteCommand = new DelegateCommand<IList>(async (selectedItems) => await BatchDeleteRoles(selectedItems));
        }

        public DelegateCommand<RoleListViewModel> AddButtonCommand { get; }
        public DelegateCommand<SysRole> EditButtonCommand { get; }
        public DelegateCommand<SysRole> DeleteButtonCommand { get; }
        public DelegateCommand<SysRole> DataScopeButtonCommand { get; }
        public DelegateCommand<SysRole> AuthUserButtonCommand { get; }
        public DelegateCommand<IList> BatchDeleteCommand { get; }

        private async Task AddRole()
        {
            await _dialogHostService.ShowDialogAsync<RoleFormView>(vm =>
            {
                var form = (RoleFormViewModel)vm;
                form.DialogTitle = "新增角色";
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
            if (role.RoleId == SecurityUtils.SUPER_ADMIN_ROLE_ID)
            {
                _snackbarService.EnqueueWarning("不允许操作超级管理员角色");
                return;
            }

            await _dialogHostService.ShowDialogAsync<RoleFormView>(vm =>
            {
                var form = (RoleFormViewModel)vm;
                form.DialogTitle = "修改角色";
                form.IsEditMode = true;
                form.RoleId = role.RoleId;
                form.RoleName = role.RoleName;
                form.RoleKey = role.RoleKey;
                form.RoleSort = role.RoleSort;
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
            if (role.RoleId == SecurityUtils.SUPER_ADMIN_ROLE_ID)
            {
                _snackbarService.EnqueueWarning("不允许操作超级管理员角色");
                return;
            }

            try
            {
                var result = await _dialogHostService.ConfirmAsync($"确定要删除角色 '{role.RoleName}' 吗？", "确认删除");
                if (result)
                {
                    _roleService.DeleteRoleById(role.RoleId);
                    _snackbarService.EnqueueSuccess("删除成功");
                    SearchButtonCommand.Execute(this);
                }
            }
            catch (System.Exception ex)
            {
                _snackbarService.EnqueueError($"删除失败：{ex.Message}");
            }
        }

        private async Task BatchDeleteRoles(IList selectedItems)
        {
            if (selectedItems == null || selectedItems.Count == 0)
            {
                _snackbarService.EnqueueWarning("请先选择要删除的角色");
                return;
            }

            var roles = selectedItems.Cast<SysRole>().ToList();
            if (roles.Any(r => r.RoleId == SecurityUtils.SUPER_ADMIN_ROLE_ID))
            {
                _snackbarService.EnqueueWarning("不允许操作超级管理员角色");
                return;
            }

            var names = string.Join("、", roles.Select(r => r.RoleName));
            var result = await _dialogHostService.ConfirmAsync($"确定要删除选中的 {roles.Count} 个角色（{names}）吗？", "确认批量删除");
            if (result)
            {
                try
                {
                    foreach (var role in roles)
                        _roleService.DeleteRoleById(role.RoleId);
                    _snackbarService.EnqueueSuccess($"成功删除 {roles.Count} 个角色");
                    SearchButtonCommand.Execute(this);
                }
                catch (System.Exception ex)
                {
                    _snackbarService.EnqueueError($"批量删除失败：{ex.Message}");
                }
            }
        }

        private async Task OpenDataScope(SysRole role)
        {
            if (role == null) return;
            if (role.RoleId == SecurityUtils.SUPER_ADMIN_ROLE_ID)
            {
                _snackbarService.EnqueueWarning("不允许操作超级管理员角色");
                return;
            }

            await _dialogHostService.ShowDialogAsync<RolePermissionView>(vm =>
            {
                var form = (RolePermissionViewModel)vm;
                form.DialogTitle = "分配权限";
                form.RoleId = role.RoleId;
                form.RoleName = role.RoleName;
                form.RoleKey = role.RoleKey;
                form.DataScope = role.DataScope;
                form.MenuCheckStrictly = role.MenuCheckStrictly ?? false;
                form.DeptCheckStrictly = role.DeptCheckStrictly ?? false;
                form.OnSaveSuccessCallback = () =>
                {
                    SearchButtonCommand.Execute(this);
                };
            });
        }

        private async Task OpenAuthUser(SysRole role)
        {
            if (role == null) return;
            if (role.RoleId == SecurityUtils.SUPER_ADMIN_ROLE_ID)
            {
                _snackbarService.EnqueueWarning("不允许操作超级管理员角色");
                return;
            }

            await _dialogHostService.ShowDialogAsync<RoleAuthUserView>(vm =>
            {
                var form = (RoleAuthUserViewModel)vm;
                form.DialogTitle = "分配用户";
                form.RoleId = role.RoleId;
                form.RoleName = role.RoleName;
                form.OnSaveSuccessCallback = () =>
                {
                    SearchButtonCommand.Execute(this);
                };
            });
        }
    }
}