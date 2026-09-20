using MaiziWPF.Common;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using System.Collections.Generic;

namespace MaiziWPF.Services.Application
{
    public class CurrentUserService : ICurrentUserService
    {
        private SysUser _currentUser;
        private List<SysMenu> _menuTree = new();
        private HashSet<string> _permissions = new();
        private List<string> _roleKeys = new();

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

            var requiredPerms = perms.Split(',', System.StringSplitOptions.RemoveEmptyEntries);
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
                if (!string.Equals(requiredParts[i], grantedParts[i], System.StringComparison.OrdinalIgnoreCase))
                    return false;
            }
            return true;
        }

        public void Clear()
        {
            _currentUser = null;
            _menuTree = new List<SysMenu>();
            _permissions = new HashSet<string>();
            _roleKeys = new List<string>();
        }
    }
}