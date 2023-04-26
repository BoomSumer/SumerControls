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
    
    public class MessageTool : Control
    {
        private bool _isAutoCloseTime;
        static MessageTool()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MessageTool), new FrameworkPropertyMetadata(typeof(MessageTool)));


        }

        public static readonly DependencyProperty AutoCloseTimeProperty = DependencyProperty.Register(
            "AutoCloseTime",
            typeof(int),
            typeof(MessageTool),
            new PropertyMetadata(0));

        public int AutoCloseTime
        {
            get { return (int)GetValue(AutoCloseTimeProperty); }
            set { SetValue(AutoCloseTimeProperty, value); }
        }



        public static readonly DependencyProperty VisibilitysProperty = DependencyProperty.Register(
            "Visibilitys",
            typeof(Visibility),
            typeof(MessageTool),
            new PropertyMetadata(Visibility.Collapsed));

        public Visibility Visibilitys
        {
            get { return (Visibility)GetValue(VisibilitysProperty); }
            set { SetValue(VisibilitysProperty, value); }
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon",
            typeof(string),
            typeof(MessageTool),
            new PropertyMetadata("", OnIconChanged));

        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            "Message",
            typeof(string),
            typeof(MessageTool),
            new PropertyMetadata(""));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty TokenProperty = DependencyProperty.Register(
            "Token",
            typeof(string),
            typeof(MessageTool),
            new PropertyMetadata(""));

        public string Token
        {
            get { return (string)GetValue(TokenProperty); }
            set { SetValue(TokenProperty, value); }
        }
        /// <summary>
        /// 通知消息显示
        /// </summary>
        /// <param name="iconNumber">枚举编号 1 = 正确， 2 = 错误</param>
        /// <param name="message">通知消息</param>
        /// <param name="token">视图标记</param>
        /// <param name="autoCloseTime">自动关闭计时，默认 = 0 （不关闭）</param>
        public static void show(int iconNumber, string message, string token, int autoCloseTime = 0)
        {
            var view = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            if (view == null)
            {
                view = Application.Current.MainWindow;
            }

            var messageTool = FindMessageTool(view, token);
            if (messageTool != null)
            {
                messageTool.Icon = IconSou(iconNumber);
                messageTool.Message = message;
                messageTool.Visibilitys = Visibility.Visible;
                messageTool._isAutoCloseTime = autoCloseTime > 0;
                if (messageTool._isAutoCloseTime)
                {
                    messageTool.StartAutoClose(autoCloseTime);
                }
            }



        }

        private static string IconSou(int iconNumber)
        {
            if (iconNumber == 1)
            {
                return "pack://application:,,,/Member.Management.Client.Assets;component/SysImage/Success.png";
            }
            if (iconNumber == 2)
            {
                return "pack://application:,,,/Member.Management.Client.Assets;component/SysImage/Error.png";
            }
            return null;
        }


        private async void StartAutoClose(int autoCloseTime)
        {
            await Task.Delay(autoCloseTime * 1000);
            Visibilitys = Visibility.Collapsed;
        }

        private static MessageTool FindMessageTool(DependencyObject parent, string token)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                var messageTool = child as MessageTool;
                if (messageTool != null && messageTool.Token == token)
                {
                    return messageTool;
                }

                // 递归查找
                var result = FindMessageTool(child, token);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        private Image IconImage;
        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var messageTool = d as MessageTool;
            if (messageTool != null)
            {
                messageTool.IconImage.Source = new BitmapImage(new Uri((string)e.NewValue, UriKind.RelativeOrAbsolute));
            }
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            IconImage = GetTemplateChild("PART_IconImage") as Image;
        }
    }
}
