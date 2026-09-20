using MaiziWPF.Services.Domain;
using System.Collections.Generic;
using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Application.Contracts
{
    public interface ISysMenuService : ITransientDependency
    {
        List<SysMenu> SelectMenuList(SysMenu menu);

        List<SysMenu> SelectMenuTreeByUserId(long userId);

        SysMenu SelectMenuById(long menuId);

        int InsertMenu(SysMenu menu);

        int UpdateMenu(SysMenu menu);

        int DeleteMenuById(long menuId);

        int DeleteMenuCascade(long menuId);

        bool HasChildByMenuId(long menuId);

        bool CheckMenuExistRole(long menuId);

        bool CheckMenuNameUnique(SysMenu menu);
    }
}