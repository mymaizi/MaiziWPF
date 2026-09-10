using FreeSql;
using MaiziWPF.Services.Domain;
using System;
using System.Linq;

namespace MaiziWPF.Services.MySql
{
    public class SysMenuRepository : BaseRepository<SysMenu, int>, ISysMenuRepository
    {
        private readonly IFreeSql _fsql;

        public SysMenuRepository(IFreeSql fsql) : base(fsql)
        {
            _fsql = fsql;
        }

        public List<SysMenu> SelectMenuList(SysMenu menu)
        {
            System.Linq.Expressions.Expression<Func<SysMenu, bool>> where = w => w.DelFlag == "0";
            if (!string.IsNullOrEmpty(menu.MenuName))
                where = where.And(w => w.MenuName.Contains(menu.MenuName));
            if (!string.IsNullOrEmpty(menu.Status))
                where = where.And(w => w.Status == menu.Status);
            if (!string.IsNullOrEmpty(menu.MenuType))
                where = where.And(w => menu.MenuType.Split(',').Contains(w.MenuType));

            return _fsql.Select<SysMenu>().Where(where).OrderBy(o => o.OrderNum).ToTreeList();
        }

        public List<SysMenu> SelectMenuListByUserId(SysMenu menu, long userId)
        {
            System.Linq.Expressions.Expression<Func<SysMenu, bool>> where = w => w.DelFlag == "0";
            if (!string.IsNullOrEmpty(menu.MenuName))
                where = where.And(w => w.MenuName.Contains(menu.MenuName));
            if (!string.IsNullOrEmpty(menu.Status))
                where = where.And(w => w.Status == menu.Status);

            return _fsql.Select<SysMenu, SysRoleMenu, SysUserRole, SysUserMenu>()
                     .LeftJoin((m, rm, ur, um) => m.Id == rm.MenuId)
                     .LeftJoin((m, rm, ur, um) => rm.RoleId == ur.RoleId)
                     .LeftJoin((m, rm, ur, um) => m.Id == um.MenuId)
                     .Where((m, rm, ur, um) => ur.UserId == userId || um.UserId == userId)
                     .WithTempQuery((m, rm, ur, um) => m)
                     .Where(where)
                     .OrderBy(o => o.OrderNum)
                     .ToTreeList();
        }

        public SysMenu SelectMenuById(long menuId)
        {
            return _fsql.Select<SysMenu>().Where(m => m.Id == menuId && m.DelFlag == "0").First();
        }

        public int InsertMenu(SysMenu menu)
        {
            menu.CreateTime = DateTime.Now;
            return (int)_fsql.Insert(menu).ExecuteAffrows();
        }

        public int UpdateMenu(SysMenu menu)
        {
            menu.UpdateTime = DateTime.Now;
            return _fsql.Update<SysMenu>().SetSource(menu).ExecuteAffrows();
        }

        public int DeleteMenuById(long menuId)
        {
            return _fsql.Update<SysMenu>()
                .Set(m => m.DelFlag, "2")
                .Where(m => m.Id == menuId)
                .ExecuteAffrows();
        }

        public bool HasChildByMenuId(long menuId)
        {
            return _fsql.Select<SysMenu>()
                .Where(m => m.ParentId == menuId && m.DelFlag == "0")
                .Any();
        }

        public bool CheckMenuExistRole(long menuId)
        {
            return _fsql.Select<SysRoleMenu>()
                .Where(r => r.MenuId == menuId)
                .Any();
        }

        public bool CheckMenuNameUnique(SysMenu menu)
        {
            var query = _fsql.Select<SysMenu>()
                .Where(m => m.MenuName == menu.MenuName && m.ParentId == menu.ParentId && m.DelFlag == "0");
            if (menu.Id != 0)
                query = query.Where(m => m.Id != menu.Id);
            return !query.Any();
        }
    }
}