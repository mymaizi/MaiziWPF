﻿﻿using MaiziWPF.Services.Domain;
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

        int UpdateRoleBaseInfo(SysRole role);

        int UpdateRolePermission(SysRole role, long[] menuIds, long[] deptIds);

        int UpdateRoleStatus(long roleId, string status);

        int DeleteRoleById(long roleId);

        int DeleteRoleByIds(List<long> roleIds);

        bool CheckRoleNameUnique(SysRole role);

        bool CheckRoleKeyUnique(SysRole role);

        bool CheckRoleExistUser(long roleId);

        void CheckRoleAllowed(SysRole role);

        List<long> SelectRoleMenuIds(long roleId);

        List<long> SelectRoleDeptIds(long roleId);

        List<SysUser> SelectAllocatedList(long roleId);

        List<SysUser> SelectUnallocatedList(long roleId, string userName, string phonenumber);

        int InsertAuthUsers(long roleId, long[] userIds);

        int CancelAuthUser(long roleId, long userId);
    }
}