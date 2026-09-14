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

            SearchButtonCommand = new DelegateCommand<UserListViewModel>((vm) =>
            {
                LoadDataList();
            });

            ResetButtonCommand = new DelegateCommand<UserListViewModel>((vm) =>
            {
                QueryPageInfo = new QueryUserInput();
                LoadDataList();
            });

            AddButtonCommand = new DelegateCommand<UserListViewModel>(async (vm) =>
            {
                await AddUser();
            });

            EditButtonCommand = new DelegateCommand<SysUser>(async (user) =>
            {
                await EditUser(user);
            });

            DeleteButtonCommand = new DelegateCommand<SysUser>(async (user) =>
            {
                await DeleteUser(user);
            });

            ResetPwdButtonCommand = new DelegateCommand<SysUser>(async (user) =>
            {
                await ResetPwd(user);
            });
        }

        public DelegateCommand<UserListViewModel> SearchButtonCommand { get; }
        public DelegateCommand<UserListViewModel> ResetButtonCommand { get; }
        public DelegateCommand<UserListViewModel> AddButtonCommand { get; }
        public DelegateCommand<SysUser> EditButtonCommand { get; }
        public DelegateCommand<SysUser> DeleteButtonCommand { get; }
        public DelegateCommand<SysUser> ResetPwdButtonCommand { get; }

        public override void LoadDataList()
        {
            var users = _userService.SelectUserList(QueryPageInfo);
            DataList = new ObservableCollection<SysUser>(users);
        }

        private async Task AddUser()
        {
            await _dialogHostService.ShowDialogAsync<UserFormView>(view =>
            {
                var vm = view.DataContext as UserFormViewModel;
                vm.IsEditMode = false;
                vm.UserId = 0;
                vm.Depts = _deptService.SelectDeptList(new SysDept(), false).Select(d => new Checked() { Id = d.Id, Name = d.DeptName }).ToList();
                vm.Roles = _userService.SelectAllRoles().Select(r => new Checked() { Id = r.RoleId, Name = r.RoleName }).ToList();
                vm.Posts = _userService.SelectAllPosts().Select(p => new Checked() { Id = p.PostId, Name = p.PostName }).ToList();
                vm.OnSaveSuccessCallback = () =>
                {
                    LoadDataList();
                };
            });
        }

        private async Task EditUser(SysUser user)
        {
            if (user == null) return;

            await _dialogHostService.ShowDialogAsync<UserFormView>(view =>
            {
                var vm = view.DataContext as UserFormViewModel;
                vm.IsEditMode = true;
                vm.UserId = user.UserId;
                vm.UserName = user.UserName;
                vm.NickName = user.NickName;
                vm.PhoneNumber = user.PhoneNumber;
                vm.Email = user.Email;
                vm.Status = user.Status;
                vm.Sex = user.Sex;
                vm.Remark = user.Remark;
                vm.Depts = _deptService.SelectDeptList(new SysDept(), false).Select(d => new Checked() { Id = d.Id, Name = d.DeptName }).ToList();
                vm.Roles = _userService.SelectAllRoles().Select(r => new Checked() { Id = r.RoleId, Name = r.RoleName }).ToList();
                vm.Posts = _userService.SelectAllPosts().Select(p => new Checked() { Id = p.PostId, Name = p.PostName }).ToList();
                vm.InitialRoleIds = _userService.SelectUserRoleIds(user.UserId);
                vm.InitialPostIds = _userService.SelectUserPostIds(user.UserId);
                vm.InitialDeptIds = _userService.SelectUserDeptIds(user.UserId);

                vm.Roles?.ForEach(r => r.IsChecked = vm.InitialRoleIds.Contains(r.Id));
                vm.Posts?.ForEach(p => p.IsChecked = vm.InitialPostIds.Contains(p.Id));
                vm.Depts?.ForEach(d => d.IsChecked = vm.InitialDeptIds.Contains(d.Id));

                vm.OnSaveSuccessCallback = () =>
                {
                    LoadDataList();
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
                    LoadDataList();
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