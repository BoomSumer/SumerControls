using SumerControls.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SumerControls.Controls
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SumerControls.Controls.MyDialog"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SumerControls.Controls.MyDialog;assembly=SumerControls.Controls.MyDialog"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误:
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:MyDialog/>
    ///
    /// </summary>
    [TemplatePart(Name = "BackElement", Type = typeof(Border))]
    public class MyDialog : ContentControl
    {
        static MyDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MyDialog), new FrameworkPropertyMetadata(typeof(MyDialog)));
        }

        #region 字段
        private const string BackElement = "PART_BackElement";

        private string? _token;

        private Border? _backElement;

        private AdornerContainer? _container;

        private TaskCompletionSource<object?> _tcs;




        private static readonly Dictionary<string, FrameworkElement> ContainerDic = new();

        private static readonly Dictionary<string, MyDialog> DialogDict = new();
        #endregion

        #region 附加属性 / 依赖属性

        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(MyDialog), new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty MaskCanCloseProperty = DependencyProperty.RegisterAttached("MaskCanClose", typeof(bool), typeof(MyDialog), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty MaskBrushProperty = DependencyProperty.Register("MaskBrush", typeof(Brush), typeof(MyDialog), new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty TokenProperty = DependencyProperty.RegisterAttached("Token", typeof(string), typeof(MyDialog), new PropertyMetadata(default(string), OnTokenChanged));


        #region 属性实例

        public bool IsOpen
        {
            get
            {
                return (bool)GetValue(IsOpenProperty);
            }
            internal set
            {
                SetValue(IsOpenProperty, value);
            }
        }

        public Brush MaskBrush
        {
            get
            {
                return (Brush)GetValue(MaskBrushProperty);
            }
            set
            {
                SetValue(MaskBrushProperty, value);
            }
        }

        public static void SetToken(DependencyObject element, string value)
        {
            element.SetValue(TokenProperty, value);
        }

        public static string? GetToken(DependencyObject element)
        {
            return (string)element.GetValue(TokenProperty);
        }

        public static void SetMaskCanClose(DependencyObject element, bool value)
        {
            element.SetValue(MaskCanCloseProperty, value);
        }

        public static bool GetMaskCanClose(DependencyObject element)
        {
            return (bool)element.GetValue(MaskCanCloseProperty);
        }

        #endregion

        #endregion

        #region 属性更改事件

        /// <summary>
        /// Token 值更改事件
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnTokenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement? frameworkElement = d as FrameworkElement;
            if (frameworkElement != null)
            {
                if (e.NewValue == null)
                {
                    Unregister(frameworkElement);
                }
                else
                {
                    Register(e.NewValue.ToString(), frameworkElement);
                }
            }
        }

        #endregion

        #region 注册 / 取消注册  方法

        /// <summary>
        /// 注册Dialog，
        /// </summary>
        /// <param name="token"></param>
        /// <param name="element"></param>
        public static void Register(string token, FrameworkElement element)
        {
            if (!string.IsNullOrEmpty(token) && element != null)
            {
                ContainerDic[token] = element;
            }
        }
        /// <summary>
        /// 取消注册 Dialog
        /// </summary>
        /// <param name="token"></param>
        /// <param name="element"></param>
        public static void Unregister(string token, FrameworkElement element)
        {
            if (!string.IsNullOrEmpty(token) && element != null && ContainerDic.ContainsKey(token) && ContainerDic[token] == element)
            {
                ContainerDic.Remove(token);
            }
        }

        public static void Unregister(UIElement element)
        {
            if (element != null)
            {
                KeyValuePair<string, FrameworkElement> keyValuePair = ContainerDic.FirstOrDefault((item) => element == item.Value);
                if (!string.IsNullOrEmpty(keyValuePair.Key))
                {
                    ContainerDic.Remove(keyValuePair.Key);
                }
            }
        }

        public static void Unregister(string token)
        {
            if (!string.IsNullOrEmpty(token) && ContainerDic.ContainsKey(token))
            {
                ContainerDic.Remove(token);
            }
        }

        #endregion

        #region Show 方法

        /// <summary>
        /// 打开遮罩对话框方法
        /// </summary>
        /// <param name="content"> 对话框中显示的内容 </param>
        /// <returns> 异步等待返回object类型的参数，为传递回的参数</returns>
        public static async Task<object?> Show(object content)
        {
            if (content is null)
            {
                return null;
            }
            return await Show(content, "");
        }
        /// <summary>
        /// 打开遮罩对话框方法
        /// </summary>
        /// <param name="content">对话框中显示的内容</param>
        /// <param name="token"> 与 附加属性Token 对应的参数，一般对应XAML文件中定义的Dialog.Token值</param>
        /// <returns>异步等待返回object类型的参数，为传递回的参数</returns>
        public static async Task<object?> Show(object content, string token)
        {
            // 实例化一个MyDialog
            MyDialog dialog = new MyDialog
            {
                _token = token,
                Content = content
            };

            FrameworkElement? _element;
            AdornerDecorator _adornerDecorator;
            if (string.IsNullOrEmpty(token))
            {
                // 当token值为空时  向字典类型添加一个虚构（key,value），用于关闭对话框时获取要关闭对话框的实例
                DialogDict[BackElement] = dialog;

                // 当token值为空时，获取当前活动的窗口实例
                _element = WindowHelper.GetActiveWindow();
            }
            else
            {
                dialog.Close();
                // 向字典类型 DialogDict 中添加一个键值对 (key,value)，用于关闭对话框时获取要关闭对话框的实例
                DialogDict[token] = dialog;

                //  通过token值，返回token所注册的元素根
                ContainerDic.TryGetValue(token, out _element);
            }
            if (_element is null)
            {
                return null;
            }

            //  获取 对应元素的 AdornerDecorator 
            _adornerDecorator = VisualHelper.GetChild<AdornerDecorator>(_element);

            if (_adornerDecorator != null)
            {
                if (_adornerDecorator.Child != null)
                {
                    _adornerDecorator.Child.IsEnabled = false;
                }
                else
                {
                    // 当获取到的 AdornerDecorator 没有子元素时，手动向AdornerDecorator 添加一个Grid子元素，用于显示遮罩层
                    Grid grid = new Grid();
                    _adornerDecorator.Child = grid;
                }

                AdornerLayer _adornerLayer;

                //  获取 AdornerDecorator 的 装饰器 AdornerLayer
                _adornerLayer = _adornerDecorator.AdornerLayer;

                if (_adornerLayer != null)
                {

                    //  将 继承自 Adorner 的自定义类 ，添加到 AdornerLayer 中，并将 当前对话框实例设置成其 子元素
                    AdornerContainer adorner = new AdornerContainer(_adornerLayer)
                    {
                        Child = dialog

                    };
                    dialog._container = adorner;
                    dialog.IsOpen = true;
                    _adornerLayer.Add(adorner);


                }
            }

            //  用于等待用户继续操作
            dialog._tcs = new TaskCompletionSource<object?>();
            return await dialog._tcs.Task;
        }

        #endregion


        #region Close  方法

        /// <summary>
        /// 关闭对话框
        /// </summary>
        /// <param name="token"> 关闭与之对应的 xaml中定义的Token的对话框</param>
        /// <param name="parameters"> 关闭时传递的参数 object 类型</param>
        public static void Close(string token, object parameters)
        {
            MyDialog? dialog;
            if (DialogDict.TryGetValue(token, out dialog) || DialogDict.TryGetValue(BackElement, out dialog))
            {
                dialog.CloseDialog(parameters);
                dialog.Close();
            }
        }
        // 关闭对话框时 配置要传递的参数
        private void CloseDialog(object parameters)
        {
            _tcs.SetResult(parameters);
        }

        private void Close()
        {
            if (string.IsNullOrEmpty(_token))
            {
                Close(WindowHelper.GetActiveWindow());
                DialogDict.Remove(BackElement);
            }
            else if (ContainerDic.TryGetValue(_token, out var element))
            {
                Close(element);
                DialogDict.Remove(_token);
            }
        }

        private void Close(FrameworkElement element)
        {
            if (element != null && _container != null)
            {
                var decorator = VisualHelper.GetChild<AdornerDecorator>(element);
                if (decorator != null)
                {
                    if (decorator.Child != null)
                    {
                        decorator.Child.IsEnabled = true;
                    }
                    var layer = decorator.AdornerLayer;
                    layer?.Remove(_container);
                    IsOpen = false;
                }
            }
        }

        #endregion


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _backElement = GetTemplateChild("BackElement") as Border;
        }
    }
}
