using System;
using System.Collections.Generic;

namespace MaiziWPF.Common
{
    public class SecurityUtils
    {
        public const long SUPER_ADMIN_USER_ID = 1761100000000000001;

        public const long SUPER_ADMIN_ROLE_ID = 1761300000000000001;

        public const string SUPER_ADMIN_ROLE_KEY = "superadmin";

        /// <summary>
        /// 是否为管理员（用户ID匹配）
        /// </summary>
        public static bool IsSuperAdmin(long userId)
        {
            return userId == SUPER_ADMIN_USER_ID;
        }

        /// <summary>
        /// 是否为管理员（角色ID匹配）
        /// </summary>
        public static bool IsSuperAdminRole(long roleId)
        {
            return roleId == SUPER_ADMIN_ROLE_ID;
        }

        /// <summary>
        /// 是否为管理员（角色Key匹配）
        /// </summary>
        public static bool IsSuperAdminRole(string roleKey)
        {
            return SUPER_ADMIN_ROLE_KEY.Equals(roleKey);
        }

    }
}