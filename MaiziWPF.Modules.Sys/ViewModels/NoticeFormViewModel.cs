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

        private string _remark;
        public string Remark
        {
            get { return _remark; }
            set { SetProperty(ref _remark, value); }
        }

        public NoticeFormViewModel(ISysNoticeService noticeService, ISnackbarService snackbarService)
            : base(snackbarService)
        {
            _noticeService = noticeService;

            AcceptCommand = new DelegateCommand(() =>
            {
                SaveNotice();
            });
        }

        private async void SaveNotice()
        {
            if (string.IsNullOrWhiteSpace(NoticeTitle))
            {
                ShowWarning("请输入公告标题");
                return;
            }

            var notice = new SysNotice
            {
                NoticeId = NoticeId,
                NoticeTitle = NoticeTitle,
                NoticeType = NoticeType,
                NoticeContent = NoticeContent,
                Status = Status,
                Remark = Remark
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
                ShowSuccess("保存成功");
                CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
    }
}