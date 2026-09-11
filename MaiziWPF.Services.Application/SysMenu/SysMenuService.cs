using MaiziWPF.Common;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using System.Collections.Generic;

namespace MaiziWPF.Services.Application
{
    public class SysMenuService : ISysMenuService
    {
        private readonly ISysMenuRepository _repository;

        public SysMenuService(ISysMenuRepository repository)
        {
            _repository = repository;
        }

        public List<SysMenu> SelectMenuList(SysMenu menu, long userId)
        {
            List<SysMenu> menuList;
            if (SecurityUtils.IsAdmin(userId))
            {
                menuList = _repository.SelectMenuList(menu);
            }
            else
            {
                menuList = _repository.SelectMenuListByUserId(menu, userId);
            }
            SetMenuLevel(menuList, 1);
            return menuList;
        }

        private void SetMenuLevel(List<SysMenu> menus, int level)
        {
            if (menus == null) return;
            foreach (var menu in menus)
            {
                menu.Level = level;
                if (menu.Childs != null && menu.Childs.Count > 0)
                {
                    SetMenuLevel(menu.Childs, level + 1);
                }
            }
        }

        public SysMenu SelectMenuById(long menuId)
        {
            return _repository.SelectMenuById(menuId);
        }

        public int InsertMenu(SysMenu menu)
        {
            return _repository.InsertMenu(menu);
        }

        public int UpdateMenu(SysMenu menu)
        {
            return _repository.UpdateMenu(menu);
        }

        public int DeleteMenuById(long menuId)
        {
            return _repository.DeleteMenuById(menuId);
        }

        public bool HasChildByMenuId(long menuId)
        {
            return _repository.HasChildByMenuId(menuId);
        }

        public bool CheckMenuExistRole(long menuId)
        {
            return _repository.CheckMenuExistRole(menuId);
        }

        public bool CheckMenuNameUnique(SysMenu menu)
        {
            return _repository.CheckMenuNameUnique(menu);
        }
    }
}