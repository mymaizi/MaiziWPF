using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Application.Contracts
{
    public interface ISysUserService: ITransientDependency
    {
        SysUser SelectUserByUserName(String userName);

        SysUser SelectUserById(long userId);

        List<SysUser> SelectUserList(QueryUserInput input);

        long InsertUser(SysUser user);

        bool DeleteUser(long userId);

        bool UpdateUser(SysUser user);

        bool CheckUserNameUnique(SysUser user);

        bool CheckPhoneUnique(SysUser user);

        bool CheckEmailUnique(SysUser user);

        void InsertUserRole(SysUser user);

        List<long> SelectUserRoleIds(long userId);

        List<long> SelectUserPostIds(long userId);

        List<SysRole> SelectAllRoles();

        List<SysPost> SelectAllPosts();

        void ResetPwd(long userId);

        (SysUser User, List<SysRole> Roles) GetAuthRole(long userId);

        void UpdateAuthRole(long userId, List<long> roleIds);

        List<SysRole> SelectAllocatedRolesByUserId(long userId);

        List<SysRole> SelectUnallocatedRolesByUserId(long userId, string roleName, string roleKey);

        int InsertAuthRoles(long userId, long[] roleIds);

        int CancelAuthRole(long userId, long roleId);
    }
}