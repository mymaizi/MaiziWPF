using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using Prism.Commands;
using System;
using System.Collections;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MaiziWPF.Modules.Sys
{
    public class OssListViewModel : PageBindableBase<SysOss, QueryOssInput>
    {
        private readonly ISysOssService _ossService;
        private readonly IDialogHostService _dialogHostService;
        private readonly ISnackbarService _snackbarService;

        private string _fileName;
        public string FileName
        {
            get => _fileName;
            set => SetProperty(ref _fileName, value);
        }

        private string _originalName;
        public string OriginalName
        {
            get => _originalName;
            set => SetProperty(ref _originalName, value);
        }

        private string _fileSuffix;
        public string FileSuffix
        {
            get => _fileSuffix;
            set => SetProperty(ref _fileSuffix, value);
        }

        private string _service;
        public string Service
        {
            get => _service;
            set => SetProperty(ref _service, value);
        }

        public ICommand BatchDeleteCommand { get; }

        public OssListViewModel(ISysOssService ossService, IDialogHostService dialogHostService, ISnackbarService snackbarService)
        {
            _ossService = ossService;
            _dialogHostService = dialogHostService;
            _snackbarService = snackbarService;

            RegisterQueryFunc(input =>
            {
                input.FileName = FileName;
                input.OriginalName = OriginalName;
                input.FileSuffix = FileSuffix;
                input.Service = Service;
                return _ossService.SelectOssList(input);
            }, new QueryOssInput() { PageNumber = 1, PageSize = 10 });

            DeleteButtonCommand = new DelegateCommand<SysOss>(async (oss) => await DeleteOss(oss));
            BatchDeleteCommand = new DelegateCommand<IList>(async (selectedItems) => await BatchDeleteOss(selectedItems));
            ResetButtonCommand = new DelegateCommand(ResetQuery);
        }

        private void ResetQuery()
        {
            FileName = string.Empty;
            OriginalName = string.Empty;
            FileSuffix = string.Empty;
            Service = string.Empty;
            SearchButtonCommand.Execute(this);
        }

        private async Task DeleteOss(SysOss oss)
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

        private async Task BatchDeleteOss(IList selectedItems)
        {
            if (selectedItems == null || selectedItems.Count == 0)
            {
                _snackbarService.EnqueueWarning("请选择要删除的文件");
                return;
            }

            var result = await _dialogHostService.ConfirmAsync($"确定要删除选中的 {selectedItems.Count} 个文件吗？", "批量删除确认");
            if (!result) return;

            try
            {
                var ids = new System.Collections.Generic.List<long>();
                foreach (SysOss item in selectedItems)
                {
                    ids.Add(item.OssId);
                }
                _ossService.DeleteOssByIds(ids);
                _snackbarService.EnqueueSuccess("批量删除成功");
                SearchButtonCommand.Execute(this);
            }
            catch (Exception ex)
            {
                _snackbarService.EnqueueError(ex.Message);
            }
        }
    }
}