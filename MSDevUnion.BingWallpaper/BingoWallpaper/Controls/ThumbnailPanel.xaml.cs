using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace BingoWallpaper.Controls
{
    public sealed partial class ThumbnailPanel : UserControl
    {
        public ThumbnailPanel()
        {
            this.InitializeComponent();
            Window.Current.SizeChanged += delegate
            {
                ResetThumbnailGrid();
            };
        }

        private void Wallpaper_Click(object sender, ItemClickEventArgs e)
        {
            if (ItemClick != null)
            {
                ItemClick(sender, e);
            }
        }

        public event ItemClickEventHandler ItemClick;

        private void ThumbnailGrid_Loaded(object sender, RoutedEventArgs e)
        {
            _thumbnailGrids.Add(new WeakReference<VariableSizedWrapGrid>((VariableSizedWrapGrid)sender));
            ResetThumbnailGrid();
        }

        private List<WeakReference<VariableSizedWrapGrid>> _thumbnailGrids = new List<WeakReference<VariableSizedWrapGrid>>();

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
    }
}
