using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace MaiziWPF.Modules.Sys
{
    public class ProfileViewModel : BindableBase, ITabItemInfo
    {
        public string Header { get; set; }
        public string Component { get; set; }

        private readonly ICurrentUserService _currentUserService;
        private readonly ISysUserService _sysUserService;
        private readonly ISnackbarService _snackbarService;
        private readonly IRegionManager _regionManager;

        private Services.Domain.SysUser _currentUser;
        public Services.Domain.SysUser CurrentUser
        {
            get { return _currentUser; }
            set { SetProperty(ref _currentUser, value); }
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

        private string _gender;
        public string Gender
        {
            get { return _gender; }
            set
            {
                if (SetProperty(ref _gender, value))
                {
                    UpdateGenderRadioButtons();
                }
            }
        }

        private bool _isMale;
        public bool IsMale
        {
            get { return _isMale; }
            set
            {
                if (SetProperty(ref _isMale, value))
                {
                    if (value)
                    {
                        _isFemale = false;
                        _isUnknown = false;
                        RaisePropertyChanged(nameof(IsFemale));
                        RaisePropertyChanged(nameof(IsUnknown));
                        Gender = "0";
                    }
                }
            }
        }

        private bool _isFemale;
        public bool IsFemale
        {
            get { return _isFemale; }
            set
            {
                if (SetProperty(ref _isFemale, value))
                {
                    if (value)
                    {
                        _isMale = false;
                        _isUnknown = false;
                        RaisePropertyChanged(nameof(IsMale));
                        RaisePropertyChanged(nameof(IsUnknown));
                        Gender = "1";
                    }
                }
            }
        }

        private bool _isUnknown;
        public bool IsUnknown
        {
            get { return _isUnknown; }
            set
            {
                if (SetProperty(ref _isUnknown, value))
                {
                    if (value)
                    {
                        _isMale = false;
                        _isFemale = false;
                        RaisePropertyChanged(nameof(IsMale));
                        RaisePropertyChanged(nameof(IsFemale));
                        Gender = "2";
                    }
                }
            }
        }

        private bool _isBasicInfoTab = true;
        public bool IsBasicInfoTab
        {
            get { return _isBasicInfoTab; }
            set { SetProperty(ref _isBasicInfoTab, value); }
        }

        private bool _isPasswordTab;
        public bool IsPasswordTab
        {
            get { return _isPasswordTab; }
            set { SetProperty(ref _isPasswordTab, value); }
        }

        private string _oldPassword;
        public string OldPassword
        {
            get { return _oldPassword; }
            set { SetProperty(ref _oldPassword, value); }
        }

        private string _newPassword;
        public string NewPassword
        {
            get { return _newPassword; }
            set { SetProperty(ref _newPassword, value); }
        }

        private string _confirmPassword;
        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set { SetProperty(ref _confirmPassword, value); }
        }

        public DelegateCommand SaveBasicInfoCommand { get; }
        public DelegateCommand ChangePasswordCommand { get; }
        public DelegateCommand CloseCommand { get; }

        public ProfileViewModel(
            IRegionManager regionManager,
            ICurrentUserService currentUserService,
            ISysUserService sysUserService,
            ISnackbarService snackbarService)
        {
            _regionManager = regionManager;
            _currentUserService = currentUserService;
            _sysUserService = sysUserService;
            _snackbarService = snackbarService;

            SaveBasicInfoCommand = new DelegateCommand(SaveBasicInfo);
            ChangePasswordCommand = new DelegateCommand(ChangePassword);
            CloseCommand = new DelegateCommand(CloseProfile);

            LoadUserData();
        }

        private void LoadUserData()
        {
            var user = _currentUserService.CurrentUser;
            if (user != null)
            {
                CurrentUser = user;
                NickName = user.NickName;
                PhoneNumber = user.PhoneNumber;
                Email = user.Email;
                Gender = user.Gender ?? "2";
                UpdateGenderRadioButtons();
            }
        }

        private void UpdateGenderRadioButtons()
        {
            IsMale = Gender == "0";
            IsFemale = Gender == "1";
            IsUnknown = Gender == "2" || string.IsNullOrEmpty(Gender);
        }

        private void SaveBasicInfo()
        {
            if (string.IsNullOrWhiteSpace(NickName))
            {
                _snackbarService.EnqueueWarning("用户昵称不能为空");
                return;
            }

            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                _snackbarService.EnqueueWarning("手机号码不能为空");
                return;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                _snackbarService.EnqueueWarning("邮箱不能为空");
                return;
            }

            var user = _currentUserService.CurrentUser;
            if (user == null) return;

            user.NickName = NickName;
            user.PhoneNumber = PhoneNumber;
            user.Email = Email;
            user.Gender = Gender;

            bool result = _sysUserService.UpdateUser(user);
            if (result)
            {
                _currentUserService.SetCurrentUser(user);
                _snackbarService.EnqueueSuccess("保存成功");
            }
            else
            {
                _snackbarService.EnqueueError("保存失败");
            }
        }

        private void ChangePassword()
        {
            if (string.IsNullOrWhiteSpace(OldPassword))
            {
                _snackbarService.EnqueueWarning("旧密码不能为空");
                return;
            }

            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                _snackbarService.EnqueueWarning("新密码不能为空");
                return;
            }

            if (string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                _snackbarService.EnqueueWarning("确认密码不能为空");
                return;
            }

            if (NewPassword != ConfirmPassword)
            {
                _snackbarService.EnqueueWarning("两次输入的密码不一致");
                return;
            }

            if (NewPassword.Length < 6)
            {
                _snackbarService.EnqueueWarning("密码长度至少6位");
                return;
            }

            var hasNumber = Regex.IsMatch(NewPassword, @"\d");
            var hasLetter = Regex.IsMatch(NewPassword, @"[a-zA-Z]");
            var hasSpecialChar = Regex.IsMatch(NewPassword, @"[^a-zA-Z\d]");

            if (!hasNumber || !hasLetter || !hasSpecialChar)
            {
                _snackbarService.EnqueueWarning("密码必须包含数字、字母和特殊字符");
                return;
            }

            var user = _currentUserService.CurrentUser;
            if (user == null) return;

            if (!BCrypt.Net.BCrypt.Verify(OldPassword, user.Password))
            {
                _snackbarService.EnqueueWarning("旧密码不正确");
                return;
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(NewPassword);
            bool result = _sysUserService.UpdateUser(user);
            if (result)
            {
                _snackbarService.EnqueueSuccess("密码修改成功");
                OldPassword = "";
                NewPassword = "";
                ConfirmPassword = "";
            }
            else
            {
                _snackbarService.EnqueueError("密码修改失败");
            }
        }

        private void CloseProfile()
        {
            var tabRegion = _regionManager.Regions[RegionNames.TabRegion];
            var currentView = tabRegion.Views.FirstOrDefault(v => v.GetType().Name == "ProfileView");
            if (currentView != null)
            {
                tabRegion.Remove(currentView);
            }
        }
    }
}