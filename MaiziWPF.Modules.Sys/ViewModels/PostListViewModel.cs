using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using Prism.Commands;
using Prism.Ioc;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MaiziWPF.Modules.Sys
{
    public class PostListViewModel : PageBindableBase<SysPost, QueryPostInput>
    {
        private readonly ISysPostService _postService;
        private readonly ISnackbarService _snackbarService;
        private readonly IDialogHostService _dialogHostService;
        private readonly IContainerProvider _containerProvider;

        public PostListViewModel(ISysPostService postService, ISnackbarService snackbarService, IDialogHostService dialogHostService, IContainerProvider containerProvider)
        {
            _postService = postService;
            _snackbarService = snackbarService;
            _dialogHostService = dialogHostService;
            _containerProvider = containerProvider;

            SearchButtonCommand = new DelegateCommand<PostListViewModel>((vm) =>
            {
                LoadDataList();
            });

            ResetButtonCommand = new DelegateCommand<PostListViewModel>((vm) =>
            {
                QueryPageInfo = new QueryPostInput();
                LoadDataList();
            });

            AddButtonCommand = new DelegateCommand<PostListViewModel>(async (vm) =>
            {
                await AddPost();
            });

            EditButtonCommand = new DelegateCommand<SysPost>(async (post) =>
            {
                await EditPost(post);
            });

            DeleteButtonCommand = new DelegateCommand<SysPost>(async (post) =>
            {
                await DeletePost(post);
            });
        }

        public DelegateCommand<PostListViewModel> SearchButtonCommand { get; }
        public DelegateCommand<PostListViewModel> ResetButtonCommand { get; }
        public DelegateCommand<PostListViewModel> AddButtonCommand { get; }
        public DelegateCommand<SysPost> EditButtonCommand { get; }
        public DelegateCommand<SysPost> DeleteButtonCommand { get; }

        public override void LoadDataList()
        {
            DataList = new ObservableCollection<SysPost>(_postService.SelectPostList(QueryPageInfo));
        }

        private async Task AddPost()
        {
            await _dialogHostService.ShowDialogAsync<PostFormView>(view =>
            {
                var vm = view.DataContext as PostFormViewModel;
                vm.IsEditMode = false;
                vm.PostId = 0;
                vm.OnSaveSuccessCallback = () =>
                {
                    LoadDataList();
                };
            });
        }

        private async Task EditPost(SysPost post)
        {
            if (post == null) return;

            await _dialogHostService.ShowDialogAsync<PostFormView>(view =>
            {
                var vm = view.DataContext as PostFormViewModel;
                vm.IsEditMode = true;
                vm.PostId = post.PostId;
                vm.PostCode = post.PostCode;
                vm.PostName = post.PostName;
                vm.PostSort = post.PostSort;
                vm.Status = post.Status;
                vm.Remark = post.Remark;
                vm.OnSaveSuccessCallback = () =>
                {
                    LoadDataList();
                };
            });
        }

        private async Task DeletePost(SysPost post)
        {
            if (post == null) return;

            var result = await _dialogHostService.ConfirmAsync($"确定要删除岗位 '{post.PostName}' 吗？", "确认删除");
            if (result)
            {
                try
                {
                    _postService.DeletePostById(post.PostId);
                    _snackbarService.EnqueueSuccess("删除成功");
                    LoadDataList();
                }
                catch (System.Exception ex)
                {
                    _snackbarService.EnqueueError($"删除失败：{ex.Message}");
                }
            }
        }
    }
}