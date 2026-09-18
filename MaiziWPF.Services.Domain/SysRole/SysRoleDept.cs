using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaiziWPF.Services.Domain
{
    /// <summary>
    /// 角色和部门关联 sys_role_dept
    /// </summary>
    [Table(Name = "sys_role_dept")]
    public class SysRoleDept
    {
        /** 角色ID */
        [Column(Name = "role_id", IsPrimary = true)]
        public Int64 RoleId { get; set; }

        /** 部门ID */
        [Column(Name = "dept_id", IsPrimary = true)]
        public Int64 DeptId { get; set; }
    }
}