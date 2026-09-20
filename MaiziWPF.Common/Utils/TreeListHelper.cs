using System;
using System.Collections.Generic;
using System.Text;

namespace MaiziWPF.Common
{
    public static class TreeListHelper
    {
        /// <summary>
        ///  基于父子关系的平面列表构建树形列表
        /// </summary>
        /// <typeparam name="T">树形对象</typeparam>
        /// <param name="flatList">平面列表</param>
        /// <param name="idSelector">字典id选择器</param>
        /// <param name="parentIdSelector">父级id选择器</param>
        /// <param name="childsSetter">子级设置(T1为父级,T2为子级),把子级向父级添加</param>
        /// <returns></returns>
        public static List<T> BuildTreeList<T>(this List<T> flatList, Func<T, long> idSelector,Func<T, long> parentIdSelector, Action<T, T> childsSetter) where T : class, new()
        {
            if (flatList == null || flatList.Count == 0)
                return new List<T>();
            var nodeDict = flatList.ToDictionary(idSelector, node => node);
            var rootNodes = new List<T>();
            foreach (var node in flatList)
            {
                long parentId = parentIdSelector(node);
                if (parentId == 0)
                {
                    rootNodes.Add(node);
                    continue;
                }
                if (nodeDict.TryGetValue(parentId, out var parentNode))
                {
                    childsSetter(parentNode,node);
                }
            }
            return rootNodes;
        }
    }
}