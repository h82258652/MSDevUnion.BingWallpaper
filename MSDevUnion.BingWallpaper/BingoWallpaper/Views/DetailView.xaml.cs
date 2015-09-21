using BingoWallpaper.Datas;
using BingoWallpaper.Models;
using BingoWallpaper.ViewModels;
using SoftwareKobo.UniversalToolkit.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System.UserProfile;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

namespace BingoWallpaper.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DetailView : Page
    {
        private long? _listenAccentColorChangedToken;

        public DetailView()
        {
            this.InitializeComponent();
            this.ListenAccentColorChanged();
        }

        public DetailViewModel ViewModel
        {
            get
            {
                return (DetailViewModel)this.DataContext;
            }
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.Frame.UnRegisterNavigateBack();

            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.Frame.RegisterNavigateBack();

            this.ViewModel.Wallpaper = e.Parameter as Wallpaper;

            base.OnNavigatedTo(e);
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void BtnSetLockScreen_Click(object sender, RoutedEventArgs e)
        {
            bool isSuccess = false;
            if (UserProfilePersonalizationSettings.IsSupported())
            {
                try
                {
                    StorageFile file = await GetWallpaperFile();
                    isSuccess = await UserProfilePersonalizationSettings.Current.TrySetLockScreenImageAsync(file);
                }
                catch
                {
                }
            }
            if (isSuccess)
            {
                await new MessageDialog("设置成功").ShowAsync();
            }
            else
            {
                await new MessageDialog("设置失败").ShowAsync();
            }
        }

        private async void BtnSetWallpaper_Click(object sender, RoutedEventArgs e)
        {
            bool isSuccess = false;
            if (UserProfilePersonalizationSettings.IsSupported())
            {
                try
                {
                    StorageFile file = await GetWallpaperFile();
                    isSuccess = await UserProfilePersonalizationSettings.Current.TrySetWallpaperImageAsync(file);
                }
                catch
                {
                }
            }
            if (isSuccess)
            {
                await new MessageDialog("设置成功").ShowAsync();
            }
            else
            {
                await new MessageDialog("设置失败").ShowAsync();
            }
        }

        private void BtnShare_Click(object sender, RoutedEventArgs e)
        {

        }

        private async Task<StorageFile> GetWallpaperFile()
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(Guid.NewGuid().ToString());
            using (HttpClient client = new HttpClient())
            {
                string size = AppSetting.WallpaperSize.ToString();
                string url = "http://www.bing.com" + this.ViewModel.Wallpaper.Image.UrlBase + "_" + size + ".jpg";
                await FileIO.WriteBufferAsync(file, await client.GetBufferAsync(new Uri(url)));
            }
            return file;
        }

        /// <summary>
        /// 使用 hack 方法监听主题色发生变化。
        /// </summary>
        private void ListenAccentColorChanged()
        {
            SolidColorBrush appBarBackgroundBrush = this.AppBar.Background as SolidColorBrush;
            if (appBarBackgroundBrush != null)
            {
                this.Loaded += (sender, e) =>
                {
                    _listenAccentColorChangedToken = appBarBackgroundBrush.RegisterPropertyChangedCallback(SolidColorBrush.ColorProperty, async (obj, property) => await App.SetTitleBar());
                };
                this.Unloaded += (sender, e) =>
                {
                    if (this._listenAccentColorChangedToken.HasValue)
                    {
                        appBarBackgroundBrush.UnregisterPropertyChangedCallback(SolidColorBrush.ColorProperty, _listenAccentColorChangedToken.Value);
                    }
                };
            }
        }

        private void ImageOpened(object sender, RoutedEventArgs e)
        {
            this.LoadingRing.IsActive = false;
        }
    }
}
