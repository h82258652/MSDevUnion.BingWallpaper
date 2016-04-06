using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Graphics.Display;
using Windows.UI.Xaml.Controls;

namespace BingoWallpaper.Utils
{
    /// <summary>
    /// 最适合本设备的壁纸大小。
    /// </summary>
    public class SuitableWallpaperSize
    {
        private static bool _hadInit;
        private static int _height;
        private static int _width;

        /// <summary>
        /// 获取最适合本设备的壁纸的高度。
        /// </summary>
        public static int Height
        {
            get
            {
                if (_hadInit == false)
                {
                    throw new InvalidOperationException();
                }
                return _height;
            }
        }

        /// <summary>
        /// 获取最适合本设备的壁纸的宽度。
        /// </summary>
        public static int Width
        {
            get
            {
                if (_hadInit == false)
                {
                    throw new InvalidOperationException();
                }
                return _width;
            }
        }

        public static async Task InitAsync()
        {
            if (DesignMode.DesignModeEnabled == false)
            {
                WebView webView = new WebView(WebViewExecutionMode.SeparateThread);
                int width = int.Parse(await webView.InvokeScriptAsync("eval", new[] { "window.screen.width.toString()" }));
                int height = int.Parse(await webView.InvokeScriptAsync("eval", new[] { "window.screen.height.toString()" }));
                double scale = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
                _width = (int)Math.Ceiling(width * scale);
                _height = (int)Math.Ceiling(height * scale);
            }
            _hadInit = true;
        }
    }
}