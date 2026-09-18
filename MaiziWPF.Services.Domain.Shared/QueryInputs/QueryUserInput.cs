using FreeSql.Internal.Model;
using System.ComponentModel;
using System.Xml.Linq;

namespace MaiziWPF.Services.Domain.Shared
{
    public class QueryUserInput : BasePagingInfo, IPagingInfo
    {
        public String UserName { get; set; }
        public String Phonenumber { get; set; }
        public String Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Int64? DeptId { get; set; }
        public String DeptName { get; set; }
    }
}