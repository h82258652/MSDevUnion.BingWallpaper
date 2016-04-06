using BingoWallpaper.Models;
using SoftwareKobo.UniversalToolkit.Extensions;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BingoWallpaper.Controls
{
    public sealed partial class Thumbnail
    {
        public Thumbnail()
        {
            InitializeComponent();
        }

        public Wallpaper Wallpaper
        {
            get
            {
                return (Wallpaper)DataContext;
            }
        }

        private void Thumbnail_Loaded(object sender, RoutedEventArgs e)
        {
            if (Wallpaper != null && Wallpaper.IsLastInMonth)
            {
                var item = this.GetAncestorsOfType<GridViewItem>().FirstOrDefault();
                if (item != null)
                {
                    VariableSizedWrapGrid.SetColumnSpan(item, 2);
                    VariableSizedWrapGrid.SetRowSpan(item, 2);
                }
            }
        }
    }
}