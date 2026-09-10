using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using Prism.Commands;
using System.Collections.Generic;
using System.Linq;

namespace MaiziWPF.Modules.Sys
{
    public class UserFormViewModel : FormBindableBase
    {
        #region 表单字段
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

        private string _nickName;
        public string NickName
        {
            get { return _nickName; }
            set { SetProperty(ref _nickName, value); }
        }

        private string _phoneNumber;
        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set { SetProperty(ref _phoneNumber, value); }
        }

        private string _email;
        public string Email
        {
            get { return _email; }
            set { SetProperty(ref _email, value); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }
        private string _status = "0";
        public string Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }
        public string _sex;
        public string Sex
        {
            get { return _sex; }
            set { SetProperty(ref _sex, value); }
        }
        public List<Checked> _roles;
        public List<Checked> Roles
        {
            get { return _roles; }
            set { SetProperty(ref _roles, value); }
        }
        public List<Checked> _posts;
        public List<Checked> Posts
        {
            get { return _posts; }
            set { SetProperty(ref _posts, value); }
        }
        private List<Checked> _depts;
        public List<Checked> Depts
        {
            get { return _depts; }
            set { SetProperty(ref _depts, value); }
        }
        private string _remark;
        public string Remark
        {
            get { return _remark; }
            set { SetProperty(ref _remark, value); }
        }
        private bool _isEditMode;
        public bool IsEditMode
        {
            get { return _isEditMode; }
            set { SetProperty(ref _isEditMode, value); }
        }

        private List<long> _initialRoleIds;
        public List<long> InitialRoleIds
        {
            get { return _initialRoleIds; }
            set { SetProperty(ref _initialRoleIds, value); }
        }

        private List<long> _initialPostIds;
        public List<long> InitialPostIds
        {
            get { return _initialPostIds; }
            set { SetProperty(ref _initialPostIds, value); }
        }

        private List<long> _initialDeptIds;
        public List<long> InitialDeptIds
        {
            get { return _initialDeptIds; }
            set { SetProperty(ref _initialDeptIds, value); }
        }
        #endregion

        #region 验证方法
        private bool ValidateForm()
        {
            MaiziWPF.Core.NotEmptyValidationRule.ShowValidationErrors = true;
            MaiziWPF.Core.LengthValidationRule.ShowValidationErrors = true;
            MaiziWPF.Core.PasswordValidationRule.ShowValidationErrors = true;

            if (!IsEditMode)
            {
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(UserName)));
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(Password)));
            }
            OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(NickName)));

            if (!IsEditMode)
            {
                return !string.IsNullOrWhiteSpace(NickName) &&
                  !string.IsNullOrWhiteSpace(UserName) &&
                  !string.IsNullOrWhiteSpace(Password);
            }
            return !string.IsNullOrWhiteSpace(NickName);
        }
        #endregion

        private readonly ISysUserService _userService;
        private readonly IDialogHostService _dialogHostService;

        public UserFormViewModel(ISysUserService userService, IDialogHostService dialogHostService) : base(dialogHostService)
        {
            _userService = userService;
            _dialogHostService = dialogHostService;
            this.AcceptCommand = new DelegateCommand(async () =>
            {
                if (!ValidateForm())
                {
                    return;
                }

                if (UserId != 0)
                {
                    var user = new SysUser()
                    {
                        UserId = this.UserId,
                        NickName = this.NickName,
                        PhoneNumber = this.PhoneNumber,
                        Email = this.Email,
                        Status = this.Status,
                        Sex = this.Sex,
                        Remark = this.Remark,
                        Posts = this.Posts?.Select(p => new SysPost() { PostId = p.Id }).ToList(),
                        Roles = this.Roles?.Select(r => new SysRole() { RoleId = r.Id }).ToList(),
                        Depts = this.Depts?.Select(d => new SysDept() { Id = d.Id }).ToList(),
                    };

                    var success = _userService.UpdateUser(user);

                    if (success)
                    {
                        await _dialogHostService.AlertAsync("修改成功！", () =>
                        {
                            _dialogHostService.CloseDialogAsync();
                            OnSaveSuccessCallback?.Invoke();
                        }, "UserDialog");
                    }
                    else
                    {
                        await _dialogHostService.AlertAsync("修改失败！", AlertType.Error);
                    }
                }
                else
                {
                    var user = new SysUser()
                    {
                        UserName = this.UserName,
                        NickName = this.NickName,
                        PhoneNumber = this.PhoneNumber,
                        Email = this.Email,
                        Password = this.Password,
                        Status = this.Status,
                        Sex = this.Sex,
                        Remark = this.Remark,
                        Posts = this.Posts?.Select(p => new SysPost() { PostId = p.Id }).ToList(),
                        Roles = this.Roles?.Select(r => new SysRole() { RoleId = r.Id }).ToList(),
                        Depts = this.Depts?.Select(d => new SysDept() { Id = d.Id }).ToList(),
                    };

                    _userService.InsertUser(user);

                    await _dialogHostService.AlertAsync("添加成功！", () =>
                    {
                        _dialogHostService.CloseDialogAsync();
                        OnSaveSuccessCallback?.Invoke();
                    }, "UserDialog");
                }
            });
        }
    }
}