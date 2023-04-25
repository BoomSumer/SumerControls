using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows;

namespace SumerControls.Tools
{
    public static class WindowHelper
    {
        //
        // 摘要:
        //     获取当前应用中处于激活的一个窗口
        public static Window GetActiveWindow()
        {
            IntPtr activeWindow = GetIntPtrActiveWindow();
            return Application.Current.Windows.OfType<Window>().FirstOrDefault((Window x) => x.GetHandle() == activeWindow);
        }

        public static IntPtr CreateHandle()
        {
            return new WindowInteropHelper(new Window()).EnsureHandle();
        }

        public static IntPtr GetHandle(this Window window)
        {
            return new WindowInteropHelper(window).EnsureHandle();
        }

        public static HwndSource GetHwndSource(this Window window)
        {
            return HwndSource.FromHwnd(window.GetHandle());
        }

        [DllImport("user32.dll")]
        internal static extern IntPtr GetIntPtrActiveWindow();
    }
}
