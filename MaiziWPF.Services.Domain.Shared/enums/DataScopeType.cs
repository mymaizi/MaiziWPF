using System.ComponentModel;

namespace MaiziWPF.Services.Domain.Shared
{
    public enum DataScopeType
    {
        [Description("全部数据权限")]
        ALL = 1,

        [Description("自定义数据权限")]
        CUSTOM = 2,

        [Description("本部门数据权限")]
        DEPT = 3,

        [Description("本部门及以下数据权限")]
        DEPT_AND_CHILD = 4,

        [Description("仅本人数据权限")]
        SELF = 5,

        [Description("本部门及以下或本人数据权限")]
        DEPT_AND_CHILD_OR_SELF = 6
    }
}