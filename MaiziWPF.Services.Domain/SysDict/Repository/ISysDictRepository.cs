using FreeSql;
using MaiziWPF.Services.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Domain
{
    public interface ISysDictRepository : IBaseRepository<SysDictType, int>, ITransientDependency
    {
        List<SysDictData> SelectDictDataByType(String dictType);

        List<SysDictType> SelectDictTypeList(QueryDictTypeInput input);

        SysDictType SelectDictTypeById(long dictId);

        int InsertDictType(SysDictType dictType);

        int UpdateDictType(SysDictType dictType);

        int DeleteDictTypeById(long dictId);

        bool CheckDictTypeUnique(SysDictType dictType);

        List<SysDictData> SelectDictDataList(QueryDictDataInput input);

        SysDictData SelectDictDataById(long dictCode);

        int InsertDictData(SysDictData dictData);

        int UpdateDictData(SysDictData dictData);

        int DeleteDictDataById(long dictCode);
    }
}