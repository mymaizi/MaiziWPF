using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using Prism.Commands;
using Prism.Ioc;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MaiziWPF.Modules.Sys
{
    public class UserListViewModel : PageBindableBase<SysUser, QueryUserInput>
    {
        private readonly ISysUserService _userService;
        private readonly ISysDeptService _deptService;
        private readonly ISnackbarService _snackbarService;
        private readonly IDialogHostService _dialogHostService;
        private readonly IContainerProvider _containerProvider;

        private string _userName;
        public string UserName
        {
            get => _userName;
            set
            {
                if (SetProperty(ref _userName, value) && QueryPageInfo is QueryUserInput qpi)
                    qpi.UserName = value;
            }
        }

        private string _phoneNumber;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                if (SetProperty(ref _phoneNumber, value) && QueryPageInfo is QueryUserInput qpi)
                    qpi.Phonenumber = value;
            }
        }

        private string _status;
        public string Status
        {
            get => _status;
            set
            {
                if (SetProperty(ref _status, value) && QueryPageInfo is QueryUserInput qpi)
                    qpi.Status = value;
            }
        }

        private long _deptId;
        public long DeptId
        {
            get => _deptId;
            set
            {
                if (SetProperty(ref _deptId, value) && QueryPageInfo is QueryUserInput qpi)
                {
                    qpi.DeptId = value;
                    SearchButtonCommand?.Execute(this);
                }
            }
        }

        public UserListViewModel(ISysUserService userService, ISysDeptService deptService, ISnackbarService snackbarService, IDialogHostService dialogHostService, IContainerProvider containerProvider)
        {
            _userService = userService;
            _deptService = deptService;
            _snackbarService = snackbarService;
            _dialogHostService = dialogHostService;
            _containerProvider = containerProvider;

            RegisterQueryFunc(input => _userService.SelectUserList(input), new QueryUserInput() { PageNumber = 1, PageSize = 10 },
                resetAction: qpi =>
                {
                    UserName = null;
                    PhoneNumber = null;
                    Status = null;
                });

            AddButtonCommand = new DelegateCommand<UserListViewModel>(async (vm) => await AddUser());
            EditButtonCommand = new DelegateCommand<SysUser>(async (user) => await EditUser(user));
            DeleteButtonCommand = new DelegateCommand<SysUser>(async (user) => await DeleteUser(user));
            ResetPwdButtonCommand = new DelegateCommand<SysUser>(async (user) => await ResetPwd(user));
            BatchDeleteCommand = new DelegateCommand<IList>(async (selectedItems) => await BatchDeleteUsers(selectedItems));
            AuthRoleButtonCommand = new DelegateCommand<SysUser>(async (user) => await AuthRole(user));
        }

        public DelegateCommand<UserListViewModel> AddButtonCommand { get; }
        public DelegateCommand<SysUser> EditButtonCommand { get; }
        public DelegateCommand<SysUser> DeleteButtonCommand { get; }
        public DelegateCommand<SysUser> ResetPwdButtonCommand { get; }
        public DelegateCommand<IList> BatchDeleteCommand { get; }
        public DelegateCommand<SysUser> AuthRoleButtonCommand { get; }

        private async Task AddUser()
        {
            await _dialogHostService.ShowDialogAsync<UserFormView>(vm =>
            {
                var form = (UserFormViewModel)vm;
                form.DialogTitle = "新增用户";
                form.IsEditMode = false;
                form.UserId = 0;
                form.DeptId = DeptId;
                form.SelectedPostId = 0;
                form.SelectedRoles = new System.Collections.ObjectModel.ObservableCollection<Checked>();
                form.OnSaveSuccessCallback = () =>
                {
                    SearchButtonCommand.Execute(this);
                };
            });
        }

        private async Task EditUser(SysUser user)
        {
            if (user == null) return;

            await _dialogHostService.ShowDialogAsync<UserFormView>(vm =>
            {
                var form = (UserFormViewModel)vm;
                form.DialogTitle = "编辑用户";
                form.IsEditMode = true;
                form.UserId = user.UserId;
                form.UserName = user.UserName;
                form.DeptId = user.DeptId;
                form.NickName = user.NickName;
                form.PhoneNumber = user.PhoneNumber;
                form.Email = user.Email;
                form.Status = user.Status;
                form.Gender = user.Gender;
                form.Remark = user.Remark;
                var roleIds = _userService.SelectUserRoleIds(user.UserId);
                var postIds = _userService.SelectUserPostIds(user.UserId);
                form.SelectedRoles = new System.Collections.ObjectModel.ObservableCollection<Checked>(
                    form.GetAllRoles().Where(r => roleIds.Contains(r.Id)).ToList());
                form.SelectedPostId = postIds.Count > 0 ? postIds[0] : 0;

                form.OnSaveSuccessCallback = () =>
                {
                    SearchButtonCommand.Execute(this);
                };
            });
        }

        private async Task DeleteUser(SysUser user)
        {
            if (user == null) return;

            var result = await _dialogHostService.ConfirmAsync($"确定要删除用户 '{user.UserName}' 吗？", "确认删除");
            if (result)
            {
                try
                {
                    _userService.DeleteUser(user.UserId);
                    _snackbarService.EnqueueSuccess("删除成功");
                    SearchButtonCommand.Execute(this);
                }
                catch (System.Exception ex)
                {
                    _snackbarService.EnqueueError($"删除失败：{ex.Message}");
                }
            }
        }

        private async Task BatchDeleteUsers(IList selectedItems)
        {
            if (selectedItems == null || selectedItems.Count == 0)
            {
                _snackbarService.EnqueueWarning("请先选择要删除的用户");
                return;
            }

            var result = await _dialogHostService.ConfirmAsync($"确定要删除选中的 {selectedItems.Count} 个用户吗？", "确认删除");
            if (result)
            {
                try
                {
                    foreach (var item in selectedItems.Cast<SysUser>())
                    {
                        _userService.DeleteUser(item.UserId);
                    }
                    _snackbarService.EnqueueSuccess("批量删除成功");
                    SearchButtonCommand.Execute(this);
                }
                catch (System.Exception ex)
                {
                    _snackbarService.EnqueueError($"删除失败：{ex.Message}");
                }
            }
        }

        private async Task ResetPwd(SysUser user)
        {
            if (user == null) return;

            var result = await _dialogHostService.ConfirmAsync($"确定要重置用户 '{user.UserName}' 的密码吗？", "确认重置");
            if (result)
            {
                try
                {
                    _userService.ResetPwd(user.UserId);
                    _snackbarService.EnqueueSuccess("密码重置成功");
                }
                catch (System.Exception ex)
                {
                    _snackbarService.EnqueueError($"密码重置失败：{ex.Message}");
                }
            }
        }

        private async Task AuthRole(SysUser user)
        {
            if (user == null) return;

            await _dialogHostService.ShowDialogAsync<AuthRoleView>(vm =>
            {
                var form = (AuthRoleViewModel)vm;
                form.DialogTitle = "分配角色";
                form.UserId = user.UserId;
                form.UserName = user.NickName;
                form.OnSaveSuccessCallback = () =>
                {
                    SearchButtonCommand.Execute(this);
                };
            });
        }
    }
}