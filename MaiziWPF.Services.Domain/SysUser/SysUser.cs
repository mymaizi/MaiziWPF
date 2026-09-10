using FreeSql.DataAnnotations;
using System.Xml.Linq;

namespace MaiziWPF.Services.Domain
{
    /// <summary>
    /// 用户表 sys_user
    /// 注意：与原表有差异
    /// </summary>
    [Table(Name = "sys_user")]
    public class SysUser : BaseEntity
    {
        /** 用户ID */
        [Column(Name = "user_id", IsIdentity = true, IsPrimary = true)]
        public Int64 UserId { get; set; }
        /** 用户账号 */
        [Column(Name = "user_name",DbType ="varchar(30)")]
        public String UserName { get; set; }
        /** 用户昵称 */
        [Column(Name = "nick_name", DbType = "varchar(30)")]
        public String NickName { get; set; }
        /** 用户类型 */
        [Column(Name = "user_type", DbType = "varchar(10)")]
        public String UserType { get; set; }
        /** 用户邮箱 */
        [Column(Name = "email", DbType = "varchar(50)")]
        public String Email { get; set; }
        /** 手机号码 */
        [Column(Name = "phonenumber", DbType = "varchar(11)")]
        public String PhoneNumber { get; set; }
        /** 用户性别 */
        [Column(Name = "sex", DbType = "char(1)")]
        public String Sex { get; set; }
        /** 用户头像 */
        [Column(Name = "avatar", DbType = "varchar(100)")]
        public String Avatar { get; set; }
        /** 密码 */
        [Column(Name = "password", DbType = "varchar(100)")]
        public String Password { get; set; }
        /** 账号状态（0正常 1停用） */
        [Column(Name = "status", DbType = "char(1)")]
        public String Status { get; set; }
        /** 最后登录IP */
        [Column(Name = "login_ip", DbType = "varchar(128)")]
        public String LoginIp { get; set; }
        /** 最后登录时间 */
        [Column(Name = "login_date")]
        public DateTime LoginDate { get; set; }
        /** 密码最后更新时间 */
        [Column(Name = "pwd_update_date")]
        public DateTime PwdUpdateDate { get; set; }
        /** 备注 */
        [Column(Name = "remark", DbType = "varchar(500)")]
        public String Remark { get; set; }

        [Navigate(ManyToMany = typeof(SysUserRole))]
        public List<SysRole> Roles { get; set; }
        [Navigate(ManyToMany = typeof(SysUserDept))]
        public List<SysDept> Depts { get; set; }
        [Navigate(ManyToMany = typeof(SysUserPost))]
        public List<SysPost> Posts { get; set; }
    }
}