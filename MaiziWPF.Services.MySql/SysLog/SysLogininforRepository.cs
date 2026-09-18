using FreeSql;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using System;

namespace MaiziWPF.Services.MySql
{
    public class SysLogininforRepository : BaseRepository<SysLogininfo, int>, ISysLogininforRepository
    {
        private readonly IFreeSql _fsql;

        public SysLogininforRepository(IFreeSql fsql) : base(fsql)
        {
            _fsql = fsql;
        }

        public List<SysLogininfo> SelectLogininforList(QueryLoginInfoInput input)
        {
            System.Linq.Expressions.Expression<Func<SysLogininfo, bool>> where = d => d.DelFlag == "0";
            if (!string.IsNullOrEmpty(input.UserName))
                where = where.And(d => d.UserName.Contains(input.UserName));
            if (!string.IsNullOrEmpty(input.Ipaddr))
                where = where.And(d => d.Ipaddr.Contains(input.Ipaddr));
            if (!string.IsNullOrEmpty(input.Status))
                where = where.And(d => d.Status == input.Status);
            if (input.StartDate.HasValue && input.EndDate.HasValue)
                where = where.And(d => d.LoginTime.Value.Between(input.StartDate.Value, input.EndDate.Value));

            return _fsql.Select<SysLogininfo>()
                .Where(where)
                .OrderByDescending(d => d.LoginTime)
                .Page(input)
                .ToList();
        }

        public int InsertLogininfor(SysLogininfo logininfor)
        {
            logininfor.CreateTime = DateTime.Now;
            return (int)_fsql.Insert(logininfor).ExecuteAffrows();
        }

        public int DeleteLogininforById(long infoId)
        {
            return _fsql.Delete<SysLogininfo>()
                .Where(d => d.InfoId == infoId)
                .ExecuteAffrows();
        }

        public int CleanLogininfor()
        {
            return _fsql.Delete<SysLogininfo>()
                .Where(d => 1 == 1)
                .ExecuteAffrows();
        }
    }
}