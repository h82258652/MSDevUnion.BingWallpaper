using MSDevUnion.BingWallpaper.Models;
using MSDevUnion.BingWallpaper.ViewModels;
using SoftwareKobo.UniversalToolkit.Helpers;
using System.Collections.Generic;
using Windows.Phone.UI.Input;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace MSDevUnion.BingWallpaper.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DetailView : Page
    {
        public DetailViewModel ViewModel
        {
            get
            {
                return (DetailViewModel)this.DataContext;
            }
        }

        public DetailView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Frame.RegisterNavigateBack();

            BingWallpaperModel bingWallpaper = e.Parameter as BingWallpaperModel;
            if (bingWallpaper != null)
            {
                ViewModel.SetBingWallpaper(bingWallpaper);
            }

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Frame.UnRegisterNavigateBack();

            base.OnNavigatedFrom(e);
        }

        private void Hotspot_Loaded(object sender, RoutedEventArgs e)
        {
            Border brdHotspot = sender as Border;
            if (brdHotspot != null)
            {
                _brdHotspots.Add(brdHotspot);
                ResetHotspotsLocation();
            }
        }

        private void ResetHotspotsLocation()
        {
            foreach (var brdHotspot in _brdHotspots)
            {
                Hotspot hotspot = brdHotspot.DataContext as Hotspot;
                if (hotspot == null)
                {
                    continue;
                }

                Canvas.SetLeft(brdHotspot, hotspot.LocationX / 100.0d * icsHotspots.ActualWidth - brdHotspot.ActualWidth / 2);
                Canvas.SetTop(brdHotspot, hotspot.LocationY / 100.0d * icsHotspots.ActualHeight - brdHotspot.ActualHeight / 2);
            }
        }

        private List<Border> _brdHotspots = new List<Border>();

        private void IcsHotspot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResetHotspotsLocation();
        }

        private void PlayHotspotAnimate(FrameworkElement hotspot)
        {
            if (hotspot.Resources.ContainsKey("HotspotAnimate"))
            {
                Storyboard storyboard = (Storyboard)hotspot.Resources["HotspotAnimate"];
                storyboard.Begin();
            }
        }

        private void Hotspot_Tapped(object sender, TappedRoutedEventArgs e)
        {
            PlayHotspotAnimate(sender as FrameworkElement);
        }

        private void BtnEnlarge_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            EnlargeScale.ScaleX = 1.5;
            EnlargeScale.ScaleY = 1.5;
        }

        private void BtnEnlarge_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            EnlargeScale.ScaleX = 1;
            EnlargeScale.ScaleY = 1;
        }

        private void BtnEnlarge_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            EnlargeMode.Visibility = Visibility.Visible;
        }        
    }
}