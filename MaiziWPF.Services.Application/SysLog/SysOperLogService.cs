using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;

namespace MaiziWPF.Services.Application
{
    public class SysOperLogService : ISysOperLogService
    {
        private readonly ISysOperLogRepository _repository;

        public SysOperLogService(ISysOperLogRepository repository)
        {
            _repository = repository;
        }

        public List<SysOperLog> SelectOperLogList(QueryOperLogInput input)
        {
            return _repository.SelectOperLogList(input);
        }

        public int InsertOperLog(SysOperLog operLog)
        {
            return _repository.InsertOperLog(operLog);
        }

        public int DeleteOperLogById(long operId)
        {
            return _repository.DeleteOperLogById(operId);
        }

        public int CleanOperLog()
        {
            return _repository.CleanOperLog();
        }
    }
}