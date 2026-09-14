using FreeSql.Internal.Model;
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
using System.Windows.Controls;
using System.Windows.Input;

namespace MaiziWPF.Core
{
    public class PageBindableBase<T,T1> : BindableBase, ITabItemInfo where T1 : class,new()
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
        private ObservableCollection<T> _dataList=new();
        public ObservableCollection<T> DataList { get => _dataList; set => SetProperty(ref _dataList, value); }
        private T _entity;
        public T Entity { get => _entity; set => SetProperty(ref _entity, value); }
        public ICommand SearchButtonCommand { get; set; }
        public ICommand PrevButtonCommand { get; set; }
        public ICommand NextButtonCommand { get; set; }
        public ICommand NewOrEditButtonCommand { get; set; }
        public ICommand DeleteButtonCommand { get; set; }
        public PageBindableBase()
        {
        }
        /// <summary>
        /// 注册查询函数
        /// </summary>
        /// <param name="loadDataFunc">查询数据接口</param>
        /// <param name="t1">分页对象</param>
        public void RegisterQueryFunc(Func<T1, List<T>> loadDataFunc, T1 t1)
        {
            QueryPageInfo = t1;
            SearchButtonCommand = new DelegateCommand(() =>
            {
                (QueryPageInfo as BasePagingInfo).PageNumber=1;
                LoadData(loadDataFunc);
            });
            PrevButtonCommand = new DelegateCommand(() =>
            {
                (QueryPageInfo as BasePagingInfo).PageNumber--;
                LoadData(loadDataFunc);
            });
            NextButtonCommand = new DelegateCommand(() =>
            {
                (QueryPageInfo as BasePagingInfo).PageNumber++;
                LoadData(loadDataFunc);
            });
        }
        #region 私有方法
        /// <summary>
        /// 加载数据列表
        /// </summary>
        public virtual void LoadDataList()
        {
        }

        /// <summary>
        /// 加载分页数据
        /// </summary>
        /// <param name="loadDataFunc">业务查询逻辑</param>
        private void LoadData(Func<T1, List<T>> loadDataFunc)
        {
            DataList.Clear();
            var data = loadDataFunc(QueryPageInfo).ToObservableCollection();
            DataList.AddRange(data);
            var queryPageInfo = QueryPageInfo as BasePagingInfo;
            Count = queryPageInfo.Count;
            PageNumber = queryPageInfo.PageNumber;
            PageSize = queryPageInfo.PageSize;
        }
        #endregion
    }
}