using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using Prism.Commands;
using Prism.Ioc;
using System;
using System.Windows.Input;

namespace MaiziWPF.Modules.Sys
{
    public class JobListViewModel : PageBindableBase<SysJob, QueryJobInput>
    {
        private readonly ISysJobService _jobService;
        private readonly IContainerProvider _containerProvider;
        private readonly IDialogHostService _dialogHostService;

        public ICommand AddJobCommand { get; }

        public JobListViewModel(ISysJobService jobService, IContainerProvider containerProvider, IDialogHostService dialogHostService)
        {
            _jobService = jobService;
            _containerProvider = containerProvider;
            _dialogHostService = dialogHostService;

            RegisterQueryFunc(input =>
            {
                return _jobService.SelectJobList(input);
            }, new QueryJobInput() { PageNumber = 1, PageSize = 10 });

            AddJobCommand = new DelegateCommand(() =>
            {
                OpenJobForm(null);
            });

            NewOrEditButtonCommand = new DelegateCommand<SysJob>((job) =>
            {
                OpenJobForm(job);
            });

            DeleteButtonCommand = new DelegateCommand<SysJob>(async (job) =>
            {
                await DeleteJob(job);
            });

            SearchButtonCommand.Execute(this);
        }

        private void OpenJobForm(SysJob job)
        {
            var view = _containerProvider.Resolve<JobFormView>();
            var model = view.DataContext as JobFormViewModel;

            if (job != null)
            {
                model.IsEditMode = true;
                model.JobId = job.JobId;
                model.JobName = job.JobName;
                model.JobGroup = job.JobGroup;
                model.InvokeTarget = job.InvokeTarget;
                model.CronExpression = job.CronExpression;
                model.MisfirePolicy = job.MisfirePolicy;
                model.Concurrent = job.Concurrent;
                model.Status = job.Status;
                model.Remark = job.Remark;
            }
            else
            {
                model.IsEditMode = false;
            }

            model.OnSaveSuccessCallback = () =>
            {
                SearchButtonCommand.Execute(this);
            };

            view.DataContext = model;
            _dialogHostService.ShowDialogAsync(view, autoClose: false);
        }

        private async System.Threading.Tasks.Task DeleteJob(SysJob job)
        {
            if (job == null) return;

            var result = await _dialogHostService.ConfirmAsync($"确定要删除任务【{job.JobName}】吗？", "删除确认");
            if (!result) return;

            try
            {
                _jobService.DeleteJobById(job.JobId);
                await _dialogHostService.AlertAsync("删除成功", AlertType.Info);
                SearchButtonCommand.Execute(this);
            }
            catch (Exception ex)
            {
                await _dialogHostService.AlertAsync(ex.Message, AlertType.Error);
            }
        }
    }
}