using FreeSql;
using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Domain
{
    public interface ISysMenuRepository : IBaseRepository<SysMenu, int>, ITransientDependency
    {
        List<SysMenu> SelectMenuList(SysMenu menu);

        List<SysMenu> SelectMenuListByUserId(SysMenu menu, long userId);

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