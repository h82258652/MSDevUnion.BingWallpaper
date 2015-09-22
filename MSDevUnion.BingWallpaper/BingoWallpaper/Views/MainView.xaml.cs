using BingoWallpaper.Datas;
using BingoWallpaper.ViewModels;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace BingoWallpaper.Views
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

        public MainViewModel ViewModel
        {
            get
            {
                return (MainViewModel)this.DataContext;
            }
        }

        private static string _lastViewArea;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (string.Equals(_lastViewArea, AppSetting.Area) == false)
            {
                ViewModel.LoadWallpaper();
            }

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _lastViewArea = AppSetting.Area;

            base.OnNavigatedFrom(e);
        }

        private void BackgroundImage_Loaded(object sender, RoutedEventArgs e)
        {
            if (Frame.ForwardStack.Count == 0)
            {
                this.backgroundStoryboard.Begin();
            }
        }

        private void Wallpaper_Click(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(DetailView), e.ClickedItem);
        }
    }
}