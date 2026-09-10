using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Application.Contracts
{
    public interface ISysUserOnlineService : ITransientDependency
    {
        List<SysUserOnline> SelectUserOnlineList(QueryUserOnlineInput input);

        SysUserOnline SelectOnlineById(string sessionId);

        int DeleteOnlineById(string sessionId);

        int DeleteOnlineBySessionIds(List<string> sessionIds);
    }
}