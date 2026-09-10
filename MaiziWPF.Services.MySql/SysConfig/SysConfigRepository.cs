using FreeSql;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using System;

namespace MaiziWPF.Services.MySql
{
    internal class SysConfigRepository : BaseRepository<SysConfig, int>, ISysConfigRepository
    {
        private readonly IFreeSql _fsql;

        public SysConfigRepository(IFreeSql fsql) : base(fsql)
        {
            _fsql = fsql;
        }

        public List<SysConfig> SelectConfigList(QueryConfigInput input)
        {
            System.Linq.Expressions.Expression<Func<SysConfig, bool>> where = d => d.DelFlag == "0";
            if (!string.IsNullOrEmpty(input.ConfigName))
                where = where.And(d => d.ConfigName == input.ConfigName);
            if (!string.IsNullOrEmpty(input.ConfigKey))
                where = where.And(d => d.ConfigKey == input.ConfigKey);
            if (!string.IsNullOrEmpty(input.ConfigType))
                where = where.And(d => d.ConfigType == input.ConfigType);
            if (input.StartDate.HasValue && input.EndDate.HasValue)
                where = where.And(u => u.CreateTime.Between(input.StartDate.Value, input.EndDate.Value));

            return _fsql.Select<SysConfig>().Where(where).Page(input).ToList();
        }

        public SysConfig SelectConfigById(long configId)
        {
            return _fsql.Select<SysConfig>()
                .Where(c => c.ConfigId == configId && c.DelFlag == "0")
                .First();
        }

        public int InsertConfig(SysConfig config)
        {
            config.CreateTime = DateTime.Now;
            return (int)_fsql.Insert(config).ExecuteAffrows();
        }

        public int UpdateConfig(SysConfig config)
        {
            config.UpdateTime = DateTime.Now;
            return _fsql.Update<SysConfig>().SetSource(config).ExecuteAffrows();
        }

        public int DeleteConfigById(long configId)
        {
            return _fsql.Update<SysConfig>()
                .Set(c => c.DelFlag, "2")
                .Where(c => c.ConfigId == configId)
                .ExecuteAffrows();
        }

        public bool CheckConfigKeyUnique(SysConfig config)
        {
            var query = _fsql.Select<SysConfig>()
                .Where(c => c.ConfigKey == config.ConfigKey && c.DelFlag == "0");
            if (config.ConfigId != 0)
                query = query.Where(c => c.ConfigId != config.ConfigId);
            return !query.Any();
        }
    }
}