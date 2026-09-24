using FreeSql.Aop;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MaiziWPF.Services.MySql.Audit
{
    public class AuditValueHandler
    {
        private readonly ICurrentUserService _currentUser;

        public AuditValueHandler(ICurrentUserService currentUser)
        {
            _currentUser = currentUser;
        }

        public void Handle(object? sender, AuditValueEventArgs e)
        {
            if (e.Object is not BaseEntity) return;

            if (e.AuditValueType == AuditValueType.Insert)
            {
                switch (e.Property.Name)
                {
                    case nameof(BaseEntity.CreateBy):
                        if (_currentUser.IsAuthenticated)
                            e.Value = _currentUser.UserId;
                        break;
                    case nameof(BaseEntity.CreateDept):
                        if (_currentUser.IsAuthenticated)
                            e.Value = _currentUser.DeptId;
                        break;
                    case nameof(BaseEntity.CreateTime):
                        e.Value = DateTime.Now;
                        break;
                    case nameof(BaseEntity.UpdateBy):
                        if (_currentUser.IsAuthenticated)
                            e.Value = _currentUser.UserId;
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
                        if (_currentUser.IsAuthenticated)
                            e.Value = _currentUser.UserId;
                        break;
                    case nameof(BaseEntity.UpdateTime):
                        e.Value = DateTime.Now;
                        break;
                }
            }
        }
    }

    public static class AuditServiceExtensions
    {
        public static IServiceCollection AddAudit(this IServiceCollection services)
        {
            services.AddSingleton<AuditValueHandler>();
            return services;
        }

        public static IFreeSql UseAuditValue(this IFreeSql fsql, IServiceProvider serviceProvider)
        {
            var handler = serviceProvider.GetRequiredService<AuditValueHandler>();
            fsql.Aop.AuditValue += handler.Handle;
            return fsql;
        }
    }
}