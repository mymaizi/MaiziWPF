using FreeSql.Internal.Model;
using System.ComponentModel;
using System.Xml.Linq;

namespace MaiziWPF.Services.Domain.Shared
{
    public class QueryRoleInput : BasePagingInfo, IPagingInfo
    {
        public String RoleName { get; set; }
        public String RoleKey { get; set; }
        public String Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}