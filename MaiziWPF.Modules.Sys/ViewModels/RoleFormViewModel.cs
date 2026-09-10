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
        private readonly IDialogHostService _dialogHostService;

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

        public RoleFormViewModel(ISysRoleService roleService, IDialogHostService dialogHostService)
            : base(dialogHostService)
        {
            _roleService = roleService;
            _dialogHostService = dialogHostService;

            AcceptCommand = new DelegateCommand(() =>
            {
                SaveRole();
            });
        }

        private async void SaveRole()
        {
            if (string.IsNullOrWhiteSpace(RoleName))
            {
                await _dialogHostService.AlertAsync("请输入角色名称", AlertType.Info);
                return;
            }
            if (string.IsNullOrWhiteSpace(RoleKey))
            {
                await _dialogHostService.AlertAsync("请输入权限字符", AlertType.Info);
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
                await _dialogHostService.AlertAsync("角色名称已存在", AlertType.Info);
                return;
            }
            if (!_roleService.CheckRoleKeyUnique(role))
            {
                await _dialogHostService.AlertAsync("角色权限字符已存在", AlertType.Info);
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
                await _dialogHostService.CloseDialogAsync();
            }
            catch (Exception ex)
            {
                await _dialogHostService.AlertAsync(ex.Message, AlertType.Error);
            }
        }
    }
}