using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using System.Collections.Generic;
using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Application.Contracts
{
    public interface ICurrentUserService : ISingletonDependency
    {
        SysUser CurrentUser { get; }

        long UserId { get; }

        long DeptId { get; }

        string UserName { get; }

        bool IsAuthenticated { get; }

        bool IsSuperAdmin { get; }

        List<SysMenu> MenuTree { get; }

        HashSet<string> Permissions { get; }

        List<string> RoleKeys { get; }

        List<DataScopeRule> GetDataScopeRules();

        bool HasPermission(string perms);

        void SetCurrentUser(SysUser user);

        void SetMenuTree(List<SysMenu> menuTree);

        void SetPermissions(HashSet<string> permissions);

        void SetRoles(List<string> roleKeys);

        void InitializeDataPermissions();

        void Clear();
    }
}