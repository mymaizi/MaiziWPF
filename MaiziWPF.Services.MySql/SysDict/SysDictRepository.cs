using FreeSql;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using System;

namespace MaiziWPF.Services.MySql
{
    public class SysDictRepository : BaseRepository<SysDictType, int>, ISysDictRepository
    {
        private readonly IFreeSql _fsql;

        public SysDictRepository(IFreeSql fsql) : base(fsql)
        {
            _fsql = fsql;
        }

        public List<SysDictData> SelectDictDataByType(string dictType)
        {
            return _fsql.Select<SysDictData>().Where(w => w.Status == "N" && w.DictType == dictType).OrderBy(o => o.DictSort).ToList();
        }

        public List<SysDictType> SelectDictTypeList(QueryDictTypeInput input)
        {
            System.Linq.Expressions.Expression<Func<SysDictType, bool>> where = w => w.DelFlag == "0";
            if (!string.IsNullOrEmpty(input.DictName))
                where = where.And(w => w.DictName.Contains(input.DictName));
            if (!string.IsNullOrEmpty(input.Status))
                where = where.And(w => w.Status == input.Status);
            if (!string.IsNullOrEmpty(input.DictType))
                where = where.And(w => w.DictType.Contains(input.DictType));
            if (input.StartDate.HasValue && input.EndDate.HasValue)
                where = where.And(u => u.CreateTime.Between(input.StartDate.Value, input.EndDate.Value));

            return _fsql.Select<SysDictType>().Where(where).Page(input).ToList();
        }

        public SysDictType SelectDictTypeById(long dictId)
        {
            return _fsql.Select<SysDictType>()
                .Where(d => d.DictId == dictId && d.DelFlag == "0"&& d.Status == "N")
                .First();
        }

        public int InsertDictType(SysDictType dictType)
        {
            dictType.CreateTime = DateTime.Now;
            return (int)_fsql.Insert(dictType).ExecuteAffrows();
        }

        public int UpdateDictType(SysDictType dictType)
        {
            dictType.UpdateTime = DateTime.Now;
            return _fsql.Update<SysDictType>().SetSource(dictType).ExecuteAffrows();
        }

        public int DeleteDictTypeById(long dictId)
        {
            return _fsql.Update<SysDictType>()
                .Set(d => d.DelFlag, "1")
                .Where(d => d.DictId == dictId)
                .ExecuteAffrows();
        }

        public bool CheckDictTypeUnique(SysDictType dictType)
        {
            var query = _fsql.Select<SysDictType>()
                .Where(d => d.DictType == dictType.DictType && d.DelFlag == "0");
            if (dictType.DictId != 0)
                query = query.Where(d => d.DictId != dictType.DictId);
            return !query.Any();
        }

        public List<SysDictData> SelectDictDataList(QueryDictDataInput input)
        {
            System.Linq.Expressions.Expression<Func<SysDictData, bool>> where = d => d.DelFlag == "0";
            if (!string.IsNullOrEmpty(input.DictType))
                where = where.And(d => d.DictType == input.DictType);
            if (!string.IsNullOrEmpty(input.DictLabel))
                where = where.And(d => d.DictLabel.Contains(input.DictLabel));
            if (!string.IsNullOrEmpty(input.Status))
                where = where.And(d => d.Status == input.Status);

            return _fsql.Select<SysDictData>()
                .Where(where)
                .OrderBy(d => d.DictSort)
                .Page(input)
                .ToList();
        }

        public SysDictData SelectDictDataById(long dictCode)
        {
            return _fsql.Select<SysDictData>()
                .Where(d => d.DictCode == dictCode && d.DelFlag == "0")
                .First();
        }

        public int InsertDictData(SysDictData dictData)
        {
            dictData.CreateTime = DateTime.Now;
            return (int)_fsql.Insert(dictData).ExecuteAffrows();
        }

        public int UpdateDictData(SysDictData dictData)
        {
            dictData.UpdateTime = DateTime.Now;
            return _fsql.Update<SysDictData>().SetSource(dictData).ExecuteAffrows();
        }

        public int DeleteDictDataById(long dictCode)
        {
            return _fsql.Update<SysDictData>()
                .Set(d => d.DelFlag, "1")
                .Where(d => d.DictCode == dictCode)
                .ExecuteAffrows();
        }
    }
}