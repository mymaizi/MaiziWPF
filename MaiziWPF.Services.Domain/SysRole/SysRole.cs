using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaiziWPF.Services.Domain
{
    /// <summary>
    ///  角色表 sys_role
    /// </summary>
    [Table(Name = "sys_role")]
    public class SysRole : BaseEntity
    {
        /** 角色ID */
        [Column(Name = "role_id", IsIdentity = true, IsPrimary = true)]
        public Int64 RoleId { get; set; }
        /** 角色名称 */
        [Column(Name = "role_name", DbType = "varchar(30)")]
        public String RoleName { get; set; }
        /** 角色权限字符串 */
        [Column(Name = "role_key", DbType = "varchar(100)")]
        public String RoleKey { get; set; }
        /** 显示顺序 */
        [Column(Name = "role_sort")]
        public Int32 RoleSort { get; set; }
        /** 数据范围（1：全部数据权限 2：自定数据权限 3：本部门数据权限 4：本部门及以下数据权限 5：仅本人数据权限） */
        [Column(Name = "data_scope", DbType = "char(1)")]
        public String DataScope { get; set; }
        /** 菜单树选择项是否关联显示 */
        [Column(Name = "menu_check_strictly", DbType = "tinyint(1)")]
        public Boolean? MenuCheckStrictly { get; set; }
        /** 部门树选择项是否关联显示 */
        [Column(Name = "dept_check_strictly", DbType = "tinyint(1)")]
        public Boolean? DeptCheckStrictly { get; set; }
        /** 角色状态（0正常 1停用） */
        [Column(Name = "status", DbType = "char(1)")]
        public String Status { get; set; }

        [Navigate(nameof(SysRoleMenu.RoleId))]
        public List<SysRoleMenu> RoleMenus { get; set; }

        [Navigate(nameof(SysRoleDept.RoleId))]
        public List<SysRoleDept> RoleDepts { get; set; }
    }
}