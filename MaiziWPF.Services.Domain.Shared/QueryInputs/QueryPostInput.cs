using FreeSql.Internal.Model;
using System;

namespace MaiziWPF.Services.Domain.Shared
{
    public class QueryPostInput : BasePagingInfo, IPagingInfo
    {
        public String PostCode { get; set; }
        public String PostCategory { get; set; }
        public String PostName { get; set; }
        public Int64 DeptId { get; set; }
        public String Status { get; set; }
    }
}