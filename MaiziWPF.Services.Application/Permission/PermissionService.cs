using MaiziWPF.Common;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using System.Collections.Generic;
using System.Linq;

namespace MaiziWPF.Services.Application
{
    public class PermissionService : IPermissionService
    {
        private readonly ISysMenuRepository _menuRepository;
        private readonly ISysUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;

        public PermissionService(
            ISysMenuRepository menuRepository,
            ISysUserRepository userRepository,
            ICurrentUserService currentUserService)
        {
            _menuRepository = menuRepository;
            _userRepository = userRepository;
            _currentUserService = currentUserService;
        }

        public void LoadUserPermissions(long userId)
        {
            if (_currentUserService.IsSuperAdmin)
            {
                LoadAdminPermissions();
                return;
            }

            var roleIds = _userRepository.SelectUserRoleIds(userId);
            if (roleIds == null || roleIds.Count == 0)
            {
                _currentUserService.SetPermissions(new HashSet<string>());
                _currentUserService.SetMenuTree(new List<SysMenu>());
                return;
            }

            var allMenus = _menuRepository.SelectMenuListByRoleIds(roleIds);

            var permissions = allMenus
                .Where(m => m.MenuType == "F" && !string.IsNullOrEmpty(m.Perms))
                .Select(m => m.Perms)
                .ToHashSet();

            var menuItems = allMenus
                .Where(m => m.MenuType == "M" || m.MenuType == "C")
                .OrderBy(m => m.OrderNum)
                .ToList();

            _currentUserService.SetPermissions(permissions);
            _currentUserService.SetMenuTree(BuildTree(menuItems));
        }

        public void Clear()
        {
            _currentUserService.SetMenuTree(new List<SysMenu>());
        }

        private void LoadAdminPermissions()
        {
            var allMenus = _menuRepository.SelectMenuList(new SysMenu());
            var permissions = new HashSet<string>();
            var menuItems = new List<SysMenu>();

            FlattenTree(allMenus, permissions, menuItems);

            foreach (var item in menuItems)
            {
                item.Childs = null;
            }

            _currentUserService.SetPermissions(permissions);
            _currentUserService.SetMenuTree(BuildTree(menuItems));
        }

        private static void FlattenTree(List<SysMenu> nodes, HashSet<string> permissions, List<SysMenu> menuItems)
        {
            if (nodes == null) return;
            foreach (var node in nodes)
            {
                if (node.MenuType == "F" && !string.IsNullOrEmpty(node.Perms))
                    permissions.Add(node.Perms);
                if (node.MenuType is "M" or "C")
                    menuItems.Add(node);
                if (node.Childs != null && node.Childs.Count > 0)
                    FlattenTree(node.Childs, permissions, menuItems);
            }
        }

        private static void SetTreeLevel(List<SysMenu> nodes, int level)
        {
            if (nodes == null) return;
            foreach (var node in nodes)
            {
                node.Level = level;
                if (node.Childs != null && node.Childs.Count > 0)
                    SetTreeLevel(node.Childs, level + 1);
            }
        }

        private static List<SysMenu> BuildTree(List<SysMenu> flatList)
        {
            var tree = flatList.BuildTreeList(
                m => m.Id,
                m => m.ParentId,
                (p, c) =>
                {
                    p.Childs ??= new List<SysMenu>();
                    p.Childs.Add(c);
                });
            SetTreeLevel(tree, 1);
            return tree;
        }
    }
}