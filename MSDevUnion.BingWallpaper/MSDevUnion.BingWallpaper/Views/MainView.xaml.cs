using MSDevUnion.BingWallpaper.Datas;
using MSDevUnion.BingWallpaper.ViewModels;
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

            #region 初始化控制板的背景色
            Color accentColor = ((SolidColorBrush)App.Current.Resources["SystemControlBackgroundAccentBrush"]).Color;
            accentColor.A = 192;
            this.grdControl.Background = new SolidColorBrush(accentColor);
            #endregion

            #region 初始化日期选择器
            dtpViewingMonth.MinYear = new DateTime(2015, 1, 28);
            dtpViewingMonth.MaxYear = DateTime.Now;
            dtpViewingMonth.Date = AppSettings.LastViewDate;
            #endregion

            this.Loaded += MainView_Loaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.LoadBingWallpapers();
        }

        private void BtnSetting_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingView));
        }

        private void BtnAbout_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutView));
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

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var rr = e.ClickedItem;

            Frame.Navigate(typeof(DetailView), rr);
        }
    }
}