using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;

namespace MaiziWPF.Services.Application
{
    public class SysOssService : ISysOssService
    {
        private readonly ISysOssRepository _repository;

        public SysOssService(ISysOssRepository repository)
        {
            _repository = repository;
        }

        public List<SysOss> SelectOssList(QueryOssInput input)
        {
            return _repository.SelectOssList(input);
        }

        public SysOss SelectOssById(long ossId)
        {
            return _repository.SelectOssById(ossId);
        }

        public int InsertOss(SysOss oss)
        {
            return _repository.InsertOss(oss);
        }

        public int DeleteOssById(long ossId)
        {
            return _repository.DeleteOssById(ossId);
        }

        public int DeleteOssByIds(List<long> ossIds)
        {
            return _repository.DeleteOssByIds(ossIds);
        }
    }
}