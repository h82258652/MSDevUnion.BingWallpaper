using BingoWallpaper.Models;
using SoftwareKobo.UniversalToolkit.Extensions;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace BingoWallpaper.Controls
{
    public sealed partial class Thumbnail : UserControl
    {
        public Thumbnail()
        {
            this.InitializeComponent();
            this.Loaded += this.Thumbnail_Loaded;
        }

        public Wallpaper Wallpaper
        {
            get
            {
                return (Wallpaper)this.DataContext;
            }
        }

        private void Thumbnail_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Wallpaper != null && this.Wallpaper.IsLastInMonth)
            {
                var item = this.GetAncestorsOfType<GridViewItem>().FirstOrDefault();
                if (item != null)
                {
                    VariableSizedWrapGrid.SetColumnSpan(item, 2);
                }
            }
        }
    }
}