using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaiziWPF.Services.Application
{
    public class SysDeptService : ISysDeptService
    {
        private readonly ISysDeptRepository _repository;

        public SysDeptService(ISysDeptRepository repository)
        {
            _repository = repository;
        }

        public List<SysDept> SelectDeptList(SysDept dept, bool isTreeQuery = true, bool disableDataPermissionFilter = false)
        {
            return _repository.SelectDeptList(dept, isTreeQuery, disableDataPermissionFilter);
        }

        public SysDept SelectDeptById(long deptId)
        {
            return _repository.SelectDeptById(deptId);
        }

        public int InsertDept(SysDept dept)
        {
            var parent = _repository.SelectDeptById(dept.ParentId);
            if (parent != null && parent.Status == "1")
                throw new Exception("部门停用，不允许新增");
            dept.Ancestors = parent != null ? parent.Ancestors + "," + dept.ParentId : "0";
            return _repository.InsertDept(dept);
        }

        public int UpdateDept(SysDept dept)
        {
            var oldDept = _repository.SelectDeptById(dept.Id);
            if (oldDept == null)
                throw new Exception("部门不存在，无法修改");
            if (dept.ParentId != oldDept.ParentId)
            {
                var newParent = _repository.SelectDeptById(dept.ParentId);
                dept.Ancestors = newParent != null ? newParent.Ancestors + "," + dept.ParentId : "0";
            }
            else
            {
                dept.Ancestors = oldDept.Ancestors;
            }
            return _repository.UpdateDept(dept);
        }

        public int DeleteDeptById(long deptId)
        {
            return _repository.DeleteDeptById(deptId);
        }

        public bool HasChildByDeptId(long deptId)
        {
            return _repository.HasChildByDeptId(deptId);
        }


        public bool CheckDeptNameUnique(SysDept dept)
        {
            return _repository.CheckDeptNameUnique(dept);
        }
    }
}