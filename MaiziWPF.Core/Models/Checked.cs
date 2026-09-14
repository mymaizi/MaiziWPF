using FreeSql.DataAnnotations;
using MaiziWPF.Services.Domain;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MaiziWPF.Core
{
    public class Checked: BindableBase
    {
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { SetProperty(ref _Name, value); }
        }
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }
        public bool IsChecked
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }
        private long _id;
        public long Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }
        private long _parentId;
        public long ParentId
        {
            get { return _parentId; }
            set { SetProperty(ref _parentId, value); }
        }
        private ObservableCollection<Checked> _childs;
        [Navigate(nameof(ParentId))]
        public ObservableCollection<Checked> Childs
        {
            get { return _childs; }
            set { SetProperty(ref _childs, value); }
        }
    }
}