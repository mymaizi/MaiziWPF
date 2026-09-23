using FreeSql;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MaiziWPF.Services.MySql
{

    public class SysUserRepository : BaseRepository<SysUser, int>, ISysUserRepository
    {
        private readonly IFreeSql _fsql;

        public SysUserRepository(IFreeSql fsql) : base(fsql)
        {
            _fsql = fsql;
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
                    .IncludeMany(a=>a.Posts)
                    .Where(a=>a.UserName == userName)
                    .First();
        }

        public List<SysUser> SelectUserList(QueryUserInput input)
        {
            var query = _fsql.Select<SysUser>()
                .Include(a => a.Dept)
                .Where(d => d.DelFlag == "0");

            if (!string.IsNullOrEmpty(input.UserName))
                query = query.Where(u => u.UserName.Contains(input.UserName));
            if (!string.IsNullOrEmpty(input.Phonenumber))
                query = query.Where(u => u.PhoneNumber.Contains(input.Phonenumber));
            if (!string.IsNullOrEmpty(input.Status))
                query = query.Where(u => u.Status == input.Status);
            if (input.DeptId.HasValue && input.DeptId.Value > 0)
                query = query.Where(u => u.DeptId == input.DeptId.Value);
            if (input.StartDate.HasValue && input.EndDate.HasValue)
                query = query.Where(u => u.CreateTime.Between(input.StartDate.Value, input.EndDate.Value));

            return query.Page(input).ToList();
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
            var result = _fsql.Update<SysUser>()
                .Set(u => u.DeptId, user.DeptId)
                .Set(u => u.NickName, user.NickName)
                .Set(u => u.PhoneNumber, user.PhoneNumber)
                .Set(u => u.Email, user.Email)
                .Set(u => u.Gender, user.Gender)
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
                .Include(a => a.Dept)
                .IncludeMany(a => a.Roles)
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

     

        public void ResetPwd(long userId)
        {
            var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes("123456")));
            _fsql.Update<SysUser>()
                .Set(u => u.Password, hash)
                .Where(u => u.UserId == userId)
                .ExecuteAffrows();
        }

        public List<SysRole> SelectAllocatedRolesByUserId(long userId)
        {
            var roleIds = _fsql.Select<SysUserRole>()
                .Where(r => r.UserId == userId)
                .ToList(r => r.RoleId);
            if (!roleIds.Any()) return new List<SysRole>();
            return _fsql.Select<SysRole>()
                .Where(r => roleIds.Contains(r.RoleId) && r.DelFlag == "0")
                .OrderBy(r => r.RoleSort)
                .ToList();
        }

        public List<SysRole> SelectUnallocatedRolesByUserId(long userId, string roleName, string roleKey)
        {
            var roleIds = _fsql.Select<SysUserRole>()
                .Where(r => r.UserId == userId)
                .ToList(r => r.RoleId);

            var query = _fsql.Select<SysRole>()
                .Where(r => r.DelFlag == "0");

            if (roleIds.Any())
                query = query.Where(r => !roleIds.Contains(r.RoleId));

            if (!string.IsNullOrEmpty(roleName))
                query = query.Where(r => r.RoleName.Contains(roleName));
            if (!string.IsNullOrEmpty(roleKey))
                query = query.Where(r => r.RoleKey.Contains(roleKey));

            return query.OrderBy(r => r.RoleSort).ToList();
        }

        public int InsertAuthRoles(long userId, long[] roleIds)
        {
            if (roleIds == null || roleIds.Length == 0) return 0;
            var entities = roleIds.Select(roleId => new SysUserRole
            {
                UserId = userId,
                RoleId = roleId
            }).ToList();
            return _fsql.Insert(entities).ExecuteAffrows();
        }

        public int CancelAuthRole(long userId, long roleId)
        {
            return _fsql.Delete<SysUserRole>()
                .Where(r => r.UserId == userId && r.RoleId == roleId)
                .ExecuteAffrows();
        }

        public void UpdateOnlineStatus(long userId, int onlineStatus)
        {
            _fsql.Update<SysUser>()
                .Set(u => u.OnlineStatus, onlineStatus)
                .Set(u => u.LastHeartbeat, DateTime.Now)
                .Where(u => u.UserId == userId)
                .ExecuteAffrows();
        }

        public void UpdateHeartbeat(long userId)
        {
            _fsql.Update<SysUser>()
                .Set(u => u.LastHeartbeat, DateTime.Now)
                .Where(u => u.UserId == userId)
                .ExecuteAffrows();
        }
    }
}