using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using Prism.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MaiziWPF.Modules.Sys
{
    public class OperLogListViewModel : PageBindableBase<SysOperLog, QueryOperLogInput>
    {
        private readonly ISysOperLogService _operLogService;
        private readonly IDialogHostService _dialogHostService;
        private readonly ISnackbarService _snackbarService;

        public ICommand CleanLogCommand { get; }
        public ICommand BatchDeleteCommand { get; }
        public ICommand ViewDetailCommand { get; }

        public OperLogListViewModel(ISysOperLogService operLogService, IDialogHostService dialogHostService, ISnackbarService snackbarService)
        {
            _operLogService = operLogService;
            _dialogHostService = dialogHostService;
            _snackbarService = snackbarService;

            RegisterQueryFunc(input =>
            {
                return _operLogService.SelectOperLogList(input);
            }, new QueryOperLogInput() { PageNumber = 1, PageSize = 10 });

            BatchDeleteCommand = new DelegateCommand<IList>(async (selectedItems) => await BatchDeleteLogs(selectedItems));

            ViewDetailCommand = new DelegateCommand<SysOperLog>(async (log) => await ViewDetail(log));

            CleanLogCommand = new DelegateCommand(async () =>
            {
                await CleanLog();
            });
        }

        private async Task BatchDeleteLogs(IList selectedItems)
        {
            if (selectedItems == null || selectedItems.Count == 0)
            {
                _snackbarService.EnqueueWarning("请先选择要删除的日志");
                return;
            }

            var logs = selectedItems.Cast<SysOperLog>().ToList();
            var result = await _dialogHostService.ConfirmAsync($"确定要删除选中的 {logs.Count} 条操作日志吗？", "确认删除");
            if (!result) return;

            try
            {
                var ids = logs.Select(x => x.OperId).ToArray();
                _operLogService.DeleteOperLogByIds(ids);
                _snackbarService.EnqueueSuccess("批量删除成功");
                SearchButtonCommand.Execute(this);
            }
            catch (Exception ex)
            {
                _snackbarService.EnqueueError($"删除失败：{ex.Message}");
            }
        }

        private async Task ViewDetail(SysOperLog log)
        {
            if (log == null) return;

            await _dialogHostService.ShowDialogAsync<OperLogDetailView>(vm =>
            {
                var detailVm = (OperLogDetailViewModel)vm;
                detailVm.DialogTitle = "操作日志详情";
                detailVm.SetOperLog(log);
            });
        }

        private async Task CleanLog()
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