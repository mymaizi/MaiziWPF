using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Application.Contracts
{
    public interface ISysDictService : ITransientDependency
    {
        List<SysDictData> SelectDictDataByType(String dictType);

        List<SysDictType> SelectDictTypeList(QueryDictTypeInput dictType);

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