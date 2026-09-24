using FreeSql;
using MaiziWPF.Common;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MaiziWPF.Services.Application
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IFreeSql _fsql;
        private readonly ILogger<CurrentUserService> _logger;

        private SysUser _currentUser;
        private List<SysMenu> _menuTree = new();
        private HashSet<string> _permissions = new();
        private List<string> _roleKeys = new();

        public CurrentUserService(IFreeSql fsql, ILogger<CurrentUserService> logger)
        {
            _fsql = fsql;
            _logger = logger;
        }

        public SysUser CurrentUser => _currentUser;

        public bool IsAuthenticated => _currentUser != null;

        public List<SysMenu> MenuTree => _menuTree;

        public HashSet<string> Permissions => _permissions;

        public List<string> RoleKeys => _roleKeys;

        public bool IsSuperAdmin => _currentUser != null && SecurityUtils.IsSuperAdmin(_currentUser.UserId);

        public long UserId => _currentUser?.UserId ?? 0;

        public string UserName => _currentUser?.UserName;

        public long DeptId => _currentUser?.DeptId ?? 0;

        public void SetCurrentUser(SysUser user)
        {
            _currentUser = user;
        }

        public void SetPermissions(HashSet<string> permissions)
        {
            _permissions = permissions ?? new HashSet<string>();
        }

        public void SetRoles(List<string> roleKeys)
        {
            _roleKeys = roleKeys ?? new List<string>();
        }

        public void SetMenuTree(List<SysMenu> menuTree)
        {
            _menuTree = menuTree ?? new List<SysMenu>();
        }

        public bool HasPermission(string perms)
        {
            if (string.IsNullOrEmpty(perms)) return true;
            if (IsSuperAdmin) return true;
            if (_permissions == null || _permissions.Count == 0) return false;

            var requiredPerms = perms.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var required in requiredPerms)
            {
                var trimmed = required.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;

                foreach (var granted in _permissions)
                {
                    if (MatchPermission(trimmed, granted))
                        return true;
                }
            }
            return false;
        }

        private static bool MatchPermission(string required, string granted)
        {
            if (granted == "*:*:*") return true;

            var requiredParts = required.Split(':');
            var grantedParts = granted.Split(':');

            if (requiredParts.Length != grantedParts.Length) return false;

            for (int i = 0; i < requiredParts.Length; i++)
            {
                if (grantedParts[i] == "*") continue;
                if (!string.Equals(requiredParts[i], grantedParts[i], StringComparison.OrdinalIgnoreCase))
                    return false;
            }
            return true;
        }

        public void InitializeDataPermissions()
        {
            if (!IsAuthenticated || IsSuperAdmin)
            {
                DataPermissionManager.Clear();
                return;
            }

            var rules = GetDataScopeRules();
            DataPermissionManager.ApplyFromRules(rules);

            _logger.LogDebug(
                "数据权限初始化: UserId={UserId}, DeptIds=[{DeptIds}], UserIds=[{UserIds}]",
                UserId,
                string.Join(",", DataPermissionManager.PermittedDeptIds),
                string.Join(",", DataPermissionManager.PermittedUserIds));
        }

        public List<DataScopeRule> GetDataScopeRules()
        {
            if (!IsAuthenticated || IsSuperAdmin)
                return new List<DataScopeRule>();

            if (_currentUser == null)
                return new List<DataScopeRule>();

            using (_fsql.GlobalFilter.Disable("DataPermission"))
            {
                var roles = _fsql.Select<SysRole>()
                    .InnerJoin<SysUserRole>((r, ur) => r.RoleId == ur.RoleId && ur.UserId == _currentUser.UserId)
                    .Where(r => r.Status == "0" && r.DelFlag == "0")
                    .ToList();

                if (roles == null || roles.Count == 0)
                    return new List<DataScopeRule>();

                var rules = new List<DataScopeRule>();

                foreach (var role in roles)
                {
                    if (!Enum.TryParse<DataScopeType>(role.DataScope, out var scopeType))
                        continue;

                    var rule = new DataScopeRule
                    {
                        ScopeType = scopeType,
                        RoleId = role.RoleId,
                        DeptId = _currentUser.DeptId,
                        UserId = _currentUser.UserId
                    };

                    switch (scopeType)
                    {
                        case DataScopeType.CUSTOM:
                            rule.CustomDeptIds = GetRoleCustomDeptIds(role.RoleId);
                            break;

                        case DataScopeType.DEPT_AND_CHILD:
                        case DataScopeType.DEPT_AND_CHILD_OR_SELF:
                            rule.DeptAndChildIds = GetDeptAndChildIds(_currentUser.DeptId);
                            break;
                    }

                    rules.Add(rule);
                }

                return rules;
            }
        }

        private List<long> GetRoleCustomDeptIds(long roleId)
        {
            return _fsql.Select<SysRoleDept>()
                .Where(rd => rd.RoleId == roleId)
                .ToList(rd => rd.DeptId);
        }

        private List<long> GetDeptAndChildIds(long deptId)
        {
            var result = new List<long> { deptId };
            CollectChildDeptIds(deptId, result);
            return result;
        }

        private void CollectChildDeptIds(long parentId, List<long> result)
        {
            var childIds = _fsql.Select<SysDept>()
                .Where(d => d.ParentId == parentId && d.DelFlag == "0" && d.Status == "0")
                .ToList(d => d.Id);

            foreach (var childId in childIds)
            {
                if (!result.Contains(childId))
                {
                    result.Add(childId);
                    CollectChildDeptIds(childId, result);
                }
            }
        }

        public void Clear()
        {
            _currentUser = null;
            _menuTree = new List<SysMenu>();
            _permissions = new HashSet<string>();
            _roleKeys = new List<string>();
            DataPermissionManager.Clear();
        }
    }
}