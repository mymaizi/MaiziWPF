using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaiziWPF.Services.Domain
{
    /// <summary>
    /// 字典数据表 sys_dict_data
    /// </summary>
    [Table(Name = "sys_dict_data")]
    public class SysDictData: BaseEntity
    {
        /// <summary>
        /// 字典编码
        /// </summary>
        [Column(Name = "dict_code", IsIdentity = true, IsPrimary = true)]
        public Int64 DictCode { get; set; }

        /// <summary>
        /// 字典排序
        /// </summary>
        [Column(Name = "dict_sort")]
        public Int32 DictSort { get; set; }

        /// <summary>
        /// 字典标签
        /// </summary>
        [Column(Name = "dict_label", DbType = "varchar(100)")]
        public String DictLabel { get; set; }

        /// <summary>
        /// 字典键值
        /// </summary>
        [Column(Name = "dict_value", DbType = "varchar(100)")]
        public String DictValue { get; set; }

        /// <summary>
        /// 字典类型
        /// </summary>
        [Column(Name = "dict_type", DbType = "varchar(100)")]
        public String DictType { get; set; }

        /// <summary>
        /// 是否默认（Y是 N否）
        /// </summary>
        [Column(Name = "is_default", DbType = "char(1)")]
        public String IsDefault { get; set; }
    }
}