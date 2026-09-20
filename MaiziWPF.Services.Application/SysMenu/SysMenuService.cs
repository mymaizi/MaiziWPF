using MaiziWPF.Common;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using System.Collections.Generic;

namespace MaiziWPF.Services.Application
{
    public class SysMenuService : ISysMenuService
    {
        private readonly ISysMenuRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public SysMenuService(ISysMenuRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public List<SysMenu> SelectMenuList(SysMenu menu)
        {
            var flatList = _repository.SelectMenuList(menu);
            return BuildMenuTree(flatList);
        }

        public List<SysMenu> SelectMenuTreeByUserId(long userId)
        {
            List<SysMenu> flatList;
            if (_currentUserService.IsSuperAdmin)
            {
                flatList = _repository.SelectMenuList(new SysMenu());
            }
            else
            {
                flatList = _repository.SelectMenuListByUserId(new SysMenu(), userId);
            }
            return BuildMenuTree(flatList);
        }

        private static List<SysMenu> BuildMenuTree(List<SysMenu> flatList)
        {
            return flatList.BuildTreeList(
                m => m.Id,
                m => m.ParentId,
                (p, c) =>
                {
                    p.Childs ??= new List<SysMenu>();
                    p.Childs.Add(c);
                },
                m => m.Childs);
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

        public int DeleteMenuCascade(long menuId)
        {
            return _repository.DeleteMenuCascade(menuId);
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