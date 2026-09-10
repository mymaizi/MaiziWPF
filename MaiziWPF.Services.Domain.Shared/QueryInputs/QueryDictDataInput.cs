using FreeSql.Internal.Model;

namespace MaiziWPF.Services.Domain.Shared
{
    public class QueryDictDataInput : BasePagingInfo
    {
        public string DictType { get; set; }
        public string DictLabel { get; set; }
        public string Status { get; set; }
    }
}