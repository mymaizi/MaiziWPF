using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using Prism.Commands;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace MaiziWPF.Modules.Sys
{
    public class UserListViewModel : PageBindableBase<SysUser,QueryUserInput>
    {
        public ObservableCollection<SysDept> DeptItems { get; set; } = new();
        private readonly ISysDeptService _deptService;
        private readonly ISysUserService _userService;
        public ICommand DeptSelectionCommand { get; }
        public ICommand DeptQueryCommand { get; }

        private readonly IContainerProvider _containerProvider;
        private readonly IDialogHostService _dialogHostService;

        public UserListViewModel(ISysDeptService deptService, ISysUserService userService,IContainerProvider containerProvider, IDialogHostService dialogHostService)
        {
            _deptService = deptService;
            _userService = userService;
            _containerProvider = containerProvider;
            _dialogHostService = dialogHostService;
            this.NewOrEditButtonCommand = new DelegateCommand<SysUser>(user =>
            {
                var view = _containerProvider.Resolve<UserFormView>();
                var model = view.DataContext as UserFormViewModel;
                if (user != null)
                {
                    model.UserId = user.UserId;
                    model.UserName = user.UserName;
                    model.NickName = user.NickName;
                    model.PhoneNumber = user.PhoneNumber;
                    model.Email = user.Email;
                    model.Status = user.Status;
                    model.Remark = user.Remark;
                    model.Sex = user.Sex;
                    model.IsEditMode = true;
                    model.InitialRoleIds = _userService.SelectUserRoleIds(user.UserId);
                    model.InitialPostIds = _userService.SelectUserPostIds(user.UserId);
                    model.InitialDeptIds = _userService.SelectUserDeptIds(user.UserId);
                }
                else
                {
                    model.IsEditMode = false;
                    model.InitialRoleIds = null;
                    model.InitialPostIds = null;
                    model.InitialDeptIds = null;
                }
                model.OnSaveSuccessCallback = () =>
                {
                    SearchButtonCommand?.Execute(null);
                };
                view.DataContext = model;
                _dialogHostService.ShowDialogAsync(view, autoClose: false);
            });
            this.DeleteButtonCommand = new DelegateCommand<SysUser>(async user =>
            {
                if (user == null) return;
                var result = await _dialogHostService.ConfirmAsync($"确定要删除用户 '{user.UserName}' 吗？", "确认删除");
                if (result)
                {
                    try
                    {
                        var success = _userService.DeleteUser(user.UserId);
                        if (success)
                        {
                            await _dialogHostService.AlertAsync("删除成功", AlertType.Info);
                            SearchButtonCommand.Execute(this);
                        }
                        else
                        {
                            await _dialogHostService.AlertAsync("删除失败", AlertType.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        await _dialogHostService.AlertAsync($"删除失败：{ex.Message}", AlertType.Error);
                    }
                }
            });
            RegisterQueryFunc(input =>
            {
                return _userService.SelectUserList(input);
            },new QueryUserInput() { PageNumber=1,PageSize=10 });
            DeptSelectionCommand = new DelegateCommand(() =>
            {
              
            });
            DeptQueryCommand = new DelegateCommand(() =>
            {
                SearchDept();
            });
            DeptQueryCommand.Execute(this);
            SearchButtonCommand.Execute(this);
        }
        private void SearchDept()
        {
            DeptItems.Clear();
            var data = _deptService.SelectDeptList(new SysDept()
            {
                DeptName = QueryPageInfo.DeptName
            });
            DeptItems.AddRange(data);
        }
    }
}