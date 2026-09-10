using FreeSql;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;

namespace MaiziWPF.Services.MySql
{

    public class SysUserRepository : BaseRepository<SysUser, int>, ISysUserRepository
    {
        private readonly IFreeSql _fsql;

        public SysUserRepository(IFreeSql fsql) : base(fsql)
        {
            _fsql = fsql;
        }

        public int BatchUserDept(List<SysUserDept> userDeptList)
        {
            return _fsql.Insert(userDeptList).ExecuteAffrows();
        }

        public int BatchUserPost(List<SysUserPost> userPostList)
        {
            return _fsql.Insert(userPostList).ExecuteAffrows();
        }

        public int BatchUserRole(List<SysUserRole> userRoleList)
        {
            return _fsql.Insert(userRoleList).ExecuteAffrows();
        }

        public long InsertUser(SysUser user)
        {
            return _fsql.Insert(user).ExecuteIdentity();
        }

        public SysUser SelectUserByUserName(string userName)
        {
               return _fsql.Select<SysUser>()
                    .IncludeMany(a => a.Roles)
                    .IncludeMany(a=>a.Depts)
                    .IncludeMany(a=>a.Posts)
                    .Where(a=>a.UserName == userName)
                    .First();
          
        }

        public List<SysUser> SelectUserList(QueryUserInput input)
        {
            System.Linq.Expressions.Expression<Func<SysUser, bool>> where = d => d.DelFlag == "0";

            if (!string.IsNullOrEmpty(input.UserName))
                where = where.And(u => u.UserName.Contains(input.UserName));
            if (!string.IsNullOrEmpty(input.Phonenumber))
                where = where.And(u => u.PhoneNumber.Contains(input.Phonenumber));
            if (!string.IsNullOrEmpty(input.Status))
                where = where.And(u => u.Status.Contains(input.Status));
            if (input.StartDate.HasValue && input.EndDate.HasValue)
                where = where.And(u => u.CreateTime.Between(input.StartDate.Value, input.EndDate.Value));

            return _fsql.Select<SysUser, SysUserDept, SysDept>()
                       .LeftJoin((u, ud, d) => u.UserId == ud.UserId)
                       .LeftJoin((u, ud, d) => ud.DeptId == d.Id)
                       .Distinct()
                       .WhereIf(input.DeptId.HasValue, (u, ud, d) => d.Ancestors.Contains(input.DeptId.Value.ToString()))
                       .WithTempQuery((u, ud, d) => u)
                       .IncludeMany(a => a.Depts)
                       .Where(where)
                       .Page(input)
                       .ToList();
        }

        public bool DeleteUser(long userId)
        {
            // 逻辑删除，设置删除标志为 "2"
            var result = _fsql.Update<SysUser>()
                .Set(u => u.DelFlag, "2")
                .Set(u => u.UpdateTime, DateTime.Now)
                .Where(u => u.UserId == userId)
                .ExecuteAffrows();
            
            return result > 0;
        }

        public bool UpdateUser(SysUser user)
        {
            // 更新用户信息
            var result = _fsql.Update<SysUser>()
                .Set(u => u.NickName, user.NickName)
                .Set(u => u.PhoneNumber, user.PhoneNumber)
                .Set(u => u.Email, user.Email)
                .Set(u => u.Sex, user.Sex)
                .Set(u => u.Status, user.Status)
                .Set(u => u.Remark, user.Remark)
                .Set(u => u.UpdateTime, DateTime.Now)
                .Where(u => u.UserId == user.UserId)
                .ExecuteAffrows();
            
            return result > 0;
        }

        public SysUser SelectUserById(long userId)
        {
            return _fsql.Select<SysUser>()
                .IncludeMany(a => a.Roles)
                .IncludeMany(a => a.Depts)
                .IncludeMany(a => a.Posts)
                .Where(a => a.UserId == userId && a.DelFlag == "0")
                .First();
        }

        public bool CheckUserNameUnique(SysUser user)
        {
            var query = _fsql.Select<SysUser>()
                .Where(u => u.UserName == user.UserName && u.DelFlag == "0");
            if (user.UserId != 0)
                query = query.Where(u => u.UserId != user.UserId);
            return !query.Any();
        }

        public bool CheckPhoneUnique(SysUser user)
        {
            var query = _fsql.Select<SysUser>()
                .Where(u => u.PhoneNumber == user.PhoneNumber && u.DelFlag == "0");
            if (user.UserId != 0)
                query = query.Where(u => u.UserId != user.UserId);
            return !query.Any();
        }

        public bool CheckEmailUnique(SysUser user)
        {
            var query = _fsql.Select<SysUser>()
                .Where(u => u.Email == user.Email && u.DelFlag == "0");
            if (user.UserId != 0)
                query = query.Where(u => u.UserId != user.UserId);
            return !query.Any();
        }

        public int DeleteUserRoles(long userId)
        {
            return _fsql.Delete<SysUserRole>()
                .Where(r => r.UserId == userId)
                .ExecuteAffrows();
        }

        public int DeleteUserPosts(long userId)
        {
            return _fsql.Delete<SysUserPost>()
                .Where(p => p.UserId == userId)
                .ExecuteAffrows();
        }

        public int DeleteUserDepts(long userId)
        {
            return _fsql.Delete<SysUserDept>()
                .Where(d => d.UserId == userId)
                .ExecuteAffrows();
        }

        public List<long> SelectUserRoleIds(long userId)
        {
            return _fsql.Select<SysUserRole>()
                .Where(r => r.UserId == userId)
                .ToList(r => r.RoleId);
        }

        public List<long> SelectUserPostIds(long userId)
        {
            return _fsql.Select<SysUserPost>()
                .Where(p => p.UserId == userId)
                .ToList(p => p.PostId);
        }

        public List<long> SelectUserDeptIds(long userId)
        {
            return _fsql.Select<SysUserDept>()
                .Where(d => d.UserId == userId)
                .ToList(d => d.DeptId);
        }
    }
}