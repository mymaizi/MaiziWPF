using FreeSql;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using System;
using System.Linq;

namespace MaiziWPF.Services.MySql
{
    public class SysRoleRepository : BaseRepository<SysRole, int>, ISysRoleRepository
    {
        private readonly IFreeSql _fsql;

        public SysRoleRepository(IFreeSql fsql) : base(fsql)
        {
            _fsql = fsql;
        }

        public List<SysRole> SelectRoleList(QueryRoleInput input)
        {
            System.Linq.Expressions.Expression<Func<SysRole, bool>> where = d => d.DelFlag == "0";

            if (!string.IsNullOrEmpty(input.RoleName))
                where = where.And(u => u.RoleName.Contains(input.RoleName));
            if (!string.IsNullOrEmpty(input.RoleKey))
                where = where.And(u => u.RoleKey.Contains(input.RoleKey));
            if (!string.IsNullOrEmpty(input.Status))
                where = where.And(u => u.Status == input.Status);
            if (input.StartDate.HasValue && input.EndDate.HasValue)
                where = where.And(u => u.CreateTime.Between(input.StartDate.Value, input.EndDate.Value));

            return _fsql.Select<SysRole>()
                       .Where(where)
                       .Page(input)
                       .ToList();
        }

        public SysRole SelectRoleById(long roleId)
        {
            return _fsql.Select<SysRole>().Where(r => r.RoleId == roleId && r.DelFlag == "0").First();
        }

        public int InsertRole(SysRole role)
        {
            role.CreateTime = DateTime.Now;
            return (int)_fsql.Insert(role).ExecuteAffrows();
        }

        public int UpdateRole(SysRole role)
        {
            role.UpdateTime = DateTime.Now;
            return _fsql.Update<SysRole>().SetSource(role).ExecuteAffrows();
        }

        public int DeleteRoleById(long roleId)
        {
            return _fsql.Update<SysRole>()
                .Set(r => r.DelFlag, "2")
                .Where(r => r.RoleId == roleId)
                .ExecuteAffrows();
        }

        public bool CheckRoleNameUnique(SysRole role)
        {
            var query = _fsql.Select<SysRole>()
                .Where(r => r.RoleName == role.RoleName && r.DelFlag == "0");
            if (role.RoleId != 0)
                query = query.Where(r => r.RoleId != role.RoleId);
            return !query.Any();
        }

        public bool CheckRoleKeyUnique(SysRole role)
        {
            var query = _fsql.Select<SysRole>()
                .Where(r => r.RoleKey == role.RoleKey && r.DelFlag == "0");
            if (role.RoleId != 0)
                query = query.Where(r => r.RoleId != role.RoleId);
            return !query.Any();
        }

        public bool CheckRoleExistUser(long roleId)
        {
            return _fsql.Select<SysUserRole>()
                .Where(r => r.RoleId == roleId)
                .Any();
        }
    }
}