using FreeSql;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using System;

namespace MaiziWPF.Services.MySql
{
    public class SysOssRepository : BaseRepository<SysOss, int>, ISysOssRepository
    {
        private readonly IFreeSql _fsql;

        public SysOssRepository(IFreeSql fsql) : base(fsql)
        {
            _fsql = fsql;
        }

        public List<SysOss> SelectOssList(QueryOssInput input)
        {
            System.Linq.Expressions.Expression<Func<SysOss, bool>> where = d => d.DelFlag == "0";
            if (!string.IsNullOrEmpty(input.FileName))
                where = where.And(d => d.FileName.Contains(input.FileName));
            if (!string.IsNullOrEmpty(input.OriginalName))
                where = where.And(d => d.OriginalName.Contains(input.OriginalName));
            if (!string.IsNullOrEmpty(input.Service))
                where = where.And(d => d.Service == input.Service);

            return _fsql.Select<SysOss>()
                .Where(where)
                .OrderByDescending(d => d.CreateTime)
                .Page(input)
                .ToList();
        }

        public SysOss SelectOssById(long ossId)
        {
            return _fsql.Select<SysOss>()
                .Where(d => d.OssId == ossId)
                .First();
        }

        public int InsertOss(SysOss oss)
        {
            return _fsql.Insert<SysOss>().AppendData(oss).ExecuteAffrows();
        }

        public int DeleteOssById(long ossId)
        {
            return _fsql.Update<SysOss>()
                .Set(d => d.DelFlag, "1")
                .Where(d => d.OssId == ossId)
                .ExecuteAffrows();
        }

        public int DeleteOssByIds(List<long> ossIds)
        {
            return _fsql.Update<SysOss>()
                .Set(d => d.DelFlag, "1")
                .Where(d => ossIds.Contains(d.OssId))
                .ExecuteAffrows();
        }
    }
}