using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using BingoWallpaper.ThirdParty.Helper;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace BingoWallpaper.ThirdParty
{
    public sealed class VirtualizingFlipViewItem : FlipViewItem, IVirtualizingItem
    {
        public VirtualizingFlipViewItem()
        {
            this.DefaultStyleKey = typeof(VirtualizingFlipViewItem);
        }

        public static readonly DependencyProperty IsVirtualizedProperty = DependencyProperty.Register(
            "IsVirtualized", typeof (bool), typeof (VirtualizingFlipViewItem), new PropertyMetadata(true));
        
        public bool IsVirtualized
        {
            get { return (bool) GetValue(IsVirtualizedProperty); }
            set { SetValue(IsVirtualizedProperty, value); }
        }

        public bool CanRealize => IsVirtualized;

        public bool CanVirtualize => !IsVirtualized;
    
        public void Realize()
        {
            if (CanRealize)
            {
                if (ContentPresenter != null)
                {
                    ContentPresenter.Visibility = Visibility.Visible;
                }
                IsVirtualized = false;
            }
        }

        public void Virtualize()
        {
            if (CanVirtualize)
            {
                if (ContentPresenter != null)
                {
                    ContentPresenter.Visibility = Visibility.Collapsed;
                }
                IsVirtualized = true;
            }
        }

        public ContentPresenter ContentPresenter
        {
            get { return VisualThreeHelper.FindVisualElementFormName(this,"ContentPresenter") as ContentPresenter; }
        }

        //protected override Size ArrangeOverride(Size finalSize)
        //{
        //    if (IsVirtualized)
        //    {
        //        return this.DesiredSize;
        //    }
        //    else
        //    {
        //        return base.ArrangeOverride(finalSize);
        //    }
        //}
    }
}
