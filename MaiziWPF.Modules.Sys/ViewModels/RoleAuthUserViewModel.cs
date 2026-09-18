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
    public class RoleAuthUserViewModel : FormBindableBase
    {
        private readonly ISysRoleService _roleService;

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

        private string _searchUserName;
        public string SearchUserName
        {
            get { return _searchUserName; }
            set { SetProperty(ref _searchUserName, value); }
        }

        private string _searchPhone;
        public string SearchPhone
        {
            get { return _searchPhone; }
            set { SetProperty(ref _searchPhone, value); }
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

        public ObservableCollection<SysUser> AllocatedList { get; set; } = new();
        public ObservableCollection<SelectableUser> UnallocatedList { get; set; } = new();

        public DelegateCommand LoadDataCommand { get; }
        public DelegateCommand<SysUser> CancelUserCommand { get; }
        public DelegateCommand BatchAddCommand { get; }

        public RoleAuthUserViewModel(ISysRoleService roleService, ISnackbarService snackbarService)
            : base(snackbarService)
        {
            _roleService = roleService;

            LoadDataCommand = new DelegateCommand(LoadData);
            CancelUserCommand = new DelegateCommand<SysUser>(CancelUser);
            BatchAddCommand = new DelegateCommand(BatchAddUsers);
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
            var list = _roleService.SelectAllocatedList(RoleId);
            AllocatedList.Clear();
            foreach (var user in list)
                AllocatedList.Add(user);
        }

        private void LoadUnallocatedList()
        {
            var list = _roleService.SelectUnallocatedList(RoleId, SearchUserName, SearchPhone);
            UnallocatedList.Clear();
            foreach (var user in list)
                UnallocatedList.Add(new SelectableUser { User = user, IsSelected = false });
            SelectAll = false;
        }

        private async void CancelUser(SysUser user)
        {
            if (user == null) return;
            try
            {
                _roleService.CancelAuthUser(RoleId, user.UserId);
                AllocatedList.Remove(user);
                LoadUnallocatedList();
                ShowSuccess("已取消用户授权");
            }
            catch (Exception ex)
            {
                ShowError($"取消授权失败：{ex.Message}");
            }
        }

        private async void BatchAddUsers()
        {
            var selectedIds = UnallocatedList
                .Where(u => u.IsSelected)
                .Select(u => u.User.UserId)
                .ToArray();
            if (selectedIds.Length == 0)
            {
                ShowWarning("请选择需要授权的用户");
                return;
            }
            try
            {
                _roleService.InsertAuthUsers(RoleId, selectedIds);

                var selected = UnallocatedList.Where(u => u.IsSelected).ToList();
                foreach (var item in selected)
                {
                    AllocatedList.Add(item.User);
                    UnallocatedList.Remove(item);
                }
                SelectAll = false;

                OnSaveSuccessCallback?.Invoke();
                ShowSuccess("用户授权成功");
            }
            catch (Exception ex)
            {
                ShowError($"用户授权失败：{ex.Message}");
            }
        }
    }

    public class SelectableUser : BindableBase
    {
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }

        public SysUser User { get; set; }
        public string UserName => User?.UserName;
        public string NickName => User?.NickName;
        public string PhoneNumber => User?.PhoneNumber;
    }
}