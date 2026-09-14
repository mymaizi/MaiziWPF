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
        private readonly ISnackbarService _snackbarService;

        public ICommand CleanLogCommand { get; }

        public OperLogListViewModel(ISysOperLogService operLogService, IDialogHostService dialogHostService, ISnackbarService snackbarService)
        {
            _operLogService = operLogService;
            _dialogHostService = dialogHostService;
            _snackbarService = snackbarService;

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
                _snackbarService.EnqueueSuccess("删除成功");
                SearchButtonCommand.Execute(this);
            }
            catch (Exception ex)
            {
                _snackbarService.EnqueueError(ex.Message);
            }
        }

        private async System.Threading.Tasks.Task CleanLog()
        {
            var result = await _dialogHostService.ConfirmAsync("确定要清空所有操作日志吗？", "清空确认");
            if (!result) return;

            try
            {
                _operLogService.CleanOperLog();
                _snackbarService.EnqueueSuccess("清空成功");
                SearchButtonCommand.Execute(this);
            }
            catch (Exception ex)
            {
                _snackbarService.EnqueueError(ex.Message);
            }
        }
    }
}