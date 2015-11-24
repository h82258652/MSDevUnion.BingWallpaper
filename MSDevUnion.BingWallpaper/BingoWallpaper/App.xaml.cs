using BingoWallpaper.Controls;
using BingoWallpaper.Views;
using SoftwareKobo.UniversalToolkit;
using SoftwareKobo.UniversalToolkit.Extensions;
using System;
using System.Threading.Tasks;
using UmengSDK;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace BingoWallpaper
{
    public sealed partial class App : Bootstrapper
    {
        internal const string WechatAppID = @"wxff94ab33c2c89267";

        private const string UmengAppkey = @"56137ba2e0f55adb22005d2b";

        public App()
        {
            this.InitializeComponent();

#if DEBUG
            // 下面语句用于测试其他语言。
            Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "";

            this.DebugSettings.EnableFrameRateCounter = true;
            this.DebugSettings.EnableDisplayMemoryUsage();
#endif

            this.DefaultNavigatePage = typeof(MainView);
            this.DefaultExtendedSplashScreen = () => new ExtendedSplashScreen();
        }

        protected override async Task OnPreStartAsync(IActivatedEventArgs args, AppStartInfo info)
        {
            await UmengAnalytics.StartTrackAsync(UmengAppkey);

            await SetTitleBar();

            if (RootFrame != null)
            {
                info.NavigatePage = null;
            }
        }

        protected override async void OnResuming(object sender, object e)
        {
            await UmengAnalytics.StartTrackAsync(UmengAppkey);
        }

        protected override async Task OnSuspendingAsync(object sender, SuspendingEventArgs e)
        {
            await UmengAnalytics.EndTrackAsync();
        }

        public static async Task SetTitleBar()
        {
            await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                UISettings uiSettings = new UISettings();
                try
                {
                    ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
                    titleBar.BackgroundColor = uiSettings.GetColorValue(UIColorType.Accent);
                    titleBar.InactiveBackgroundColor = uiSettings.GetColorValue(UIColorType.AccentDark1);
                    titleBar.ButtonBackgroundColor = uiSettings.GetColorValue(UIColorType.Accent);
                    titleBar.ButtonInactiveBackgroundColor = uiSettings.GetColorValue(UIColorType.AccentDark1);
                    titleBar.ButtonHoverBackgroundColor = uiSettings.GetColorValue(UIColorType.AccentLight1);
                    titleBar.ButtonHoverForegroundColor = uiSettings.GetColorValue(UIColorType.Accent);
                    titleBar.ButtonPressedBackgroundColor = uiSettings.GetColorValue(UIColorType.AccentLight3);
                }
                catch (InvalidCastException)
                {
                }
            });
        }
    }
}