using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;

namespace MaiziWPF.Services.Application
{
    public class SysJobService : ISysJobService
    {
        private readonly ISysJobRepository _repository;

        public SysJobService(ISysJobRepository repository)
        {
            _repository = repository;
        }

        public List<SysJob> SelectJobList(QueryJobInput input)
        {
            return _repository.SelectJobList(input);
        }

        public SysJob SelectJobById(long jobId)
        {
            return _repository.SelectJobById(jobId);
        }

        public int InsertJob(SysJob job)
        {
            return _repository.InsertJob(job);
        }

        public int UpdateJob(SysJob job)
        {
            return _repository.UpdateJob(job);
        }

        public int DeleteJobById(long jobId)
        {
            return _repository.DeleteJobById(jobId);
        }

        public bool CheckJobNameUnique(string jobName, long? jobId = null)
        {
            return _repository.CheckJobNameUnique(jobName, jobId);
        }
    }
}