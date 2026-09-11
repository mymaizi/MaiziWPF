using FreeSql.DataAnnotations;
using System;

namespace MaiziWPF.Services.Domain
{
    public class BaseEntity
    {
        /// <summary>
        /// 创建部门
        /// </summary>
        [Column(Name = "create_dept")]
        public Int64 CreateDept { get; set; }
        
        /// <summary>
        /// 创建者
        /// </summary>
        [Column(Name = "create_by")]
        public Int64 CreateBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column(Name = "create_time")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新者
        /// </summary>
        [Column(Name = "update_by")]
        public Int64 UpdateBy { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Column(Name = "update_time")]
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Column(Name = "remark", DbType = "varchar(500)")]
        public String Remark { get; set; }

        /// <summary>
        /// 删除标志（0代表存在 1代表删除）
        /// </summary>
        [Column(Name = "del_flag", DbType = "char(1) default '0'")]
        public String DelFlag { get; set; } = "0";
    }
}