using FreeSql.DataAnnotations;
using System;

namespace MaiziWPF.Services.Domain
{
    /// <summary>
    /// 岗位信息表 sys_post
    /// </summary>
    [Table(Name = "sys_post")]
    public class SysPost : BaseEntity
    {
        /// <summary>
        /// 岗位ID
        /// </summary>
        [Column(Name = "post_id", IsIdentity = true, IsPrimary = true)]
        public Int64 PostId { get; set; }

        /// <summary>
        /// 部门ID
        /// </summary>
        [Column(Name = "dept_id")]
        public Int64 DeptId { get; set; }

        /// <summary>
        /// 岗位编码
        /// </summary>
        [Column(Name = "post_code", DbType = "varchar(64)")]
        public String PostCode { get; set; }

        /// <summary>
        /// 岗位类别编码
        /// </summary>
        [Column(Name = "post_category", DbType = "varchar(100)")]
        public String PostCategory { get; set; }

        /// <summary>
        /// 岗位名称
        /// </summary>
        [Column(Name = "post_name", DbType = "varchar(50)")]
        public String PostName { get; set; }

        /// <summary>
        /// 显示顺序
        /// </summary>
        [Column(Name = "post_sort")]
        public Int32 PostSort { get; set; }

        /// <summary>
        /// 状态（0正常 1停用）
        /// </summary>
        [Column(Name = "status", DbType = "char(1)")]
        public String Status { get; set; }
    }
}