using MaiziWPF.Core;
using MaiziWPF.Services.Application;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using Prism.Commands;
using System;

namespace MaiziWPF.Modules.Sys
{
    public class ConfigFormViewModel : FormBindableBase
    {
        private readonly ISysConfigService _configService;

        private bool _isEditMode;
        public bool IsEditMode
        {
            get { return _isEditMode; }
            set { SetProperty(ref _isEditMode, value); }
        }

        private long _configId;
        public long ConfigId
        {
            get { return _configId; }
            set { SetProperty(ref _configId, value); }
        }

        private string _configName;
        public string ConfigName
        {
            get { return _configName; }
            set { SetProperty(ref _configName, value); }
        }

        private string _configKey;
        public string ConfigKey
        {
            get { return _configKey; }
            set { SetProperty(ref _configKey, value); }
        }

        private string _configValue;
        public string ConfigValue
        {
            get { return _configValue; }
            set { SetProperty(ref _configValue, value); }
        }

        private string _configType = "Y";
        public string ConfigType
        {
            get { return _configType; }
            set { SetProperty(ref _configType, value); }
        }

        private string _remark;
        public string Remark
        {
            get { return _remark; }
            set { SetProperty(ref _remark, value); }
        }

        public ConfigFormViewModel(ISysConfigService configService, ISnackbarService snackbarService)
            : base(snackbarService)
        {
            _configService = configService;

            AcceptCommand = new DelegateCommand(() =>
            {
                SaveConfig();
            });
        }

        private async void SaveConfig()
        {
            if (string.IsNullOrWhiteSpace(ConfigName))
            {
                ShowWarning("请输入参数名称");
                return;
            }
            if (string.IsNullOrWhiteSpace(ConfigKey))
            {
                ShowWarning("请输入参数键名");
                return;
            }

            var config = new SysConfig
            {
                ConfigId = ConfigId,
                ConfigName = ConfigName,
                ConfigKey = ConfigKey,
                ConfigValue = ConfigValue,
                ConfigType = ConfigType,
                Remark = Remark
            };

            if (!_configService.CheckConfigKeyUnique(config))
            {
                ShowWarning("参数键名已存在");
                return;
            }

            try
            {
                if (IsEditMode)
                {
                    _configService.UpdateConfig(config);
                }
                else
                {
                    _configService.InsertConfig(config);
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