using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using Prism.Commands;
using System;

namespace MaiziWPF.Modules.Sys
{
    public class RoleFormViewModel : FormBindableBase
    {
        private readonly ISysRoleService _roleService;

        private bool _isEditMode;
        public bool IsEditMode
        {
            get { return _isEditMode; }
            set { SetProperty(ref _isEditMode, value); }
        }

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

        private int _roleSort;
        public int RoleSort
        {
            get { return _roleSort; }
            set { SetProperty(ref _roleSort, value); }
        }

        private string _dataScope = "1";
        public string DataScope
        {
            get { return _dataScope; }
            set { SetProperty(ref _dataScope, value); }
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

        public RoleFormViewModel(ISysRoleService roleService, ISnackbarService snackbarService)
            : base(snackbarService)
        {
            _roleService = roleService;

            AcceptCommand = new DelegateCommand(() =>
            {
                SaveRole();
            });
        }

        private async void SaveRole()
        {
            if (string.IsNullOrWhiteSpace(RoleName))
            {
                ShowWarning("请输入角色名称");
                return;
            }
            if (string.IsNullOrWhiteSpace(RoleKey))
            {
                ShowWarning("请输入权限字符");
                return;
            }

            var role = new SysRole
            {
                RoleId = RoleId,
                RoleName = RoleName,
                RoleKey = RoleKey,
                RoleSort = RoleSort,
                DataScope = DataScope,
                Status = Status,
                Remark = Remark
            };

            if (!_roleService.CheckRoleNameUnique(role))
            {
                ShowWarning("角色名称已存在");
                return;
            }
            if (!_roleService.CheckRoleKeyUnique(role))
            {
                ShowWarning("角色权限字符已存在");
                return;
            }

            try
            {
                if (IsEditMode)
                {
                    _roleService.UpdateRole(role);
                }
                else
                {
                    _roleService.InsertRole(role);
                }
                OnSaveSuccessCallback?.Invoke();
                ShowSuccess("保存成功");
                CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
    }
}