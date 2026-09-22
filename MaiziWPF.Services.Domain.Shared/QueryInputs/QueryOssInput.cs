using FreeSql.Internal.Model;

namespace MaiziWPF.Services.Domain.Shared
{
    public class QueryOssInput : BasePagingInfo, IPagingInfo
    {
        public string FileName { get; set; }
        public string OriginalName { get; set; }
        public string FileSuffix { get; set; }
        public string Service { get; set; }
        public System.DateTime? StartDate { get; set; }
        public System.DateTime? EndDate { get; set; }
    }
}