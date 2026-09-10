using FreeSql;
using MaiziWPF.Services.Domain.Shared;
using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Domain
{
    public interface ISysLogininforRepository : IBaseRepository<SysLogininfor, int>, ITransientDependency
    {
        List<SysLogininfor> SelectLogininforList(QueryLogininforInput input);

        int InsertLogininfor(SysLogininfor logininfor);

        int DeleteLogininforById(long infoId);

        int CleanLogininfor();
    }
}