using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Application.Contracts
{
    public interface ISysLogininforService : ITransientDependency
    {
        List<SysLogininfor> SelectLogininforList(QueryLogininforInput input);

        int InsertLogininfor(SysLogininfor logininfor);

        int DeleteLogininforById(long infoId);

        int CleanLogininfor();
    }
}