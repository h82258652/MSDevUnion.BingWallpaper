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
                    var area = ApplicationRoamingSettings.Read<string>(nameof(Area));
                    return area;
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