using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;

namespace MaiziWPF.Services.Application
{
    public class SysUserOnlineService : ISysUserOnlineService
    {
        private readonly ISysUserOnlineRepository _repository;

        public SysUserOnlineService(ISysUserOnlineRepository repository)
        {
            _repository = repository;
        }

        public List<SysUserOnline> SelectUserOnlineList(QueryUserOnlineInput input)
        {
            return _repository.SelectUserOnlineList(input);
        }

        public SysUserOnline SelectOnlineById(string sessionId)
        {
            return _repository.SelectOnlineById(sessionId);
        }

        public int DeleteOnlineById(string sessionId)
        {
            return _repository.DeleteOnlineById(sessionId);
        }

        public int DeleteOnlineBySessionIds(List<string> sessionIds)
        {
            return _repository.DeleteOnlineBySessionIds(sessionIds);
        }
    }
}