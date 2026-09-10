using FreeSql.DataAnnotations;
using System;

namespace MaiziWPF.Services.Domain
{
    [Table(Name = "sys_oss")]
    public class SysOss : BaseEntity
    {
        [Column(Name = "oss_id", IsIdentity = true, IsPrimary = true)]
        public Int64 OssId { get; set; }

        [Column(Name = "file_name", DbType = "varchar(255)")]
        public String FileName { get; set; }

        [Column(Name = "original_name", DbType = "varchar(255)")]
        public String OriginalName { get; set; }

        [Column(Name = "file_suffix", DbType = "varchar(10)")]
        public String FileSuffix { get; set; }

        [Column(Name = "url", DbType = "varchar(500)")]
        public String Url { get; set; }

        [Column(Name = "service", DbType = "varchar(20)")]
        public String Service { get; set; }

        [Column(Name = "create_dept", DbType = "bigint")]
        public Int64? CreateDept { get; set; }
    }
}