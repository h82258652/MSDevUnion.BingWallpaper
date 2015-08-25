using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace MSDevUnion.BingWallpaper.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainView : Page
    {
        public MainView()
        {
            this.InitializeComponent();
        }

        private void BtnSetting_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingView));
        }

        private void BtnAbout_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutView));
        }

        private List<FrameworkElement> _thumbnailItems = new List<FrameworkElement>();

        private void ThumbnailView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var item in _thumbnailItems)
            {
                //item.MaxWidth = thumbnailView.ActualWidth / 7;
                //item.MaxHeight = thumbnailView.ActualWidth / 7;
            }
        }

        private void ThumbnailItem_Loaded(object sender, RoutedEventArgs e)
        {
            var item = sender as FrameworkElement;
            if (item != null)
            {
                //item.MaxWidth = thumbnailView.ActualWidth / 7;
                //item.MaxHeight = thumbnailView.ActualWidth / 7;
                _thumbnailItems.Add(item);
            }
        }

        private void DatePicker_Loaded(object sender, RoutedEventArgs e)
        {
            DatePicker picker = sender as DatePicker;
            if (picker != null)
            {
                picker.MinYear = new DateTime(2015, 1, 28);
            }
        }
    }
}