using FreeSql.Internal.Model;
using System.ComponentModel;
using System.Xml.Linq;

namespace MaiziWPF.Services.Domain.Shared
{
    public class QueryDictTypeInput : BasePagingInfo, IPagingInfo
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public String DictType { get; set; }
        public String DictName { get; set; }
    }
}