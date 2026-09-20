using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MaiziWPF.Common
{
    public static class TreeListHelper
    {
        private static readonly Dictionary<Type, PropertyInfo> _levelPropCache = new();

        /// <summary>
        /// 基于父子关系的平面列表构建树形列表
        /// </summary>
        /// <typeparam name="T">树形对象</typeparam>
        /// <param name="flatList">平面列表</param>
        /// <param name="idSelector">id选择器</param>
        /// <param name="parentIdSelector">父级id选择器</param>
        /// <param name="childsSetter">子级添加器(T1为父级,T2为子级),把子级向父级添加</param>
        /// <param name="childsGetter">子级获取器,提供后将自动递归设置Level属性（实体需有int Level {get;set;}）</param>
        /// <returns></returns>
        public static List<T> BuildTreeList<T>(
            this List<T> flatList,
            Func<T, long> idSelector,
            Func<T, long> parentIdSelector,
            Action<T, T> childsSetter,
            Func<T, IEnumerable<T>> childsGetter = null) where T : class, new()
        {
            if (flatList == null || flatList.Count == 0)
            {
                return new List<T>();
            }

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
                    childsSetter(parentNode, node);
                }
            }

            if (childsGetter != null)
            {
                SetTreeLevel(rootNodes, 1, childsGetter);
            }

            return rootNodes;
        }

        private static void SetTreeLevel<T>(List<T> nodes, int level, Func<T, IEnumerable<T>> childsGetter)
        {
            if (nodes == null) return;

            var levelProp = GetLevelProperty(typeof(T));

            foreach (var node in nodes)
            {
                if (levelProp != null)
                {
                    levelProp.SetValue(node, level);
                }

                var children = childsGetter(node);
                if (children != null)
                {
                    var childList = children as IReadOnlyCollection<T> ?? children.ToList();
                    if (childList.Count > 0)
                    {
                        SetTreeLevel(childList as List<T> ?? childList.ToList(), level + 1, childsGetter);
                    }
                }
            }
        }

        private static PropertyInfo GetLevelProperty(Type type)
        {
            if (!_levelPropCache.TryGetValue(type, out var prop))
            {
                prop = type.GetProperty("Level");
                if (prop != null && (!prop.CanWrite || prop.PropertyType != typeof(int)))
                {
                    prop = null;
                }
                _levelPropCache[type] = prop;
            }
            return prop;
        }
    }
}