using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using Prism.Commands;
using Prism.Ioc;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MaiziWPF.Modules.Sys
{
    public class DictListViewModel : PageBindableBase<SysDictType, QueryDictTypeInput>
    {
        private readonly ISysDictService _dictService;
        private readonly ISnackbarService _snackbarService;
        private readonly IDialogHostService _dialogHostService;
        private readonly IContainerProvider _containerProvider;

        private SysDictType _selectedDictType;
        public SysDictType SelectedDictType
        {
            get => _selectedDictType;
            set
            {
                if (SetProperty(ref _selectedDictType, value))
                {
                    OnDictTypeSelected(value);
                }
            }
        }

        private string _selectedDictInfo;
        public string SelectedDictInfo
        {
            get => _selectedDictInfo;
            set => SetProperty(ref _selectedDictInfo, value);
        }

        private ObservableCollection<SysDictData> _dictDataList = new();
        public ObservableCollection<SysDictData> DictDataList
        {
            get => _dictDataList;
            set => SetProperty(ref _dictDataList, value);
        }

        private QueryDictDataInput _dictDataQuery;
        public QueryDictDataInput DictDataQuery
        {
            get => _dictDataQuery;
            set => SetProperty(ref _dictDataQuery, value);
        }

        private int _dictDataPageNumber;
        public int DictDataPageNumber
        {
            get => _dictDataPageNumber;
            set => SetProperty(ref _dictDataPageNumber, value);
        }

        private int _dictDataPageSize;
        public int DictDataPageSize
        {
            get => _dictDataPageSize;
            set => SetProperty(ref _dictDataPageSize, value);
        }

        private long _dictDataCount;
        public long DictDataCount
        {
            get => _dictDataCount;
            set => SetProperty(ref _dictDataCount, value);
        }

        private SysDictData _selectedDictData;
        public SysDictData SelectedDictData
        {
            get => _selectedDictData;
            set => SetProperty(ref _selectedDictData, value);
        }

        public DelegateCommand AddDictTypeCommand { get; }
        public DelegateCommand<SysDictType> EditDictTypeCommand { get; }
        public DelegateCommand<SysDictType> DeleteDictTypeCommand { get; }
        public DelegateCommand RefreshDictTypeCommand { get; }

        public DelegateCommand AddDictDataCommand { get; }
        public DelegateCommand<SysDictData> EditDictDataCommand { get; }
        public DelegateCommand<SysDictData> DeleteDictDataCommand { get; }
        public DelegateCommand SearchDictDataCommand { get; }
        public DelegateCommand ResetDictDataCommand { get; }
        public DelegateCommand RefreshDictDataCommand { get; }
        public DelegateCommand DictDataPrevCommand { get; }
        public DelegateCommand DictDataNextCommand { get; }

        public DictListViewModel(ISysDictService dictService, ISnackbarService snackbarService, IDialogHostService dialogHostService, IContainerProvider containerProvider)
        {
            _dictService = dictService;
            _snackbarService = snackbarService;
            _dialogHostService = dialogHostService;
            _containerProvider = containerProvider;

            DictDataQuery = new QueryDictDataInput { PageNumber = 1, PageSize = 10 };

            RegisterQueryFunc(input => _dictService.SelectDictTypeList(input), new QueryDictTypeInput { PageNumber = 1, PageSize = 10 },
                resetAction: qpi =>
                {
                    QueryPageInfo = new QueryDictTypeInput { PageNumber = 1, PageSize = 10 };
                },
                loadedAction: () =>
                {
                    if (DataList != null && DataList.Count > 0)
                    {
                        SelectedDictType = DataList[0];
                    }
                });

            AddDictTypeCommand = new DelegateCommand(() => OpenDictTypeForm(null));
            EditDictTypeCommand = new DelegateCommand<SysDictType>(dictType => OpenDictTypeForm(dictType));
            DeleteDictTypeCommand = new DelegateCommand<SysDictType>(async dictType => await DeleteDictType(dictType));
            RefreshDictTypeCommand = new DelegateCommand(() => SearchButtonCommand.Execute(this));

            AddDictDataCommand = new DelegateCommand(() => OpenDictDataForm(null));
            EditDictDataCommand = new DelegateCommand<SysDictData>(dictData => OpenDictDataForm(dictData));
            DeleteDictDataCommand = new DelegateCommand<SysDictData>(async dictData => await DeleteDictData(dictData));

            SearchDictDataCommand = new DelegateCommand(async () =>
            {
                DictDataQuery.PageNumber = 1;
                await LoadDictDataAsync();
            });

            ResetDictDataCommand = new DelegateCommand(() =>
            {
                DictDataQuery = new QueryDictDataInput { PageNumber = 1, PageSize = 10, DictType = SelectedDictType?.DictType };
                LoadDictDataAsync();
            });

            RefreshDictDataCommand = new DelegateCommand(() => LoadDictDataAsync());

            DictDataPrevCommand = new DelegateCommand(async () =>
            {
                DictDataQuery.PageNumber--;
                await LoadDictDataAsync();
            });

            DictDataNextCommand = new DelegateCommand(async () =>
            {
                DictDataQuery.PageNumber++;
                await LoadDictDataAsync();
            });
        }

        private async void OnDictTypeSelected(SysDictType dictType)
        {
            if (dictType != null)
            {
                SelectedDictInfo = $"{dictType.DictName} / {dictType.DictType}";
                DictDataQuery = new QueryDictDataInput { PageNumber = 1, PageSize = 10, DictType = dictType.DictType };
                await LoadDictDataAsync();
            }
            else
            {
                SelectedDictInfo = "";
                DictDataList = new ObservableCollection<SysDictData>();
                DictDataCount = 0;
            }
        }

        private async Task LoadDictDataAsync()
        {
            if (string.IsNullOrEmpty(DictDataQuery?.DictType))
            {
                DictDataList = new ObservableCollection<SysDictData>();
                DictDataCount = 0;
                return;
            }

            try
            {
                var data = _dictService.SelectDictDataList(DictDataQuery);
                DictDataList = data.ToObservableCollection();
                DictDataPageNumber = DictDataQuery.PageNumber;
                DictDataPageSize = DictDataQuery.PageSize;
                DictDataCount = DictDataQuery.Count;
            }
            catch (Exception ex)
            {
                _snackbarService.EnqueueError($"加载字典数据失败：{ex.Message}");
            }
        }

        private void OpenDictTypeForm(SysDictType dictType)
        {
            _dialogHostService.ShowDialogAsync<DictTypeFormView>(vm =>
            {
                var form = (DictTypeFormViewModel)vm;
                if (dictType == null)
                {
                    form.DialogTitle = "新增字典类型";
                    form.IsEditMode = false;
                }
                else
                {
                    form.DialogTitle = "修改字典类型";
                    form.IsEditMode = true;
                    form.DictId = dictType.DictId;
                    form.DictName = dictType.DictName;
                    form.DictType = dictType.DictType;
                    form.Remark = dictType.Remark;
                }
                form.OnSaveSuccessCallback = () =>
                {
                    SearchButtonCommand.Execute(this);
                };
            });
        }

        private async Task DeleteDictType(SysDictType dictType)
        {
            if (dictType == null) return;

            var result = await _dialogHostService.ConfirmAsync($"确定要删除字典类型 '{dictType.DictName}' 吗？", "确认删除");
            if (result)
            {
                try
                {
                    _dictService.DeleteDictTypeById(dictType.DictId);
                    _snackbarService.EnqueueSuccess("删除成功");
                    SearchButtonCommand.Execute(this);
                }
                catch (Exception ex)
                {
                    _snackbarService.EnqueueError($"删除失败：{ex.Message}");
                }
            }
        }

        private void OpenDictDataForm(SysDictData dictData)
        {
            if (SelectedDictType == null)
            {
                _snackbarService.EnqueueWarning("请先选择字典类型");
                return;
            }

            _dialogHostService.ShowDialogAsync<DictDataFormView>(vm =>
            {
                var form = (DictDataFormViewModel)vm;
                if (dictData == null)
                {
                    form.DialogTitle = "新增字典数据";
                    form.IsEditMode = false;
                    form.DictType = SelectedDictType.DictType;
                }
                else
                {
                    form.DialogTitle = "修改字典数据";
                    form.IsEditMode = true;
                    form.DictCode = dictData.DictCode;
                    form.DictType = dictData.DictType;
                    form.DictLabel = dictData.DictLabel;
                    form.DictValue = dictData.DictValue;
                    form.DictSort = dictData.DictSort;
                    form.IsDefault = dictData.IsDefault;
                    form.Remark = dictData.Remark;
                }
                form.OnSaveSuccessCallback = () =>
                {
                    LoadDictDataAsync();
                };
            });
        }

        private async Task DeleteDictData(SysDictData dictData)
        {
            if (dictData == null) return;

            var result = await _dialogHostService.ConfirmAsync($"确定要删除字典数据 '{dictData.DictLabel}' 吗？", "确认删除");
            if (result)
            {
                try
                {
                    _dictService.DeleteDictDataById(dictData.DictCode);
                    _snackbarService.EnqueueSuccess("删除成功");
                    LoadDictDataAsync();
                }
                catch (Exception ex)
                {
                    _snackbarService.EnqueueError($"删除失败：{ex.Message}");
                }
            }
        }
    }
}