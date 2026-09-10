using FreeSql.Internal.Model;

namespace MaiziWPF.Services.Domain.Shared
{
    public class QueryJobInput : BasePagingInfo
    {
        public string JobName { get; set; }
        public string JobGroup { get; set; }
        public string Status { get; set; }
    }
}