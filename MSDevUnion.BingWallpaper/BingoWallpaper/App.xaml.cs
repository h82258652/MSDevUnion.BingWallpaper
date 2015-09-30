﻿using BingoWallpaper.Controls;
using BingoWallpaper.Views;
using SoftwareKobo.UniversalToolkit;
using SoftwareKobo.UniversalToolkit.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.Storage;

namespace BingoWallpaper
{
    public sealed partial class App : Bootstrapper
    {
        internal const string WechatAppID = @"wxd930ea5d5a258f4f";

        public App()
        {
            this.InitializeComponent();

            // 下面语句用于测试其他语言。
            Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "";

            //DebugSettings.EnableFrameRateCounter = true;

            this.DefaultMainPage = typeof(MainView);
            this.DefaultExtendedSplashScreen = () => new ExtendedSplashScreen();
        }

        protected override async Task OnPreStartAsync(IActivatedEventArgs args, AppStartInfo info)
        {
            await SetTitleBar();
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

    //    /// <summary>
    //    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    //    /// </summary>
    //    sealed partial class App : Application
    //    {
    //        /// <summary>
    //        /// 初始化单一实例应用程序对象。这是执行的创作代码的第一行，
    //        /// 已执行，逻辑上等同于 main() 或 WinMain()。
    //        /// </summary>
    //        public App()
    //        {
    //            this.InitializeComponent();
    //            this.Suspending += OnSuspending;
    //        }

    //        /// <summary>
    //        /// 在应用程序由最终用户正常启动时进行调用。
    //        /// 将在启动应用程序以打开特定文件等情况下使用。
    //        /// </summary>
    //        /// <param name="e">有关启动请求和过程的详细信息。</param>
    //        protected override void OnLaunched(LaunchActivatedEventArgs e)
    //        {

    //#if DEBUG
    //            if (System.Diagnostics.Debugger.IsAttached)
    //            {
    //                this.DebugSettings.EnableFrameRateCounter = true;
    //            }
    //#endif

    //            Frame rootFrame = Window.Current.Content as Frame;

    //            // 不要在窗口已包含内容时重复应用程序初始化，
    //            // 只需确保窗口处于活动状态
    //            if (rootFrame == null)
    //            {
    //                // 创建要充当导航上下文的框架，并导航到第一页
    //                rootFrame = new Frame();

    //                rootFrame.NavigationFailed += OnNavigationFailed;

    //                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
    //                {
    //                    //TODO: 从之前挂起的应用程序加载状态
    //                }

    //                // 将框架放在当前窗口中
    //                Window.Current.Content = rootFrame;
    //            }

    //            if (rootFrame.Content == null)
    //            {
    //                // 当导航堆栈尚未还原时，导航到第一页，
    //                // 并通过将所需信息作为导航参数传入来配置
    //                // 参数
    //                rootFrame.Navigate(typeof(MainPage), e.Arguments);
    //            }
    //            // 确保当前窗口处于活动状态
    //            Window.Current.Activate();
    //        }

    //        /// <summary>
    //        /// 导航到特定页失败时调用
    //        /// </summary>
    //        ///<param name="sender">导航失败的框架</param>
    //        ///<param name="e">有关导航失败的详细信息</param>
    //        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
    //        {
    //            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
    //        }

    //        /// <summary>
    //        /// 在将要挂起应用程序执行时调用。  在不知道应用程序
    //        /// 无需知道应用程序会被终止还是会恢复，
    //        /// 并让内存内容保持不变。
    //        /// </summary>
    //        /// <param name="sender">挂起的请求的源。</param>
    //        /// <param name="e">有关挂起请求的详细信息。</param>
    //        private void OnSuspending(object sender, SuspendingEventArgs e)
    //        {
    //            var deferral = e.SuspendingOperation.GetDeferral();
    //            //TODO: 保存应用程序状态并停止任何后台活动
    //            deferral.Complete();
    //        }
    //    }
}
