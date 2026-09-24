﻿using FreeSql;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using MaiziWPF.Services.MySql.Audit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.Modularity;

namespace MaiziWPF.Services.MySql
{
    [DependsOn(
        typeof(DomainModule)
    )]
    public class MySqlModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            IFreeSql fsql = new FreeSql.FreeSqlBuilder()
                  .UseConnectionString(FreeSql.DataType.MySql, "Data Source=127.0.0.1;Port=3306;User ID=root;Password=; Initial Catalog=maiziwpf;Charset=utf8mb4; SslMode=none;Min pool size=1")
                  .UseMonitorCommand(cmd =>
                  {
                      var logger = context.Services.GetRequiredService<ILogger<MySqlModule>>();
                      logger.LogInformation(cmd.CommandText);
                  })
                  .UseAutoSyncStructure(true)
                  .Build();

            fsql.GlobalFilter.ApplyIf<IDataPermission>(
                "DataPermission",
                () => DataPermissionManager.ShouldApplyFilter(),
                a => DataPermissionManager.PermittedDeptIds.Contains(a.CreateDept)
                     || DataPermissionManager.PermittedUserIds.Contains(a.CreateBy));

            var serviceProvider = context.Services.BuildServiceProvider();
            fsql.UseAuditValue(serviceProvider);

            context.Services.AddSingleton<IFreeSql>(fsql);
            context.Services.AddFreeRepository();
            context.Services.AddScoped<IFreeSql>(r => r.GetService<UnitOfWorkManager>().Orm);
            context.Services.AddScoped<UnitOfWorkManager>(r => new UnitOfWorkManager(fsql));
            TransactionalAttribute.SetServiceProvider(serviceProvider);
        }
    }
}