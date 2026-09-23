using FreeSql;
using MaiziWPF.Services.Application.Contracts;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.IO;

namespace MaiziWPF.Services.Application
{
    public class LocalDbService : ILocalDbService
    {
        private IFreeSql _fsql;
        private readonly string _dbDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "MaiziWPF");
        private readonly string _dbName = "local.db";
        private bool _initialized = false;

        public void Init()
        {
            if (_initialized) return;

            Batteries.Init();

            Directory.CreateDirectory(_dbDir);
            var dbPath = Path.Combine(_dbDir, _dbName);

            _fsql = new FreeSqlBuilder()
                .UseConnectionString(FreeSql.DataType.Sqlite,
                    $"Data Source={dbPath};Pooling=pool_size=5;")
                .UseAutoSyncStructure(true)
                .Build();

            _fsql.CodeFirst.SyncStructure<LocalMsgStatus>();
            _fsql.CodeFirst.SyncStructure<LocalSyncState>();
            _initialized = true;
        }

        public long GetLastSyncId(long userId)
        {
            EnsureInit();
            var val = _fsql.Select<LocalSyncState>()
                .Where(a => a.UserId == userId && a.Key == "last_sync_id")
                .First(a => a.Value);
            return long.TryParse(val, out var id) ? id : 0;
        }

        public void SetLastSyncId(long userId, long id)
        {
            EnsureInit();
            _fsql.InsertOrUpdate<LocalSyncState>()
                .SetSource(new LocalSyncState
                {
                    UserId = userId,
                    Key = "last_sync_id",
                    Value = id.ToString()
                })
                .ExecuteAffrows();
        }

        public void SaveMessage(long userId, long msgId)
        {
            EnsureInit();
            _fsql.InsertOrUpdate<LocalMsgStatus>()
                .SetSource(new LocalMsgStatus
                {
                    UserId = userId,
                    MsgId = msgId,
                    IsRead = false
                })
                .ExecuteAffrows();
        }

        public void MarkRead(long userId, long msgId)
        {
            EnsureInit();
            _fsql.Update<LocalMsgStatus>()
                .Set(a => a.IsRead, true)
                .Where(a => a.UserId == userId && a.MsgId == msgId)
                .ExecuteAffrows();
        }

        public void MarkAllRead(long userId)
        {
            EnsureInit();
            _fsql.Update<LocalMsgStatus>()
                .Set(a => a.IsRead, true)
                .Where(a => a.UserId == userId)
                .ExecuteAffrows();
        }

        public void DeleteMessage(long userId, long msgId)
        {
            EnsureInit();
            var isRead = _fsql.Select<LocalMsgStatus>()
                .Where(a => a.UserId == userId && a.MsgId == msgId)
                .First(a => a.IsRead);
            if (!isRead)
                throw new InvalidOperationException("请先阅读该消息后再删除");

            _fsql.Delete<LocalMsgStatus>()
                .Where(a => a.UserId == userId && a.MsgId == msgId)
                .ExecuteAffrows();
        }

        public void ClearReadMessages(long userId)
        {
            EnsureInit();
            _fsql.Delete<LocalMsgStatus>()
                .Where(a => a.UserId == userId && a.IsRead)
                .ExecuteAffrows();
        }

        public List<LocalMsgStatus> GetAll(long userId)
        {
            EnsureInit();
            return _fsql.Select<LocalMsgStatus>()
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.MsgId)
                .ToList();
        }

        public int GetUnreadCount(long userId)
        {
            EnsureInit();
            return (int)_fsql.Select<LocalMsgStatus>()
                .Where(a => a.UserId == userId && !a.IsRead)
                .Count();
        }

        private void EnsureInit()
        {
            if (!_initialized) Init();
        }
    }

}