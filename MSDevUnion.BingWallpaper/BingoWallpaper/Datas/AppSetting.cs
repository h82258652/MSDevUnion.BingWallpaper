using BingoWallpaper.Models;
using SoftwareKobo.UniversalToolkit.Storage;
using System.Text.RegularExpressions;

namespace BingoWallpaper.Datas
{
    internal static partial class AppSetting
    {
        /// <summary>
        /// 保存壁纸时，保存到哪个位置。
        /// </summary>
        public static SaveLocation SaveLocation
        {
            get
            {
                if (ApplicationRoamingSettings.Exists(nameof(SaveLocation)))
                {
                    return ApplicationRoamingSettings.Read<SaveLocation>(nameof(SaveLocation));
                }
                else
                {
                    return SaveLocation.PictureLibrary;
                }
            }
            set
            {
                ApplicationRoamingSettings.Write(nameof(SaveLocation), value);
            }
        }

        /// <summary>
        /// 用户设置的壁纸大小。
        /// </summary>
        public static WallpaperSize WallpaperSize
        {
            get
            {
                if (ApplicationLocalSettings.Exists(nameof(WallpaperSize)))
                {
                    string value = ApplicationLocalSettings.Read<string>(nameof(WallpaperSize));
                    Regex regex = new Regex(@"^(\d+)x(\d+)$");
                    Match match = regex.Match(value);
                    if (match.Success)
                    {
                        GroupCollection groups = match.Groups;
                        int width = int.Parse(groups[1].Value);
                        int height = int.Parse(groups[2].Value);
                        return new WallpaperSize(width, height);
                    }
                }
                return null;
            }
            set
            {
                ApplicationLocalSettings.Write(nameof(WallpaperSize), value.ToString());
            }
        }
    }
}