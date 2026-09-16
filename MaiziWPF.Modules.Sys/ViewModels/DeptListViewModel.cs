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
    public class DeptListViewModel : PageBindableBase<SysDept, QueryDeptInput>
    {
        private readonly ISysDeptService _deptService;
        private readonly ISnackbarService _snackbarService;
        private readonly IDialogHostService _dialogHostService;
        private readonly IContainerProvider _containerProvider;

        public DeptListViewModel(ISysDeptService deptService, ISnackbarService snackbarService, IDialogHostService dialogHostService, IContainerProvider containerProvider)
        {
            _deptService = deptService;
            _snackbarService = snackbarService;
            _dialogHostService = dialogHostService;
            _containerProvider = containerProvider;

            SearchButtonCommand = new DelegateCommand<DeptListViewModel>((vm) =>
            {
                LoadDataList();
            });

            ResetButtonCommand = new DelegateCommand<DeptListViewModel>((vm) =>
            {
                QueryPageInfo = new QueryDeptInput();
                LoadDataList();
            });

            AddButtonCommand = new DelegateCommand<DeptListViewModel>(async (vm) =>
            {
                await AddDept();
            });

            EditButtonCommand = new DelegateCommand<SysDept>(async (dept) =>
            {
                await EditDept(dept);
            });

            DeleteButtonCommand = new DelegateCommand<SysDept>(async (dept) =>
            {
                await DeleteDept(dept);
            });
        }

        public DelegateCommand<DeptListViewModel> SearchButtonCommand { get; }
        public DelegateCommand<DeptListViewModel> ResetButtonCommand { get; }
        public DelegateCommand<DeptListViewModel> AddButtonCommand { get; }
        public DelegateCommand<SysDept> EditButtonCommand { get; }
        public DelegateCommand<SysDept> DeleteButtonCommand { get; }

        public override void LoadDataList()
        {
            DataList = new ObservableCollection<SysDept>(_deptService.SelectDeptList(new SysDept(), false));
        }

        private async Task AddDept()
        {
            await _dialogHostService.ShowDialogAsync<DeptFormView>(vm =>
            {
                var form = (DeptFormViewModel)vm;
                form.IsEditMode = false;
                form.DeptId = 0;
                form.ParentId = 0;
                form.ParentName = "顶级部门";
                form.LoadDeptTree();
                form.OnSaveSuccessCallback = () =>
                {
                    LoadDataList();
                };
            });
        }

        private async Task EditDept(SysDept dept)
        {
            if (dept == null) return;

            await _dialogHostService.ShowDialogAsync<DeptFormView>(vm =>
            {
                var form = (DeptFormViewModel)vm;
                form.IsEditMode = true;
                form.DeptId = dept.Id;
                form.ParentId = dept.ParentId;
                form.DeptName = dept.DeptName;
                form.OrderNum = dept.OrderNum;
                form.Leader = dept.Leader;
                form.Phone = dept.Phone;
                form.Email = dept.Email;
                form.Status = dept.Status;
                form.LoadDeptTree();
                form.OnSaveSuccessCallback = () =>
                {
                    LoadDataList();
                };
            });
        }

        private async Task DeleteDept(SysDept dept)
        {
            if (dept == null) return;

            var result = await _dialogHostService.ConfirmAsync($"确定要删除部门 '{dept.DeptName}' 吗？", "确认删除");
            if (result)
            {
                try
                {
                    _deptService.DeleteDeptById(dept.Id);
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