﻿﻿using FreeSql;
using FreeSql.Aop;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Volo.Abp.Modularity;

namespace MaiziWPF.Services.MySql
{
    [DependsOn(
        typeof(DomainModule)
    )]
    public class MySqlModule : AbpModule
    {
        private static IAuditUserProvider _auditProvider;

        public static void SetAuditProvider(IAuditUserProvider provider) => _auditProvider = provider;

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            IFreeSql fsql = new FreeSql.FreeSqlBuilder()
                  .UseConnectionString(FreeSql.DataType.MySql, "Data Source=127.0.0.1;Port=3306;User ID=root;Password=123456; Initial Catalog=maiziwpf;Charset=utf8mb4; SslMode=none;Min pool size=1")
                  .UseMonitorCommand(cmd =>
                  {
                      var logger = context.Services.GetRequiredService<ILogger<MySqlModule>>();
                      logger.LogInformation(cmd.CommandText);
                  })
                  .UseAutoSyncStructure(true)
                  .Build();

            var sp = context.Services.BuildServiceProvider();

            fsql.Aop.AuditValue += (s, e) =>
            {
                if (e.Object is not BaseEntity) return;

                var auditProvider = _auditProvider;

                if (e.AuditValueType == AuditValueType.Insert)
                {
                    switch (e.Property.Name)
                    {
                        case nameof(BaseEntity.CreateBy):
                            if (auditProvider != null && auditProvider.IsAuthenticated)
                                e.Value = auditProvider.UserId;
                            break;
                        case nameof(BaseEntity.CreateDept):
                            if (auditProvider != null && auditProvider.IsAuthenticated)
                                e.Value = auditProvider.DeptId;
                            break;
                        case nameof(BaseEntity.CreateTime):
                            e.Value = DateTime.Now;
                            break;
                        case nameof(BaseEntity.UpdateBy):
                            if (auditProvider != null && auditProvider.IsAuthenticated)
                                e.Value = auditProvider.UserId;
                            break;
                        case nameof(BaseEntity.UpdateTime):
                            e.Value = DateTime.Now;
                            break;
                    }
                }
                else if (e.AuditValueType == AuditValueType.Update)
                {
                    switch (e.Property.Name)
                    {
                        case nameof(BaseEntity.UpdateBy):
                            if (auditProvider != null && auditProvider.IsAuthenticated)
                                e.Value = auditProvider.UserId;
                            break;
                        case nameof(BaseEntity.UpdateTime):
                            e.Value = DateTime.Now;
                            break;
                    }
                }
            };

            context.Services.AddSingleton<IFreeSql>(fsql);
            context.Services.AddFreeRepository();
            context.Services.AddScoped<IFreeSql>(r => r.GetService<UnitOfWorkManager>().Orm);
            context.Services.AddScoped<UnitOfWorkManager>(r => new UnitOfWorkManager(fsql));
            TransactionalAttribute.SetServiceProvider(sp);
        }
    }
}