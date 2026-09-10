using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Application.Contracts
{
    public interface ISysOperLogService : ITransientDependency
    {
        List<SysOperLog> SelectOperLogList(QueryOperLogInput input);

        int InsertOperLog(SysOperLog operLog);

        int DeleteOperLogById(long operId);

        int CleanOperLog();
    }
}