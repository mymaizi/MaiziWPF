using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using Prism.Commands;
using Prism.Ioc;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MaiziWPF.Modules.Sys
{
    public class DeptListViewModel : PageBindableBase<SysDept, QueryDeptInput>
    {
        private readonly ISysDeptService _deptService;
        private readonly ISnackbarService _snackbarService;
        private readonly IDialogHostService _dialogHostService;
        private readonly IContainerProvider _containerProvider;

        private string _deptName;
        public string DeptName
        {
            get { return _deptName; }
            set { SetProperty(ref _deptName, value); }
        }

        private string _deptCategory;
        public string DeptCategory
        {
            get { return _deptCategory; }
            set { SetProperty(ref _deptCategory, value); }
        }

        private string _status;
        public string Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }

        public DeptListViewModel(ISysDeptService deptService, ISnackbarService snackbarService, IDialogHostService dialogHostService, IContainerProvider containerProvider)
        {
            _deptService = deptService;
            _snackbarService = snackbarService;
            _dialogHostService = dialogHostService;
            _containerProvider = containerProvider;

            RegisterQueryFunc(input =>
            {
                var filter = new SysDept { DeptName = DeptName, DeptCategory = DeptCategory, Status = Status };
                return _deptService.SelectDeptList(filter, true);
            }, new QueryDeptInput() { PageNumber = 1, PageSize = 10 },
                resetAction: qpi =>
                {
                    DeptName = null;
                    DeptCategory = null;
                    Status = null;
                    QueryPageInfo = new QueryDeptInput();
                });

            AddButtonCommand = new DelegateCommand<SysDept>(async (parent) => await AddDept(parent));
            EditButtonCommand = new DelegateCommand<SysDept>(async (dept) => await EditDept(dept));
            DeleteButtonCommand = new DelegateCommand<SysDept>(async (dept) => await DeleteDept(dept));
        }

        public DelegateCommand<SysDept> AddButtonCommand { get; }
        public DelegateCommand<SysDept> EditButtonCommand { get; }
        public DelegateCommand<SysDept> DeleteButtonCommand { get; }

        private async Task AddDept(SysDept parent = null)
        {
            await _dialogHostService.ShowDialogAsync<DeptFormView>(vm =>
            {
                var form = (DeptFormViewModel)vm;
                form.DialogTitle = "新增部门";
                form.IsEditMode = false;
                form.DeptId = 0;
                form.ParentId = parent?.Id ?? 0;
                form.ParentName = parent?.DeptName ?? "顶级部门";
                form.OnSaveSuccessCallback = () =>
                {
                    SearchButtonCommand.Execute(this);
                };
            });
        }

        private async Task EditDept(SysDept dept)
        {
            if (dept == null) return;

            await _dialogHostService.ShowDialogAsync<DeptFormView>(vm =>
            {
                var form = (DeptFormViewModel)vm;
                form.DialogTitle = "编辑部门";
                form.IsEditMode = true;
                form.DeptId = dept.Id;
                form.ParentId = dept.ParentId;
                form.DeptName = dept.DeptName;
                form.DeptCategory = dept.DeptCategory;
                form.OrderNum = dept.OrderNum;
                form.Leader = dept.Leader;
                form.Phone = dept.Phone;
                form.Email = dept.Email;
                form.Status = dept.Status;
                form.OnSaveSuccessCallback = () =>
                {
                    SearchButtonCommand.Execute(this);
                };
            });
        }

        private async Task DeleteDept(SysDept dept)
        {
            if (dept == null) return;

            var childCount = CountAllDescendants(dept.Id);
            var message = childCount > 0
                ? $"确定要删除部门 '{dept.DeptName}' 及其 {childCount} 个子部门吗？此操作不可恢复！"
                : $"确定要删除部门 '{dept.DeptName}' 吗？此操作不可恢复！";

            var result = await _dialogHostService.ConfirmAsync(message, "确认删除");
            if (result)
            {
                try
                {
                    _deptService.DeleteDeptById(dept.Id);
                    _snackbarService.EnqueueSuccess("删除成功");
                    SearchButtonCommand.Execute(this);
                }
                catch (System.Exception ex)
                {
                    _snackbarService.EnqueueError($"删除失败：{ex.Message}");
                }
            }
        }

        private int CountAllDescendants(long deptId)
        {
            int count = 0;
            var tree = _deptService.SelectDeptList(new SysDept(), true);
            var dept = FindDeptInTree(tree, deptId);
            if (dept?.Childs != null)
            {
                count = CountTree(dept.Childs);
            }
            return count;

            int CountTree(List<SysDept> children)
            {
                int n = children.Count;
                foreach (var c in children)
                {
                    if (c.Childs?.Count > 0)
                        n += CountTree(c.Childs);
                }
                return n;
            }
        }

        private SysDept FindDeptInTree(List<SysDept> tree, long id)
        {
            foreach (var d in tree)
            {
                if (d.Id == id) return d;
                if (d.Childs?.Count > 0)
                {
                    var found = FindDeptInTree(d.Childs, id);
                    if (found != null) return found;
                }
            }
            return null;
        }
    }
}