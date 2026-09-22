using FreeSql.DataAnnotations;
using System;

namespace MaiziWPF.Services.Domain
{
    /// <summary>
    /// 通知公告表
    /// </summary>
    [Table(Name = "sys_notice")]
    public class SysNotice : BaseEntity
    {
        /// <summary>
        /// 公告ID
        /// </summary>
        [Column(Name = "notice_id", IsIdentity = true, IsPrimary = true)]
        public Int64 NoticeId { get; set; }

        /// <summary>
        /// 公告标题
        /// </summary>
        [Column(Name = "notice_title", DbType = "varchar(50)")]
        public String NoticeTitle { get; set; }

        /// <summary>
        /// 公告类型（1通知 2公告）
        /// </summary>
        [Column(Name = "notice_type", DbType = "char(1)")]
        public String NoticeType { get; set; }

        /// <summary>
        /// 公告内容
        /// </summary>
        [Column(Name = "notice_content", DbType = "longtext")]
        public String NoticeContent { get; set; }

        /// <summary>
        /// 公告状态（0正常 1关闭）
        /// </summary>
        [Column(Name = "status", DbType = "char(1)")]
        public String Status { get; set; }

        /// <summary>
        /// 创建者导航属性（关联用户表）
        /// </summary>
        [Navigate(nameof(CreateBy))]
        public SysUser CreateUser { get; set; }
    }
}