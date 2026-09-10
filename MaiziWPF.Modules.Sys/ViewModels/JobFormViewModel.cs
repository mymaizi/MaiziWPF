using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using Prism.Commands;
using System;

namespace MaiziWPF.Modules.Sys
{
    public class JobFormViewModel : FormBindableBase
    {
        private readonly ISysJobService _jobService;
        private readonly IDialogHostService _dialogHostService;

        private bool _isEditMode;
        public bool IsEditMode
        {
            get { return _isEditMode; }
            set { SetProperty(ref _isEditMode, value); }
        }

        private long _jobId;
        public long JobId
        {
            get { return _jobId; }
            set { SetProperty(ref _jobId, value); }
        }

        private string _jobName;
        public string JobName
        {
            get { return _jobName; }
            set { SetProperty(ref _jobName, value); }
        }

        private string _jobGroup = "DEFAULT";
        public string JobGroup
        {
            get { return _jobGroup; }
            set { SetProperty(ref _jobGroup, value); }
        }

        private string _invokeTarget;
        public string InvokeTarget
        {
            get { return _invokeTarget; }
            set { SetProperty(ref _invokeTarget, value); }
        }

        private string _cronExpression;
        public string CronExpression
        {
            get { return _cronExpression; }
            set { SetProperty(ref _cronExpression, value); }
        }

        private string _misfirePolicy = "3";
        public string MisfirePolicy
        {
            get { return _misfirePolicy; }
            set { SetProperty(ref _misfirePolicy, value); }
        }

        private string _concurrent = "1";
        public string Concurrent
        {
            get { return _concurrent; }
            set { SetProperty(ref _concurrent, value); }
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

        public JobFormViewModel(ISysJobService jobService, IDialogHostService dialogHostService)
            : base(dialogHostService)
        {
            _jobService = jobService;
            _dialogHostService = dialogHostService;

            AcceptCommand = new DelegateCommand(() =>
            {
                SaveJob();
            });
        }

        private async void SaveJob()
        {
            if (string.IsNullOrWhiteSpace(JobName))
            {
                await _dialogHostService.AlertAsync("请输入任务名称", AlertType.Info);
                return;
            }
            if (string.IsNullOrWhiteSpace(InvokeTarget))
            {
                await _dialogHostService.AlertAsync("请输入调用目标", AlertType.Info);
                return;
            }
            if (string.IsNullOrWhiteSpace(CronExpression))
            {
                await _dialogHostService.AlertAsync("请输入Cron表达式", AlertType.Info);
                return;
            }

            if (!IsEditMode && !_jobService.CheckJobNameUnique(JobName))
            {
                await _dialogHostService.AlertAsync("任务名称已存在", AlertType.Info);
                return;
            }

            var job = new SysJob
            {
                JobId = JobId,
                JobName = JobName,
                JobGroup = JobGroup,
                InvokeTarget = InvokeTarget,
                CronExpression = CronExpression,
                MisfirePolicy = MisfirePolicy,
                Concurrent = Concurrent,
                Status = Status,
                Remark = Remark
            };

            try
            {
                if (IsEditMode)
                {
                    _jobService.UpdateJob(job);
                }
                else
                {
                    _jobService.InsertJob(job);
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