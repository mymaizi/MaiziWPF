using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using Prism.Commands;
using System;

namespace MaiziWPF.Modules.Sys
{
    public class DeptFormViewModel : FormBindableBase
    {
        private readonly ISysDeptService _deptService;

        private bool _isEditMode;
        public bool IsEditMode
        {
            get { return _isEditMode; }
            set { SetProperty(ref _isEditMode, value); }
        }

        private long _deptId;
        public long DeptId
        {
            get { return _deptId; }
            set { SetProperty(ref _deptId, value); }
        }

        private long _parentId;
        public long ParentId
        {
            get { return _parentId; }
            set
            {
                if (SetProperty(ref _parentId, value))
                    ResolveParentName();
            }
        }

        private void ResolveParentName()
        {
            if (_parentId == 0)
                ParentName = "顶级部门";
            else if (_deptService != null)
            {
                var dept = _deptService.SelectDeptById(_parentId);
                ParentName = dept?.DeptName ?? _parentId.ToString();
            }
        }

        private string _parentName;
        public string ParentName
        {
            get { return _parentName; }
            set { SetProperty(ref _parentName, value); }
        }

        private string _deptName;
        public string DeptName
        {
            get { return _deptName; }
            set { SetProperty(ref _deptName, value); }
        }

        private int _orderNum;
        public int OrderNum
        {
            get { return _orderNum; }
            set { SetProperty(ref _orderNum, value); }
        }

        private long _leader;
        public long Leader
        {
            get { return _leader; }
            set { SetProperty(ref _leader, value); }
        }

        private string _deptCategory;
        public string DeptCategory
        {
            get { return _deptCategory; }
            set { SetProperty(ref _deptCategory, value); }
        }

        private string _phone;
        public string Phone
        {
            get { return _phone; }
            set { SetProperty(ref _phone, value); }
        }

        private string _email;
        public string Email
        {
            get { return _email; }
            set { SetProperty(ref _email, value); }
        }

        private string _status = "0";
        public string Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }

        public DeptFormViewModel(ISysDeptService deptService, ISnackbarService snackbarService)
            : base(snackbarService)
        {
            _deptService = deptService;

            AcceptCommand = new DelegateCommand(() =>
            {
                SaveDept();
            });
        }

        private async void SaveDept()
        {
            if (string.IsNullOrWhiteSpace(DeptName))
            {
                ShowWarning("请输入部门名称");
                return;
            }

            var dept = new SysDept
            {
                Id = DeptId,
                ParentId = ParentId,
                DeptName = DeptName,
                DeptCategory = DeptCategory,
                OrderNum = OrderNum,
                Leader = Leader,
                Phone = Phone,
                Email = Email,
                Status = Status
            };

            if (!_deptService.CheckDeptNameUnique(dept))
            {
                ShowWarning("部门名称已存在");
                return;
            }

            try
            {
                if (IsEditMode)
                {
                    _deptService.UpdateDept(dept);
                }
                else
                {
                    _deptService.InsertDept(dept);
                }
                OnSaveSuccessCallback?.Invoke();
                ShowSuccess("保存成功");
                CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
    }
}