using FreeSql;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using System;

namespace MaiziWPF.Services.MySql
{
    public class SysOperLogRepository : BaseRepository<SysOperLog, int>, ISysOperLogRepository
    {
        private readonly IFreeSql _fsql;

        public SysOperLogRepository(IFreeSql fsql) : base(fsql)
        {
            _fsql = fsql;
        }

        public List<SysOperLog> SelectOperLogList(QueryOperLogInput input)
        {
            System.Linq.Expressions.Expression<Func<SysOperLog, bool>> where = d => d.DelFlag == "0";
            if (!string.IsNullOrEmpty(input.Title))
                where = where.And(d => d.Title.Contains(input.Title));
            if (input.BusinessType.HasValue)
                where = where.And(d => d.BusinessType == input.BusinessType.Value);
            if (input.Status.HasValue)
                where = where.And(d => d.Status == input.Status.Value);
            if (!string.IsNullOrEmpty(input.OperName))
                where = where.And(d => d.OperName.Contains(input.OperName));
            if (input.StartDate.HasValue && input.EndDate.HasValue)
                where = where.And(d => d.OperTime.Value.Between(input.StartDate.Value, input.EndDate.Value));

            return _fsql.Select<SysOperLog>()
                .Where(where)
                .OrderByDescending(d => d.OperTime)
                .Page(input)
                .ToList();
        }

        public int InsertOperLog(SysOperLog operLog)
        {
            operLog.CreateTime = DateTime.Now;
            return (int)_fsql.Insert(operLog).ExecuteAffrows();
        }

        public int DeleteOperLogById(long operId)
        {
            return _fsql.Delete<SysOperLog>()
                .Where(d => d.OperId == operId)
                .ExecuteAffrows();
        }

        public int CleanOperLog()
        {
            return _fsql.Delete<SysOperLog>()
                .Where(d => 1 == 1)
                .ExecuteAffrows();
        }
    }
}