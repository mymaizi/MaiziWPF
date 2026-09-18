using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using Prism.Commands;
using Prism.Ioc;
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

        public UserListViewModel(ISysUserService userService, ISysDeptService deptService, ISnackbarService snackbarService, IDialogHostService dialogHostService, IContainerProvider containerProvider)
        {
            _userService = userService;
            _deptService = deptService;
            _snackbarService = snackbarService;
            _dialogHostService = dialogHostService;
            _containerProvider = containerProvider;

            RegisterQueryFunc(input => _userService.SelectUserList(input), new QueryUserInput() { PageNumber = 1, PageSize = 10 },
                resetAction: qpi => QueryPageInfo = new QueryUserInput());

            AddButtonCommand = new DelegateCommand<UserListViewModel>(async (vm) => await AddUser());
            EditButtonCommand = new DelegateCommand<SysUser>(async (user) => await EditUser(user));
            DeleteButtonCommand = new DelegateCommand<SysUser>(async (user) => await DeleteUser(user));
            ResetPwdButtonCommand = new DelegateCommand<SysUser>(async (user) => await ResetPwd(user));
        }

        public DelegateCommand<UserListViewModel> AddButtonCommand { get; }
        public DelegateCommand<SysUser> EditButtonCommand { get; }
        public DelegateCommand<SysUser> DeleteButtonCommand { get; }
        public DelegateCommand<SysUser> ResetPwdButtonCommand { get; }

        private async Task AddUser()
        {
            await _dialogHostService.ShowDialogAsync<UserFormView>(vm =>
            {
                var form = (UserFormViewModel)vm;
                form.IsEditMode = false;
                form.UserId = 0;
                form.Depts = _deptService.SelectDeptList(new SysDept(), false).Select(d => new Checked() { Id = d.Id, Name = d.DeptName }).ToList();
                form.Roles = _userService.SelectAllRoles().Select(r => new Checked() { Id = r.RoleId, Name = r.RoleName }).ToList();
                form.Posts = _userService.SelectAllPosts().Select(p => new Checked() { Id = p.PostId, Name = p.PostName }).ToList();
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
                form.IsEditMode = true;
                form.UserId = user.UserId;
                form.UserName = user.UserName;
                form.NickName = user.NickName;
                form.PhoneNumber = user.PhoneNumber;
                form.Email = user.Email;
                form.Status = user.Status;
                form.Sex = user.Sex;
                form.Remark = user.Remark;
                form.Depts = _deptService.SelectDeptList(new SysDept(), false).Select(d => new Checked() { Id = d.Id, Name = d.DeptName }).ToList();
                form.Roles = _userService.SelectAllRoles().Select(r => new Checked() { Id = r.RoleId, Name = r.RoleName }).ToList();
                form.Posts = _userService.SelectAllPosts().Select(p => new Checked() { Id = p.PostId, Name = p.PostName }).ToList();
                form.InitialRoleIds = _userService.SelectUserRoleIds(user.UserId);
                form.InitialPostIds = _userService.SelectUserPostIds(user.UserId);

                form.Roles?.ForEach(r => r.IsChecked = form.InitialRoleIds.Contains(r.Id));
                form.Posts?.ForEach(p => p.IsChecked = form.InitialPostIds.Contains(p.Id));
                form.Depts?.ForEach(d => d.IsChecked = form.InitialDeptIds.Contains(d.Id));

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
    }
}