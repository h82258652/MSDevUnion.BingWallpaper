﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BingoWallpaper.Controls
{
    public sealed partial class ThumbnailPanel : UserControl
    {
        private VariableSizedWrapGrid _thumbnailGrid;

        public ThumbnailPanel()
        {
            this.InitializeComponent();
            Window.Current.SizeChanged += delegate
            {
                ResetThumbnailGrid();
            };
        }

        public event ItemClickEventHandler ItemClick;

        private void ResetThumbnailGrid()
        {
            if (_thumbnailGrid == null)
            {
                return;
            }

            var size = Window.Current.Bounds;
            if (size.Width > 960)
            {
                _thumbnailGrid.ItemWidth = 320;
                _thumbnailGrid.ItemHeight = 192;
            }
            else
            {
                _thumbnailGrid.ItemWidth = 160;
                _thumbnailGrid.ItemHeight = 96;
            }
        }

        private void ThumbnailGrid_Loaded(object sender, RoutedEventArgs e)
        {
            _thumbnailGrid = (VariableSizedWrapGrid)sender;
            ResetThumbnailGrid();
        }

        private void Wallpaper_Click(object sender, ItemClickEventArgs e)
        {
            if (ItemClick != null)
            {
                ItemClick(sender, e);
            }
        }
    }
}