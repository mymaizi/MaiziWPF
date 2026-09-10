using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using Prism.Commands;
using System;
using System.Windows.Input;

namespace MaiziWPF.Modules.Sys
{
    public class LoginLogListViewModel : PageBindableBase<SysLogininfor, QueryLogininforInput>
    {
        private readonly ISysLogininforService _logininforService;
        private readonly IDialogHostService _dialogHostService;

        public ICommand CleanLogCommand { get; }

        public LoginLogListViewModel(ISysLogininforService logininforService, IDialogHostService dialogHostService)
        {
            _logininforService = logininforService;
            _dialogHostService = dialogHostService;

            RegisterQueryFunc(input =>
            {
                return _logininforService.SelectLogininforList(input);
            }, new QueryLogininforInput() { PageNumber = 1, PageSize = 10 });

            DeleteButtonCommand = new DelegateCommand<SysLogininfor>(async (log) =>
            {
                await DeleteLog(log);
            });

            CleanLogCommand = new DelegateCommand(async () =>
            {
                await CleanLog();
            });

            SearchButtonCommand.Execute(this);
        }

        private async System.Threading.Tasks.Task DeleteLog(SysLogininfor log)
        {
            if (log == null) return;

            var result = await _dialogHostService.ConfirmAsync($"确定要删除该登录日志吗？", "删除确认");
            if (!result) return;

            try
            {
                _logininforService.DeleteLogininforById(log.InfoId);
                SearchButtonCommand.Execute(this);
            }
            catch (Exception ex)
            {
                await _dialogHostService.AlertAsync(ex.Message, AlertType.Error);
            }
        }

        private async System.Threading.Tasks.Task CleanLog()
        {
            var result = await _dialogHostService.ConfirmAsync("确定要清空所有登录日志吗？", "清空确认");
            if (!result) return;

            try
            {
                _logininforService.CleanLogininfor();
                await _dialogHostService.AlertAsync("清空成功", AlertType.Info);
                SearchButtonCommand.Execute(this);
            }
            catch (Exception ex)
            {
                await _dialogHostService.AlertAsync(ex.Message, AlertType.Error);
            }
        }
    }
}