using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace MaiziWPF.Modules.Sys
{
    public class OnlineUserListViewModel : PageBindableBase<SysUserOnline, QueryUserOnlineInput>
    {
        private readonly ISysUserOnlineService _onlineService;
        private readonly IDialogHostService _dialogHostService;
        private readonly ISnackbarService _snackbarService;

        public ICommand ForceLogoutCommand { get; }

        public OnlineUserListViewModel(ISysUserOnlineService onlineService, IDialogHostService dialogHostService, ISnackbarService snackbarService)
        {
            _onlineService = onlineService;
            _dialogHostService = dialogHostService;
            _snackbarService = snackbarService;

            RegisterQueryFunc(input =>
            {
                return _onlineService.SelectUserOnlineList(input);
            }, new QueryUserOnlineInput() { PageNumber = 1, PageSize = 10 });

            ForceLogoutCommand = new DelegateCommand<SysUserOnline>(async (user) =>
            {
                await ForceLogout(user);
            });

            DeleteButtonCommand = new DelegateCommand<SysUserOnline>(async (user) =>
            {
                await ForceLogout(user);
            });

            SearchButtonCommand.Execute(this);
        }

        private async System.Threading.Tasks.Task ForceLogout(SysUserOnline user)
        {
            if (user == null) return;

            var result = await _dialogHostService.ConfirmAsync($"确定要强退用户【{user.LoginName}】吗？", "强退确认");
            if (!result) return;

            try
            {
                _onlineService.DeleteOnlineById(user.SessionId);
                _snackbarService.EnqueueSuccess("强退成功");
                SearchButtonCommand.Execute(this);
            }
            catch (Exception ex)
            {
                _snackbarService.EnqueueError(ex.Message);
            }
        }
    }
}