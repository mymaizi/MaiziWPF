using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace MaiziWPF.Modules.Sys
{
    public class DeptListViewModel : PageBindableBase<SysDept, QueryDeptInput>
    {
        public ObservableCollection<SysDept> DeptItems { get; set; } = new();

        private string _deptName;
        public string DeptName { get => _deptName; set => SetProperty(ref _deptName, value); }

        private string _status;
        public string Status { get => _status; set => SetProperty(ref _status, value); }

        private readonly ISysDeptService _deptService;
        private readonly IContainerProvider _containerProvider;
        private readonly IDialogHostService _dialogHostService;

        public ICommand AddDeptCommand { get; }

        public DeptListViewModel(ISysDeptService deptService, IContainerProvider containerProvider, IDialogHostService dialogHostService)
        {
            _deptService = deptService;
            _containerProvider = containerProvider;
            _dialogHostService = dialogHostService;

            SearchButtonCommand = new DelegateCommand(() =>
            {
                SearchDept();
            });

            NewOrEditButtonCommand = new DelegateCommand<SysDept>(dept =>
            {
                OpenDeptForm(dept);
            });

            DeleteButtonCommand = new DelegateCommand<SysDept>(async dept =>
            {
                if (dept == null) return;
                await DeleteDept(dept);
            });

            AddDeptCommand = new DelegateCommand(() =>
            {
                OpenDeptForm(null);
            });

            SearchButtonCommand.Execute(this);
        }

        private void SearchDept()
        {
            DeptItems.Clear();
            var list = _deptService.SelectDeptList(new SysDept()
            {
                DeptName = DeptName,
                Status = Status,
            });
            DeptItems.AddRange(list);
        }

        private void OpenDeptForm(SysDept dept)
        {
            var view = _containerProvider.Resolve<DeptFormView>();
            var model = view.DataContext as DeptFormViewModel;
            model.LoadDeptTree();

            if (dept != null)
            {
                model.IsEditMode = true;
                model.DeptId = dept.Id;
                model.ParentId = dept.ParentId;
                model.DeptName = dept.DeptName;
                model.OrderNum = dept.OrderNum;
                model.Leader = dept.Leader;
                model.Phone = dept.Phone;
                model.Email = dept.Email;
                model.Status = dept.Status;
            }
            else
            {
                model.IsEditMode = false;
            }

            model.OnSaveSuccessCallback = () =>
            {
                SearchDept();
            };

            view.DataContext = model;
            _dialogHostService.ShowDialogAsync(view, autoClose: false);
        }

        private async System.Threading.Tasks.Task DeleteDept(SysDept dept)
        {
            if (_deptService.HasChildByDeptId(dept.Id))
            {
                await _dialogHostService.AlertAsync("存在下级部门,不允许删除", AlertType.Info);
                return;
            }

            if (_deptService.CheckDeptExistUser(dept.Id))
            {
                await _dialogHostService.AlertAsync("部门存在用户,不允许删除", AlertType.Info);
                return;
            }

            var result = await _dialogHostService.ConfirmAsync($"确定要删除部门 '{dept.DeptName}' 吗？", "确认删除");
            if (result)
            {
                try
                {
                    _deptService.DeleteDeptById(dept.Id);
                    await _dialogHostService.AlertAsync("删除成功", AlertType.Info);
                    SearchDept();
                }
                catch (Exception ex)
                {
                    await _dialogHostService.AlertAsync($"删除失败：{ex.Message}", AlertType.Error);
                }
            }
        }
    }
}