using BingoWallpaper.Datas;
using BingoWallpaper.Models;
using BingoWallpaper.ViewModels;
using MicroMsg;
using MicroMsg.sdk;
using SoftwareKobo.Social.Sina.Weibo;
using SoftwareKobo.Social.Sina.Weibo.Models;
using SoftwareKobo.UniversalToolkit.Extensions;
using SoftwareKobo.UniversalToolkit.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System;
using Windows.System.UserProfile;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

namespace BingoWallpaper.Views
{
    public sealed partial class DetailView : Page
    {
        public DetailView()
        {
            this.InitializeComponent();
            this.InitSystemShare();
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
            base.OnNavigatedFrom(e);

            NavigationHelper.Unregister(this.Frame);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            NavigationHelper.Register(this.Frame, () =>
            {
                if (Frame.CanGoBack && PopupExecuting.IsOpen == false)
                {
                    Frame.GoBack();
                }
            });

            this.ViewModel.Wallpaper = e.Parameter as Wallpaper;
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            PopupExecuting.IsOpen = true;

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
            }

            PopupExecuting.IsOpen = false;
        }

        private async void BtnSetLockScreen_Click(object sender, RoutedEventArgs e)
        {
            PopupExecuting.IsOpen = true;

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

            await new MessageDialog(isSuccess ? LocalizedStrings.SetSuccess : LocalizedStrings.SetFailed).ShowAsyncEnqueue();

            PopupExecuting.IsOpen = false;
        }

        private async void BtnSetWallpaper_Click(object sender, RoutedEventArgs e)
        {
            PopupExecuting.IsOpen = true;

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

            await new MessageDialog(isSuccess ? LocalizedStrings.SetSuccess : LocalizedStrings.SetFailed).ShowAsyncEnqueue();

            PopupExecuting.IsOpen = false;
        }

        private void BtnShare_Click(object sender, RoutedEventArgs e)
        {
            PopupShare.IsOpen = true;
        }

        private async Task<byte[]> GetImageData()
        {
            using (HttpClient client = new HttpClient())
            {
                return (await client.GetBufferAsync(new Uri(ViewModel.Wallpaper.GetCacheUrl(AppSetting.WallpaperSize)))).ToArray();
            }
        }

        private string GetImageTitle()
        {
            return ViewModel.Wallpaper.Archive.Info;
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

        private async void HotspotClick(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            // 获取父级的 TextBlock。
            var textBlock = sender.GetAncestorsOfType<TextBlock>().First();

            // 获取点击的热点。
            var hotspot = textBlock.DataContext as Hotspot;
            if (hotspot != null)
            {
                var uri = new Uri(hotspot.Link);
                await Launcher.LaunchUriAsync(uri);

                // 获取子级的 Run。
                var runs = sender.Inlines.OfType<Run>();
                foreach (var run in runs)
                {
                    // 设置为紫色，表示已经访问过。
                    run.Foreground = new SolidColorBrush(Colors.Purple);
                }
            }
        }

        private void InitSystemShare()
        {
            DataTransferManager.GetForCurrentView().DataRequested += (DataTransferManager sender, DataRequestedEventArgs args) =>
            {
                DataRequest request = args.Request;
                var deferral = request.GetDeferral();
                request.Data.Properties.Title = GetImageTitle();
                request.Data.SetBitmap(RandomAccessStreamReference.CreateFromUri(new Uri(ViewModel.Wallpaper.GetCacheUrl(AppSetting.WallpaperSize))));
                deferral.Complete();
            };
        }

        private async Task SaveFile(StorageFile file)
        {
            string url = ViewModel.Wallpaper.GetCacheUrl(AppSetting.WallpaperSize);
            using (HttpClient client = new HttpClient())
            {
                await FileIO.WriteBufferAsync(file, await client.GetBufferAsync(new Uri(url)));
            }
        }

        private async Task SaveToChooseLocation()
        {
            try
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
                else
                {
                    return;
                }
                await new MessageDialog(LocalizedStrings.SaveSuccess).ShowAsyncEnqueue();
            }
            catch
            {
                await new MessageDialog(LocalizedStrings.SaveFailed).ShowAsyncEnqueue();
            }
        }

        private async Task SaveToPictureLibrary()
        {
            try
            {
                StorageFile file = await KnownFolders.PicturesLibrary.CreateFileAsync(Path.GetFileNameWithoutExtension(ViewModel.Wallpaper.Image.UrlBase) + ".jpg", CreationCollisionOption.ReplaceExisting);
                await SaveFile(file);
                await new MessageDialog(LocalizedStrings.SaveSuccess).ShowAsyncEnqueue();
            }
            catch
            {
                await new MessageDialog(LocalizedStrings.SaveFailed).ShowAsyncEnqueue();
            }
        }

        private async Task SaveToSavedPictures()
        {
            try
            {
                StorageFile file = await KnownFolders.SavedPictures.CreateFileAsync(Path.GetFileNameWithoutExtension(ViewModel.Wallpaper.Image.UrlBase) + ".jpg", CreationCollisionOption.ReplaceExisting);
                await SaveFile(file);
                await new MessageDialog(LocalizedStrings.SaveSuccess).ShowAsyncEnqueue();
            }
            catch
            {
                await new MessageDialog(LocalizedStrings.SaveFailed).ShowAsyncEnqueue();
            }
        }

        private async void SinaShare_Tapped(object sender, TappedRoutedEventArgs e)
        {
            PopupExecuting.IsOpen = true;

            await SinShare();

            PopupExecuting.IsOpen = false;
        }

        private async Task SinShare()
        {
            try
            {
                WeiboClient client = await WeiboClient.CreateAsync();
                byte[] shareData = await GetImageData();
                string shareText = GetImageTitle() + " " + ViewModel.Wallpaper.GetCacheUrl(AppSetting.WallpaperSize);
                Weibo shareResult = await client.ShareImageAsync(shareData, shareText);
                if (shareResult.ErrorCode == 21332)
                {
                    // 用户在微博平台上清除了授权，需要清除本地 access token，重新进行授权。

                    // 清除本地 access token。
                    WeiboClient.ClearAuthorize();

                    // 重新授权。
                    client = await WeiboClient.CreateAsync();

                    // 重新分享。
                    shareResult = await client.ShareImageAsync(shareData, shareText);
                }

                if (shareResult.IsSuccess)
                {
                    await new MessageDialog(LocalizedStrings.ShareSuccess).ShowAsyncEnqueue();
                }
                else
                {
                    await new MessageDialog("error:" + shareResult.Error + Environment.NewLine + "error code:" + shareResult.ErrorCode).ShowAsyncEnqueue();
                }
            }
            catch
            {
                await new MessageDialog(LocalizedStrings.ShareFailed).ShowAsyncEnqueue();
            }
        }

        private void SystemShare_Tapped(object sender, TappedRoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }

        private void Wallpaper_Opened(object sender, RoutedEventArgs e)
        {
            this.LoadingRing.IsActive = false;

            // 调整 scrollviewer 到最适合缩放。
            var scrollViewerWidth = this.ScrollViewer.ActualWidth;
            var wallpaperWidth = AppSetting.WallpaperSize.Width;
            var zoomFactor = scrollViewerWidth / wallpaperWidth;
            this.ScrollViewer.ChangeView(null, null, (float)zoomFactor, true);
        }

        private async Task WechatShare()
        {
            try
            {
                var scene = SendMessageToWX.Req.WXSceneChooseByUser;
                var pic = await this.GetImageData();
                var message = new WXImageMessage()
                {
                    Title = this.GetImageTitle(),
                    ImageData = pic
                };
                SendMessageToWX.Req req = new SendMessageToWX.Req(message, scene);
                IWXAPI api = WXAPIFactory.CreateWXAPI(App.WechatAppID);
                var isValid = await api.SendReq(req);
            }
            catch
            {
                await new MessageDialog(LocalizedStrings.ShareFailed).ShowAsyncEnqueue();
            }
        }

        private void WechatShare_Loaded(object sender, RoutedEventArgs e)
        {
            UIElement wechatShare = sender as UIElement;
            wechatShare.Visibility = DeviceFamilyHelper.IsDesktop ? Visibility.Collapsed : Visibility.Visible;
        }

        private async void WechatShare_Tapped(object sender, TappedRoutedEventArgs e)
        {
            PopupExecuting.IsOpen = true;

            await WechatShare();

            PopupExecuting.IsOpen = false;
        }
    }
}