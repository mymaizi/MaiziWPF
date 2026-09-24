﻿﻿﻿﻿﻿﻿﻿﻿﻿using MaiziWPF.Core;
using MaiziWPF.Core.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Navigation.Regions;

namespace MaiziWPF.Modules.Sys
{
    public class SysModule : IModule
    {
        private readonly IRegionManager _regionManager;
        public SysModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }
       
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<DashboardView>();
            containerRegistry.RegisterForNavigation<ProfileView>();
            containerRegistry.RegisterForNavigation<UserListView>();
            containerRegistry.RegisterForNavigation<RoleListView>();
            containerRegistry.RegisterForNavigation<MenuListView>();
            containerRegistry.RegisterForNavigation<DeptListView>();
            containerRegistry.RegisterForNavigation<PostListView>();
            containerRegistry.RegisterForNavigation<DictListView>();
            containerRegistry.RegisterForNavigation<ConfigListView>();
            containerRegistry.RegisterForNavigation<OperLogListView>();
            containerRegistry.RegisterForNavigation<LoginInfoListView>();
            containerRegistry.RegisterForNavigation<NoticeListView>();
            containerRegistry.RegisterForNavigation<OssListView>();
            containerRegistry.RegisterDialog<UserFormView>();
            containerRegistry.RegisterDialog<DeptFormView>();
            containerRegistry.RegisterDialog<MenuFormView>();
            containerRegistry.RegisterDialog<IconPickerView>();
            containerRegistry.RegisterDialog<RoleFormView>();
            containerRegistry.RegisterDialog<RolePermissionView>();
            containerRegistry.RegisterDialog<RoleAuthUserView>();
            containerRegistry.RegisterDialog<AuthRoleView>();
            containerRegistry.RegisterDialog<PostFormView>();
            containerRegistry.RegisterDialog<DictTypeFormView>();
            containerRegistry.RegisterDialog<DictDataFormView>();
            containerRegistry.RegisterDialog<ConfigFormView>();
            containerRegistry.RegisterDialog<NoticeFormView>();
            containerRegistry.RegisterDialog<NoticeDetailView>();
            containerRegistry.RegisterDialog<OssUploadView, OssUploadViewModel>();
            containerRegistry.RegisterDialog<ConfirmDialog, ConfirmDialogViewModel>();
            containerRegistry.RegisterDialog<MessageDialog, MessageDialogViewModel>();
        }
    }
}