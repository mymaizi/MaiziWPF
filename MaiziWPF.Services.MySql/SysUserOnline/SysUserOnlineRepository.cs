using FreeSql;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using System;

namespace MaiziWPF.Services.MySql
{
    public class SysUserOnlineRepository : BaseRepository<SysUserOnline, int>, ISysUserOnlineRepository
    {
        private readonly IFreeSql _fsql;

        public SysUserOnlineRepository(IFreeSql fsql) : base(fsql)
        {
            _fsql = fsql;
        }

        public List<SysUserOnline> SelectUserOnlineList(QueryUserOnlineInput input)
        {
            System.Linq.Expressions.Expression<Func<SysUserOnline, bool>> where = d => d.DelFlag == "0";
            if (!string.IsNullOrEmpty(input.LoginName))
                where = where.And(d => d.LoginName.Contains(input.LoginName));
            if (!string.IsNullOrEmpty(input.Ipaddr))
                where = where.And(d => d.Ipaddr.Contains(input.Ipaddr));

            return _fsql.Select<SysUserOnline>()
                .Where(where)
                .OrderByDescending(d => d.StartTimestamp)
                .Page(input)
                .ToList();
        }

        public SysUserOnline SelectOnlineById(string sessionId)
        {
            return _fsql.Select<SysUserOnline>()
                .Where(d => d.SessionId == sessionId)
                .First();
        }

        public int DeleteOnlineById(string sessionId)
        {
            return _fsql.Delete<SysUserOnline>()
                .Where(d => d.SessionId == sessionId)
                .ExecuteAffrows();
        }

        public int DeleteOnlineBySessionIds(List<string> sessionIds)
        {
            return _fsql.Delete<SysUserOnline>()
                .Where(d => sessionIds.Contains(d.SessionId))
                .ExecuteAffrows();
        }
    }
}