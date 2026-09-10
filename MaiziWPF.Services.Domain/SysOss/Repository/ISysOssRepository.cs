using FreeSql;
using MaiziWPF.Services.Domain.Shared;
using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Domain
{
    public interface ISysOssRepository : IBaseRepository<SysOss, int>, ITransientDependency
    {
        List<SysOss> SelectOssList(QueryOssInput input);

        SysOss SelectOssById(long ossId);

        int InsertOss(SysOss oss);

        int DeleteOssById(long ossId);

        int DeleteOssByIds(List<long> ossIds);
    }
}