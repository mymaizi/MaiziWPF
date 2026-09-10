using FreeSql.Internal.Model;

namespace MaiziWPF.Services.Domain.Shared
{
    public class QueryOperLogInput : BasePagingInfo
    {
        public string Title { get; set; }
        public int? BusinessType { get; set; }
        public int? Status { get; set; }
        public string OperName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}