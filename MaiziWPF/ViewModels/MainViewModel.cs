using MaiziWPF.Core;
using MaiziWPF.Modules.Sys;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Views;
using Prism.Commands;
using Prism.Container.DryIoc;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MaiziWPF.ViewModels
{
    public class MainViewModel : BindableBase, INavigationAware
    {
        private readonly IPermissionService _permissionService;
        private readonly ICurrentUserService _currentUserService;
        private readonly Window _mainWindow;
        private readonly IRegionManager _regionManager;
        private String _maxsizeIcon = "Maximize";
        private SysUser _currentUser;
        private Boolean _isMoreMenuOpen;
        public ICommand CloseWindowCommand { get; }
        public ICommand MinimizeWindowCommand { get; }
        public ICommand MaximizeWindowCommand { get; }
        public ICommand CloseTabCommand { get; }
        public ICommand MenuSelectionCommand { get; }
        public ICommand ToggleMoreMenuCommand { get; }
        public ICommand LogoutCommand { get; }
        public List<SysMenu> MenuItems { get; set; }
        public object _selectedItem;
        public object SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value); }
        }

        private String _selectedComponent;
        public String SelectedComponent
        {
            get { return _selectedComponent; }
            set
            {
                if (SetProperty(ref _selectedComponent, value))
                {
                    SyncMenuSelection(value);
                }
            }
        }
        public String MaxsizeIcon
        {
            get { return _maxsizeIcon; }
            set { SetProperty(ref _maxsizeIcon, value); }
        }
        public SysUser CurrentUser
        {
            get { return _currentUser; }
            set { SetProperty(ref _currentUser, value); }
        }
        public Boolean IsMoreMenuOpen
        {
            get { return _isMoreMenuOpen; }
            set { SetProperty(ref _isMoreMenuOpen, value); }
        }

        public MainViewModel(
            IRegionManager regionManager,
            IContainerProvider containerProvider,
            IPermissionService permissionService,
            ICurrentUserService currentUserService)
        {
            _permissionService = permissionService;
            _currentUserService = currentUserService;
            _mainWindow = Application.Current.MainWindow as Window;
            _regionManager = regionManager;
            CloseWindowCommand = new DelegateCommand(() =>
            {
                _mainWindow.Hide();
            });
            MinimizeWindowCommand = new DelegateCommand(() =>
            {
                _mainWindow?.WindowState = WindowState.Minimized;
            });
            MaximizeWindowCommand = new DelegateCommand(() =>
            {
                _mainWindow?.WindowState = _mainWindow?.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                MaxsizeIcon = _mainWindow?.WindowState == WindowState.Maximized ? "WindowMaximize" : "Maximize";
            });
            CloseTabCommand = new DelegateCommand<string>(viewName =>
            {
                var tabRegion = _regionManager.Regions[RegionNames.TabRegion];
                var currentView = tabRegion.Views.FirstOrDefault(v => v.GetType().Name == viewName);
                if (currentView != null)
                {
                    tabRegion.Remove(currentView);
                    UpdateSelectionAfterTabClose(tabRegion);
                }
            });
            ToggleMoreMenuCommand = new DelegateCommand(() =>
            {
                IsMoreMenuOpen = !IsMoreMenuOpen;
            });
            LogoutCommand = new DelegateCommand(() =>
            {
                _currentUserService.Clear();
                _regionManager.Regions[RegionNames.TabRegion].RemoveAll();
                _regionManager.RequestNavigate(RegionNames.ContentRegion, nameof(Views.LoginView));
            });

            MenuItems = _currentUserService.MenuTree;

            MenuSelectionCommand = new DelegateCommand<SysMenu>(m =>
            {
                SelectedComponent = m.Component;
                var tabRegion = _regionManager.Regions[RegionNames.TabRegion];
                if (!tabRegion.Views.Any(v => v.GetType().Name == m.Component))
                {
                    var view = GetView(m);
                    if (view != null)
                    {
                        tabRegion.Add(view);
                        SelectedItem = view;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[MenuSelection] Failed to load view for: {m.MenuName} ({m.Component})");
                    }
                }
                else
                {
                    var view = tabRegion.Views.FirstOrDefault(v => v.GetType().Name == m.Component);
                    if (view != null)
                    {
                        SelectedItem = view;
                    }
                }
            });
        }

        private FrameworkElement GetView(SysMenu m)
        {
            try
            {
                string ns = string.IsNullOrEmpty(m.Path) ? "MaiziWPF.Modules.Sys" : m.Path;
                string fullName = $"{ns}.{m.Component}, {ns}";
                Type viewType = Type.GetType(fullName);
                if (viewType == null)
                {
                    System.Diagnostics.Debug.WriteLine($"[GetView] Type not found: {fullName}");
                    return null;
                }
                var view = ContainerLocator.Container.Resolve(viewType) as FrameworkElement;
                if (view == null)
                {
                    System.Diagnostics.Debug.WriteLine($"[GetView] Resolve failed: {fullName}");
                    return null;
                }
                (view.DataContext as ITabItemInfo)?.Header = m.MenuName;
                (view.DataContext as ITabItemInfo)?.Component = m.Component;
                return view;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GetView] Error: {ex.Message}");
                return null;
            }
        }
     
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            CurrentUser = _currentUserService.CurrentUser;
            MenuItems = _currentUserService.MenuTree;

            var m = MenuItems.FirstOrDefault(x => !string.IsNullOrEmpty(x.Component));
            if (m != null)
            {
                var view = GetView(m);
                if (view != null)
                {
                    _regionManager.Regions[RegionNames.TabRegion].Add(view);
                    SelectedComponent = m.Component;
                    SelectedItem = view;
                }
            }
        }

        private void UpdateSelectionAfterTabClose(IRegion tabRegion)
        {
            var views = tabRegion.Views.OfType<FrameworkElement>().ToList();
            if (views.Count == 0)
            {
                SelectedComponent = null;
            }
            else if (views.Count == 1)
            {
                var lastView = views[0];
                var component = (lastView.DataContext as ITabItemInfo)?.Component ?? lastView.GetType().Name;
                SelectedComponent = component;
            }
            else
            {
                var activeView = tabRegion.ActiveViews.FirstOrDefault() as FrameworkElement;
                if (activeView != null)
                {
                    var component = (activeView.DataContext as ITabItemInfo)?.Component ?? activeView.GetType().Name;
                    SelectedComponent = component;
                }
            }
        }

        private void SyncMenuSelection(String activeComponent)
        {
            SyncMenuItemsRecursive(MenuItems, activeComponent);
        }

        private void SyncMenuItemsRecursive(List<SysMenu> items, String activeComponent)
        {
            if (items == null) return;
            foreach (var item in items)
            {
                item.IsMenuSelected = !string.IsNullOrEmpty(activeComponent) && item.Component == activeComponent;
                if (item.Childs != null && item.Childs.Count > 0)
                {
                    SyncMenuItemsRecursive(item.Childs, activeComponent);
                }
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
    }
}