using FreeSql.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Application.Contracts
{
    public interface ILocalDbService : ISingletonDependency
    {
        void Init();
        long GetLastSyncId(long userId);
        void SetLastSyncId(long userId, long id);
        void SaveMessage(long userId, long msgId);
        void MarkRead(long userId, long msgId);
        void MarkAllRead(long userId);
        void DeleteMessage(long userId, long msgId);
        void ClearReadMessages(long userId);
        List<LocalMsgStatus> GetAll(long userId);
        int GetUnreadCount(long userId);
    }

    [Table(Name = "local_msg_status")]
    public class LocalMsgStatus
    {
        [Column(Name = "user_id", IsPrimary = true)]
        public long UserId { get; set; }

        [Column(Name = "msg_id", IsPrimary = true)]
        public long MsgId { get; set; }

        [Column(Name = "is_read")]
        public bool IsRead { get; set; }
    }

    [Table(Name = "local_sync_state")]
    public class LocalSyncState
    {
        [Column(Name = "user_id", IsPrimary = true)]
        public long UserId { get; set; }

        [Column(Name = "key", IsPrimary = true)]
        public string Key { get; set; }

        [Column(Name = "value", DbType = "varchar(50)")]
        public string Value { get; set; }
    }
}