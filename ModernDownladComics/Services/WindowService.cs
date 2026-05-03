using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Runtime.InteropServices;
using Windows.Graphics;
using WinRT.Interop;

namespace ModernDownloadComics.Services
{
    public partial class WindowService
    {
        private static readonly Lazy<WindowService> _instance = new(new WindowService());
        public static readonly WindowService Instance = _instance.Value;

        private Window? _owner;

        public void InitOwner(Window? owner)
        {
            _owner = owner;
        }

        public Window? GetOwner()
        {
            return _owner;
        }

        public void SetOwner(Window window)
        {
            if (_owner != null)
            {
                IntPtr ownerHandle = GetHandler(_owner);
                IntPtr ownedHandle = GetHandler(window);

                if (IntPtr.Size == 8)
                {
                    SetWindowLongPtr(ownedHandle, -8, ownerHandle);
                }
                else
                {
                    SetWindowLong(ownedHandle, -8, ownerHandle);
                }
            }

        }
        

        public void Center(Window window)
        {
            if (_owner != null)
            {
                AppWindow appWindow = AppWindow.GetFromWindowId(Win32Interop.GetWindowIdFromWindow(GetHandler(_owner)));
                SizeInt32 ownerSize = appWindow.Size;
                PointInt32 ownerPos = appWindow.Position;

                int x = ownerPos.X + (ownerSize.Width - window.AppWindow.Size.Width) / 2;
                int y = ownerPos.Y + (ownerSize.Height - window.AppWindow.Size.Height) / 2;
                window.AppWindow.Move(new(x, y));
            }

        }

        public static IntPtr GetHandler(Window window)
        {
            return WindowNative.GetWindowHandle(window);
        }

        [DllImport("User32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("User32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLong")]
        private static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);


    }
}
