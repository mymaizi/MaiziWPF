using FreeSql.DataAnnotations;
using System;

namespace MaiziWPF.Services.Domain
{
    [Table(Name = "sys_login_info")]
    public class SysLogininfo
    {
        /// <summary>访问ID</summary>
        [Column(Name = "info_id", IsIdentity = true, IsPrimary = true)]
        public Int64 InfoId { get; set; }

        /// <summary>用户账号</summary>
        [Column(Name = "user_name", DbType = "varchar(50)")]
        public String UserName { get; set; }

        /// <summary>客户端版本</summary>
        [Column(Name = "client_version", DbType = "varchar(32)")]
        public String ClientVersion { get; set; }

        /// <summary>登录IP地址</summary>
        [Column(Name = "ipaddr", DbType = "varchar(128)")]
        public String Ipaddr { get; set; }

        /// <summary>登录地点</summary>
        [Column(Name = "login_location", DbType = "varchar(255)")]
        public String LoginLocation { get; set; }

        /// <summary>MAC地址</summary>
        [Column(Name = "mac_address", DbType = "varchar(50)")]
        public String MacAddress { get; set; }

        /// <summary>操作系统</summary>
        [Column(Name = "os", DbType = "varchar(50)")]
        public String Os { get; set; }

        /// <summary>登录状态（0正常 1异常）</summary>
        [Column(Name = "status", DbType = "char(1)")]
        public String Status { get; set; }

        /// <summary>提示消息</summary>
        [Column(Name = "msg", DbType = "varchar(255)")]
        public String Msg { get; set; }

        /// <summary>访问时间</summary>
        [Column(Name = "login_time")]
        public DateTime? LoginTime { get; set; }
    }
}