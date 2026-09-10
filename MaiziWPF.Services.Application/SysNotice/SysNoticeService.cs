using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;

namespace MaiziWPF.Services.Application
{
    public class SysNoticeService : ISysNoticeService
    {
        private readonly ISysNoticeRepository _repository;

        public SysNoticeService(ISysNoticeRepository repository)
        {
            _repository = repository;
        }

        public List<SysNotice> SelectNoticeList(QueryNoticeInput input)
        {
            return _repository.SelectNoticeList(input);
        }

        public SysNotice SelectNoticeById(long noticeId)
        {
            return _repository.SelectNoticeById(noticeId);
        }

        public int InsertNotice(SysNotice notice)
        {
            return _repository.InsertNotice(notice);
        }

        public int UpdateNotice(SysNotice notice)
        {
            return _repository.UpdateNotice(notice);
        }

        public int DeleteNoticeById(long noticeId)
        {
            return _repository.DeleteNoticeById(noticeId);
        }
    }
}