using FreeSql.Internal.Model;

namespace MaiziWPF.Services.Domain.Shared
{
    public class QueryOssInput : BasePagingInfo
    {
        public string FileName { get; set; }
        public string OriginalName { get; set; }
        public string Service { get; set; }
    }
}