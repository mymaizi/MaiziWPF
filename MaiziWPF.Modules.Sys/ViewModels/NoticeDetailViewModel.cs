using MaiziWPF.Core;
using MaiziWPF.Services.Domain;

namespace MaiziWPF.Modules.Sys
{
    public class NoticeDetailViewModel : FormBindableBase
    {
        private SysNotice _notice;
        public SysNotice Notice
        {
            get => _notice;
            set => SetProperty(ref _notice, value);
        }

        public string NoticeTitle => Notice?.NoticeTitle;
        public string NoticeType => Notice?.NoticeType;
        public string NoticeContent => Notice?.NoticeContent;
        public string Status => Notice?.Status;
        public string CreateBy => Notice?.CreateUser?.UserName;
        public System.DateTime? CreateTime => Notice?.CreateTime;

        public NoticeDetailViewModel(ISnackbarService snackbarService) : base(snackbarService)
        {
        }

        public void SetNotice(SysNotice notice)
        {
            _notice = notice;
            RaisePropertyChanged(nameof(NoticeTitle));
            RaisePropertyChanged(nameof(NoticeType));
            RaisePropertyChanged(nameof(NoticeContent));
            RaisePropertyChanged(nameof(Status));
            RaisePropertyChanged(nameof(CreateBy));
            RaisePropertyChanged(nameof(CreateTime));
        }
    }
}