﻿﻿﻿﻿using FreeSql;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using System;
using System.Collections.Generic;
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
                       .OrderBy(r => r.RoleSort)
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

        public int UpdateRoleBaseInfo(SysRole role)
        {
            role.UpdateTime = DateTime.Now;
            return _fsql.Update<SysRole>()
                .Set(r => r.RoleName, role.RoleName)
                .Set(r => r.RoleKey, role.RoleKey)
                .Set(r => r.RoleSort, role.RoleSort)
                .Set(r => r.DataScope, role.DataScope)
                .Set(r => r.MenuCheckStrictly, role.MenuCheckStrictly)
                .Set(r => r.DeptCheckStrictly, role.DeptCheckStrictly)
                .Set(r => r.Status, role.Status)
                .Set(r => r.Remark, role.Remark)
                .Set(r => r.UpdateTime, role.UpdateTime)
                .Where(r => r.RoleId == role.RoleId)
                .ExecuteAffrows();
        }

        public int UpdateRolePermission(SysRole role, long[] menuIds, long[] deptIds)
        {
            role.UpdateTime = DateTime.Now;
            _fsql.Update<SysRole>()
                .Set(r => r.DataScope, role.DataScope)
                .Set(r => r.MenuCheckStrictly, role.MenuCheckStrictly)
                .Set(r => r.DeptCheckStrictly, role.DeptCheckStrictly)
                .Set(r => r.UpdateTime, role.UpdateTime)
                .Where(r => r.RoleId == role.RoleId)
                .ExecuteAffrows();

            _fsql.Delete<SysRoleMenu>()
                .Where(r => r.RoleId == role.RoleId)
                .ExecuteAffrows();
            if (menuIds != null && menuIds.Length > 0)
            {
                var roleMenus = menuIds.Select(menuId => new SysRoleMenu
                {
                    RoleId = role.RoleId,
                    MenuId = menuId
                }).ToList();
                _fsql.Insert(roleMenus).ExecuteAffrows();
            }

            _fsql.Delete<SysRoleDept>()
                .Where(r => r.RoleId == role.RoleId)
                .ExecuteAffrows();
            if (deptIds != null && deptIds.Length > 0)
            {
                var roleDepts = deptIds.Select(deptId => new SysRoleDept
                {
                    RoleId = role.RoleId,
                    DeptId = deptId
                }).ToList();
                _fsql.Insert(roleDepts).ExecuteAffrows();
            }

            return 1;
        }

        public int UpdateRoleStatus(long roleId, string status)
        {
            return _fsql.Update<SysRole>()
                .Set(r => r.Status, status)
                .Set(r => r.UpdateTime, DateTime.Now)
                .Where(r => r.RoleId == roleId)
                .ExecuteAffrows();
        }

        public int DeleteRoleById(long roleId)
        {
            return _fsql.Update<SysRole>()
                .Set(r => r.DelFlag, "2")
                .Where(r => r.RoleId == roleId)
                .ExecuteAffrows();
        }

        public int DeleteRoleByIds(List<long> roleIds)
        {
            _fsql.Delete<SysRoleMenu>()
                .Where(r => roleIds.Contains(r.RoleId))
                .ExecuteAffrows();
            _fsql.Delete<SysRoleDept>()
                .Where(r => roleIds.Contains(r.RoleId))
                .ExecuteAffrows();
            return _fsql.Update<SysRole>()
                .Set(r => r.DelFlag, "2")
                .Where(r => roleIds.Contains(r.RoleId))
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

        public List<long> SelectRoleMenuIds(long roleId)
        {
            return _fsql.Select<SysRoleMenu>()
                .Where(r => r.RoleId == roleId)
                .ToList(r => r.MenuId);
        }

        public List<long> SelectRoleDeptIds(long roleId)
        {
            return _fsql.Select<SysRoleDept>()
                .Where(r => r.RoleId == roleId)
                .ToList(r => r.DeptId);
        }

        public List<SysUser> SelectAllocatedList(long roleId)
        {
            return _fsql.Select<SysUser>()
                .Where(u => u.DelFlag == "0")
                .Where(u => _fsql.Select<SysUserRole>()
                    .Where(r => r.RoleId == roleId)
                    .Where(r => r.UserId == u.UserId)
                    .Any())
                .ToList();
        }

        public List<SysUser> SelectUnallocatedList(long roleId, string userName, string phonenumber)
        {
            var query = _fsql.Select<SysUser>()
                .Where(u => u.DelFlag == "0")
                .Where(u => !_fsql.Select<SysUserRole>()
                    .Where(r => r.RoleId == roleId)
                    .Where(r => r.UserId == u.UserId)
                    .Any());
            if (!string.IsNullOrEmpty(userName))
                query = query.Where(u => u.UserName.Contains(userName));
            if (!string.IsNullOrEmpty(phonenumber))
                query = query.Where(u => u.PhoneNumber.Contains(phonenumber));
            return query.ToList();
        }

        public int InsertAuthUsers(long roleId, long[] userIds)
        {
            if (userIds == null || userIds.Length == 0) return 0;
            var entities = userIds.Select(userId => new SysUserRole
            {
                UserId = userId,
                RoleId = roleId
            }).ToList();
            return (int)_fsql.Insert(entities).ExecuteAffrows();
        }

        public int CancelAuthUser(long roleId, long userId)
        {
            return _fsql.Delete<SysUserRole>()
                .Where(r => r.RoleId == roleId && r.UserId == userId)
                .ExecuteAffrows();
        }
    }
}