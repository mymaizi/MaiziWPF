using FreeSql.DataAnnotations;

namespace MaiziWPF.Services.Domain
{
    [Table(Name = "sys_message")]
    public class SysMessage : BaseEntity
    {
        [Column(Name = "message_id", IsIdentity = true, IsPrimary = true)]
        public Int64 MessageId { get; set; }

        [Column(Name = "category", DbType = "varchar(20)")]
        public String Category { get; set; }

        [Column(Name = "type", DbType = "varchar(20)")]
        public String Type { get; set; }

        [Column(Name = "source", DbType = "varchar(20)")]
        public String Source { get; set; }

        [Column(Name = "title", DbType = "varchar(100)")]
        public String Title { get; set; }

        [Column(Name = "message", DbType = "varchar(500)")]
        public String Message { get; set; }

        [Column(Name = "content", DbType = "longtext")]
        public String Content { get; set; }

        [Column(Name = "data_json", DbType = "longtext")]
        public String DataJson { get; set; }

        [Column(Name = "path", DbType = "varchar(500)")]
        public String Path { get; set; }

        [Column(Name = "send_user_ids", DbType = "varchar(2000)")]
        public String SendUserIds { get; set; }

        [Navigate(nameof(CreateBy))]
        public SysUser CreateUser { get; set; }
    }
}