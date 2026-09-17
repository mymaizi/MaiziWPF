using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaiziWPF.Services.Domain
{
    /// <summary>
    /// 部门表 sys_dept
    /// </summary>
    [Table(Name = "sys_dept")]
    public class SysDept: BaseEntity
    {
        /** 部门ID */
        [Column(Name = "dept_id", IsIdentity = true, IsPrimary = true)]
        public Int64 Id { get; set; }
        /** 父部门ID */
        [Column(Name = "parent_id")]
        public Int64 ParentId { get; set; }
        /** 祖级列表 */
        [Column(Name = "ancestors", DbType = "varchar(500)")]
        public String Ancestors { get; set; }
        /** 部门名称 */
        [Column(Name = "dept_name", DbType = "varchar(30)")]
        public String DeptName { get; set; }
        /** 部门类别编码 */
        [Column(Name = "dept_category", DbType = "varchar(100)")]
        public String DeptCategory { get; set; }
        /** 显示顺序 */
        [Column(Name = "order_num")]
        public Int32 OrderNum { get; set; }
        /** 负责人 */
        [Column(Name = "leader")]
        public Int64 Leader { get; set; }
        /** 负责人名称（非数据库字段） */
        [Column(IsIgnore = true)]
        public String LeaderName { get; set; }
        /** 联系电话 */
        [Column(Name = "phone", DbType = "varchar(11)")]
        public String Phone { get; set; }
        /** 邮箱 */
        [Column(Name = "email", DbType = "varchar(50)")]
        public String Email { get; set; }
        /** 部门状态（0正常 1停用） */
        [Column(Name = "status", DbType = "char(1)")]
        public String Status { get; set; }
        /** 子部门 */
        [Navigate(nameof(ParentId))]
        public List<SysDept> Childs { get; set; }
    }
}