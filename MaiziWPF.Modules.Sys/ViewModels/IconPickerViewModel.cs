using MaiziWPF.Core;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MaiziWPF.Modules.Sys
{
    public class IconPickerViewModel : FormBindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        
        private bool _isOpen;
        public bool IsOpen
        {
            get => _isOpen;
            set => SetProperty(ref _isOpen, value);
        }

        private bool _isLoading = true;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                SetProperty(ref _searchText, value);
                _loadedCount = PageSize;
                FilterIcons();
            }
        }

        private string _manualInput;
        public string ManualInput
        {
            get { return _manualInput; }
            set { SetProperty(ref _manualInput, value); }
        }

        private string _selectedIcon;
        public string SelectedIcon
        {
            get { return _selectedIcon; }
            set { SetProperty(ref _selectedIcon, value); }
        }

        private readonly List<string> _allIconsList = new();
        public ObservableCollection<string> FilteredIcons { get; } = new();

        private int _loadedCount;
        private const int PageSize = 35;

        public DelegateCommand<string> SelectIconCommand { get; }
        public DelegateCommand UseManualCommand { get; }
        public DelegateCommand<ScrollChangedEventArgs> ScrollChangedCommand { get; }

        public IconPickerViewModel(ISnackbarService snackbarService, IEventAggregator eventAggregator)
            : base(snackbarService)
        {
            _eventAggregator = eventAggregator;
            _loadedCount = PageSize;
            LoadIconsAsync();

            SelectIconCommand = new DelegateCommand<string>(icon =>
            {
                SelectedIcon = icon;
                IsOpen = false;
            });

            UseManualCommand = new DelegateCommand(() =>
            {
                if (!string.IsNullOrWhiteSpace(ManualInput))
                {
                    SelectedIcon = ManualInput.Trim();
                    IsOpen = false;
                }
            });

            ScrollChangedCommand = new DelegateCommand<ScrollChangedEventArgs>(e =>
            {
                if (e.VerticalOffset + e.ViewportHeight >= e.ExtentHeight - 50)
                {
                    int totalCount;
                    lock (_allIconsList)
                    {
                        totalCount = _allIconsList.Count;
                    }

                    if (_loadedCount < totalCount)
                    {
                        _loadedCount += PageSize;
                        if (_loadedCount > totalCount)
                            _loadedCount = totalCount;
                        FilterIcons();
                    }
                }
            });
        }

        private async void LoadIconsAsync()
        {
            IsLoading = true;

            await Task.Run(() =>
            {
                var kinds = Enum.GetValues(typeof(MaterialDesignThemes.Wpf.PackIconKind))
                    .Cast<MaterialDesignThemes.Wpf.PackIconKind>()
                    .Select(k => k.ToString())
                    .ToList();

                lock (_allIconsList)
                {
                    _allIconsList.Clear();
                    _allIconsList.AddRange(kinds);
                }
            });

            Application.Current.Dispatcher.Invoke(() =>
            {
                IsLoading = false;
                FilterIcons();
            });
        }

        private void FilterIcons()
        {
            FilteredIcons.Clear();
            var query = SearchText?.Trim().ToLower();

            List<string> source;
            lock (_allIconsList)
            {
                if (string.IsNullOrEmpty(query))
                {
                    source = _allIconsList.Take(_loadedCount).ToList();
                }
                else
                {
                    source = _allIconsList
                        .Where(x => x.ToLower().Contains(query))
                        .ToList();
                }
            }

            foreach (var icon in source)
            {
                FilteredIcons.Add(icon);
            }
        }
    }
}