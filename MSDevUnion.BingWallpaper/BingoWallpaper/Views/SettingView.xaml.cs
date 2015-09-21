using BingoWallpaper.ViewModels;
using SoftwareKobo.UniversalToolkit.Helpers;
using SoftwareKobo.UniversalToolkit.Storage;
using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace BingoWallpaper.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingView : Page
    {
        public SettingView()
        {
            this.InitializeComponent();
        }

        public SettingViewModel ViewModel
        {
            get
            {
                return (SettingViewModel)this.DataContext;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Frame.UnRegisterNavigateBack();

            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Frame.RegisterNavigateBack();

            CalcCachedSize();

            base.OnNavigatedTo(e);
        }

        private async void BtnCleanupCachedImage_Click(object sender, RoutedEventArgs e)
        {
            Control control = sender as Control;
            if (control != null)
            {
                control.IsEnabled = false;
            }
            StorageCachedImage.CleanUpCachedImages();
            await new MessageDialog("清空成功").ShowAsync();
            this.CalcCachedSize();
            if (control != null)
            {
                control.IsEnabled = true;
            }
        }

        private void CalcCachedSize()
        {
            // 获取已缓存图片大小，单位字节。
            long bytes = StorageCachedImage.GetCachedImagesSize();
            double kb = bytes / 1024.0;
            if (kb < 1.0)
            {
                txtCachedSize.Text = $"{bytes} B";
                return;
            }
            double mb = kb / 1024.0;
            if (mb < 1.0)
            {
                txtCachedSize.Text = kb.ToString("f2") + " KB";
                return;
            }
            else
            {
                txtCachedSize.Text = mb.ToString("f2") + " MB";
                return;
            }
        }
    }
}