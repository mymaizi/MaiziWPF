using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using Prism.Commands;
using System;

namespace MaiziWPF.Modules.Sys
{
    public class DictTypeFormViewModel : FormBindableBase
    {
        private readonly ISysDictService _dictService;
        private readonly IDialogHostService _dialogHostService;

        private bool _isEditMode;
        public bool IsEditMode
        {
            get { return _isEditMode; }
            set { SetProperty(ref _isEditMode, value); }
        }

        private long _dictId;
        public long DictId
        {
            get { return _dictId; }
            set { SetProperty(ref _dictId, value); }
        }

        private string _dictName;
        public string DictName
        {
            get { return _dictName; }
            set { SetProperty(ref _dictName, value); }
        }

        private string _dictType;
        public string DictType
        {
            get { return _dictType; }
            set { SetProperty(ref _dictType, value); }
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

        public DictTypeFormViewModel(ISysDictService dictService, IDialogHostService dialogHostService)
            : base(dialogHostService)
        {
            _dictService = dictService;
            _dialogHostService = dialogHostService;

            AcceptCommand = new DelegateCommand(() =>
            {
                SaveDictType();
            });
        }

        private async void SaveDictType()
        {
            if (string.IsNullOrWhiteSpace(DictName))
            {
                await _dialogHostService.AlertAsync("请输入字典名称", AlertType.Info);
                return;
            }
            if (string.IsNullOrWhiteSpace(DictType))
            {
                await _dialogHostService.AlertAsync("请输入字典类型", AlertType.Info);
                return;
            }

            var dictType = new SysDictType
            {
                DictId = DictId,
                DictName = DictName,
                DictType = DictType,
                Status = Status,
                Remark = Remark
            };

            if (!_dictService.CheckDictTypeUnique(dictType))
            {
                await _dialogHostService.AlertAsync("字典类型已存在", AlertType.Info);
                return;
            }

            try
            {
                if (IsEditMode)
                {
                    _dictService.UpdateDictType(dictType);
                }
                else
                {
                    _dictService.InsertDictType(dictType);
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