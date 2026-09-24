using MaiziWPF.Common.Oss;
using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using Prism.Commands;
using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MaiziWPF.Modules.Sys
{
    public class OssListViewModel : PageBindableBase<SysOss, QueryOssInput>
    {
        private readonly ISysOssService _ossService;
        private readonly IDialogHostService _dialogHostService;
        private readonly ISnackbarService _snackbarService;
        private readonly IS3ClientService _s3Client;

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
        public ICommand UploadCommand { get; }
        public ICommand DownloadCommand { get; }

        public OssListViewModel(ISysOssService ossService, IDialogHostService dialogHostService, ISnackbarService snackbarService, IS3ClientService s3Client)
        {
            _ossService = ossService;
            _dialogHostService = dialogHostService;
            _snackbarService = snackbarService;
            _s3Client = s3Client;

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
            UploadCommand = new DelegateCommand(async () => await OpenUploadDialog());
            DownloadCommand = new DelegateCommand<SysOss>(async (oss) => await DownloadOss(oss));
            ResetButtonCommand = new DelegateCommand(ResetQuery);
        }

        private async Task OpenUploadDialog()
        {
            await _dialogHostService.ShowDialogAsync<OssUploadView>();
            SearchButtonCommand.Execute(this);
        }

        private async Task DownloadOss(SysOss oss)
        {
            if (oss == null) return;

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = oss.OriginalName,
                Title = "保存文件"
            };

            if (dialog.ShowDialog() != true) return;

            try
            {
                using var stream = await _s3Client.DownloadAsync(oss.Url);
                using var fileStream = File.Create(dialog.FileName);
                await stream.CopyToAsync(fileStream);
                _snackbarService.EnqueueSuccess("下载成功");
            }
            catch (Exception ex)
            {
                _snackbarService.EnqueueError($"下载失败: {ex.Message}");
            }
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