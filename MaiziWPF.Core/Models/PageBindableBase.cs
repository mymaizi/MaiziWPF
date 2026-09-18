using MaiziWPF.Services.Domain.Shared;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Dialogs;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace MaiziWPF.Core
{
    public class PageBindableBase<T, T1> : BindableBase, ITabItemInfo where T1 : IPagingInfo, new()
    {
        public string Header { get; set; }
        public string Component { get; set; }
        private T1 _queryPageInfo;
        public T1 QueryPageInfo { get => _queryPageInfo; set => SetProperty(ref _queryPageInfo, value); }
        private int _pageNumber;
        public int PageNumber { get => _pageNumber; set => SetProperty(ref _pageNumber, value); }
        private int _pageSize;
        public int PageSize { get => _pageSize; set => SetProperty(ref _pageSize, value); }
        private long _count;
        public long Count { get => _count; set => SetProperty(ref _count, value); }
        private ObservableCollection<T> _dataList = new();
        public ObservableCollection<T> DataList { get => _dataList; set => SetProperty(ref _dataList, value); }
        private T _entity;
        public T Entity { get => _entity; set => SetProperty(ref _entity, value); }
        private bool _isBusy;
        public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }
        public ICommand SearchButtonCommand { get; set; }
        public ICommand PrevButtonCommand { get; set; }
        public ICommand NextButtonCommand { get; set; }
        public ICommand PageSizeChangedCommand { get; set; }
        public ICommand ResetButtonCommand { get; set; }
        public ICommand NewOrEditButtonCommand { get; set; }
        public ICommand DeleteButtonCommand { get; set; }

        public PageBindableBase()
        {
        }

        public void RegisterQueryFunc(Func<T1, List<T>> loadDataFunc, T1 t1,
            Action<T1> resetAction = null, Action<Exception> onError = null)
        {
            RegisterQueryFunc(input => Task.FromResult(loadDataFunc(input)), t1, resetAction, onError);
        }

        public void RegisterQueryFunc(Func<T1, Task<List<T>>> loadDataFuncAsync, T1 t1,
            Action<T1> resetAction = null, Action<Exception> onError = null)
        {
            QueryPageInfo = t1;

            SearchButtonCommand = new DelegateCommand(async () =>
            {
                QueryPageInfo.PageNumber = 1;
                await LoadDataAsync(loadDataFuncAsync, onError);
            });

            PrevButtonCommand = new DelegateCommand(async () =>
            {
                QueryPageInfo.PageNumber--;
                await LoadDataAsync(loadDataFuncAsync, onError);
            });

            NextButtonCommand = new DelegateCommand(async () =>
            {
                QueryPageInfo.PageNumber++;
                await LoadDataAsync(loadDataFuncAsync, onError);
            });

            PageSizeChangedCommand = new DelegateCommand<int?>(async size =>
            {
                if (size.HasValue && size.Value > 0)
                {
                    QueryPageInfo.PageSize = size.Value;
                    await LoadDataAsync(loadDataFuncAsync, onError);
                }
            });

            ResetButtonCommand = new DelegateCommand(() =>
            {
                resetAction?.Invoke(QueryPageInfo);
                SearchButtonCommand.Execute(this);
            });

            SearchButtonCommand.Execute(this);
        }

        #region 私有方法

        private async Task LoadDataAsync(Func<T1, Task<List<T>>> loadDataFuncAsync, Action<Exception> onError)
        {
            try
            {
                IsBusy = true;
                var data = await loadDataFuncAsync(QueryPageInfo);
                DataList = data.ToObservableCollection();
                PageNumber = QueryPageInfo.PageNumber;
                PageSize = QueryPageInfo.PageSize;
                Count = QueryPageInfo.Count;
            }
            catch (Exception ex)
            {
                if (onError != null)
                    onError(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion
    }
}