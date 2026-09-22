using FreeSql.DataAnnotations;
using System;

namespace MaiziWPF.Services.Domain
{
    /// <summary>
    /// 文件存储表
    /// </summary>
    [Table(Name = "sys_oss")]
    public class SysOss : BaseEntity
    {
        /// <summary>
        /// 对象存储主键
        /// </summary>
        [Column(Name = "oss_id", IsIdentity = true, IsPrimary = true)]
        public Int64 OssId { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        [Column(Name = "file_name", DbType = "varchar(255)")]
        public String FileName { get; set; }

        /// <summary>
        /// 原名
        /// </summary>
        [Column(Name = "original_name", DbType = "varchar(255)")]
        public String OriginalName { get; set; }

        /// <summary>
        /// 文件后缀名
        /// </summary>
        [Column(Name = "file_suffix", DbType = "varchar(10)")]
        public String FileSuffix { get; set; }

        /// <summary>
        /// URL地址
        /// </summary>
        [Column(Name = "url", DbType = "varchar(500)")]
        public String Url { get; set; }

        /// <summary>
        /// 扩展字段
        /// </summary>
        [Column(Name = "ext1", DbType = "text")]
        public String Ext1 { get; set; }

        /// <summary>
        /// 服务商
        /// </summary>
        [Column(Name = "service", DbType = "varchar(20)")]
        public String Service { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Column(Name = "remark", DbType = "varchar(255)")]
        public String Remark { get; set; }

        /// <summary>
        /// 上传人导航属性
        /// </summary>
        [Navigate(nameof(CreateBy))]
        public SysUser CreateUser { get; set; }
    }
}