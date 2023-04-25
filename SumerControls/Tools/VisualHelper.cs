using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows;

namespace SumerControls.Tools
{
    public static class VisualHelper
    {
        /// <summary>
        /// 获取视觉树分组的状态
        /// </summary>
        /// <param name="d"> 依赖对象</param>
        /// <param name="groupName"> 分组名称</param>
        /// <returns></returns>
        internal static VisualStateGroup TryGetVisualStateGroup(DependencyObject d, string groupName)
        {
            FrameworkElement implementationRoot = GetImplementationRoot(d);
            if (implementationRoot == null)
            {
                return null;
            }

            return VisualStateManager.GetVisualStateGroups(implementationRoot)?.OfType<VisualStateGroup>().FirstOrDefault((VisualStateGroup group) => string.CompareOrdinal(groupName, group.Name) == 0);
        }

        /// <summary>
        /// 获取实现根
        /// </summary>
        /// <param name="d"></param>
        /// <returns> 返回UI框架元素</returns>
        internal static FrameworkElement GetImplementationRoot(DependencyObject d)
        {
            if (1 != VisualTreeHelper.GetChildrenCount(d))
            {
                return null;
            }

            return VisualTreeHelper.GetChild(d, 0) as FrameworkElement;
        }
        /// <summary>
        /// 获取子元素
        /// </summary>
        /// <typeparam name="T">子元素定义类</typeparam>
        /// <param name="d">依赖对象</param>
        /// <returns>返回对象为 依赖对象类型 的子元素</returns>
        public static T GetChild<T>(DependencyObject d) where T : DependencyObject
        {
            if (d == null)
            {
                return default;
            }


            if (d is T t)
            {
                return t;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(d); i++)
            {
                T child = GetChild<T>(VisualTreeHelper.GetChild(d, i));
                if (child != null)
                {
                    return child;
                }
            }

            return null;
        }
        /// <summary>
        /// 获取父元素
        /// </summary>
        /// <typeparam name="T">父元素定义类</typeparam>
        /// <param name="d">依赖对象</param>
        /// <returns>返回对象为 依赖对象类型 的父元素</returns>
        public static T GetParent<T>(DependencyObject d) where T : DependencyObject
        {
            if (d != null)
            {
                T val = d as T;
                if (val == null)
                {
                    if (d is Window)
                    {
                        return null;
                    }

                    return GetParent<T>(VisualTreeHelper.GetParent(d));
                }

                return val;
            }

            return null;
        }

        public static IntPtr GetHandle(this Visual visual)
        {
            return (PresentationSource.FromVisual(visual) as HwndSource)?.Handle ?? IntPtr.Zero;
        }

        internal static void HitTestVisibleElements(Visual visual, HitTestResultCallback resultCallback, HitTestParameters parameters)
        {
            VisualTreeHelper.HitTest(visual, ExcludeNonVisualElements, resultCallback, parameters);
        }

        private static HitTestFilterBehavior ExcludeNonVisualElements(DependencyObject potentialHitTestTarget)
        {
            if (!(potentialHitTestTarget is Visual))
            {
                return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
            }

            UIElement uIElement = potentialHitTestTarget as UIElement;
            if (uIElement == null || (uIElement.IsVisible && uIElement.IsEnabled))
            {
                return HitTestFilterBehavior.Continue;
            }

            return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
        }


    }
}
