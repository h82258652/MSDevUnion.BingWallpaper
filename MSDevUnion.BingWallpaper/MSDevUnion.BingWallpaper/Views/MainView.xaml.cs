using MSDevUnion.BingWallpaper.Controls;
using MSDevUnion.BingWallpaper.Datas;
using MSDevUnion.BingWallpaper.ViewModels;
using SoftwareKobo.UniversalToolkit.Extensions;
using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace MSDevUnion.BingWallpaper.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainView : Page
    {
        public MainViewModel ViewModel
        {
            get
            {
                return (MainViewModel)this.DataContext;
            }
        }

        public MainView()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;

            #region 初始化控制板的背景色
            Color accentColor = ColorExtensions.AccentColor;
            accentColor.A = 192;
            this.grdControl.Background = new SolidColorBrush(accentColor);
            #endregion

            #region 初始化日期选择器
            dtpViewingMonth.MinYear = new DateTime(2015, 1, 28);
            dtpViewingMonth.MaxYear = DateTime.Now;
            dtpViewingMonth.Date = AppSettings.LastViewDate;
            #endregion
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadBingWallpapers();

            base.OnNavigatedTo(e);
        }

        private async void LoadBingWallpapers()
        {
            await ViewModel.LoadBingWallpapers();
            var bingWallpapers = ViewModel.ViewingBingWallpapers;
            //for (int i = 0; i < bingWallpapers.Count; i++)
            //{
            //    Thumbnail thumbnail = new Thumbnail() { DataContext = bingWallpapers[i] };
            //    if (i==0)
            //    {
            //       VariableSizedWrapGrid.SetColumnSpan(thumbnail, 2);
            //    }
            //    else
            //    {
            //        VariableSizedWrapGrid.SetColumnSpan(thumbnail, 1);
            //    }
            //    grid.Children.Add(thumbnail);
            //}
        }

        private void BtnNextYear_Click(object sender, RoutedEventArgs e)
        {
            var currentViewingMonth = dtpViewingMonth.Date;
            var newViewingMonth = currentViewingMonth.AddYears(1);
            dtpViewingMonth.Date = newViewingMonth;
            AppSettings.LastViewDate = newViewingMonth;
            ViewModel.LoadBingWallpapers();
        }

        private void BtnNextMonth_Click(object sender, RoutedEventArgs e)
        {
            var currentViewingMonth = dtpViewingMonth.Date;
        }

        private void BtnPreviousMonth_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BtnPreviousYear_Click(object sender, RoutedEventArgs e)
        {
        }

        private void DtpViewingMonth_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
        }
    }
}