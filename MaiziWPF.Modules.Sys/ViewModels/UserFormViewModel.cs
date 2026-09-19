using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using Prism.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MaiziWPF.Modules.Sys
{
    public class UserFormViewModel : FormBindableBase
    {
        #region 表单字段
        private long _userId;
        public long UserId
        {
            get { return _userId; }
            set { SetProperty(ref _userId, value); }
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }

        private string _nickName;
        public string NickName
        {
            get { return _nickName; }
            set { SetProperty(ref _nickName, value); }
        }

        private string _phoneNumber;
        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set { SetProperty(ref _phoneNumber, value); }
        }

        private string _email;
        public string Email
        {
            get { return _email; }
            set { SetProperty(ref _email, value); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }
        private string _status = "0";
        public string Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }
        public string _gender;
        public string Gender
        {
            get { return _gender; }
            set { SetProperty(ref _gender, value); }
        }
        private long _deptId;
        public long DeptId
        {
            get { return _deptId; }
            set { SetProperty(ref _deptId, value); }
        }
        private long _selectedPostId;
        public long SelectedPostId
        {
            get { return _selectedPostId; }
            set { SetProperty(ref _selectedPostId, value); }
        }
        private ObservableCollection<Checked> _selectedRoles = new ObservableCollection<Checked>();
        public ObservableCollection<Checked> SelectedRoles
        {
            get { return _selectedRoles; }
            set { SetProperty(ref _selectedRoles, value); }
        }
        private ObservableCollection<Checked> _roles = new ObservableCollection<Checked>();
        public ObservableCollection<Checked> Roles
        {
            get { return _roles; }
            set { SetProperty(ref _roles, value); }
        }
        private int _rolePageSize = 10;
        private int _roleCurrentPage = 1;
        private bool _hasMoreRoles = true;
        private List<Checked> _allRolesCache;

        public void LoadRoles(int page = 1, bool append = false)
        {
            if (_allRolesCache == null)
            {
                _allRolesCache = _userService.SelectAllRoles().Select(r => new Checked() { Id = r.RoleId, Name = r.RoleName }).ToList();
            }

            var pagedRoles = _allRolesCache.Skip((page - 1) * _rolePageSize).Take(_rolePageSize).ToList();
            _hasMoreRoles = page * _rolePageSize < _allRolesCache.Count;

            if (append)
            {
                foreach (var role in pagedRoles)
                {
                    if (!Roles.Any(r => r.Id == role.Id))
                    {
                        Roles.Add(role);
                    }
                }
            }
            else
            {
                Roles.Clear();
                foreach (var role in pagedRoles)
                {
                    Roles.Add(role);
                }
            }

            _roleCurrentPage = page;
        }

        public void LoadMoreRoles()
        {
            if (_hasMoreRoles)
            {
                LoadRoles(_roleCurrentPage + 1, append: true);
            }
        }

        public bool HasMoreRoles => _hasMoreRoles;

        public List<Checked> GetAllRoles()
        {
            if (_allRolesCache == null)
            {
                _allRolesCache = _userService.SelectAllRoles().Select(r => new Checked() { Id = r.RoleId, Name = r.RoleName }).ToList();
            }
            return _allRolesCache;
        }

        private DelegateCommand _loadMoreRolesCommand;
        public DelegateCommand LoadMoreRolesCommand =>
            _loadMoreRolesCommand ?? (_loadMoreRolesCommand = new DelegateCommand(LoadMoreRoles));

        private List<Checked> _allPostsCache;
        private int _postPageSize = 10;
        private int _postCurrentPage = 1;
        private bool _hasMorePosts = true;

        public void LoadPosts(int page = 1, bool append = false)
        {
            if (_allPostsCache == null)
            {
                _allPostsCache = _userService.SelectAllPosts().Select(p => new Checked() { Id = p.PostId, Name = p.PostName }).ToList();
            }

            var pagedPosts = _allPostsCache.Skip((page - 1) * _postPageSize).Take(_postPageSize).ToList();
            _hasMorePosts = page * _postPageSize < _allPostsCache.Count;

            if (append)
            {
                foreach (var post in pagedPosts)
                {
                    if (!Posts.Any(r => r.Id == post.Id))
                    {
                        Posts.Add(post);
                    }
                }
            }
            else
            {
                Posts.Clear();
                foreach (var post in pagedPosts)
                {
                    Posts.Add(post);
                }
            }

            _postCurrentPage = page;
        }

        public void LoadMorePosts()
        {
            if (_hasMorePosts)
            {
                LoadPosts(_postCurrentPage + 1, append: true);
            }
        }

        public bool HasMorePosts => _hasMorePosts;

        private DelegateCommand _loadMorePostsCommand;
        public DelegateCommand LoadMorePostsCommand =>
            _loadMorePostsCommand ?? (_loadMorePostsCommand = new DelegateCommand(LoadMorePosts));

        public List<Checked> GetAllPosts()
        {
            if (_allPostsCache == null)
            {
                _allPostsCache = _userService.SelectAllPosts().Select(p => new Checked() { Id = p.PostId, Name = p.PostName }).ToList();
            }
            return _allPostsCache;
        }
        private ObservableCollection<Checked> _posts = new ObservableCollection<Checked>();
        public ObservableCollection<Checked> Posts
        {
            get { return _posts; }
            set { SetProperty(ref _posts, value); }
        }
        private string _remark;
        public string Remark
        {
            get { return _remark; }
            set { SetProperty(ref _remark, value); }
        }
        private bool _isEditMode;
        public bool IsEditMode
        {
            get { return _isEditMode; }
            set { SetProperty(ref _isEditMode, value); }
        }

        private List<long> _initialRoleIds;
        public List<long> InitialRoleIds
        {
            get { return _initialRoleIds; }
            set { SetProperty(ref _initialRoleIds, value); }
        }

        private List<long> _initialPostIds;
        public List<long> InitialPostIds
        {
            get { return _initialPostIds; }
            set { SetProperty(ref _initialPostIds, value); }
        }
        #endregion

        #region 验证方法
        private bool ValidateForm()
        {
            MaiziWPF.Core.NotEmptyValidationRule.ShowValidationErrors = true;
            MaiziWPF.Core.LengthValidationRule.ShowValidationErrors = true;
            MaiziWPF.Core.PasswordValidationRule.ShowValidationErrors = true;

            if (!IsEditMode)
            {
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(UserName)));
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(Password)));
            }
            OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(NickName)));

            if (!IsEditMode)
            {
                return !string.IsNullOrWhiteSpace(NickName) &&
                  !string.IsNullOrWhiteSpace(UserName) &&
                  !string.IsNullOrWhiteSpace(Password);
            }
            return !string.IsNullOrWhiteSpace(NickName);
        }
        #endregion

        private readonly ISysUserService _userService;

        public UserFormViewModel(ISysUserService userService, ISnackbarService snackbarService) : base(snackbarService)
        {
            _userService = userService;

            LoadRoles(page: 1, append: false);
            LoadPosts(page: 1, append: false);

            this.AcceptCommand = new DelegateCommand(async () =>
            {
                if (!ValidateForm())
                {
                    return;
                }

                if (UserId != 0)
                {
                    var user = new SysUser()
                    {
                        UserId = this.UserId,
                        DeptId = this.DeptId,
                        NickName = this.NickName,
                        PhoneNumber = this.PhoneNumber,
                        Email = this.Email,
                        Status = this.Status,
                        Gender = this.Gender,
                        Remark = this.Remark,
                        Posts = this.SelectedPostId > 0 ? new List<SysPost> { new SysPost { PostId = this.SelectedPostId } } : null,
                        Roles = this.SelectedRoles.Any() ? this.SelectedRoles.Select(r => new SysRole { RoleId = r.Id }).ToList() : null,
                    };

                    var success = _userService.UpdateUser(user);

                    if (success)
                    {
                        ShowSuccess("修改成功！");
                        OnSaveSuccessCallback?.Invoke();
                        CloseDialog();
                    }
                    else
                    {
                        ShowError("修改失败！");
                    }
                }
                else
                {
                    var user = new SysUser()
                    {
                        UserName = this.UserName,
                        DeptId = this.DeptId,
                        NickName = this.NickName,
                        PhoneNumber = this.PhoneNumber,
                        Email = this.Email,
                        Password = this.Password,
                        Status = this.Status,
                        Gender = this.Gender,
                        Remark = this.Remark,
                        Posts = this.SelectedPostId > 0 ? new List<SysPost> { new SysPost { PostId = this.SelectedPostId } } : null,
                        Roles = this.SelectedRoles.Any() ? this.SelectedRoles.Select(r => new SysRole { RoleId = r.Id }).ToList() : null,
                    };

                    _userService.InsertUser(user);

                    ShowSuccess("添加成功！");
                    OnSaveSuccessCallback?.Invoke();
                    CloseDialog();
                }
            });
        }
    }
}