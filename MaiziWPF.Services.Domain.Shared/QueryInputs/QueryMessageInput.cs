using FreeSql.Internal.Model;
using System;

namespace MaiziWPF.Services.Domain.Shared
{
    public class QueryMessageInput : BasePagingInfo, IPagingInfo
    {
        public string Category { get; set; }
        public string Type { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}