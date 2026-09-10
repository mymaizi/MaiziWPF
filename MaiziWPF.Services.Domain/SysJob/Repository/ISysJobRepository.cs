using FreeSql;
using MaiziWPF.Services.Domain.Shared;
using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Domain
{
    public interface ISysJobRepository : IBaseRepository<SysJob, int>, ITransientDependency
    {
        List<SysJob> SelectJobList(QueryJobInput input);

        SysJob SelectJobById(long jobId);

        int InsertJob(SysJob job);

        int UpdateJob(SysJob job);

        int DeleteJobById(long jobId);

        bool CheckJobNameUnique(string jobName, long? jobId = null);
    }
}