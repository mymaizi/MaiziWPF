using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Application.Contracts
{
    public interface ISysNoticeService : ITransientDependency
    {
        List<SysNotice> SelectNoticeList(QueryNoticeInput input);

        SysNotice SelectNoticeById(long noticeId);

        int InsertNotice(SysNotice notice);

        int UpdateNotice(SysNotice notice);

        int DeleteNoticeById(long noticeId);
    }
}