using FreeSql;
using MaiziWPF.Services.Domain.Shared;
using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Domain
{
    public interface ISysUserOnlineRepository : IBaseRepository<SysUserOnline, int>, ITransientDependency
    {
        List<SysUserOnline> SelectUserOnlineList(QueryUserOnlineInput input);

        SysUserOnline SelectOnlineById(string sessionId);

        int DeleteOnlineById(string sessionId);

        int DeleteOnlineBySessionIds(List<string> sessionIds);
    }
}