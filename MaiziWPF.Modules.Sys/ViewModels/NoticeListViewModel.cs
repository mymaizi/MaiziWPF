using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using Prism.Commands;
using Prism.Ioc;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace MaiziWPF.Modules.Sys
{
    public class NoticeListViewModel : PageBindableBase<SysNotice, QueryNoticeInput>
    {
        private readonly ISysNoticeService _noticeService;
        private readonly IContainerProvider _containerProvider;
        private readonly IDialogHostService _dialogHostService;
        private readonly ISnackbarService _snackbarService;

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

            AddButtonCommand = new DelegateCommand(async () => await AddNotice());
            EditButtonCommand = new DelegateCommand<SysNotice>(async (notice) => await EditNotice(notice));
            ViewButtonCommand = new DelegateCommand<SysNotice>(async (notice) => await ViewNotice(notice));
            DeleteButtonCommand = new DelegateCommand<SysNotice>(async (notice) => await DeleteNotice(notice));
            BatchDeleteCommand = new DelegateCommand<IList>(async (selectedItems) => await BatchDeleteNotices(selectedItems));
        }

        public DelegateCommand AddButtonCommand { get; }
        public DelegateCommand<SysNotice> EditButtonCommand { get; }
        public DelegateCommand<SysNotice> ViewButtonCommand { get; }
        public DelegateCommand<SysNotice> DeleteButtonCommand { get; }
        public DelegateCommand<IList> BatchDeleteCommand { get; }

        private async Task AddNotice()
        {
            await _dialogHostService.ShowDialogAsync<NoticeFormView>(vm =>
            {
                var form = (NoticeFormViewModel)vm;
                form.DialogTitle = "新增公告";
                form.IsEditMode = false;
                form.NoticeId = 0;
                form.OnSaveSuccessCallback = () =>
                {
                    SearchButtonCommand.Execute(this);
                };
            });
        }

        private async Task EditNotice(SysNotice notice)
        {
            if (notice == null) return;

            await _dialogHostService.ShowDialogAsync<NoticeFormView>(vm =>
            {
                var form = (NoticeFormViewModel)vm;
                form.DialogTitle = "修改公告";
                form.IsEditMode = true;
                form.NoticeId = notice.NoticeId;
                form.NoticeTitle = notice.NoticeTitle;
                form.NoticeType = notice.NoticeType;
                form.NoticeContent = notice.NoticeContent;
                form.Status = notice.Status;
                form.Remark = notice.Remark;
                form.OnSaveSuccessCallback = () =>
                {
                    SearchButtonCommand.Execute(this);
                };
            });
        }

        private async Task ViewNotice(SysNotice notice)
        {
            if (notice == null) return;

            await _dialogHostService.ShowDialogAsync<NoticeDetailView>(vm =>
            {
                var detail = (NoticeDetailViewModel)vm;
                detail.SetNotice(notice);
            });
        }

        private async Task DeleteNotice(SysNotice notice)
        {
            if (notice == null) return;

            try
            {
                var result = await _dialogHostService.ConfirmAsync($"确定要删除公告 '{notice.NoticeTitle}' 吗？", "确认删除");
                if (result)
                {
                    _noticeService.DeleteNoticeById(notice.NoticeId);
                    _snackbarService.EnqueueSuccess("删除成功");
                    SearchButtonCommand.Execute(this);
                }
            }
            catch (Exception ex)
            {
                _snackbarService.EnqueueError($"删除失败：{ex.Message}");
            }
        }

        private async Task BatchDeleteNotices(IList selectedItems)
        {
            if (selectedItems == null || selectedItems.Count == 0)
            {
                _snackbarService.EnqueueWarning("请先选择要删除的公告");
                return;
            }

            var notices = selectedItems.Cast<SysNotice>().ToList();

            var titles = string.Join("、", notices.Select(n => n.NoticeTitle));
            var result = await _dialogHostService.ConfirmAsync($"确定要删除选中的 {notices.Count} 条公告（{titles}）吗？", "确认批量删除");
            if (result)
            {
                try
                {
                    foreach (var notice in notices)
                        _noticeService.DeleteNoticeById(notice.NoticeId);
                    _snackbarService.EnqueueSuccess($"成功删除 {notices.Count} 条公告");
                    SearchButtonCommand.Execute(this);
                }
                catch (Exception ex)
                {
                    _snackbarService.EnqueueError($"批量删除失败：{ex.Message}");
                }
            }
        }
    }
}