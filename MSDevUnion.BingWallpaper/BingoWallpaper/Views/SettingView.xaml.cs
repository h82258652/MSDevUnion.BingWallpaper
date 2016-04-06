using SoftwareKobo.UniversalToolkit.Helpers;
using Windows.UI.Xaml.Navigation;

namespace BingoWallpaper.Views
{
    public sealed partial class SettingView
    {
        public SettingView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            NavigationHelper.Unregister(Frame);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            NavigationHelper.Register(Frame);
        }
    }
}