﻿﻿﻿﻿﻿﻿using MaiziWPF.Core;
using MaiziWPF.Core.Views;
using MaiziWPF.Modules.Sys;
using MaiziWPF.Services.Domain.Shared;
using MaiziWPF.Services.MySql;
using MaiziWPF.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prism.Container.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Navigation.Regions;
using Serilog;
using System.Windows;
using System.Windows.Controls;
using Volo.Abp;

namespace MaiziWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<LoginView>();
            containerRegistry.RegisterForNavigation<MainView>();
            containerRegistry.RegisterDialogWindow<BorderlessDialogWindow>();
            containerRegistry.RegisterSingleton<IDialogHostService, DialogHostService>();
            containerRegistry.RegisterSingleton<ISnackbarService>(provider => 
                new SnackbarService(MaiziWPF.Views.MainWindow.SnackbarMessageQueue));
        }
        protected override void InitializeShell(Window shell)
        {
            base.InitializeShell(shell);
            MySqlModule.SetAuditProvider(Container.Resolve<IAuditUserProvider>());
            var regionManager = Container.Resolve<IRegionManager>();
            var snackbarService = Container.Resolve<ISnackbarService>();
            regionManager.RequestNavigate(RegionNames.ContentRegion, nameof(LoginView));
            Application.Current.DispatcherUnhandledException += async (sender, e) =>
            {
                if (e.Exception is UserFriendlyException d)
                {
                    snackbarService.EnqueueError(d.Message);
                }
                e.Handled = true;
            };
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<SysModule>();
        }
        protected override IContainerExtension CreateContainerExtension()
        {
            var containerExtension = base.CreateContainerExtension() as DryIocContainerExtension;
            var app = AbpApplicationFactory.Create<MaiziWPFModule>(options =>
            {
                var builder = new ConfigurationBuilder();
                builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                var configuration = builder.Build();
                options.Services.ReplaceConfiguration(configuration);
                options.Services.Configure<MqttOptions>(configuration.GetSection("Mqtt"));

                Log.Logger = new LoggerConfiguration()
#if DEBUG
                    .MinimumLevel.Debug()
#else
                    .MinimumLevel.Information()
#endif
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day) // 按天滚动保存日志文件
                .CreateLogger();

                options.Services.AddLogging(log => log.AddSerilog());
            });
            app.Initialize();
            containerExtension.Populate(app.Services);
            return containerExtension;
        }
        protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
        {
            base.ConfigureRegionAdapterMappings(regionAdapterMappings);
            regionAdapterMappings.RegisterMapping(typeof(TabControl), Container.Resolve<TabControlRegionAdapter>());
        }
    }
}