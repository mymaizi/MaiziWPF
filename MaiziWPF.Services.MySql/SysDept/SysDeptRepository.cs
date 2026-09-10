using FreeSql;
using MaiziWPF.Services.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MaiziWPF.Services.MySql
{
    public class SysDeptRepository : BaseRepository<SysDept, int>, ISysDeptRepository
    {
        private readonly IFreeSql _fsql;

        public SysDeptRepository(IFreeSql fsql) : base(fsql)
        {
            _fsql = fsql;
        }

        public List<SysDept> SelectDeptList(SysDept dept, bool isTreeQuery = true)
        {
            System.Linq.Expressions.Expression<Func<SysDept, bool>> where = d => d.DelFlag == "0";
            if (dept.Id != 0)
                where = where.And(d => d.Id == dept.Id);
            if (dept.ParentId != 0)
                where = where.And(d => d.ParentId == dept.ParentId);
            if (!string.IsNullOrEmpty(dept.DeptName))
                where = where.And(d => d.DeptName.Contains(dept.DeptName));
            if (!string.IsNullOrEmpty(dept.Status))
                where = where.And(d => d.Status == dept.Status);
            var query = _fsql.Select<SysDept>().Where(where).OrderBy(a => new { a.ParentId, a.OrderNum });
            return isTreeQuery ? query.ToTreeList() : query.ToList();
        }

        public SysDept SelectDeptById(long deptId)
        {
            return _fsql.Select<SysDept>().Where(d => d.Id == deptId && d.DelFlag == "0").First();
        }

        public int InsertDept(SysDept dept)
        {
            dept.CreateTime = DateTime.Now;
            return (int)_fsql.Insert(dept).ExecuteAffrows();
        }

        public int UpdateDept(SysDept dept)
        {
            dept.UpdateTime = DateTime.Now;
            return _fsql.Update<SysDept>()
                .SetSource(dept)
                .ExecuteAffrows();
        }

        public int DeleteDeptById(long deptId)
        {
            return _fsql.Update<SysDept>()
                .Set(d => d.DelFlag, "2")
                .Where(d => d.Id == deptId)
                .ExecuteAffrows();
        }

        public bool HasChildByDeptId(long deptId)
        {
            return _fsql.Select<SysDept>()
                .Where(d => d.ParentId == deptId && d.DelFlag == "0")
                .Any();
        }

        public bool CheckDeptExistUser(long deptId)
        {
            return _fsql.Select<SysUserDept>()
                .Where(d => d.DeptId == deptId)
                .Any();
        }

        public bool CheckDeptNameUnique(SysDept dept)
        {
            var query = _fsql.Select<SysDept>()
                .Where(d => d.DeptName == dept.DeptName && d.ParentId == dept.ParentId && d.DelFlag == "0");
            if (dept.Id != 0)
                query = query.Where(d => d.Id != dept.Id);
            return !query.Any();
        }
    }
}