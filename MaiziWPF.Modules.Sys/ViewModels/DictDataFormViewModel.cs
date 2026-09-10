using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using Prism.Commands;
using System;

namespace MaiziWPF.Modules.Sys
{
    public class DictDataFormViewModel : FormBindableBase
    {
        private readonly ISysDictService _dictService;
        private readonly IDialogHostService _dialogHostService;

        private bool _isEditMode;
        public bool IsEditMode
        {
            get { return _isEditMode; }
            set { SetProperty(ref _isEditMode, value); }
        }

        private long _dictCode;
        public long DictCode
        {
            get { return _dictCode; }
            set { SetProperty(ref _dictCode, value); }
        }

        private string _dictType;
        public string DictType
        {
            get { return _dictType; }
            set { SetProperty(ref _dictType, value); }
        }

        private string _dictLabel;
        public string DictLabel
        {
            get { return _dictLabel; }
            set { SetProperty(ref _dictLabel, value); }
        }

        private string _dictValue;
        public string DictValue
        {
            get { return _dictValue; }
            set { SetProperty(ref _dictValue, value); }
        }

        private int _dictSort;
        public int DictSort
        {
            get { return _dictSort; }
            set { SetProperty(ref _dictSort, value); }
        }

        private string _cssClass = string.Empty;
        public string CssClass
        {
            get { return _cssClass; }
            set { SetProperty(ref _cssClass, value); }
        }

        private string _listClass = string.Empty;
        public string ListClass
        {
            get { return _listClass; }
            set { SetProperty(ref _listClass, value); }
        }

        private string _isDefault = "N";
        public string IsDefault
        {
            get { return _isDefault; }
            set { SetProperty(ref _isDefault, value); }
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

        public DictDataFormViewModel(ISysDictService dictService, IDialogHostService dialogHostService)
            : base(dialogHostService)
        {
            _dictService = dictService;
            _dialogHostService = dialogHostService;

            AcceptCommand = new DelegateCommand(() =>
            {
                SaveDictData();
            });
        }

        private async void SaveDictData()
        {
            if (string.IsNullOrWhiteSpace(DictLabel))
            {
                await _dialogHostService.AlertAsync("请输入字典标签", AlertType.Info);
                return;
            }
            if (string.IsNullOrWhiteSpace(DictValue))
            {
                await _dialogHostService.AlertAsync("请输入字典键值", AlertType.Info);
                return;
            }

            var dictData = new SysDictData
            {
                DictCode = DictCode,
                DictType = DictType,
                DictLabel = DictLabel,
                DictValue = DictValue,
                DictSort = DictSort,
                CssClass = CssClass,
                ListClass = ListClass,
                IsDefault = IsDefault,
                Status = Status,
                Remark = Remark
            };

            try
            {
                if (IsEditMode)
                {
                    _dictService.UpdateDictData(dictData);
                }
                else
                {
                    _dictService.InsertDictData(dictData);
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