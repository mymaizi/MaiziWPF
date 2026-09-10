using FreeSql;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using System;

namespace MaiziWPF.Services.MySql
{
    public class SysJobRepository : BaseRepository<SysJob, int>, ISysJobRepository
    {
        private readonly IFreeSql _fsql;

        public SysJobRepository(IFreeSql fsql) : base(fsql)
        {
            _fsql = fsql;
        }

        public List<SysJob> SelectJobList(QueryJobInput input)
        {
            System.Linq.Expressions.Expression<Func<SysJob, bool>> where = d => d.DelFlag == "0";
            if (!string.IsNullOrEmpty(input.JobName))
                where = where.And(d => d.JobName.Contains(input.JobName));
            if (!string.IsNullOrEmpty(input.JobGroup))
                where = where.And(d => d.JobGroup == input.JobGroup);
            if (!string.IsNullOrEmpty(input.Status))
                where = where.And(d => d.Status == input.Status);

            return _fsql.Select<SysJob>()
                .Where(where)
                .OrderBy(d => d.CreateTime)
                .Page(input)
                .ToList();
        }

        public SysJob SelectJobById(long jobId)
        {
            return _fsql.Select<SysJob>()
                .Where(d => d.JobId == jobId)
                .First();
        }

        public int InsertJob(SysJob job)
        {
            return _fsql.Insert<SysJob>().AppendData(job).ExecuteAffrows();
        }

        public int UpdateJob(SysJob job)
        {
            return _fsql.Update<SysJob>()
                .SetSource(job)
                .ExecuteAffrows();
        }

        public int DeleteJobById(long jobId)
        {
            return _fsql.Update<SysJob>()
                .Set(d => d.DelFlag, "1")
                .Where(d => d.JobId == jobId)
                .ExecuteAffrows();
        }

        public bool CheckJobNameUnique(string jobName, long? jobId = null)
        {
            var query = _fsql.Select<SysJob>()
                .Where(d => d.JobName == jobName && d.DelFlag == "0");
            if (jobId.HasValue)
                query = query.Where(d => d.JobId != jobId.Value);
            return !query.Any();
        }
    }
}