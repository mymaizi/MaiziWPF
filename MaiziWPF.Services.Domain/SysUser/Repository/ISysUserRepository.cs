using FreeSql;
using MaiziWPF.Services.Domain.Shared;
using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Domain
{
    public interface ISysUserRepository: IBaseRepository<SysUser,int>, ITransientDependency
    {
        /**
         * 通过用户名查询用户
         * 
         * @param userName 用户名
         * @return 用户对象信息
         */
        public SysUser SelectUserByUserName(String userName);
        /**
         * 根据条件分页查询用户列表
         * 
         * @param sysUser 用户信息
         * @return 用户信息集合信息
         */
        public List<SysUser> SelectUserList(QueryUserInput input);
        /**
       * 新增用户信息
       * 
       * @param user 用户信息
       * @return 结果
       */
        public long InsertUser(SysUser user);
        /**
        * 批量新增用户岗位信息
        * 
        * @param userPostList 用户岗位列表
        * @return 结果
        */
        public int BatchUserPost(List<SysUserPost> userPostList);
        /**
        * 批量新增用户角色信息
        * 
        * @param userRoleList 用户角色列表
        * @return 结果
        */
        public int BatchUserRole(List<SysUserRole> userRoleList);
        /**
        * 批量新增用户部门信息
        * 
        * @param userDeptList 用户角色列表
        * @return 结果
        */
        public int BatchUserDept(List<SysUserDept> userDeptList);
        
        /**
        * 删除用户信息
        * 
        * @param userId 用户ID
        * @return 结果
        */
        public bool DeleteUser(long userId);
        
        /**
        * 修改用户信息
        * 
        * @param user 用户信息
        * @return 结果
        */
        public bool UpdateUser(SysUser user);

        SysUser SelectUserById(long userId);

        bool CheckUserNameUnique(SysUser user);

        bool CheckPhoneUnique(SysUser user);

        bool CheckEmailUnique(SysUser user);

        int DeleteUserRoles(long userId);

        int DeleteUserPosts(long userId);

        int DeleteUserDepts(long userId);

        List<long> SelectUserRoleIds(long userId);

        List<long> SelectUserPostIds(long userId);

        List<long> SelectUserDeptIds(long userId);

        void ResetPwd(long userId);
    }
}