using SoftwareKobo.UniversalToolkit.Helpers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace BingoWallpaper.Views
{
    public sealed partial class SettingView : Page
    {
        public SettingView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            NavigationHelper.Unregister(this.Frame);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            NavigationHelper.Register(this.Frame);
        }
    }
}