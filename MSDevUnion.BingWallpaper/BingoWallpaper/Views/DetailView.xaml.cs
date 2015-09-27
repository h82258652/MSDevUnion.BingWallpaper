using BingoWallpaper.Datas;
using BingoWallpaper.Models;
using BingoWallpaper.ViewModels;
using SoftwareKobo.UniversalToolkit.Extensions;
using SoftwareKobo.UniversalToolkit.Helpers;
using SoftwareKobo.UniversalToolkit.Services.LauncherServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.System.UserProfile;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Documents;
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

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Control control = sender as Control;
            if (control != null)
            {
                control.IsEnabled = false;
            }

            SaveLocation saveLocation = AppSetting.SaveLocation;
            switch (saveLocation)
            {
                case SaveLocation.PictureLibrary:
                    await SaveToPictureLibrary();
                    break;
                case SaveLocation.ChooseEveryTime:
                    await SaveToChooseLocation();
                    break;
                case SaveLocation.SavedPictures:
                    await SaveToSavedPictures();
                    break;
                default:
                    break;
            }

            if (control != null)
            {
                control.IsEnabled = true;
            }
        }

        private async Task SaveToChooseLocation()
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.FileTypeChoices.Add(".jpg", new List<string>() { ".jpg" });
            savePicker.SuggestedFileName = Path.GetFileNameWithoutExtension(ViewModel.Wallpaper.Image.UrlBase) + ".jpg";
            savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                await SaveFile(file);
            }
        }

        private async Task SaveToPictureLibrary()
        {
            try
            {
                StorageFile file = await KnownFolders.PicturesLibrary.CreateFileAsync(Path.GetFileNameWithoutExtension(ViewModel.Wallpaper.Image.UrlBase) + ".jpg", CreationCollisionOption.ReplaceExisting);
                await SaveFile(file);
            }
            catch
            {
                await new MessageDialog("保存失败").ShowAsync();
            }
        }

        private async Task SaveToSavedPictures()
        {
            try
            {
                StorageFile file = await KnownFolders.SavedPictures.CreateFileAsync(Path.GetFileNameWithoutExtension(ViewModel.Wallpaper.Image.UrlBase) + ".jpg", CreationCollisionOption.ReplaceExisting);
                await SaveFile(file);
            }
            catch
            {
                await new MessageDialog("保存失败").ShowAsync();
            }
        }

        private async Task SaveFile(StorageFile file)
        {
            string url = ViewModel.Wallpaper.GetCacheUrl(AppSetting.WallpaperSize);
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    await FileIO.WriteBufferAsync(file, await client.GetBufferAsync(new Uri(url)));
                }
                await new MessageDialog("保存成功").ShowAsync();
            }
            catch
            {
                await new MessageDialog("保存失败").ShowAsync();
            }
        }

        private async void BtnSetLockScreen_Click(object sender, RoutedEventArgs e)
        {
            Control control = sender as Control;
            if (control != null)
            {
                control.IsEnabled = false;
            }

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

            if (control != null)
            {
                control.IsEnabled = true;
            }
        }

        private async void BtnSetWallpaper_Click(object sender, RoutedEventArgs e)
        {
            Control control = sender as Control;
            if (control != null)
            {
                control.IsEnabled = false;
            }

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

            if (control != null)
            {
                control.IsEnabled = true;
            }
        }

        private async void BtnShare_Click(object sender, RoutedEventArgs e)
        {
         //   Popup popup = new Popup();
         //   popup.IsLightDismissEnabled = true;
         //   popup.Width = Window.Current.Bounds.Width;
         //   popup.Width = Window.Current.Bounds.Height;

         //Grid.

            //await new ContentDialog().ShowAsync();

            //AppBar.Visibility = Visibility.Collapsed;

            //await Task.Delay(5000);

            //AppBar.Visibility = Visibility.Visible;
            // SharePopup.IsOpen = true;
        }

        /// <summary>
        /// 获取当前壁纸文件。该方法用于壁纸设置和锁屏。
        /// </summary>
        /// <returns></returns>
        private async Task<StorageFile> GetWallpaperFile()
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(Guid.NewGuid().ToString());
            using (HttpClient client = new HttpClient())
            {
                string url = ViewModel.Wallpaper.GetCacheUrl(AppSetting.WallpaperSize);
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

        private async void BtnOpenDeviceWallpaperSetting_Click(object sender, RoutedEventArgs e)
        {
            SystemSettingsService service = new SystemSettingsService();
            await service.OpenPersonalizationPageAsync();
        }

        private async void BtnOpenDeviceLockScreenSetting_Click(object sender, RoutedEventArgs e)
        {
            SystemSettingsService service = new SystemSettingsService();
            await service.OpenLockScreenPageAsync();
        }

        private async void HotspotClick(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            var textBlock = sender.GetAncestorsOfType<TextBlock>().First();
            var hotspot = textBlock.DataContext as Hotspot;
            if (hotspot != null)
            {
                var uri = new Uri(hotspot.Link);
                await Launcher.LaunchUriAsync(uri);
            }
        }
    }
}