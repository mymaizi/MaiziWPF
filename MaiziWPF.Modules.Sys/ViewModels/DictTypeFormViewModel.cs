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

        private string _remark;
        public string Remark
        {
            get { return _remark; }
            set { SetProperty(ref _remark, value); }
        }

        public DictTypeFormViewModel(ISysDictService dictService, ISnackbarService snackbarService)
            : base(snackbarService)
        {
            _dictService = dictService;

            AcceptCommand = new DelegateCommand(() =>
            {
                SaveDictType();
            });
        }

        private async void SaveDictType()
        {
            if (string.IsNullOrWhiteSpace(DictName))
            {
                ShowWarning("请输入字典名称");
                return;
            }
            if (string.IsNullOrWhiteSpace(DictType))
            {
                ShowWarning("请输入字典类型");
                return;
            }

            var dictType = new SysDictType
            {
                DictId = DictId,
                DictName = DictName,
                DictType = DictType,
                Remark = Remark
            };

            if (!_dictService.CheckDictTypeUnique(dictType))
            {
                ShowWarning("字典类型已存在");
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