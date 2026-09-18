using FreeSql;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;

namespace MaiziWPF.Services.Application
{
    public class SysUserService : ISysUserService
    {
        private readonly ISysUserRepository _repository;

        public SysUserService(ISysUserRepository repository)
        {
            _repository = repository;
        }

        public SysUser SelectUserByUserName(string userName)
        {
            return _repository.SelectUserByUserName(userName);
        }

        public SysUser SelectUserById(long userId)
        {
            return _repository.SelectUserById(userId);
        }

        public List<SysUser> SelectUserList(QueryUserInput input)
        {
            return _repository.SelectUserList(input);
        }

        [Transactional]
        public long InsertUser(SysUser user)
        {
            long userId = _repository.InsertUser(user);
            user.UserId = userId;
            InsertUserPost(user);
            InsertUserRole(user);
            return userId;
        }

        [Transactional]
        public bool UpdateUser(SysUser user)
        {
            var result = _repository.UpdateUser(user);
            _repository.DeleteUserRoles(user.UserId);
            _repository.DeleteUserPosts(user.UserId);
            InsertUserPost(user);
            InsertUserRole(user);
            return result;
        }

        public bool DeleteUser(long userId)
        {
            return _repository.DeleteUser(userId);
        }

        public bool CheckUserNameUnique(SysUser user)
        {
            return _repository.CheckUserNameUnique(user);
        }

        public bool CheckPhoneUnique(SysUser user)
        {
            return _repository.CheckPhoneUnique(user);
        }

        public bool CheckEmailUnique(SysUser user)
        {
            return _repository.CheckEmailUnique(user);
        }

        public void InsertUserPost(SysUser user)
        {
            if (user.Posts != null && user.Posts.Any())
            {
                List<SysUserPost> list = new();
                user.Posts.ForEach(post =>
                {
                    SysUserPost up = new SysUserPost();
                    up.UserId = user.UserId;
                    up.PostId = post.PostId;
                    list.Add(up);
                });
                _repository.BatchUserPost(list);
            }
        }

        public void InsertUserRole(SysUser user)
        {
            if (user.Roles != null && user.Roles.Any())
            {
                List<SysUserRole> list = new();
                user.Roles.ForEach(role =>
                {
                    SysUserRole ur = new SysUserRole();
                    ur.UserId = user.UserId;
                    ur.RoleId = role.RoleId;
                    list.Add(ur);
                });
                _repository.BatchUserRole(list);
            }
        }

    

        public List<long> SelectUserRoleIds(long userId)
        {
            return _repository.SelectUserRoleIds(userId);
        }

        public List<long> SelectUserPostIds(long userId)
        {
            return _repository.SelectUserPostIds(userId);
        }

        public List<SysRole> SelectAllRoles()
        {
            return _repository.Orm.Select<SysRole>().ToList();
        }

        public List<SysPost> SelectAllPosts()
        {
            return _repository.Orm.Select<SysPost>().ToList();
        }

        public void ResetPwd(long userId)
        {
            _repository.ResetPwd(userId);
        }
    }
}