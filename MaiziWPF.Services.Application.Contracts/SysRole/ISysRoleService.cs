using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using System.Collections.Generic;
using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Application.Contracts
{
    public interface ISysRoleService : ITransientDependency
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