using FreeSql;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using System;
using System.Linq;

namespace MaiziWPF.Services.MySql
{
    public class SysMessageRepository : BaseRepository<SysMessage, int>, ISysMessageRepository
    {
        private readonly IFreeSql _fsql;

        public SysMessageRepository(IFreeSql fsql) : base(fsql)
        {
            _fsql = fsql;
        }

        public List<SysMessage> SelectMessageList(QueryMessageInput input)
        {
            var query = _fsql.Select<SysMessage>()
                .Include(m => m.CreateUser)
                .Where(d => d.DelFlag == "0");

            if (!string.IsNullOrEmpty(input.Category))
                query = query.Where(d => d.Category == input.Category);
            if (!string.IsNullOrEmpty(input.Type))
                query = query.Where(d => d.Type == input.Type);
            if (input.StartDate.HasValue && input.EndDate.HasValue)
                query = query.Where(d => d.CreateTime.Between(input.StartDate.Value, input.EndDate.Value));

            return query.OrderByDescending(d => d.CreateTime)
                .Page(input)
                .ToList();
        }

        public SysMessage SelectMessageById(long messageId)
        {
            return _fsql.Select<SysMessage>()
                .Include(m => m.CreateUser)
                .Where(d => d.MessageId == messageId && d.DelFlag == "0")
                .First();
        }

        public long InsertMessage(SysMessage msg)
        {
            return _fsql.Insert(msg).ExecuteIdentity();
        }

        public List<SysMessage> SelectNewMessages(long lastId, long userId)
        {
            var userIdStr = userId.ToString();
            return _fsql.Select<SysMessage>()
                .Include(m => m.CreateUser)
                .Where(d => d.MessageId > lastId && d.DelFlag == "0")
                .Where(d => d.SendUserIds == "0" || d.SendUserIds.Contains(userIdStr))
                .OrderBy(d => d.MessageId)
                .ToList();
        }

        public List<SysMessage> SelectMessagesByIds(List<long> msgIds)
        {
            if (msgIds == null || msgIds.Count == 0) return new List<SysMessage>();
            return _fsql.Select<SysMessage>()
                .Include(m => m.CreateUser)
                .Where(d => msgIds.Contains(d.MessageId) && d.DelFlag == "0")
                .OrderByDescending(d => d.CreateTime)
                .ToList();
        }
    }
}