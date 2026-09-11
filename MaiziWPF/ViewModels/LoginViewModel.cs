using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using MaiziWPF.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using Prism.Navigation;
using Serilog;
using System;
using System.Windows;
using System.Windows.Input;
using Volo.Abp;
namespace MaiziWPF.ViewModels
{
    public class LoginViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly ISysUserService _userService;
        public ICommand CloseWindowCommand { get; }
        public ICommand LoginCommand { get; }
        private String _userName;
        private String _password;
        public string UserName { get => _userName; set => SetProperty(ref _userName, value); } 
        public string Password { get => _password; set => SetProperty(ref _password, value); }

        public LoginViewModel(IRegionManager regionManager, ISysUserService userService)
        {
            _regionManager = regionManager;
            _userService = userService;
            CloseWindowCommand = new DelegateCommand(() =>
            {
                System.Windows.Application.Current.Shutdown();
            });
            LoginCommand = new DelegateCommand(LoginHandle);
            UserName = "admin";
            Password = "admin123";
        }

        private void LoginHandle()
        {
            if (String.IsNullOrEmpty(_userName) || String.IsNullOrEmpty(_password))
            {
                throw new UserFriendlyException("用户名或密码不能为空.");
            }
            SysUser user = _userService.SelectUserByUserName(_userName);
            if (user is null)
            {
                throw new UserFriendlyException($"登录用户：{_userName} 不存在.");
            }
            else if (UserStatus.DELETED.GetStringValue().Equals(user.DelFlag))
            {
                throw new UserFriendlyException($"登录用户：{_userName} 已被删除.");
            }
            else if (UserStatus.DISABLE.GetStringValue().Equals(user.Status))
            {
                throw new UserFriendlyException($"登录用户：{_userName} 已被停用.");
            }
            if (BCrypt.Net.BCrypt.Verify(_password, user.Password))
            {
                var parameters = new NavigationParameters();
                parameters.Add("CurrentUser", user);
                _regionManager.RequestNavigate(RegionNames.ContentRegion, nameof(MainView), parameters);
            }
            else
            {
                throw new UserFriendlyException($"用户 {_userName} 登录失败，密码错误.");
            }
        }
    }
}