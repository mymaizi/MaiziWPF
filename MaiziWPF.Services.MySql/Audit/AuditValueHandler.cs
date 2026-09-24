using FreeSql.Aop;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MaiziWPF.Services.MySql.Audit
{
    public static class AuditServiceExtensions
    {
        public static IServiceCollection AddAuditValue(this IServiceCollection services)
        {
            return services;
        }

        public static IFreeSql UseAuditValue(this IFreeSql fsql, IServiceProvider serviceProvider)
        {
            fsql.Aop.AuditValue += HandleAuditValue;
            return fsql;
        }

        private static void HandleAuditValue(object? sender, AuditValueEventArgs e)
        {
            if (e.Object is not BaseEntity) return;

            if (e.AuditValueType == AuditValueType.Insert)
            {
                switch (e.Property.Name)
                {
                    case nameof(BaseEntity.CreateBy):
                        if (AuditUserContext.IsAuthenticated)
                            e.Value = AuditUserContext.UserId;
                        break;
                    case nameof(BaseEntity.CreateDept):
                        if (AuditUserContext.IsAuthenticated)
                            e.Value = AuditUserContext.DeptId;
                        break;
                    case nameof(BaseEntity.CreateTime):
                        e.Value = DateTime.Now;
                        break;
                    case nameof(BaseEntity.UpdateBy):
                        if (AuditUserContext.IsAuthenticated)
                            e.Value = AuditUserContext.UserId;
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
                        if (AuditUserContext.IsAuthenticated)
                            e.Value = AuditUserContext.UserId;
                        break;
                    case nameof(BaseEntity.UpdateTime):
                        e.Value = DateTime.Now;
                        break;
                }
            }
        }
    }
}