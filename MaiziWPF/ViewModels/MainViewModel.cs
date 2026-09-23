using MaiziWPF.Core;
using MaiziWPF.Modules.Sys;
using MaiziWPF.Services.Application.Contracts;
using MaiziWPF.Services.Domain;
using MaiziWPF.Services.Domain.Shared;
using MaiziWPF.Views;
using Prism.Commands;
using Prism.Container.DryIoc;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using Prism.Navigation;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MaiziWPF.ViewModels
{
    public class MainViewModel : BindableBase, INavigationAware
    {
        private readonly IPermissionService _permissionService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMqttService _mqttService;
        private readonly ILocalDbService _localDbService;
        private readonly ISysUserRepository _userRepository;
        private readonly ISysMessageService _messageService;
        private readonly Window _mainWindow;
        private readonly IRegionManager _regionManager;
        private readonly IDialogHostService _dialogHostService;
        private readonly ISnackbarService _snackbarService;
        private String _maxsizeIcon = "Maximize";
        private SysUser _currentUser;
        private Boolean _isMoreMenuOpen;
        private Boolean _isMessagePopupOpen;
        private Int32 _selectedMessageTab = 1;
        private ObservableCollection<MessageItemViewModel> _allMessages = new ObservableCollection<MessageItemViewModel>();
        private ObservableCollection<MessageItemViewModel> _filteredMessages = new ObservableCollection<MessageItemViewModel>();
        private HashSet<long> _readSet = new HashSet<long>();
        private Int32 _systemCount;
        private Int32 _noticeCount;
        private Int32 _workflowCount;
        public ICommand CloseWindowCommand { get; }
        public ICommand MinimizeWindowCommand { get; }
        public ICommand MaximizeWindowCommand { get; }
        public ICommand CloseTabCommand { get; }
        public ICommand MenuSelectionCommand { get; }
        public ICommand ToggleMoreMenuCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand OpenProfileCommand { get; }
        public ICommand SelectMessageTabCommand { get; }
        public ICommand MarkAllReadCommand { get; }
        public ICommand ClearMessagesCommand { get; }
        public ICommand DeleteMessageCommand { get; }
        public ICommand MessageClickCommand { get; }
        private List<SysMenu> _menuItems;
        public List<SysMenu> MenuItems
        {
            get { return _menuItems; }
            set { SetProperty(ref _menuItems, value); }
        }
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
        public Boolean IsMessagePopupOpen
        {
            get { return _isMessagePopupOpen; }
            set { SetProperty(ref _isMessagePopupOpen, value); }
        }
        public Int32 SelectedMessageTab
        {
            get { return _selectedMessageTab; }
            set
            {
                if (SetProperty(ref _selectedMessageTab, value))
                {
                    RefreshFilteredMessages();
                }
            }
        }

        public ObservableCollection<MessageItemViewModel> FilteredMessages
        {
            get { return _filteredMessages; }
            set { SetProperty(ref _filteredMessages, value); }
        }

        public Int32 SystemCount
        {
            get { return _systemCount; }
            set { SetProperty(ref _systemCount, value); }
        }

        public Int32 NoticeCount
        {
            get { return _noticeCount; }
            set { SetProperty(ref _noticeCount, value); }
        }

        public Int32 WorkflowCount
        {
            get { return _workflowCount; }
            set { SetProperty(ref _workflowCount, value); }
        }

        private Boolean _hasUnreadMessage;
        public Boolean HasUnreadMessage
        {
            get { return _hasUnreadMessage; }
            set { SetProperty(ref _hasUnreadMessage, value); }
        }

        public MainViewModel(
            IRegionManager regionManager,
            IContainerProvider containerProvider,
            IPermissionService permissionService,
            ICurrentUserService currentUserService,
            IMqttService mqttService,
            ILocalDbService localDbService,
            ISysUserRepository userRepository,
            ISysMessageService messageService,
            IDialogHostService dialogHostService,
            ISnackbarService snackbarService)
        {
            _permissionService = permissionService;
            _currentUserService = currentUserService;
            _mqttService = mqttService;
            _localDbService = localDbService;
            _userRepository = userRepository;
            _messageService = messageService;
            _mainWindow = Application.Current.MainWindow as Window;
            _regionManager = regionManager;
            _dialogHostService = dialogHostService;
            _snackbarService = snackbarService;
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
            LogoutCommand = new DelegateCommand(async () =>
            {
                _userRepository.UpdateOnlineStatus(_currentUserService.UserId, 0);
                await _mqttService.StopAsync();
                _currentUserService.Clear();
                _regionManager.Regions[RegionNames.TabRegion].RemoveAll();
                _regionManager.RequestNavigate(RegionNames.ContentRegion, nameof(Views.LoginView));
            });
            OpenProfileCommand = new DelegateCommand(() =>
            {
                IsMoreMenuOpen = false;
                var tabRegion = _regionManager.Regions[RegionNames.TabRegion];
                if (!tabRegion.Views.Any(v => v.GetType().Name == "ProfileView"))
                {
                    var view = ContainerLocator.Container.Resolve(typeof(MaiziWPF.Modules.Sys.ProfileView)) as FrameworkElement;
                    if (view != null)
                    {
                        (view.DataContext as ITabItemInfo)?.Header = "个人信息";
                        (view.DataContext as ITabItemInfo)?.Component = "ProfileView";
                        tabRegion.Add(view);
                        SelectedItem = view;
                        SelectedComponent = "ProfileView";
                    }
                }
                else
                {
                    var view = tabRegion.Views.FirstOrDefault(v => v.GetType().Name == "ProfileView");
                    if (view != null)
                    {
                        SelectedItem = view;
                        SelectedComponent = "ProfileView";
                    }
                }
            });

            SelectMessageTabCommand = new DelegateCommand<object>(param =>
            {
                if (param != null && int.TryParse(param.ToString(), out int tabIndex))
                {
                    SelectedMessageTab = tabIndex;
                }
            });

            MarkAllReadCommand = new DelegateCommand(() =>
            {
                var userId = _currentUserService.UserId;
                _localDbService.MarkAllRead(userId);
                _readSet = new HashSet<long>(_localDbService.GetAll(userId).Where(s => s.IsRead).Select(s => s.MsgId));
                foreach (var item in _allMessages)
                {
                    item.IsRead = _readSet.Contains(item.Message.MessageId);
                }
                RefreshMessageCounts();
                RefreshFilteredMessages();
            });

            ClearMessagesCommand = new DelegateCommand(() =>
            {
                var userId = _currentUserService.UserId;
                _localDbService.ClearReadMessages(userId);
                _readSet = new HashSet<long>(_localDbService.GetAll(userId).Where(s => s.IsRead).Select(s => s.MsgId));
                for (int i = _allMessages.Count - 1; i >= 0; i--)
                {
                    if (_allMessages[i].IsRead)
                        _allMessages.RemoveAt(i);
                }
                RefreshMessageCounts();
                RefreshFilteredMessages();
            });

            DeleteMessageCommand = new DelegateCommand<MessageItemViewModel>(async item =>
            {
                if (item == null) return;
                var userId = _currentUserService.UserId;
                try
                {
                    _localDbService.DeleteMessage(userId, item.Message.MessageId);
                    _allMessages.Remove(item);
                    _readSet.Remove(item.Message.MessageId);
                    RefreshMessageCounts();
                    RefreshFilteredMessages();
                }
                catch (InvalidOperationException ex)
                {
                    await _dialogHostService.ShowMessageAsync(ex.Message, "提示");
                }
            });

            MessageClickCommand = new DelegateCommand<MessageItemViewModel>(async item =>
            {
                if (item == null) return;
                
                // 如果是未读消息，更新已读状态
                if (!item.IsRead)
                {
                    item.IsRead = true;
                    _readSet.Add(item.Message.MessageId);
                    _localDbService.MarkRead(_currentUserService.UserId, item.Message.MessageId);
                    RefreshMessageCounts();
                    RefreshFilteredMessages();
                }
                
                // 根据消息类型处理点击
                switch (item.Message.Category)
                {
                    case "notice":
                        await _dialogHostService.ShowDialogAsync<NoticeDetailView>(vm =>
                        {
                            var detailVm = (vm as NoticeDetailViewModel);
                            if (detailVm != null)
                            {
                                var notice = new SysNotice
                                {
                                    NoticeId = 0,
                                    NoticeTitle = item.Message.Title,
                                    NoticeContent = item.Message.Content ?? item.Message.Message,
                                    Status = "0",
                                    CreateBy = item.Message.CreateBy,
                                    CreateUser = item.Message.CreateUser
                                };
                                detailVm.SetNotice(notice);
                            }
                        });
                        break;
                    case "workflow":
                        if (!string.IsNullOrEmpty(item.Message.Path))
                        {
                            _regionManager.RequestNavigate(RegionNames.ContentRegion, item.Message.Path);
                        }
                        break;
                }
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
                        Log.Warning("[MenuSelection] Failed to load view for: {MenuName} ({Component})", m.MenuName, m.Component);
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

            _mqttService.OnKicked += reason =>
            {
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    _userRepository.UpdateOnlineStatus(_currentUserService.UserId, 0);
                    await _mqttService.StopAsync();
                    _currentUserService.Clear();
                    _regionManager.Regions[RegionNames.TabRegion].RemoveAll();
                    _regionManager.RequestNavigate(RegionNames.ContentRegion, nameof(Views.LoginView));
                    await _dialogHostService.ShowMessageAsync(reason, "系统提示");
                });
            };

            _mqttService.OnNewMessages += messages =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var userId = _currentUserService.UserId;
                    foreach (var msg in messages)
                    {
                        // 检查消息是否已存在，避免重复
                        if (!_allMessages.Any(m => m.Message.MessageId == msg.MessageId))
                        {
                            _localDbService.SaveMessage(userId, msg.MessageId);
                            _localDbService.SetLastSyncId(userId, msg.MessageId);
                            _readSet.Remove(msg.MessageId);
                            _allMessages.Insert(0, new MessageItemViewModel(msg, false));
                        }
                    }
                    RefreshMessageCounts();
                    RefreshFilteredMessages();
                });
            };

            LoadMessages();
        }

        private void LoadMessages()
        {
            var userId = _currentUserService.UserId;
            _localDbService.Init();

            var lastSyncId = _localDbService.GetLastSyncId(userId);
            var newMessages = _messageService.SelectNewMessages(lastSyncId, userId);
            if (newMessages != null && newMessages.Count > 0)
            {
                foreach (var msg in newMessages)
                    _localDbService.SaveMessage(userId, msg.MessageId);
                _localDbService.SetLastSyncId(userId, newMessages.Max(m => m.MessageId));
            }

            var localStatuses = _localDbService.GetAll(userId);
            var msgIds = localStatuses.Select(s => s.MsgId).ToList();
            var allMsgs = _messageService.SelectMessagesByIds(msgIds);
            _readSet = new HashSet<long>(localStatuses.Where(s => s.IsRead).Select(s => s.MsgId));
            _allMessages = new ObservableCollection<MessageItemViewModel>(
                (allMsgs ?? new List<SysMessage>()).Select(m => new MessageItemViewModel(m, _readSet.Contains(m.MessageId)))
            );

            RefreshMessageCounts();
            RefreshFilteredMessages();
        }

        private void RefreshMessageCounts()
        {
            SystemCount = _allMessages.Count(m => m.Message.Category == "system");
            NoticeCount = _allMessages.Count(m => m.Message.Category == "notice");
            WorkflowCount = _allMessages.Count(m => m.Message.Category == "workflow");
            HasUnreadMessage = _allMessages.Any(m => !m.IsRead);
        }

        private void RefreshFilteredMessages()
        {
            var category = _selectedMessageTab switch
            {
                1 => "system",
                2 => "notice",
                3 => "workflow",
                _ => null
            };
            var filtered = category != null
                ? _allMessages.Where(m => m.Message.Category == category).ToList()
                : _allMessages.ToList();
            FilteredMessages = new ObservableCollection<MessageItemViewModel>(filtered);
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
                    Log.Warning("[GetView] Type not found: {FullName}", fullName);
                    return null;
                }
                var view = ContainerLocator.Container.Resolve(viewType) as FrameworkElement;
                if (view == null)
                {
                    Log.Warning("[GetView] Resolve failed: {FullName}", fullName);
                    return null;
                }
                (view.DataContext as ITabItemInfo)?.Header = m.MenuName;
                (view.DataContext as ITabItemInfo)?.Component = m.Component;
                return view;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[GetView] Error: {Message}", ex.Message);
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

    public class MessageItemViewModel : BindableBase
    {
        private bool _isRead;

        public SysMessage Message { get; }

        public bool IsRead
        {
            get => _isRead;
            set => SetProperty(ref _isRead, value);
        }

        public MessageItemViewModel(SysMessage message, bool isRead = false)
        {
            Message = message;
            _isRead = isRead;
        }
    }
}