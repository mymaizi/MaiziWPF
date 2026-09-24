using System.Linq;

namespace MaiziWPF.Services.Domain.Shared
{
    public static class DataPermissionManager
    {
        private static long[] _permittedDeptIds = System.Array.Empty<long>();

        private static long[] _permittedUserIds = System.Array.Empty<long>();

        private static bool _isEnabled;

        public static long[] PermittedDeptIds => _permittedDeptIds;

        public static long[] PermittedUserIds => _permittedUserIds;

        public static bool ShouldApplyFilter() => _isEnabled;

        public static void Set(long[] deptIds, long[] userIds)
        {
            _permittedDeptIds = deptIds;
            _permittedUserIds = userIds;
            _isEnabled = true;
        }

        public static void Clear()
        {
            _permittedDeptIds = System.Array.Empty<long>();
            _permittedUserIds = System.Array.Empty<long>();
            _isEnabled = false;
        }

        public static void ApplyFromRules(List<DataScopeRule> rules)
        {
            if (rules == null || rules.Count == 0)
            {
                Set(System.Array.Empty<long>(), System.Array.Empty<long>());
                return;
            }

            if (rules.Any(r => r.ScopeType == DataScopeType.ALL))
            {
                Clear();
                return;
            }

            var deptIds = new System.Collections.Generic.List<long>();
            var userIds = new System.Collections.Generic.List<long>();

            foreach (var rule in rules)
            {
                switch (rule.ScopeType)
                {
                    case DataScopeType.CUSTOM:
                        if (rule.CustomDeptIds != null)
                            foreach (var id in rule.CustomDeptIds)
                                if (!deptIds.Contains(id)) deptIds.Add(id);
                        break;

                    case DataScopeType.DEPT:
                        if (!deptIds.Contains(rule.DeptId)) deptIds.Add(rule.DeptId);
                        break;

                    case DataScopeType.DEPT_AND_CHILD:
                    case DataScopeType.DEPT_AND_CHILD_OR_SELF:
                        if (rule.DeptAndChildIds != null)
                            foreach (var id in rule.DeptAndChildIds)
                                if (!deptIds.Contains(id)) deptIds.Add(id);
                        if (!userIds.Contains(rule.UserId)) userIds.Add(rule.UserId);
                        break;

                    case DataScopeType.SELF:
                        if (!userIds.Contains(rule.UserId)) userIds.Add(rule.UserId);
                        break;
                }
            }

            Set(deptIds.ToArray(), userIds.ToArray());
        }
    }
}