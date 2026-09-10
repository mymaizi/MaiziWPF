using FreeSql;
using MaiziWPF.Services.Domain.Shared;
using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Domain
{
    public interface ISysOperLogRepository : IBaseRepository<SysOperLog, int>, ITransientDependency
    {
        List<SysOperLog> SelectOperLogList(QueryOperLogInput input);

        int InsertOperLog(SysOperLog operLog);

        int DeleteOperLogById(long operId);

        int CleanOperLog();
    }
}