using MaiziWPF.Services.Domain;
using System.Collections.Generic;
using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Application.Contracts
{
    public interface ICurrentUserService : ISingletonDependency
    {
        SysUser CurrentUser { get; }

        bool IsAuthenticated { get; }

        List<SysMenu> MenuTree { get; }

        HashSet<string> Permissions { get; }

        List<string> RoleKeys { get; }

        bool IsSuperAdmin { get; }

        long UserId { get; }

        string UserName { get; }

        long DeptId { get; }

        void SetCurrentUser(SysUser user);

        void SetMenuTree(List<SysMenu> menuTree);

        void SetPermissions(HashSet<string> permissions);

        void SetRoles(List<string> roleKeys);

        bool HasPermission(string perms);

        void Clear();
    }
}