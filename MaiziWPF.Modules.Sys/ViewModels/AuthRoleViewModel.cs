using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using Prism.Commands;
using Prism.Dialogs;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MaiziWPF.Modules.Sys
{
    public class AuthRoleViewModel : FormBindableBase
    {
        private readonly ISysUserService _userService;

        private long _userId;
        public long UserId
        {
            get { return _userId; }
            set { SetProperty(ref _userId, value); }
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }

        private string _searchRoleName;
        public string SearchRoleName
        {
            get { return _searchRoleName; }
            set { SetProperty(ref _searchRoleName, value); }
        }

        private string _searchRoleKey;
        public string SearchRoleKey
        {
            get { return _searchRoleKey; }
            set { SetProperty(ref _searchRoleKey, value); }
        }

        private bool _selectAll;
        public bool SelectAll
        {
            get { return _selectAll; }
            set
            {
                if (SetProperty(ref _selectAll, value))
                {
                    foreach (var item in UnallocatedList)
                        item.IsSelected = value;
                }
            }
        }

        public ObservableCollection<SysRole> AllocatedList { get; set; } = new();
        public ObservableCollection<SelectableRole> UnallocatedList { get; set; } = new();

        public DelegateCommand LoadDataCommand { get; }
        public DelegateCommand<SysRole> CancelRoleCommand { get; }
        public DelegateCommand BatchAddCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public AuthRoleViewModel(ISysUserService userService, ISnackbarService snackbarService)
            : base(snackbarService)
        {
            _userService = userService;

            LoadDataCommand = new DelegateCommand(LoadData);
            CancelRoleCommand = new DelegateCommand<SysRole>(CancelRole);
            BatchAddCommand = new DelegateCommand(BatchAddRoles);
            CancelCommand = new DelegateCommand(CloseDialog);
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            base.OnDialogOpened(parameters);
            LoadData();
        }

        public void LoadData()
        {
            LoadAllocatedList();
            LoadUnallocatedList();
        }

        private void LoadAllocatedList()
        {
            var list = _userService.SelectAllocatedRolesByUserId(UserId);
            AllocatedList.Clear();
            foreach (var role in list)
                AllocatedList.Add(role);
        }

        private void LoadUnallocatedList()
        {
            var list = _userService.SelectUnallocatedRolesByUserId(UserId, SearchRoleName, SearchRoleKey);
            UnallocatedList.Clear();
            foreach (var role in list)
                UnallocatedList.Add(new SelectableRole { Role = role, IsSelected = false });
            SelectAll = false;
        }

        private async void CancelRole(SysRole role)
        {
            if (role == null) return;
            try
            {
                _userService.CancelAuthRole(UserId, role.RoleId);

                AllocatedList.Remove(role);
                LoadUnallocatedList();
                ShowSuccess("已取消角色授权");
            }
            catch (Exception ex)
            {
                ShowError($"取消授权失败：{ex.Message}");
            }
        }

        private async void BatchAddRoles()
        {
            var selectedIds = UnallocatedList
                .Where(u => u.IsSelected)
                .Select(u => u.Role.RoleId)
                .ToArray();
            if (selectedIds.Length == 0)
            {
                ShowWarning("请选择需要授权的角色");
                return;
            }
            try
            {
                _userService.InsertAuthRoles(UserId, selectedIds);

                var selected = UnallocatedList.Where(u => u.IsSelected).ToList();
                foreach (var item in selected)
                {
                    AllocatedList.Add(item.Role);
                    UnallocatedList.Remove(item);
                }
                SelectAll = false;

                OnSaveSuccessCallback?.Invoke();
                ShowSuccess("角色授权成功");
            }
            catch (Exception ex)
            {
                ShowError($"角色授权失败：{ex.Message}");
            }
        }
    }

    public class SelectableRole : BindableBase
    {
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }

        public SysRole Role { get; set; }
    }
}