using MaiziWPF.Core;
using MaiziWPF.Services.Application;
using MaiziWPF.Services.Domain;
using Prism.Commands;
using System;

namespace MaiziWPF.Modules.Sys
{
    public class ConfigFormViewModel : FormBindableBase
    {
        private readonly ISysConfigService _configService;
        private readonly IDialogHostService _dialogHostService;

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

        public ConfigFormViewModel(ISysConfigService configService, IDialogHostService dialogHostService)
            : base(dialogHostService)
        {
            _configService = configService;
            _dialogHostService = dialogHostService;

            AcceptCommand = new DelegateCommand(() =>
            {
                SaveConfig();
            });
        }

        private async void SaveConfig()
        {
            if (string.IsNullOrWhiteSpace(ConfigName))
            {
                await _dialogHostService.AlertAsync("请输入参数名称", AlertType.Info);
                return;
            }
            if (string.IsNullOrWhiteSpace(ConfigKey))
            {
                await _dialogHostService.AlertAsync("请输入参数键名", AlertType.Info);
                return;
            }
            if (string.IsNullOrWhiteSpace(ConfigValue))
            {
                await _dialogHostService.AlertAsync("请输入参数键值", AlertType.Info);
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
                await _dialogHostService.AlertAsync("参数键名已存在", AlertType.Info);
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
                await _dialogHostService.CloseDialogAsync();
            }
            catch (Exception ex)
            {
                await _dialogHostService.AlertAsync(ex.Message, AlertType.Error);
            }
        }
    }
}