using MaiziWPF.Services.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Application.Contracts
{
    public interface ISysDeptService : ITransientDependency
    {
        List<SysDept> SelectDeptList(SysDept dept, bool isTreeQuery = true, bool disableDataPermissionFilter = false);

        SysDept SelectDeptById(long deptId);

        int InsertDept(SysDept dept);

        int UpdateDept(SysDept dept);

        int DeleteDeptById(long deptId);

        bool HasChildByDeptId(long deptId);

        bool CheckDeptNameUnique(SysDept dept);
    }
}