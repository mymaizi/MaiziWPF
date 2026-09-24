using System.Collections.Generic;

namespace MaiziWPF.Services.Domain.Shared
{
    public class DataScopeRule
    {
        public DataScopeType ScopeType { get; set; }

        public long RoleId { get; set; }

        public long DeptId { get; set; }

        public long UserId { get; set; }

        public List<long> CustomDeptIds { get; set; } = new();

        public List<long> DeptAndChildIds { get; set; } = new();
    }
}