using BingoWallpaper.Datas;
using BingoWallpaper.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace BingoWallpaper.Views
{
    public sealed partial class MainView
    {
        private static string _lastViewArea;

        public MainView()
        {
            InitializeComponent();
        }

        public MainViewModel ViewModel
        {
            get
            {
                return (MainViewModel)DataContext;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            _lastViewArea = AppSetting.Area;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (string.Equals(_lastViewArea, AppSetting.Area) == false)
            {
                foreach (var wallpapers in ViewModel.AllWallpapers)
                {
                    wallpapers.Clear();
                }
            }
        }

        private void Wallpaper_Click(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(DetailView), e.ClickedItem);
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