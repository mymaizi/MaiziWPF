﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using Prism.Commands;
using Prism.Ioc;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MaiziWPF.Modules.Sys
{
    public class PostListViewModel : PageBindableBase<SysPost, QueryPostInput>
    {
        private readonly ISysPostService _postService;
        private readonly ISnackbarService _snackbarService;
        private readonly IDialogHostService _dialogHostService;
        private readonly IContainerProvider _containerProvider;

        private string _postCode;
        public string PostCode
        {
            get => _postCode;
            set
            {
                if (SetProperty(ref _postCode, value) && QueryPageInfo is QueryPostInput qpi)
                    qpi.PostCode = value;
            }
        }

        private string _postCategory;
        public string PostCategory
        {
            get => _postCategory;
            set
            {
                if (SetProperty(ref _postCategory, value) && QueryPageInfo is QueryPostInput qpi)
                    qpi.PostCategory = value;
            }
        }

        private string _postName;
        public string PostName
        {
            get => _postName;
            set
            {
                if (SetProperty(ref _postName, value) && QueryPageInfo is QueryPostInput qpi)
                    qpi.PostName = value;
            }
        }

        private long _deptId;
        public long DeptId
        {
            get => _deptId;
            set
            {
                if (SetProperty(ref _deptId, value) && QueryPageInfo is QueryPostInput qpi)
                {
                    qpi.DeptId = value;
                    SearchButtonCommand?.Execute(this);
                }
            }
        }

        private string _status;
        public string Status
        {
            get => _status;
            set
            {
                if (SetProperty(ref _status, value) && QueryPageInfo is QueryPostInput qpi)
                    qpi.Status = value;
            }
        }

        public PostListViewModel(ISysPostService postService, ISnackbarService snackbarService, IDialogHostService dialogHostService, IContainerProvider containerProvider)
        {
            _postService = postService;
            _snackbarService = snackbarService;
            _dialogHostService = dialogHostService;
            _containerProvider = containerProvider;

            RegisterQueryFunc(input => _postService.SelectPostList(input), new QueryPostInput() { PageNumber = 1, PageSize = 10 },
                resetAction: qpi =>
                {
                    PostCode = null;
                    PostCategory = null;
                    PostName = null;
                    DeptId = 0;
                    Status = null;
                });

            AddButtonCommand = new DelegateCommand<PostListViewModel>(async (vm) => await AddPost());
            EditButtonCommand = new DelegateCommand<SysPost>(async (post) => await EditPost(post));
            DeleteButtonCommand = new DelegateCommand<SysPost>(async (post) => await DeletePost(post));
            BatchDeleteCommand = new DelegateCommand<IList>(async (selectedItems) => await BatchDeletePosts(selectedItems));
        }

        public DelegateCommand<PostListViewModel> AddButtonCommand { get; }
        public DelegateCommand<SysPost> EditButtonCommand { get; }
        public DelegateCommand<SysPost> DeleteButtonCommand { get; }
        public DelegateCommand<IList> BatchDeleteCommand { get; }

        private async Task AddPost()
        {
            await _dialogHostService.ShowDialogAsync<PostFormView>(vm =>
            {
                var form = (PostFormViewModel)vm;
                form.DialogTitle = "新增岗位";
                form.IsEditMode = false;
                form.PostId = 0;
                form.DeptId = DeptId;
                form.OnSaveSuccessCallback = () => SearchButtonCommand.Execute(this);
            });
        }

        private async Task EditPost(SysPost post)
        {
            if (post == null) return;

            await _dialogHostService.ShowDialogAsync<PostFormView>(vm =>
            {
                var form = (PostFormViewModel)vm;
                form.DialogTitle = "编辑岗位";
                form.IsEditMode = true;
                form.PostId = post.PostId;
                form.DeptId = post.DeptId;
                form.PostCode = post.PostCode;
                form.PostCategory = post.PostCategory;
                form.PostName = post.PostName;
                form.PostSort = post.PostSort;
                form.Status = post.Status;
                form.Remark = post.Remark;
                form.OnSaveSuccessCallback = () => SearchButtonCommand.Execute(this);
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
                    SearchButtonCommand.Execute(this);
                }
                catch (System.Exception ex)
                {
                    _snackbarService.EnqueueError($"删除失败：{ex.Message}");
                }
            }
        }

        private async Task BatchDeletePosts(IList selectedItems)
        {
            if (selectedItems == null || selectedItems.Count == 0)
            {
                _snackbarService.EnqueueWarning("请先选择要删除的岗位");
                return;
            }

            var posts = selectedItems.Cast<SysPost>().ToList();
            var names = string.Join("、", posts.Select(p => p.PostName));
            var result = await _dialogHostService.ConfirmAsync($"确定要删除选中的 {posts.Count} 个岗位（{names}）吗？", "确认批量删除");
            if (result)
            {
                try
                {
                    foreach (var post in posts)
                        _postService.DeletePostById(post.PostId);
                    _snackbarService.EnqueueSuccess($"成功删除 {posts.Count} 个岗位");
                    SearchButtonCommand.Execute(this);
                }
                catch (System.Exception ex)
                {
                    _snackbarService.EnqueueError($"批量删除失败：{ex.Message}");
                }
            }
        }
    }
}