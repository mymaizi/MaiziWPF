using FreeSql.Internal.Model;

namespace MaiziWPF.Services.Domain.Shared
{
    public class QueryUserOnlineInput : BasePagingInfo, IPagingInfo
    {
        public string LoginName { get; set; }
        public string Ipaddr { get; set; }
    }
}