using MaiziWPF.Core;
using MaiziWPF.Services.Application;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using Prism.Commands;
using Prism.Ioc;
using System;
using System.Windows.Input;

namespace MaiziWPF.Modules.Sys
{
    public class ConfigListViewModel : PageBindableBase<SysConfig, QueryConfigInput>
    {
        private readonly ISysConfigService _configService;
        private readonly IContainerProvider _containerProvider;
        private readonly IDialogHostService _dialogHostService;
        private readonly ISnackbarService _snackbarService;

        public ICommand AddConfigCommand { get; }

        public ConfigListViewModel(ISysConfigService configService, IContainerProvider containerProvider, IDialogHostService dialogHostService, ISnackbarService snackbarService)
        {
            _configService = configService;
            _containerProvider = containerProvider;
            _dialogHostService = dialogHostService;
            _snackbarService = snackbarService;

            RegisterQueryFunc(input =>
            {
                return _configService.SelectConfigList(input);
            }, new QueryConfigInput() { PageNumber = 1, PageSize = 10 });

            AddConfigCommand = new DelegateCommand(() =>
            {
                OpenConfigForm(null);
            });

            NewOrEditButtonCommand = new DelegateCommand<SysConfig>((config) =>
            {
                OpenConfigForm(config);
            });

            DeleteButtonCommand = new DelegateCommand<SysConfig>(async (config) =>
            {
                await DeleteConfig(config);
            });

            SearchButtonCommand.Execute(this);
        }

        private async System.Threading.Tasks.Task OpenConfigForm(SysConfig config)
        {
            await _dialogHostService.ShowDialogAsync<ConfigFormView>(view =>
            {
                var model = view.DataContext as ConfigFormViewModel;

                if (config != null)
                {
                    model.IsEditMode = true;
                    model.ConfigId = config.ConfigId;
                    model.ConfigName = config.ConfigName;
                    model.ConfigKey = config.ConfigKey;
                    model.ConfigValue = config.ConfigValue;
                    model.ConfigType = config.ConfigType;
                    model.Remark = config.Remark;
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

        private async System.Threading.Tasks.Task DeleteConfig(SysConfig config)
        {
            if (config == null) return;

            var result = await _dialogHostService.ConfirmAsync($"确定要删除参数配置【{config.ConfigName}】吗？", "删除确认");
            if (!result) return;

            try
            {
                _configService.DeleteConfigById(config.ConfigId);
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