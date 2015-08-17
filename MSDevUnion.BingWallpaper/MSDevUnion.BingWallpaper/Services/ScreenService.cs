using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace MSDevUnion.BingWallpaper.Services
{
    public class ScreenService : IScreenService
    {
        public async Task<int> GetScreenHeightAsync()
        {
            WebView webView = new WebView(WebViewExecutionMode.SeparateThread);
            string strWidth = await webView.InvokeScriptAsync("eval", new string[] { "window.screen.width.toString()" });
            int iWidth;
            int.TryParse(strWidth, out iWidth);
            return iWidth;
        }

        public async Task<int> GetScreenWidthAsync()
        {
            WebView webView = new WebView(WebViewExecutionMode.SeparateThread);
            string strHeight = await webView.InvokeScriptAsync("eval", new string[] { "window.screen.height.toString()" });
            int iHeight;
            int.TryParse(strHeight, out iHeight);
            return iHeight;
        }
    }
}