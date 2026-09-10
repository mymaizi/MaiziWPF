using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using Prism.Commands;
using System;

namespace MaiziWPF.Modules.Sys
{
    public class PostFormViewModel : FormBindableBase
    {
        private readonly ISysPostService _postService;
        private readonly IDialogHostService _dialogHostService;

        private bool _isEditMode;
        public bool IsEditMode
        {
            get { return _isEditMode; }
            set { SetProperty(ref _isEditMode, value); }
        }

        private long _postId;
        public long PostId
        {
            get { return _postId; }
            set { SetProperty(ref _postId, value); }
        }

        private string _postCode;
        public string PostCode
        {
            get { return _postCode; }
            set { SetProperty(ref _postCode, value); }
        }

        private string _postName;
        public string PostName
        {
            get { return _postName; }
            set { SetProperty(ref _postName, value); }
        }

        private int _postSort;
        public int PostSort
        {
            get { return _postSort; }
            set { SetProperty(ref _postSort, value); }
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

        public PostFormViewModel(ISysPostService postService, IDialogHostService dialogHostService)
            : base(dialogHostService)
        {
            _postService = postService;
            _dialogHostService = dialogHostService;

            AcceptCommand = new DelegateCommand(() =>
            {
                SavePost();
            });
        }

        private async void SavePost()
        {
            if (string.IsNullOrWhiteSpace(PostName))
            {
                await _dialogHostService.AlertAsync("请输入岗位名称", AlertType.Info);
                return;
            }
            if (string.IsNullOrWhiteSpace(PostCode))
            {
                await _dialogHostService.AlertAsync("请输入岗位编码", AlertType.Info);
                return;
            }

            var post = new SysPost
            {
                PostId = PostId,
                PostCode = PostCode,
                PostName = PostName,
                PostSort = PostSort,
                Status = Status,
                Remark = Remark
            };

            if (!_postService.CheckPostNameUnique(post))
            {
                await _dialogHostService.AlertAsync("岗位名称已存在", AlertType.Info);
                return;
            }
            if (!_postService.CheckPostCodeUnique(post))
            {
                await _dialogHostService.AlertAsync("岗位编码已存在", AlertType.Info);
                return;
            }

            try
            {
                if (IsEditMode)
                {
                    _postService.UpdatePost(post);
                }
                else
                {
                    _postService.InsertPost(post);
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