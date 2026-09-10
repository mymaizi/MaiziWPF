using FreeSql.DataAnnotations;
using System;

namespace MaiziWPF.Services.Domain
{
    [Table(Name = "sys_oper_log")]
    public class SysOperLog : BaseEntity
    {
        [Column(Name = "oper_id", IsIdentity = true, IsPrimary = true)]
        public Int64 OperId { get; set; }

        [Column(Name = "title", DbType = "varchar(50)")]
        public String Title { get; set; }

        [Column(Name = "business_type", DbType = "int")]
        public Int32 BusinessType { get; set; }

        [Column(Name = "method", DbType = "varchar(100)")]
        public String Method { get; set; }

        [Column(Name = "request_method", DbType = "varchar(10)")]
        public String RequestMethod { get; set; }

        [Column(Name = "operator_type", DbType = "int")]
        public Int32 OperatorType { get; set; }

        [Column(Name = "oper_name", DbType = "varchar(50)")]
        public String OperName { get; set; }

        [Column(Name = "dept_name", DbType = "varchar(50)")]
        public String DeptName { get; set; }

        [Column(Name = "oper_url", DbType = "varchar(255)")]
        public String OperUrl { get; set; }

        [Column(Name = "oper_ip", DbType = "varchar(128)")]
        public String OperIp { get; set; }

        [Column(Name = "oper_location", DbType = "varchar(255)")]
        public String OperLocation { get; set; }

        [Column(Name = "oper_param", DbType = "varchar(2000)")]
        public String OperParam { get; set; }

        [Column(Name = "json_result", DbType = "varchar(2000)")]
        public String JsonResult { get; set; }

        [Column(Name = "status", DbType = "int")]
        public Int32 Status { get; set; }

        [Column(Name = "error_msg", DbType = "varchar(2000)")]
        public String ErrorMsg { get; set; }

        [Column(Name = "oper_time")]
        public DateTime? OperTime { get; set; }

        [Column(Name = "cost_time")]
        public Int64 CostTime { get; set; }
    }
}