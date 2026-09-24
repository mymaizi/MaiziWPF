namespace MaiziWPF.Services.Domain.Shared
{
    public static class AuditUserContext
    {
        private static long? _userId;
        private static long? _deptId;
        private static bool _isAuthenticated;

        public static bool IsAuthenticated => _isAuthenticated;

        public static long UserId => _userId ?? 0;

        public static long DeptId => _deptId ?? 0;

        public static void SetAuthenticated(long userId, long deptId)
        {
            _userId = userId;
            _deptId = deptId;
            _isAuthenticated = true;
        }

        public static void Clear()
        {
            _userId = null;
            _deptId = null;
            _isAuthenticated = false;
        }
    }
}