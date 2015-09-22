using BingoWallpaper.Services;
using SoftwareKobo.UniversalToolkit.Storage;

namespace BingoWallpaper.Datas
{
    internal static partial class AppSetting
    {
        /// <summary>
        /// 查看哪个国家/区域的壁纸。
        /// </summary>
        public static string Area
        {
            get
            {
                if (ApplicationRoamingSettings.Exists(nameof(Area)))
                {
                    return ApplicationRoamingSettings.Read<string>(nameof(Area));
                }
                else
                {
                    return ServiceArea.DefaultArea;
                }
            }
            set
            {
                ApplicationRoamingSettings.Write(nameof(Area), value);
            }
        }
    }
}