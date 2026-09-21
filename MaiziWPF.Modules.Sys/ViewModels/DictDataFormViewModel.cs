using MaiziWPF.Core;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using Prism.Commands;

namespace MaiziWPF.Modules.Sys
{
    public class DictDataFormViewModel : FormBindableBase
    {
        private readonly ISysDictService _dictService;

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

        private string _isDefault = "N";
        public string IsDefault
        {
            get { return _isDefault; }
            set { SetProperty(ref _isDefault, value); }
        }

        private string _remark;
        public string Remark
        {
            get { return _remark; }
            set { SetProperty(ref _remark, value); }
        }

        public DictDataFormViewModel(ISysDictService dictService, ISnackbarService snackbarService)
            : base(snackbarService)
        {
            _dictService = dictService;

            AcceptCommand = new DelegateCommand(() =>
            {
                SaveDictData();
            });
        }

        private void SaveDictData()
        {
            if (string.IsNullOrWhiteSpace(DictLabel))
            {
                ShowWarning("请输入字典标签");
                return;
            }
            if (string.IsNullOrWhiteSpace(DictValue))
            {
                ShowWarning("请输入字典键值");
                return;
            }

            var dictData = new SysDictData
            {
                DictCode = DictCode,
                DictType = DictType,
                DictLabel = DictLabel,
                DictValue = DictValue,
                DictSort = DictSort,
                IsDefault = IsDefault,
                Remark = Remark
            };

            if (IsEditMode)
            {
                _dictService.UpdateDictData(dictData);
                ShowSuccess("修改成功");
            }
            else
            {
                _dictService.InsertDictData(dictData);
                ShowSuccess("新增成功");
            }

            OnSaveSuccessCallback?.Invoke();
            CloseDialog();
        }
    }
}