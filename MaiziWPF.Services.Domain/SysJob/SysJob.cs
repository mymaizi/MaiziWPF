using FreeSql.DataAnnotations;
using System;

namespace MaiziWPF.Services.Domain
{
    [Table(Name = "sys_job")]
    public class SysJob : BaseEntity
    {
        [Column(Name = "job_id", IsIdentity = true, IsPrimary = true)]
        public Int64 JobId { get; set; }

        [Column(Name = "job_name", DbType = "varchar(64)")]
        public String JobName { get; set; }

        [Column(Name = "job_group", DbType = "varchar(64)")]
        public String JobGroup { get; set; }

        [Column(Name = "invoke_target", DbType = "varchar(500)")]
        public String InvokeTarget { get; set; }

        [Column(Name = "cron_expression", DbType = "varchar(255)")]
        public String CronExpression { get; set; }

        [Column(Name = "misfire_policy", DbType = "varchar(20)")]
        public String MisfirePolicy { get; set; }

        [Column(Name = "concurrent", DbType = "char(1)")]
        public String Concurrent { get; set; }

        [Column(Name = "status", DbType = "char(1)")]
        public String Status { get; set; }

        [Column(Name = "remark", DbType = "varchar(500)")]
        public String Remark { get; set; }
    }
}