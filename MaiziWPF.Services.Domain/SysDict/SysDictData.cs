using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaiziWPF.Services.Domain
{
    /// <summary>
    /// 字典数据表 sys_dict_data
    /// 注意：与原表有差异
    /// </summary>
    [Table(Name = "sys_dict_data")]
    public class SysDictData: BaseEntity
    {
        /** 字典编码 */
        [Column(Name = "dict_code", IsIdentity = true, IsPrimary = true)]
        public Int64 DictCode { get; set; }
        /** 字典排序 */
        [Column(Name = "dict_sort")]
        public Int32 DictSort { get; set; }
        /** 字典标签 */
        [Column(Name = "dict_label", DbType = "varchar(100)")]
        public String DictLabel { get; set; }
        /** 字典键值 */
        [Column(Name = "dict_value", DbType = "varchar(100)")]
        public String DictValue { get; set; }
        /** 字典类型 */
        [Column(Name = "dict_type", DbType = "varchar(100)")]
        public String DictType { get; set; }
        /** 是否默认（Y是 N否） */
        [Column(Name = "is_default", DbType = "char(1)")]
        public String IsDefault { get; set; }
        /** 样式属性（其他样式扩展） */
        [Column(Name = "css_class", DbType = "varchar(100)")]
        public String CssClass { get; set; }
        /** 表格回显样式 */
        [Column(Name = "list_class", DbType = "varchar(100)")]
        public String ListClass { get; set; }
        /** 状态（0正常 1停用） */
        [Column(Name = "status", DbType = "char(1)")]
        public String Status { get; set; }
        /** 备注 */
        [Column(Name = "remark", DbType = "varchar(500)")]
        public String Remark { get; set; }
    }
}