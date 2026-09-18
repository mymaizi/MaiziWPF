using FreeSql.Internal.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaiziWPF.Services.Domain.Shared
{
    public class QueryConfigInput : BasePagingInfo, IPagingInfo
    {
        public String ConfigName { get; set; }
        public String ConfigKey { get; set; }
        public String ConfigType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public String Status { get; set; }
    }
}