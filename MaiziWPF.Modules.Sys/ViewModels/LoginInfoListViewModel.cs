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
    public class LoginInfoListViewModel : PageBindableBase<SysLogininfo, QueryLoginInfoInput>
    {
        private readonly ISysLogininforService _logininforService;
        private readonly IDialogHostService _dialogHostService;
        private readonly ISnackbarService _snackbarService;

        public ICommand CleanLogCommand { get; }
        public ICommand BatchDeleteCommand { get; }

        public LoginInfoListViewModel(ISysLogininforService logininforService, IDialogHostService dialogHostService, ISnackbarService snackbarService)
        {
            _logininforService = logininforService;
            _dialogHostService = dialogHostService;
            _snackbarService = snackbarService;

            RegisterQueryFunc(input =>
            {
                return _logininforService.SelectLogininforList(input);
            }, new QueryLoginInfoInput() { PageNumber = 1, PageSize = 10 });

            BatchDeleteCommand = new DelegateCommand<IList>(async (selectedItems) => await BatchDeleteLogs(selectedItems));

            CleanLogCommand = new DelegateCommand(async () =>
            {
                await CleanLog();
            });

            SearchButtonCommand.Execute(this);
        }

        private async Task BatchDeleteLogs(IList selectedItems)
        {
            if (selectedItems == null || selectedItems.Count == 0)
            {
                _snackbarService.EnqueueWarning("请先选择要删除的日志");
                return;
            }

            var logs = selectedItems.Cast<SysLogininfo>().ToList();
            var result = await _dialogHostService.ConfirmAsync($"确定要删除选中的 {logs.Count} 条登录日志吗？", "确认删除");
            if (!result) return;

            try
            {
                var ids = logs.Select(x => x.InfoId).ToArray();
                _logininforService.DeleteLogininforByIds(ids);
                _snackbarService.EnqueueSuccess("批量删除成功");
                SearchButtonCommand.Execute(this);
            }
            catch (Exception ex)
            {
                _snackbarService.EnqueueError($"删除失败：{ex.Message}");
            }
        }

        private async Task CleanLog()
        {
            var result = await _dialogHostService.ConfirmAsync("确定要清空所有登录日志吗？", "清空确认");
            if (!result) return;

            try
            {
                _logininforService.CleanLogininfor();
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