using FreeSql.DataAnnotations;
using System;

namespace MaiziWPF.Services.Domain
{
    [Table(Name = "sys_user_online")]
    public class SysUserOnline : BaseEntity
    {
        [Column(Name = "session_id", IsPrimary = true, DbType = "varchar(50)")]
        public String SessionId { get; set; }

        [Column(Name = "login_name", DbType = "varchar(50)")]
        public String LoginName { get; set; }

        [Column(Name = "dept_name", DbType = "varchar(50)")]
        public String DeptName { get; set; }

        [Column(Name = "ipaddr", DbType = "varchar(128)")]
        public String Ipaddr { get; set; }

        [Column(Name = "login_location", DbType = "varchar(255)")]
        public String LoginLocation { get; set; }

        [Column(Name = "browser", DbType = "varchar(50)")]
        public String Browser { get; set; }

        [Column(Name = "os", DbType = "varchar(50)")]
        public String Os { get; set; }

        [Column(Name = "status", DbType = "varchar(10)")]
        public String Status { get; set; }

        [Column(Name = "start_timestamp")]
        public DateTime? StartTimestamp { get; set; }

        [Column(Name = "last_access_time")]
        public DateTime? LastAccessTime { get; set; }

        [Column(Name = "expire_time")]
        public DateTime? ExpireTime { get; set; }
    }
}