using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using Prism.Commands;
using System;
using System.Windows.Input;

namespace MaiziWPF.Modules.Sys
{
    public class OperLogListViewModel : PageBindableBase<SysOperLog, QueryOperLogInput>
    {
        private readonly ISysOperLogService _operLogService;
        private readonly IDialogHostService _dialogHostService;

        public ICommand CleanLogCommand { get; }

        public OperLogListViewModel(ISysOperLogService operLogService, IDialogHostService dialogHostService)
        {
            _operLogService = operLogService;
            _dialogHostService = dialogHostService;

            RegisterQueryFunc(input =>
            {
                return _operLogService.SelectOperLogList(input);
            }, new QueryOperLogInput() { PageNumber = 1, PageSize = 10 });

            DeleteButtonCommand = new DelegateCommand<SysOperLog>(async (log) =>
            {
                await DeleteLog(log);
            });

            CleanLogCommand = new DelegateCommand(async () =>
            {
                await CleanLog();
            });

            SearchButtonCommand.Execute(this);
        }

        private async System.Threading.Tasks.Task DeleteLog(SysOperLog log)
        {
            if (log == null) return;

            var result = await _dialogHostService.ConfirmAsync($"确定要删除该操作日志吗？", "删除确认");
            if (!result) return;

            try
            {
                _operLogService.DeleteOperLogById(log.OperId);
                SearchButtonCommand.Execute(this);
            }
            catch (Exception ex)
            {
                await _dialogHostService.AlertAsync(ex.Message, AlertType.Error);
            }
        }

        private async System.Threading.Tasks.Task CleanLog()
        {
            var result = await _dialogHostService.ConfirmAsync("确定要清空所有操作日志吗？", "清空确认");
            if (!result) return;

            try
            {
                _operLogService.CleanOperLog();
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