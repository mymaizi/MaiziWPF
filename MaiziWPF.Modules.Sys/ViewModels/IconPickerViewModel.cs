using MaiziWPF.Core;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MaiziWPF.Modules.Sys
{
    public class IconPickerViewModel : FormBindableBase
    {
        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                SetProperty(ref _searchText, value);
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

        public ObservableCollection<string> AllIcons { get; } = new();
        public ObservableCollection<string> FilteredIcons { get; } = new();

        public IconPickerViewModel(ISnackbarService snackbarService)
            : base(snackbarService)
        {
            LoadIcons();
            FilterIcons();

            SelectIconCommand = new DelegateCommand<string>(icon =>
            {
                SelectedIcon = icon;
            });

            UseManualCommand = new DelegateCommand(() =>
            {
                if (!string.IsNullOrWhiteSpace(ManualInput))
                {
                    SelectedIcon = ManualInput.Trim();
                    AcceptCommand.Execute(null);
                }
            });

            ConfirmCommand = new DelegateCommand(() =>
            {
                if (!string.IsNullOrEmpty(SelectedIcon))
                {
                    AcceptCommand.Execute(null);
                }
            });

            CancelCommand = new DelegateCommand(() =>
            {
                AcceptCommand.Execute(null);
            });
        }

        private void LoadIcons()
        {
            var kinds = Enum.GetValues(typeof(MaterialDesignThemes.Wpf.PackIconKind))
                .Cast<MaterialDesignThemes.Wpf.PackIconKind>();
            foreach (var kind in kinds)
            {
                AllIcons.Add(kind.ToString());
            }
        }

        private void FilterIcons()
        {
            FilteredIcons.Clear();
            var query = SearchText?.Trim().ToLower();
            var filtered = string.IsNullOrEmpty(query)
                ? AllIcons
                : AllIcons.Where(x => x.ToLower().Contains(query));
            foreach (var icon in filtered)
            {
                FilteredIcons.Add(icon);
            }
        }

        public DelegateCommand<string> SelectIconCommand { get; }
        public DelegateCommand UseManualCommand { get; }
        public DelegateCommand ConfirmCommand { get; }
        public DelegateCommand CancelCommand { get; }
    }
}