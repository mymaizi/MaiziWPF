using FreeSql;
using MaiziWPF.Services.Domain.Shared;
using Volo.Abp.DependencyInjection;

namespace MaiziWPF.Services.Domain
{
    public interface ISysNoticeRepository : IBaseRepository<SysNotice, int>, ITransientDependency
    {
        List<SysNotice> SelectNoticeList(QueryNoticeInput input);

        SysNotice SelectNoticeById(long noticeId);

        int InsertNotice(SysNotice notice);

        int UpdateNotice(SysNotice notice);

        int DeleteNoticeById(long noticeId);
    }
}