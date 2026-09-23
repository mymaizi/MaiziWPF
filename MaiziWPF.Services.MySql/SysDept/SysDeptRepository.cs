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
            var list = isTreeQuery ? query.ToTreeList() : query.ToList();
            PopulateLeaderNames(list);
            return list;
        }

        public SysDept SelectDeptById(long deptId)
        {
            return _fsql.Select<SysDept>().Where(d => d.Id == deptId && d.DelFlag == "0").First();
        }

        public int InsertDept(SysDept dept)
        {
            return (int)_fsql.Insert(dept).ExecuteAffrows();
        }

        public int UpdateDept(SysDept dept)
        {
            return _fsql.Update<SysDept>()
                .SetSource(dept)
                .ExecuteAffrows();
        }

        public int DeleteDeptById(long deptId)
        {
            var allIds = GetAllDescendantIds(deptId);
            return _fsql.Update<SysDept>()
                .Set(d => d.DelFlag, "1")
                .Set(d => d.UpdateTime, DateTime.Now)
                .Where(d => allIds.Contains(d.Id))
                .ExecuteAffrows();
        }

        private List<long> GetAllDescendantIds(long deptId)
        {
            var ids = new List<long> { deptId };
            var childIds = _fsql.Select<SysDept>()
                .Where(d => d.ParentId == deptId && d.DelFlag == "0")
                .ToList(d => d.Id);
            foreach (var childId in childIds)
            {
                ids.AddRange(GetAllDescendantIds(childId));
            }
            return ids;
        }

        public bool HasChildByDeptId(long deptId)
        {
            return _fsql.Select<SysDept>()
                .Where(d => d.ParentId == deptId && d.DelFlag == "0")
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

        private void PopulateLeaderNames(List<SysDept> depts)
        {
            if (depts == null || depts.Count == 0) return;

            var allDepts = FlattenDepts(depts);
            var leaderIds = allDepts.Where(d => d.Leader > 0).Select(d => d.Leader).Distinct().ToList();
            if (leaderIds.Count > 0)
            {
                var users = _fsql.Select<SysUser>().Where(u => leaderIds.Contains(u.UserId)).ToList();
                var userDict = users.ToDictionary(u => u.UserId, u => u.NickName ?? u.UserName);
                foreach (var d in allDepts.Where(d => d.Leader > 0))
                {
                    d.LeaderName = userDict.TryGetValue(d.Leader, out var name) ? name : d.Leader.ToString();
                }
            }
        }

        private List<SysDept> FlattenDepts(List<SysDept> depts)
        {
            var result = new List<SysDept>();
            foreach (var d in depts)
            {
                result.Add(d);
                if (d.Childs?.Count > 0)
                    result.AddRange(FlattenDepts(d.Childs));
            }
            return result;
        }
    }
}