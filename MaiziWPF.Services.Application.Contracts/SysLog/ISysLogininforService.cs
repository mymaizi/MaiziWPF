using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Application.Contracts
{
    public interface ISysLogininforService : ITransientDependency
    {
        List<SysLogininfo> SelectLogininforList(QueryLoginInfoInput input);

        int InsertLogininfor(SysLogininfo logininfor);

        int DeleteLogininforByIds(long[] infoIds);

        int CleanLogininfor();
    }
}