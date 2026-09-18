using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;

namespace MaiziWPF.Services.Application
{
    public class SysLogininforService : ISysLogininforService
    {
        private readonly ISysLogininforRepository _repository;

        public SysLogininforService(ISysLogininforRepository repository)
        {
            _repository = repository;
        }

        public List<SysLogininfo> SelectLogininforList(QueryLoginInfoInput input)
        {
            return _repository.SelectLogininforList(input);
        }

        public int InsertLogininfor(SysLogininfo logininfor)
        {
            return _repository.InsertLogininfor(logininfor);
        }

        public int DeleteLogininforById(long infoId)
        {
            return _repository.DeleteLogininforById(infoId);
        }

        public int CleanLogininfor()
        {
            return _repository.CleanLogininfor();
        }
    }
}