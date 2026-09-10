using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Application.Contracts
{
    public interface ISysJobService : ITransientDependency
    {
        List<SysJob> SelectJobList(QueryJobInput input);

        SysJob SelectJobById(long jobId);

        int InsertJob(SysJob job);

        int UpdateJob(SysJob job);

        int DeleteJobById(long jobId);

        bool CheckJobNameUnique(string jobName, long? jobId = null);
    }
}