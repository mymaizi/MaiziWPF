using System;
using System.Collections.Generic;

namespace MaiziWPF.Common
{
    public class SecurityUtils
    {
        public const long SUPER_ADMIN_USER_ID = 1761100000000000001;

        /// <summary>
        /// 是否为管理员（用户ID匹配）
        /// </summary>
        public static bool IsSuperAdmin(long userId)
        {
            return userId == SUPER_ADMIN_USER_ID;
        }

    }
}