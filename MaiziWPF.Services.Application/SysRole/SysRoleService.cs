﻿﻿using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using System;
using System.Collections.Generic;

namespace MaiziWPF.Services.Application
{
    public class SysRoleService : ISysRoleService
    {
        private readonly ISysRoleRepository _repository;

        private const long SUPER_ADMIN_ROLE_ID = 1761300000000000001L;
        private static readonly string[] SUPER_ADMIN_ROLE_KEYS = { "superadmin" };

        public SysRoleService(ISysRoleRepository repository)
        {
            _repository = repository;
        }

        public List<SysRole> SelectRoleList(QueryRoleInput input)
        {
            return _repository.SelectRoleList(input);
        }

        public SysRole SelectRoleById(long roleId)
        {
            return _repository.SelectRoleById(roleId);
        }

        public int InsertRole(SysRole role)
        {
            CheckRoleAllowed(role);
            return _repository.InsertRole(role);
        }

        public int UpdateRole(SysRole role)
        {
            CheckRoleAllowed(role);
            return _repository.UpdateRole(role);
        }

        public int UpdateRoleBaseInfo(SysRole role)
        {
            CheckRoleAllowed(role);
            return _repository.UpdateRoleBaseInfo(role);
        }

        public int UpdateRolePermission(SysRole role, long[] menuIds, long[] deptIds)
        {
            CheckRoleAllowed(role);
            return _repository.UpdateRolePermission(role, menuIds, deptIds);
        }

        public int UpdateRoleStatus(long roleId, string status)
        {
            var role = _repository.SelectRoleById(roleId);
            if (role == null) return 0;
            CheckRoleAllowed(role);

            if (status == "1" && _repository.CheckRoleExistUser(roleId))
            {
                throw new Exception("角色已分配，不能禁用!");
            }
            return _repository.UpdateRoleStatus(roleId, status);
        }

        public int DeleteRoleById(long roleId)
        {
            var role = _repository.SelectRoleById(roleId);
            if (role == null) return 0;
            CheckRoleAllowed(role);

            if (_repository.CheckRoleExistUser(roleId))
            {
                throw new Exception($"角色'{role.RoleName}'已分配，不能删除!");
            }
            return _repository.DeleteRoleById(roleId);
        }

        public int DeleteRoleByIds(List<long> roleIds)
        {
            foreach (var roleId in roleIds)
            {
                var role = _repository.SelectRoleById(roleId);
                if (role == null) continue;
                CheckRoleAllowed(role);

                if (_repository.CheckRoleExistUser(roleId))
                {
                    throw new Exception($"角色'{role.RoleName}'已分配，不能删除!");
                }
            }
            return _repository.DeleteRoleByIds(roleIds);
        }

        public bool CheckRoleNameUnique(SysRole role)
        {
            return _repository.CheckRoleNameUnique(role);
        }

        public bool CheckRoleKeyUnique(SysRole role)
        {
            return _repository.CheckRoleKeyUnique(role);
        }

        public bool CheckRoleExistUser(long roleId)
        {
            return _repository.CheckRoleExistUser(roleId);
        }

        public void CheckRoleAllowed(SysRole role)
        {
            if (role.RoleId != 0 && role.RoleId == SUPER_ADMIN_ROLE_ID)
            {
                throw new Exception("不允许操作超级管理员角色!");
            }
            if (role.RoleId == 0 && Array.Exists(SUPER_ADMIN_ROLE_KEYS, k => k == role.RoleKey))
            {
                throw new Exception("不允许使用系统内置管理员角色标识符!");
            }
            if (role.RoleId != 0)
            {
                var existingRole = _repository.SelectRoleById(role.RoleId);
                if (existingRole != null)
                {
                    bool isExistingSuperAdmin = Array.Exists(SUPER_ADMIN_ROLE_KEYS, k => k == existingRole.RoleKey);
                    bool isNewSuperAdmin = Array.Exists(SUPER_ADMIN_ROLE_KEYS, k => k == role.RoleKey);
                    if (existingRole.RoleKey != role.RoleKey)
                    {
                        if (isExistingSuperAdmin)
                            throw new Exception("不允许修改系统内置管理员角色标识符!");
                        if (isNewSuperAdmin)
                            throw new Exception("不允许使用系统内置管理员角色标识符!");
                    }
                }
            }
        }

        public List<long> SelectRoleMenuIds(long roleId)
        {
            return _repository.SelectRoleMenuIds(roleId);
        }

        public List<long> SelectRoleDeptIds(long roleId)
        {
            return _repository.SelectRoleDeptIds(roleId);
        }

        public List<SysUser> SelectAllocatedList(long roleId)
        {
            return _repository.SelectAllocatedList(roleId);
        }

        public List<SysUser> SelectUnallocatedList(long roleId, string userName, string phonenumber)
        {
            return _repository.SelectUnallocatedList(roleId, userName, phonenumber);
        }

        public int InsertAuthUsers(long roleId, long[] userIds)
        {
            return _repository.InsertAuthUsers(roleId, userIds);
        }

        public int CancelAuthUser(long roleId, long userId)
        {
            return _repository.CancelAuthUser(roleId, userId);
        }
    }
}