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
    public class NoticeListViewModel : PageBindableBase<SysNotice, QueryNoticeInput>
    {
        private readonly ISysNoticeService _noticeService;
        private readonly IContainerProvider _containerProvider;
        private readonly IDialogHostService _dialogHostService;
        private readonly ISnackbarService _snackbarService;

        public ICommand AddNoticeCommand { get; }

        public NoticeListViewModel(ISysNoticeService noticeService, IContainerProvider containerProvider, IDialogHostService dialogHostService, ISnackbarService snackbarService)
        {
            _noticeService = noticeService;
            _containerProvider = containerProvider;
            _dialogHostService = dialogHostService;
            _snackbarService = snackbarService;

            RegisterQueryFunc(input =>
            {
                return _noticeService.SelectNoticeList(input);
            }, new QueryNoticeInput() { PageNumber = 1, PageSize = 10 });

            AddNoticeCommand = new DelegateCommand(() =>
            {
                OpenNoticeForm(null);
            });

            NewOrEditButtonCommand = new DelegateCommand<SysNotice>((notice) =>
            {
                OpenNoticeForm(notice);
            });

            DeleteButtonCommand = new DelegateCommand<SysNotice>(async (notice) =>
            {
                await DeleteNotice(notice);
            });

            SearchButtonCommand.Execute(this);
        }

        private async System.Threading.Tasks.Task OpenNoticeForm(SysNotice notice)
        {
            await _dialogHostService.ShowDialogAsync<NoticeFormView>(vm =>
            {
                var model = (NoticeFormViewModel)vm;

                if (notice != null)
                {
                    model.IsEditMode = true;
                    model.NoticeId = notice.NoticeId;
                    model.NoticeTitle = notice.NoticeTitle;
                    model.NoticeType = notice.NoticeType;
                    model.NoticeContent = notice.NoticeContent;
                    model.Status = notice.Status;
                }
                else
                {
                    model.IsEditMode = false;
                }

                model.OnSaveSuccessCallback = () =>
                {
                    SearchButtonCommand.Execute(this);
                };
            });
        }

        private async System.Threading.Tasks.Task DeleteNotice(SysNotice notice)
        {
            if (notice == null) return;

            var result = await _dialogHostService.ConfirmAsync($"确定要删除公告【{notice.NoticeTitle}】吗？", "删除确认");
            if (!result) return;

            try
            {
                _noticeService.DeleteNoticeById(notice.NoticeId);
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