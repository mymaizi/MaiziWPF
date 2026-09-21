using MaiziWPF.Core;
using MaiziWPF.Services.Domain;

namespace MaiziWPF.Modules.Sys
{
    public class OperLogDetailViewModel : FormBindableBase
    {
        private SysOperLog _operLog;
        public SysOperLog OperLog
        {
            get => _operLog;
            set => SetProperty(ref _operLog, value);
        }

        public string Title => OperLog?.Title;
        public string OperName => OperLog?.OperName;
        public string DeptName => OperLog?.DeptName;
        public string Method => OperLog?.Method;
        public string RequestMethod => OperLog?.RequestMethod;
        public string OperUrl => OperLog?.OperUrl;
        public string OperIp => OperLog?.OperIp;
        public string OperLocation => OperLog?.OperLocation;
        public int Status => OperLog?.Status ?? 0;
        public System.DateTime? OperTime => OperLog?.OperTime;
        public long CostTime => OperLog?.CostTime ?? 0;
        public string OperParam => OperLog?.OperParam;

        public OperLogDetailViewModel(ISnackbarService snackbarService) : base(snackbarService)
        {
        }

        public void SetOperLog(SysOperLog operLog)
        {
            _operLog = operLog;
            RaisePropertyChanged(nameof(Title));
            RaisePropertyChanged(nameof(OperName));
            RaisePropertyChanged(nameof(DeptName));
            RaisePropertyChanged(nameof(Method));
            RaisePropertyChanged(nameof(RequestMethod));
            RaisePropertyChanged(nameof(OperUrl));
            RaisePropertyChanged(nameof(OperIp));
            RaisePropertyChanged(nameof(OperLocation));
            RaisePropertyChanged(nameof(Status));
            RaisePropertyChanged(nameof(OperTime));
            RaisePropertyChanged(nameof(CostTime));
            RaisePropertyChanged(nameof(OperParam));
        }
    }
}