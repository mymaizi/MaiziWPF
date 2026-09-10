using FreeSql;
using MaiziWPF.Services.Domain.Shared;
using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Domain
{
    public interface ISysConfigRepository : IBaseRepository<SysConfig, int>, ITransientDependency
    {
        List<SysConfig> SelectConfigList(QueryConfigInput input);

        SysConfig SelectConfigById(long configId);

        int InsertConfig(SysConfig config);

        int UpdateConfig(SysConfig config);

        int DeleteConfigById(long configId);

        bool CheckConfigKeyUnique(SysConfig config);
    }
}