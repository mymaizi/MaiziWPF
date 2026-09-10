using FreeSql;
using MaiziWPF.Services.Domain.Shared;
using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Domain
{
    public interface ISysRoleRepository : IBaseRepository<SysRole, int>, ITransientDependency
    {
        List<SysRole> SelectRoleList(QueryRoleInput input);

        SysRole SelectRoleById(long roleId);

        int InsertRole(SysRole role);

        int UpdateRole(SysRole role);

        int DeleteRoleById(long roleId);

        bool CheckRoleNameUnique(SysRole role);

        bool CheckRoleKeyUnique(SysRole role);

        bool CheckRoleExistUser(long roleId);
    }
}