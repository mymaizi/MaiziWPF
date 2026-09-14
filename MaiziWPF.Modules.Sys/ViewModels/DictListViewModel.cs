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
    public class DictListViewModel : PageBindableBase<SysDictType, QueryDictTypeInput>
    {
        private readonly ISysDictService _dictService;
        private readonly IContainerProvider _containerProvider;
        private readonly IDialogHostService _dialogHostService;
        private readonly ISnackbarService _snackbarService;

        public ICommand AddDictTypeCommand { get; }

        public DictListViewModel(ISysDictService dictService, IContainerProvider containerProvider, IDialogHostService dialogHostService, ISnackbarService snackbarService)
        {
            _dictService = dictService;
            _containerProvider = containerProvider;
            _dialogHostService = dialogHostService;
            _snackbarService = snackbarService;

            RegisterQueryFunc(input =>
            {
                var result = _dictService.SelectDictTypeList(input);
                return result;
            }, new QueryDictTypeInput() { PageNumber = 1, PageSize = 10 });

            AddDictTypeCommand = new DelegateCommand(() =>
            {
                OpenDictTypeForm(null);
            });

            NewOrEditButtonCommand = new DelegateCommand<SysDictType>((dictType) =>
            {
                OpenDictTypeForm(dictType);
            });

            DeleteButtonCommand = new DelegateCommand<SysDictType>(async (dictType) =>
            {
                await DeleteDictType(dictType);
            });

            SearchButtonCommand.Execute(this);
        }

        private async System.Threading.Tasks.Task OpenDictTypeForm(SysDictType dictType)
        {
            await _dialogHostService.ShowDialogAsync<DictTypeFormView>(view =>
            {
                var model = view.DataContext as DictTypeFormViewModel;

                if (dictType != null)
                {
                    model.IsEditMode = true;
                    model.DictId = dictType.DictId;
                    model.DictName = dictType.DictName;
                    model.DictType = dictType.DictType;
                    model.Status = dictType.Status;
                    model.Remark = dictType.Remark;
                }
                else
                {
                    model.IsEditMode = false;
                }

                model.OnSaveSuccessCallback = () =>
                {
                    SearchButtonCommand.Execute(this);
                };
            });
        }

        private async System.Threading.Tasks.Task DeleteDictType(SysDictType dictType)
        {
            if (dictType == null) return;

            var result = await _dialogHostService.ConfirmAsync($"确定要删除字典类型【{dictType.DictName}】和对应的字典数据吗？", "删除确认");
            if (!result) return;

            try
            {
                _dictService.DeleteDictTypeById(dictType.DictId);
                _snackbarService.EnqueueSuccess("删除成功");
                SearchButtonCommand.Execute(this);
            }
            catch (Exception ex)
            {
                _snackbarService.EnqueueError(ex.Message);
            }
        }
    }
}