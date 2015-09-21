using MSDevUnion.BingWallpaper.Models;
using SoftwareKobo.UniversalToolkit.Extensions;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MSDevUnion.BingWallpaper.Controls
{
    public sealed partial class Thumbnail : UserControl
    {
        public BingWallpaperModel BingWallpaperModel
        {
            get
            {
                return (BingWallpaperModel)this.DataContext;
            }
        }

        public Thumbnail()
        {
            this.InitializeComponent();
            this.Loaded += Thumbnail_Loaded;
        }

        private void Thumbnail_Loaded(object sender, RoutedEventArgs e)
        {
            if (BingWallpaperModel != null && BingWallpaperModel.IsFirst)
            {
                var parent = this.GetAncestorsOfType<ContentPresenter>().FirstOrDefault();
                if (parent != null)
                {
                    VariableSizedWrapGrid.SetColumnSpan(parent, 2);
                }
            }
        }
    }
}