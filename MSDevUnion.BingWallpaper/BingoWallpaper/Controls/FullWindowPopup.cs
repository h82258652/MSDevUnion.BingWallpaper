using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace BingoWallpaper.Controls
{
    public class FullWindowPopup : DependencyObject
    {
        public FullWindowPopup()
        {
            Window.Current.SizeChanged += Current_SizeChanged;
        }

        private Popup _popup;



        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(FullWindowPopup), new PropertyMetadata(false));



        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
