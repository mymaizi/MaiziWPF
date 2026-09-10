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
    public class PostListViewModel : PageBindableBase<SysPost, QueryPostInput>
    {
        private readonly ISysPostService _postService;
        private readonly IContainerProvider _containerProvider;
        private readonly IDialogHostService _dialogHostService;

        public ICommand AddPostCommand { get; }

        public PostListViewModel(ISysPostService postService, IContainerProvider containerProvider, IDialogHostService dialogHostService)
        {
            _postService = postService;
            _containerProvider = containerProvider;
            _dialogHostService = dialogHostService;

            RegisterQueryFunc(input =>
            {
                return _postService.SelectPostList(input);
            }, new QueryPostInput() { PageNumber = 1, PageSize = 10 });

            AddPostCommand = new DelegateCommand(() =>
            {
                OpenPostForm(null);
            });

            NewOrEditButtonCommand = new DelegateCommand<SysPost>((post) =>
            {
                OpenPostForm(post);
            });

            DeleteButtonCommand = new DelegateCommand<SysPost>(async (post) =>
            {
                await DeletePost(post);
            });

            SearchButtonCommand.Execute(this);
        }

        private void OpenPostForm(SysPost post)
        {
            var view = _containerProvider.Resolve<PostFormView>();
            var model = view.DataContext as PostFormViewModel;

            if (post != null)
            {
                model.IsEditMode = true;
                model.PostId = post.PostId;
                model.PostCode = post.PostCode;
                model.PostName = post.PostName;
                model.PostSort = post.PostSort;
                model.Status = post.Status;
                model.Remark = post.Remark;
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

        private async System.Threading.Tasks.Task DeletePost(SysPost post)
        {
            if (post == null) return;

            if (_postService.CheckPostExistUser(post.PostId))
            {
                await _dialogHostService.AlertAsync("岗位已分配用户,不允许删除", AlertType.Info);
                return;
            }

            var result = await _dialogHostService.ConfirmAsync($"确定要删除岗位【{post.PostName}】吗？", "删除确认");
            if (!result) return;

            try
            {
                _postService.DeletePostById(post.PostId);
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