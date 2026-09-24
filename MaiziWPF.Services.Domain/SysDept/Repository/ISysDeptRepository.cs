using FreeSql;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Domain
{
    public interface ISysDeptRepository : IBaseRepository<SysDept, int>, ITransientDependency
    {
        List<SysDept> SelectDeptList(SysDept dept, bool isTreeQuery, bool disableDataPermissionFilter = false);

        SysDept SelectDeptById(long deptId);

        int InsertDept(SysDept dept);

        int UpdateDept(SysDept dept);

        int DeleteDeptById(long deptId);

        bool HasChildByDeptId(long deptId);

        bool CheckDeptNameUnique(SysDept dept);
    }
}