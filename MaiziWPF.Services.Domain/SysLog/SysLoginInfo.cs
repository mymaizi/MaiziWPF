using FreeSql.DataAnnotations;
using System;

namespace MaiziWPF.Services.Domain
{
    [Table(Name = "sys_login_info")]
    public class SysLogininfo : BaseEntity
    {
        [Column(Name = "info_id", IsIdentity = true, IsPrimary = true)]
        public Int64 InfoId { get; set; }

        [Column(Name = "user_name", DbType = "varchar(50)")]
        public String UserName { get; set; }

        [Column(Name = "ipaddr", DbType = "varchar(128)")]
        public String Ipaddr { get; set; }

        [Column(Name = "login_location", DbType = "varchar(255)")]
        public String LoginLocation { get; set; }

        [Column(Name = "browser", DbType = "varchar(50)")]
        public String Browser { get; set; }

        [Column(Name = "os", DbType = "varchar(50)")]
        public String Os { get; set; }

        [Column(Name = "status", DbType = "char(1)")]
        public String Status { get; set; }

        [Column(Name = "msg", DbType = "varchar(255)")]
        public String Msg { get; set; }

        [Column(Name = "login_time")]
        public DateTime? LoginTime { get; set; }
    }
}