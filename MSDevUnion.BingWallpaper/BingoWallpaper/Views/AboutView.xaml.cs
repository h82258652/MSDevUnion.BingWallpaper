using SoftwareKobo.UniversalToolkit.Helpers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace BingoWallpaper.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AboutView : Page
    {
        public AboutView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Frame.UnRegisterNavigateBack();

            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Frame.RegisterNavigateBack();

            base.OnNavigatedTo(e);
        }
    }
}