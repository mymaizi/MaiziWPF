using FreeSql.DataAnnotations;
using System;

namespace MaiziWPF.Services.Domain
{
    /// <summary>
    /// 参数配置表
    /// </summary>
    [Table(Name = "sys_config")]
    public class SysConfig : BaseEntity
    {
        /// <summary>
        /// 参数主键
        /// </summary>
        [Column(Name = "config_id", IsIdentity = true, IsPrimary = true)]
        public Int64 ConfigId { get; set; }

        /// <summary>
        /// 参数名称
        /// </summary>
        [Column(Name = "config_name", DbType = "varchar(100)")]
        public String ConfigName { get; set; }

        /// <summary>
        /// 参数键名
        /// </summary>
        [Column(Name = "config_key", DbType = "varchar(100)")]
        public String ConfigKey { get; set; }

        /// <summary>
        /// 参数键值
        /// </summary>
        [Column(Name = "config_value", DbType = "varchar(500)")]
        public String ConfigValue { get; set; }

        /// <summary>
        /// 系统内置（Y是 N否）
        /// </summary>
        [Column(Name = "config_type", DbType = "char(1)")]
        public String ConfigType { get; set; }
    }
}