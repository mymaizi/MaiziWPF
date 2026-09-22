using FreeSql;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using System;

namespace MaiziWPF.Services.MySql
{
    public class SysNoticeRepository : BaseRepository<SysNotice, int>, ISysNoticeRepository
    {
        private readonly IFreeSql _fsql;

        public SysNoticeRepository(IFreeSql fsql) : base(fsql)
        {
            _fsql = fsql;
        }

        public List<SysNotice> SelectNoticeList(QueryNoticeInput input)
        {
            System.Linq.Expressions.Expression<Func<SysNotice, bool>> where = d => d.DelFlag == "0";
            if (!string.IsNullOrEmpty(input.NoticeTitle))
                where = where.And(d => d.NoticeTitle.Contains(input.NoticeTitle));
            if (!string.IsNullOrEmpty(input.NoticeType))
                where = where.And(d => d.NoticeType == input.NoticeType);
            if (!string.IsNullOrEmpty(input.Status))
                where = where.And(d => d.Status == input.Status);
            if (input.StartDate.HasValue && input.EndDate.HasValue)
                where = where.And(d => d.CreateTime.Between(input.StartDate.Value, input.EndDate.Value));

            return _fsql.Select<SysNotice>()
                .Include(n => n.CreateUser)
                .Where(where)
                .OrderByDescending(d => d.CreateTime)
                .Page(input)
                .ToList();
        }

        public SysNotice SelectNoticeById(long noticeId)
        {
            return _fsql.Select<SysNotice>()
                .Include(n => n.CreateUser)
                .Where(d => d.NoticeId == noticeId && d.DelFlag == "0")
                .First();
        }

        public int InsertNotice(SysNotice notice)
        {
            notice.CreateTime = DateTime.Now;
            return (int)_fsql.Insert(notice).ExecuteAffrows();
        }

        public int UpdateNotice(SysNotice notice)
        {
            notice.UpdateTime = DateTime.Now;
            return _fsql.Update<SysNotice>().SetSource(notice).ExecuteAffrows();
        }

        public int DeleteNoticeById(long noticeId)
        {
            return _fsql.Update<SysNotice>()
                .Set(d => d.DelFlag, "2")
                .Where(d => d.NoticeId == noticeId)
                .ExecuteAffrows();
        }
    }
}