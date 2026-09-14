using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace MaiziWPF.Modules.Sys
{
    public class OssListViewModel : PageBindableBase<SysOss, QueryOssInput>
    {
        private readonly ISysOssService _ossService;
        private readonly IDialogHostService _dialogHostService;
        private readonly ISnackbarService _snackbarService;

        public ICommand BatchDeleteCommand { get; }

        public OssListViewModel(ISysOssService ossService, IDialogHostService dialogHostService, ISnackbarService snackbarService)
        {
            _ossService = ossService;
            _dialogHostService = dialogHostService;
            _snackbarService = snackbarService;

            RegisterQueryFunc(input =>
            {
                return _ossService.SelectOssList(input);
            }, new QueryOssInput() { PageNumber = 1, PageSize = 10 });

            DeleteButtonCommand = new DelegateCommand<SysOss>(async (oss) =>
            {
                await DeleteOss(oss);
            });

            SearchButtonCommand.Execute(this);
        }

        private async System.Threading.Tasks.Task DeleteOss(SysOss oss)
        {
            if (oss == null) return;

            var result = await _dialogHostService.ConfirmAsync($"确定要删除文件【{oss.OriginalName}】吗？", "删除确认");
            if (!result) return;

            try
            {
                _ossService.DeleteOssById(oss.OssId);
                _snackbarService.EnqueueSuccess("删除成功");
                SearchButtonCommand.Execute(this);
            }
            catch (Exception ex)
            {
                _snackbarService.EnqueueError(ex.Message);
            }
        }
    }
}