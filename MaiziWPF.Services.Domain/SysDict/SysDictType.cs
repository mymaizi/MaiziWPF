using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaiziWPF.Services.Domain
{
    /// <summary>
    /// 字典类型表 sys_dict_type
    /// </summary>
    [Table(Name = "sys_dict_type")]
    public class SysDictType: BaseEntity
    {
        /// <summary>
        /// 字典主键
        /// </summary>
        [Column(Name = "dict_id", IsIdentity = true, IsPrimary = true)]
        public Int64 DictId { get; set; }

        /// <summary>
        /// 字典名称
        /// </summary>
        [Column(Name = "dict_name", DbType = "varchar(100)")]
        public String DictName { get; set; }

        /// <summary>
        /// 字典类型
        /// </summary>
        [Column(Name = "dict_type", DbType = "varchar(100)")]
        public String DictType { get; set; }

        /// <summary>
        /// 是否停用（Y是 N否）
        /// </summary>
        [Column(Name = "status", DbType = "char(1)")]
        public String Status { get; set; }
    }
}