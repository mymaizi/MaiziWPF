using MaiziWPF.Core;
using MaiziWPF.Modules.Sys;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using Prism.Commands;
using Prism.Container.DryIoc;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Navigation.Regions;
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
        private readonly ISysMenuService _menuService;
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
        public List<SysMenu> MenuItems { get; }
        public object _selectedItem;
        public object SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value); }
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

        public MainViewModel(IRegionManager regionManager, IContainerProvider containerProvider, ISysMenuService menuService)
        {
            _menuService = menuService;
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
                }
            });
            ToggleMoreMenuCommand = new DelegateCommand(() =>
            {
                IsMoreMenuOpen = !IsMoreMenuOpen;
            });
            MenuItems = _menuService.SelectMenuList(new SysMenu()
            {
                MenuType = "M,C",
                Status = "0"
            }, 1);

            MenuSelectionCommand = new DelegateCommand<SysMenu>(m =>
            {
                var tabRegion = _regionManager.Regions[RegionNames.TabRegion];
                if (!tabRegion.Views.Any(v => v.GetType().Name == m.Component))
                {
                    tabRegion.Add(GetView(m));
                }
                else {                    
                    var view = tabRegion.Views.FirstOrDefault(v => v.GetType().Name == m.Component);
                    if (view != null)
                    {
                        SelectedItem = view;
                    }
                }
            });
        }

        private  FrameworkElement GetView(SysMenu m)
        {
            string ns = string.IsNullOrEmpty(m.Path) ? "MaiziWPF.Modules.Sys" : m.Path;
            string fullName = $"{ns}.{m.Component}, {ns}";
            Type viewType = Type.GetType(fullName);
            var view = ContainerLocator.Container.Resolve(viewType) as FrameworkElement;
            (view.DataContext as ITabItemInfo)?.Header = m.MenuName;
            (view.DataContext as ITabItemInfo)?.Component = m.Component;
            return view;
        }
     
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var parameters = navigationContext.Parameters;
            if (parameters.ContainsKey("CurrentUser"))
            {
                CurrentUser = parameters.GetValue<SysUser>("CurrentUser");
            }

            var m = MenuItems.First();
            if (m != null)
            {
                _regionManager.Regions[RegionNames.TabRegion].Add(GetView(m));
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