using FreeSql;
using MaiziWPF.Services.Domain.Shared;
using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Domain
{
    public interface ISysLogininforRepository : IBaseRepository<SysLogininfo, int>, ITransientDependency
    {
        List<SysLogininfo> SelectLogininforList(QueryLoginInfoInput input);

        int InsertLogininfor(SysLogininfo logininfor);

        int DeleteLogininforById(long infoId);

        int CleanLogininfor();
    }
}