using FreeSql.DataAnnotations;
using System;

namespace MaiziWPF.Services.Domain
{
    [Table(Name = "sys_notice")]
    public class SysNotice : BaseEntity
    {
        [Column(Name = "notice_id", IsIdentity = true, IsPrimary = true)]
        public Int64 NoticeId { get; set; }

        [Column(Name = "notice_title", DbType = "varchar(50)")]
        public String NoticeTitle { get; set; }

        [Column(Name = "notice_type", DbType = "char(1)")]
        public String NoticeType { get; set; }

        [Column(Name = "notice_content", DbType = "longtext")]
        public String NoticeContent { get; set; }

        [Column(Name = "status", DbType = "char(1)")]
        public String Status { get; set; }
    }
}