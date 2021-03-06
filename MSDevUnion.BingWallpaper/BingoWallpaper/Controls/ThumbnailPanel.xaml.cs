﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BingoWallpaper.Controls
{
    public sealed partial class ThumbnailPanel
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
                _thumbnailGrid.ItemHeight = 180;
            }
            else
            {
                _thumbnailGrid.ItemWidth = 160;
                _thumbnailGrid.ItemHeight = 90;
            }
        }

        private void ThumbnailGrid_Loaded(object sender, RoutedEventArgs e)
        {
            _thumbnailGrid = (VariableSizedWrapGrid)sender;
            ResetThumbnailGrid();
        }

        private void Wallpaper_Click(object sender, ItemClickEventArgs e)
        {
            ItemClick?.Invoke(sender, e);
        }
    }
}