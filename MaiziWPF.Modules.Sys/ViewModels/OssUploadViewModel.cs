using MaiziWPF.Common.Oss;
using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using Microsoft.Extensions.Logging;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MaiziWPF.Modules.Sys
{
    public class OssUploadViewModel : FormBindableBase
    {
        private readonly IS3ClientService _s3Client;
        private readonly ISysOssService _ossService;
        private readonly ILogger<OssUploadViewModel> _logger;

        private ObservableCollection<UploadItem> _uploadItems = new();
        public ObservableCollection<UploadItem> UploadItems
        {
            get => _uploadItems;
            set => SetProperty(ref _uploadItems, value);
        }

        private bool _isUploading;
        public bool IsUploading
        {
            get => _isUploading;
            set => SetProperty(ref _isUploading, value);
        }

        public ICommand SelectFilesCommand { get; }
        public ICommand UploadCommand { get; }

        public OssUploadViewModel(IS3ClientService s3Client, ISysOssService ossService, ISnackbarService snackbarService, ILogger<OssUploadViewModel> logger)
            : base(snackbarService)
        {
            _s3Client = s3Client;
            _ossService = ossService;
            _logger = logger;

            SelectFilesCommand = new DelegateCommand(SelectFiles);
            UploadCommand = new DelegateCommand(async () => await UploadFiles(), () => !IsUploading && UploadItems.Count > 0)
                .ObservesProperty(() => IsUploading)
                .ObservesProperty(() => UploadItems.Count);
        }

        private void SelectFiles()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true,
                Title = "选择要上传的文件",
                Filter = "图片及文档|*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.svg;*.webp;*.pdf;*.doc;*.docx;*.xls;*.xlsx;*.ppt;*.pptx;*.txt;*.csv|图片|*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.svg;*.webp|文档|*.pdf;*.doc;*.docx;*.xls;*.xlsx;*.ppt;*.pptx;*.txt;*.csv"
            };

            if (dialog.ShowDialog() == true)
            {
                foreach (var file in dialog.FileNames)
                {
                    var info = new FileInfo(file);
                    UploadItems.Add(new UploadItem
                    {
                        FilePath = file,
                        FileName = info.Name,
                        FileSize = info.Length,
                        Status = "待上传"
                    });
                }
            }
        }

        private async Task UploadFiles()
        {
            IsUploading = true;
            int successCount = 0;
            int failCount = 0;

            try
            {
                await _s3Client.EnsureBucketExistsAsync();

                foreach (var item in UploadItems)
                {
                    if (item.Status == "上传成功") continue;

                    item.Status = "上传中...";
                    item.Progress = 0;

                    var progress = new Progress<int>(percent =>
                    {
                        item.Progress = Math.Min(percent, 99);
                    });

                    try
                    {
                        var result = await _s3Client.UploadAsync(item.FilePath, progress: progress);

                        if (result.Success)
                        {
                            SaveOssRecord(result);
                            item.Progress = 100;
                            item.Status = "上传成功";
                            item.Key = result.Key;
                            successCount++;
                            _logger.LogInformation("文件上传成功: {FileName} -> {Key}", item.FileName, result.Key);
                        }
                        else
                        {
                            item.Status = $"失败: {result.ErrorMessage}";
                            failCount++;
                            _logger.LogError("文件上传失败: {FileName}, 原因: {Error}", item.FileName, result.ErrorMessage);
                        }
                    }
                    catch (Exception ex)
                    {
                        item.Status = $"失败: {ex.Message}";
                        failCount++;
                        _logger.LogError(ex, "文件上传异常: {FileName}", item.FileName);
                    }
                }

                if (successCount > 0)
                    ShowSuccess($"上传完成: 成功 {successCount} 个, 失败 {failCount} 个");
                else if (failCount > 0)
                    ShowError($"全部上传失败: {failCount} 个");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "上传过程发生未预期异常");
                ShowError($"上传异常: {ex.Message}");
            }
            finally
            {
                IsUploading = false;
            }
        }

        private void SaveOssRecord(OssUploadResult result)
        {
            try
            {
                var oss = new SysOss
                {
                    FileName = result.Key,
                    OriginalName = result.OriginalName,
                    FileSuffix = result.FileSuffix,
                    Url = result.Key,
                    Service = "rustfs",
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    DelFlag = "0"
                };
                _ossService.InsertOss(oss);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "保存上传记录失败: {Key}", result.Key);
            }
        }
    }

    public class UploadItem : BindableBase
    {
        public string FilePath { get; set; } = "";
        public string FileName { get; set; } = "";

        private string _status = "待上传";
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        private int _progress;
        public int Progress
        {
            get => _progress;
            set
            {
                SetProperty(ref _progress, value);
                RaisePropertyChanged(nameof(ProgressVisibility));
            }
        }

        public System.Windows.Visibility ProgressVisibility =>
            Progress > 0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

        public long FileSize { get; set; }
        public string Key { get; set; } = "";

        public string FileSizeDisplay
        {
            get
            {
                if (FileSize < 1024) return $"{FileSize} B";
                if (FileSize < 1024 * 1024) return $"{FileSize / 1024.0:F1} KB";
                if (FileSize < 1024 * 1024 * 1024) return $"{FileSize / (1024.0 * 1024):F1} MB";
                return $"{FileSize / (1024.0 * 1024 * 1024):F1} GB";
            }
        }
    }
}