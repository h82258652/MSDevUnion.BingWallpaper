using BingoWallpaper.Datas;
using BingoWallpaper.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace BingoWallpaper.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainView : Page
    {
        private static string _lastViewArea;

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

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _lastViewArea = AppSetting.Area;

            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (string.Equals(_lastViewArea, AppSetting.Area) == false)
            {
                foreach (var wallpapers in this.ViewModel.AllWallpapers)
                {
                    wallpapers.Clear();
                }
            }

            base.OnNavigatedTo(e);
        }

        private void Wallpaper_Click(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(DetailView), e.ClickedItem);
        }

        private async void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var allWallpapers = ViewModel.AllWallpapers;
            var viewingWallpaper = ViewModel.ViewingWallpaper;

            var currentIndex = allWallpapers.IndexOf(viewingWallpaper);

            for (int i = 0; i < allWallpapers.Count; i++)
            {
                var wallpapers = allWallpapers[i];
                if (i == currentIndex - 1 || i == currentIndex || i == currentIndex + 1)
                {
                    if (wallpapers.Count <= 0)
                    {
                        await allWallpapers[i].ReLoad();
                    }
                }
                else
                {
                    wallpapers.Clear();
                }
            }
        }
    }
}