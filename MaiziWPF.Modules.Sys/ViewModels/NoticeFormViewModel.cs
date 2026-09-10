using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using Prism.Commands;
using System;

namespace MaiziWPF.Modules.Sys
{
    public class NoticeFormViewModel : FormBindableBase
    {
        private readonly ISysNoticeService _noticeService;
        private readonly IDialogHostService _dialogHostService;

        private bool _isEditMode;
        public bool IsEditMode
        {
            get { return _isEditMode; }
            set { SetProperty(ref _isEditMode, value); }
        }

        private long _noticeId;
        public long NoticeId
        {
            get { return _noticeId; }
            set { SetProperty(ref _noticeId, value); }
        }

        private string _noticeTitle;
        public string NoticeTitle
        {
            get { return _noticeTitle; }
            set { SetProperty(ref _noticeTitle, value); }
        }

        private string _noticeType = "1";
        public string NoticeType
        {
            get { return _noticeType; }
            set { SetProperty(ref _noticeType, value); }
        }

        private string _noticeContent;
        public string NoticeContent
        {
            get { return _noticeContent; }
            set { SetProperty(ref _noticeContent, value); }
        }

        private string _status = "0";
        public string Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }

        public NoticeFormViewModel(ISysNoticeService noticeService, IDialogHostService dialogHostService)
            : base(dialogHostService)
        {
            _noticeService = noticeService;
            _dialogHostService = dialogHostService;

            AcceptCommand = new DelegateCommand(() =>
            {
                SaveNotice();
            });
        }

        private async void SaveNotice()
        {
            if (string.IsNullOrWhiteSpace(NoticeTitle))
            {
                await _dialogHostService.AlertAsync("请输入公告标题", AlertType.Info);
                return;
            }
            if (string.IsNullOrWhiteSpace(NoticeContent))
            {
                await _dialogHostService.AlertAsync("请输入公告内容", AlertType.Info);
                return;
            }

            var notice = new SysNotice
            {
                NoticeId = NoticeId,
                NoticeTitle = NoticeTitle,
                NoticeType = NoticeType,
                NoticeContent = NoticeContent,
                Status = Status
            };

            try
            {
                if (IsEditMode)
                {
                    _noticeService.UpdateNotice(notice);
                }
                else
                {
                    _noticeService.InsertNotice(notice);
                }
                OnSaveSuccessCallback?.Invoke();
                await _dialogHostService.CloseDialogAsync();
            }
            catch (Exception ex)
            {
                await _dialogHostService.AlertAsync(ex.Message, AlertType.Error);
            }
        }
    }
}