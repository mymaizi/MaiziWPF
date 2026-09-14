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
            await _dialogHostService.ShowDialogAsync<DeptFormView>(view =>
            {
                var vm = view.DataContext as DeptFormViewModel;
                vm.IsEditMode = false;
                vm.DeptId = 0;
                vm.ParentId = 0;
                vm.ParentName = "顶级部门";
                vm.LoadDeptTree();
                vm.OnSaveSuccessCallback = () =>
                {
                    LoadDataList();
                };
            });
        }

        private async Task EditDept(SysDept dept)
        {
            if (dept == null) return;

            await _dialogHostService.ShowDialogAsync<DeptFormView>(view =>
            {
                var vm = view.DataContext as DeptFormViewModel;
                vm.IsEditMode = true;
                vm.DeptId = dept.Id;
                vm.ParentId = dept.ParentId;
                vm.DeptName = dept.DeptName;
                vm.OrderNum = dept.OrderNum;
                vm.Leader = dept.Leader;
                vm.Phone = dept.Phone;
                vm.Email = dept.Email;
                vm.Status = dept.Status;
                vm.LoadDeptTree();
                vm.OnSaveSuccessCallback = () =>
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