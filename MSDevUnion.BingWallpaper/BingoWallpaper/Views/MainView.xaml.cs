using BingoWallpaper.Datas;
using BingoWallpaper.ViewModels;
using System;
using System.Collections.Generic;
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
        private static string _lastViewArea;

        private List<WeakReference<VariableSizedWrapGrid>> _thumbnailGrids = new List<WeakReference<VariableSizedWrapGrid>>();

        public MainView()
        {
            this.InitializeComponent();
            Window.Current.SizeChanged += delegate
            {
                ResetThumbnailGrid();
            };
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
                this.ViewModel.ReloadAll();
            }

            base.OnNavigatedTo(e);
        }

        private void ResetThumbnailGrid()
        {
            var size = Window.Current.Bounds;
            foreach (var thumbnailGridReference in _thumbnailGrids)
            {
                VariableSizedWrapGrid grid;
                if (thumbnailGridReference.TryGetTarget(out grid))
                {
                    if (size.Width > 960)
                    {
                        grid.ItemWidth = 320;
                        grid.ItemHeight = 200;
                    }
                    else
                    {
                        grid.ItemWidth = 160;
                        grid.ItemHeight = 100;
                    }
                }
            }
        }

        private void ThumbnailGrid_Loaded(object sender, RoutedEventArgs e)
        {
            _thumbnailGrids.Add(new WeakReference<VariableSizedWrapGrid>((VariableSizedWrapGrid)sender));
            ResetThumbnailGrid();
        }

        private void Wallpaper_Click(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(DetailView), e.ClickedItem);
        }
    }
}