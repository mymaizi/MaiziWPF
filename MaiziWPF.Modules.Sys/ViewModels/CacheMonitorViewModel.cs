using MaiziWPF.Core;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace MaiziWPF.Modules.Sys
{
    public class CacheMonitorViewModel : BindableBase
    {
        private readonly IDialogHostService _dialogHostService;

        private List<CacheInfo> _cacheList;
        public List<CacheInfo> CacheList
        {
            get { return _cacheList; }
            set { SetProperty(ref _cacheList, value); }
        }

        public CacheMonitorViewModel(IDialogHostService dialogHostService)
        {
            _dialogHostService = dialogHostService;
            LoadCacheInfo();
        }

        private void LoadCacheInfo()
        {
            CacheList = new List<CacheInfo>
            {
                new CacheInfo { CacheName = "用户缓存", CacheKey = "sys_user:", CacheType = "Redis", Remark = "系统用户信息缓存" },
                new CacheInfo { CacheName = "角色缓存", CacheKey = "sys_role:", CacheType = "Redis", Remark = "角色信息缓存" },
                new CacheInfo { CacheName = "菜单缓存", CacheKey = "sys_menu:", CacheType = "Redis", Remark = "系统菜单缓存" },
                new CacheInfo { CacheName = "部门缓存", CacheKey = "sys_dept:", CacheType = "Redis", Remark = "部门信息缓存" },
                new CacheInfo { CacheName = "字典缓存", CacheKey = "sys_dict:", CacheType = "Redis", Remark = "字典数据缓存" },
                new CacheInfo { CacheName = "参数缓存", CacheKey = "sys_config:", CacheType = "Redis", Remark = "系统参数缓存" },
                new CacheInfo { CacheName = "登录缓存", CacheKey = "login_tokens:", CacheType = "Redis", Remark = "用户登录令牌缓存" },
                new CacheInfo { CacheName = "在线用户", CacheKey = "online_tokens:", CacheType = "Redis", Remark = "在线用户信息缓存" },
            };
        }
    }

    public class CacheInfo
    {
        public string CacheName { get; set; }
        public string CacheKey { get; set; }
        public string CacheType { get; set; }
        public string Remark { get; set; }
    }
}