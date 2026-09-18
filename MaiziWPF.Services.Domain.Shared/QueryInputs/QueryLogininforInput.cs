using FreeSql.Internal.Model;

namespace MaiziWPF.Services.Domain.Shared
{
    public class QueryLogininforInput : BasePagingInfo, IPagingInfo
    {
        public string UserName { get; set; }
        public string Ipaddr { get; set; }
        public string Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}