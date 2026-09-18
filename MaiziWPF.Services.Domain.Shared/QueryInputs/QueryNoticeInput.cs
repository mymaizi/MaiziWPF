using FreeSql.Internal.Model;

namespace MaiziWPF.Services.Domain.Shared
{
    public class QueryNoticeInput : BasePagingInfo, IPagingInfo
    {
        public string NoticeTitle { get; set; }
        public string NoticeType { get; set; }
        public string Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}